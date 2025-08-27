using Microsoft.Extensions.Configuration;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NArchitectureTemplate.Core.Notification.Services
{
    public class TurkcellSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;
        private string _authToken;
        private DateTime _tokenExpiry;
        public SmsProvider Provider => SmsProvider.Vodafone; // Yeni property
        public TurkcellSmsService(
            IConfiguration configuration,
            ILogger logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;

            _username = _configuration["Sms:Turkcell:Username"]
                ?? throw new ArgumentNullException("Turkcell kullanıcı adı eksik");
            _password = _configuration["Sms:Turkcell:Password"]
                ?? throw new ArgumentNullException("Turkcell şifre eksik");
            _baseUrl = _configuration["Sms:Turkcell:BaseUrl"]
                ?? "https://mesajussu.turkcell.com.tr/api";
        }

        public async Task<SmsResponse> SendAsync(SmsMessage message)
        {
            try
            {
                await EnsureValidTokenAsync();

                // Turkcell API'si için istek formatı
                var requestData = new
                {
                    message = message.Content,
                    sender = message.Sender ?? _configuration["Sms:Turkcell:Sender"],
                    etkFlag = true,
                    receivers = new[] { FormatPhoneNumber(message.To) }
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _authToken);

                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/integration/v2/sms/send/oton", httpContent);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var turkcellResponse = JsonSerializer.Deserialize<TurkcellSmsResponse>(responseContent);

                    _logger.Information("Turkcell SMS gönderildi: {To}, Message ID: {MessageId}",
                        message.To, turkcellResponse?.MessageId);

                    return new SmsResponse
                    {
                        IsSuccess = true,
                        MessageId = turkcellResponse?.MessageId,
                        ProviderResponse = responseContent,
                        Provider = SmsProvider.Turkcell
                    };
                }
                else
                {
                    _logger.Warning("Turkcell SMS gönderimi başarısız: {To}. Status: {StatusCode}, Response: {Response}",
                        message.To, response.StatusCode, responseContent);

                    return new SmsResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"HTTP {response.StatusCode}: {responseContent}",
                        ProviderResponse = responseContent,
                        Provider = SmsProvider.Turkcell
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Turkcell SMS gönderim hatası: {To}", message.To);

                return new SmsResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    Provider = SmsProvider.Turkcell
                };
            }
        }

        public async Task<SmsResponse> SendBulkAsync(List<SmsMessage> messages)
        {
            // Turkcell için toplu mesaj gönderimi
            // Aynı mesajı birden fazla numaraya gönderme
            var phoneNumbers = new List<string>();
            string commonMessage = null;
            string commonSender = null;

            foreach (var message in messages)
            {
                phoneNumbers.Add(FormatPhoneNumber(message.To));
                commonMessage = message.Content; // Tüm mesajlar aynı içeriğe sahip olmalı
                commonSender = message.Sender ?? _configuration["Sms:Turkcell:Sender"];
            }

            try
            {
                await EnsureValidTokenAsync();

                var requestData = new
                {
                    message = commonMessage,
                    sender = commonSender,
                    etkFlag = true,
                    receivers = phoneNumbers
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _authToken);

                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/integration/v2/sms/send/oton", httpContent);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var turkcellResponse = JsonSerializer.Deserialize<TurkcellSmsResponse>(responseContent);

                    _logger.Information("Turkcell toplu SMS gönderildi: {Count} numara", phoneNumbers.Count);

                    return new SmsResponse
                    {
                        IsSuccess = true,
                        MessageId = turkcellResponse?.MessageId,
                        ProviderResponse = responseContent,
                        Provider = SmsProvider.Turkcell
                    };
                }
                else
                {
                    _logger.Warning(string.Format("Turkcell toplu SMS gönderimi başarısız. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent));

                    return new SmsResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = $"HTTP {response.StatusCode}: {responseContent}",
                        ProviderResponse = responseContent,
                        Provider = SmsProvider.Turkcell
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Turkcell toplu SMS gönderim hatası");

                return new SmsResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    Provider = SmsProvider.Turkcell
                };
            }
        }

        public async Task<decimal> GetBalanceAsync()
        {
            try
            {
                await EnsureValidTokenAsync();

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _authToken);

                var response = await _httpClient.GetAsync($"{_baseUrl}/integration/v2/user/info");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var userInfo = JsonSerializer.Deserialize<TurkcellUserInfo>(responseContent);
                    return userInfo?.Credit ?? 0;
                }

                _logger.Error("Turkcell bakiye sorgulama başarısız. Status: {StatusCode}", response.StatusCode);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Turkcell bakiye sorgulama hatası");
                return 0;
            }
        }

        public async Task<SmsResponse> GetStatusAsync(string messageId)
        {
            try
            {
                await EnsureValidTokenAsync();

                var requestData = new
                {
                    messages = new[] { messageId }
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _authToken);

                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/integration/v2/sms/check/status", httpContent);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var statusResponse = JsonSerializer.Deserialize<TurkcellStatusResponse>(responseContent);
                    var status = statusResponse?.Statuses?[0]?.Status;

                    return new SmsResponse
                    {
                        IsSuccess = status == "0", // 0 = delivered
                        MessageId = messageId,
                        ProviderResponse = responseContent,
                        Provider = SmsProvider.Turkcell
                    };
                }

                return new SmsResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"HTTP {response.StatusCode}: {responseContent}",
                    ProviderResponse = responseContent,
                    Provider = SmsProvider.Turkcell
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Turkcell durum sorgulama hatası: {MessageId}", messageId);

                return new SmsResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    Provider = SmsProvider.Turkcell
                };
            }
        }

        private async Task EnsureValidTokenAsync()
        {
            if (string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiry)
            {
                await AcquireTokenAsync();
            }
        }

        private async Task AcquireTokenAsync()
        {
            try
            {
                var loginData = new
                {
                    username = _username,
                    password = _password
                };

                var jsonContent = JsonSerializer.Serialize(loginData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/integration/v2/login", httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<TurkcellLoginResponse>(responseContent);

                    _authToken = loginResponse?.Token;
                    _tokenExpiry = DateTime.UtcNow.AddHours(23); // 24 saat geçerli, 1 saat önceden yenilemek için

                    _logger.Information("Turkcell token başarıyla alındı");
                }
                else
                {
                    _logger.Error("Turkcell token alma başarısız. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);
                    throw new Exception($"Token alınamadı: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Turkcell token alma hatası");
                throw;
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            // Turkcell formatı: 5xxxxxxxxx
            if (phoneNumber.StartsWith("+90"))
            {
                return phoneNumber.Substring(3); // +905551234567 -> 5551234567
            }

            if (phoneNumber.StartsWith("90"))
            {
                return phoneNumber.Substring(2); // 905551234567 -> 5551234567
            }

            if (phoneNumber.StartsWith("0"))
            {
                return phoneNumber.Substring(1); // 05551234567 -> 5551234567
            }

            return phoneNumber;
        }

        #region Turkcell API Modelleri

        private class TurkcellLoginResponse
        {
            [JsonPropertyName("token")]
            public string Token { get; set; }

            [JsonPropertyName("tokenExpireDate")]
            public string TokenExpireDate { get; set; }
        }

        private class TurkcellSmsResponse
        {
            [JsonPropertyName("messageId")]
            public string[] MessageIds { get; set; }

            public string MessageId => MessageIds?.Length > 0 ? MessageIds[0] : null;
        }

        private class TurkcellUserInfo
        {
            [JsonPropertyName("credit")]
            public decimal Credit { get; set; }
        }

        private class TurkcellStatusResponse
        {
            [JsonPropertyName("statuses")]
            public List<StatusInfo> Statuses { get; set; }
        }

        private class StatusInfo
        {
            [JsonPropertyName("m")]
            public string MessageId { get; set; }

            [JsonPropertyName("s")]
            public string Status { get; set; }
        }

        #endregion
    }
}