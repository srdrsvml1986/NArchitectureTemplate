using Application.Services.NotificationServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Services.NotificationServices;

    public class VodafoneSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VodafoneSmsService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _senderAddress;
        private readonly string _tokenUrl;
        private readonly string _smsUrl;
        private string _accessToken;
        private DateTime _tokenExpiry;
        public SmsProvider Provider => SmsProvider.Vodafone; // Yeni property
    public VodafoneSmsService(
            IConfiguration configuration,
            ILogger<VodafoneSmsService> logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;

            // Vodafone API configuration
            _clientId = _configuration["Sms:Vodafone:ClientId"]
                ?? throw new ArgumentNullException("Vodafone ClientId is missing");
            _clientSecret = _configuration["Sms:Vodafone:ClientSecret"]
                ?? throw new ArgumentNullException("Vodafone ClientSecret is missing");
            _senderAddress = _configuration["Sms:Vodafone:SenderAddress"]
                ?? throw new ArgumentNullException("Vodafone SenderAddress is missing");
            _tokenUrl = _configuration["Sms:Vodafone:TokenUrl"]
                ?? "https://api.vodafone.com/token";
            _smsUrl = _configuration["Sms:Vodafone:SmsUrl"]
                ?? "https://api.vodafone.com/smsmessaging/v1/outbound";
        }

        public async Task<SmsResponse> SendAsync(SmsMessage message)
        {
            try
            {
                // Ensure we have a valid access token
                await EnsureValidTokenAsync();

                // Prepare Vodafone API request
                var vodafoneRequest = new VodafoneSmsRequest
                {
                    OutboundSMSMessageRequest = new OutboundSMSMessageRequest
                    {
                        Address = new List<string> { FormatPhoneNumber(message.To) },
                        SenderAddress = $"tel:{_senderAddress}",
                        OutboundSMSTextMessage = new OutboundSMSTextMessage
                        {
                            Message = message.Content
                        },
                        ClientCorrelator = message.ReferenceId ?? Guid.NewGuid().ToString(),
                        ReceiptRequest = new ReceiptRequest
                        {
                            NotifyURL = _configuration["Sms:Vodafone:CallbackUrl"],
                            CallbackData = message.ReferenceId
                        }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(vodafoneRequest);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add authorization header
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _accessToken);

                // Send SMS
                var response = await _httpClient.PostAsync(
                    $"{_smsUrl}/{_senderAddress}/requests", httpContent);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var vodafoneResponse = JsonSerializer.Deserialize<VodafoneSmsResponse>(responseContent);

                    _logger.LogInformation("SMS sent successfully to {To}, Message ID: {MessageId}",
                        message.To, vodafoneResponse?.OutboundSMSMessageRequest?.ResourceReference?.ResourceURL);

                    return new SmsResponse
                    {
                        IsSuccess = true,
                        MessageId = vodafoneResponse?.OutboundSMSMessageRequest?.ResourceReference?.ResourceURL,
                        ProviderResponse = responseContent
                    };
                }
                else
                {
                    _logger.LogError("Failed to send SMS to {To}. Status: {StatusCode}, Response: {Response}",
                        message.To, response.StatusCode, responseContent);

                    return new SmsResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"HTTP {response.StatusCode}: {responseContent}",
                        ProviderResponse = responseContent
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {To}", message.To);

                return new SmsResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<SmsResponse> SendBulkAsync(List<SmsMessage> messages)
        {
            var results = new List<SmsResponse>();
            var failedMessages = new List<SmsMessage>();

            foreach (var message in messages)
            {
                var result = await SendAsync(message);
                results.Add(result);

                if (!result.IsSuccess)
                {
                    failedMessages.Add(message);
                }

                // Vodafone API rate limiting - adjust based on your contract
                await Task.Delay(100);
            }

            return new SmsResponse
            {
                IsSuccess = failedMessages.Count == 0,
                ErrorMessage = failedMessages.Count > 0 ?
                    $"{failedMessages.Count} messages failed to send" : null,
                ProviderResponse = $"Processed {messages.Count} messages, {results.FindAll(r => r.IsSuccess).Count} successful"
            };
        }

        public async Task<decimal> GetBalanceAsync()
        {
            // Vodafone API doesn't provide balance information in the standard way
            // This would need to be implemented based on your specific Vodafone contract
            _logger.LogWarning("Balance check not implemented for Vodafone SMS API");
            return 0;
        }

        public async Task<SmsResponse> GetStatusAsync(string messageId)
        {
            try
            {
                await EnsureValidTokenAsync();

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _httpClient.GetAsync(messageId);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var statusResponse = JsonSerializer.Deserialize<VodafoneSmsStatusResponse>(responseContent);
                    var status = statusResponse?.DeliveryInfoList?.DeliveryInfo?[0]?.DeliveryStatus;

                    return new SmsResponse
                    {
                        IsSuccess = status == "DeliveredToNetwork" || status == "DeliveredToTerminal",
                        MessageId = messageId,
                        ProviderResponse = responseContent
                    };
                }

                return new SmsResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"HTTP {response.StatusCode}: {responseContent}",
                    ProviderResponse = responseContent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting SMS status for {MessageId}", messageId);

                return new SmsResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task EnsureValidTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiry)
            {
                await AcquireTokenAsync();
            }
        }

        private async Task AcquireTokenAsync()
        {
            try
            {
                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

                var request = new HttpRequestMessage(HttpMethod.Post, _tokenUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "sms")
                });

                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = JsonSerializer.Deserialize<VodafoneTokenResponse>(responseContent);

                    _accessToken = tokenResponse?.AccessToken;
                    _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse?.ExpiresIn ?? 3600);

                    _logger.LogInformation("Successfully acquired Vodafone API token");
                }
                else
                {
                    _logger.LogError("Failed to acquire Vodafone API token. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);
                    throw new Exception($"Failed to acquire token: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acquiring Vodafone API token");
                throw;
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            // Ensure phone number is in E.164 format (e.g., +905551234567)
            if (phoneNumber.StartsWith("0"))
            {
                return "+90" + phoneNumber.Substring(1);
            }

            if (!phoneNumber.StartsWith("+"))
            {
                return "+" + phoneNumber;
            }

            return phoneNumber;
        }

        #region Vodafone API Models

        private class VodafoneTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("scope")]
            public string Scope { get; set; }
        }

        private class VodafoneSmsRequest
        {
            [JsonPropertyName("outboundSMSMessageRequest")]
            public OutboundSMSMessageRequest OutboundSMSMessageRequest { get; set; }
        }

        private class OutboundSMSMessageRequest
        {
            [JsonPropertyName("address")]
            public List<string> Address { get; set; }

            [JsonPropertyName("senderAddress")]
            public string SenderAddress { get; set; }

            [JsonPropertyName("outboundSMSTextMessage")]
            public OutboundSMSTextMessage OutboundSMSTextMessage { get; set; }

            [JsonPropertyName("clientCorrelator")]
            public string ClientCorrelator { get; set; }

            [JsonPropertyName("receiptRequest")]
            public ReceiptRequest ReceiptRequest { get; set; }
        }

        private class OutboundSMSTextMessage
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }

        private class ReceiptRequest
        {
            [JsonPropertyName("notifyURL")]
            public string NotifyURL { get; set; }

            [JsonPropertyName("callbackData")]
            public string CallbackData { get; set; }
        }

        private class VodafoneSmsResponse
        {
            [JsonPropertyName("outboundSMSMessageRequest")]
            public OutboundSMSMessageResponse OutboundSMSMessageRequest { get; set; }
        }

        private class OutboundSMSMessageResponse
        {
            [JsonPropertyName("resourceReference")]
            public ResourceReference ResourceReference { get; set; }
        }

        private class ResourceReference
        {
            [JsonPropertyName("resourceURL")]
            public string ResourceURL { get; set; }
        }

        private class VodafoneSmsStatusResponse
        {
            [JsonPropertyName("deliveryInfoList")]
            public DeliveryInfoList DeliveryInfoList { get; set; }
        }

        private class DeliveryInfoList
        {
            [JsonPropertyName("deliveryInfo")]
            public List<DeliveryInfo> DeliveryInfo { get; set; }
        }

        private class DeliveryInfo
        {
            [JsonPropertyName("address")]
            public string Address { get; set; }

            [JsonPropertyName("deliveryStatus")]
            public string DeliveryStatus { get; set; }
        }

        #endregion
    }