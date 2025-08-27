using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;

namespace NArchitectureTemplate.Core.Notification.Services
{
    public class FirebasePushNotificationService : IPushNotificationService
    {
        private readonly ILogger _logger;

        public FirebasePushNotificationService(ILogger logger)
        {
            _logger = logger;

            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                try
                {
                    var credentialPath = Path.Combine(AppContext.BaseDirectory, "Configs", "serviceAccountKey.json");

                    if (!File.Exists(credentialPath))
                        throw new FileNotFoundException("Firebase service account key dosyası bulunamadı.", credentialPath);

                    var credential = GoogleCredential.FromFile(credentialPath);

                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = credential
                    });

                    _logger.Information("Firebase başlatıldı.");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Firebase başlatma hatası.");
                    throw;
                }
            }
        }

        public async Task<PushNotificationResponse> SendAsync(PushNotification notification)
        {
            var response = new PushNotificationResponse();
            var failedTokens = new List<string>();

            try
            {
                var messages = new List<Message>();

                foreach (var token in notification.DeviceTokens)
                {
                    var message = new Message
                    {
                        Token = token,
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = notification.Title,
                            Body = notification.Body
                        },
                        Data = notification.Data.Count > 0 ?
                            new Dictionary<string, string>(notification.Data) : null
                    };

                    // Android konfigürasyonu
                    if (notification.Platform == PushPlatform.Android || notification.Platform == PushPlatform.All)
                    {
                        message.Android = new AndroidConfig
                        {
                            Priority = GetAndroidPriority(notification.Priority)
                        };

                        // TimeToLive özelliğini ayarla
                        if (notification.TimeToLive > 0)
                        {
                            message.Android.TimeToLive = TimeSpan.FromSeconds(notification.TimeToLive);
                        }

                        if (!string.IsNullOrEmpty(notification.ImageUrl))
                        {
                            message.Android.Notification = new AndroidNotification
                            {
                                ImageUrl = notification.ImageUrl
                            };
                        }
                    }

                    // iOS konfigürasyonu
                    if (notification.Platform == PushPlatform.iOS || notification.Platform == PushPlatform.All)
                    {
                        message.Apns = new ApnsConfig
                        {
                            Headers = new Dictionary<string, string>
                            {
                                { "apns-priority", GetApnsPriority(notification.Priority) }
                            },
                            Aps = new Aps
                            {
                                ContentAvailable = true,
                                Badge = 1
                            }
                        };
                    }

                    messages.Add(message);
                }

                var batchResponse = await FirebaseMessaging.DefaultInstance.SendEachAsync(messages);

                response.SuccessCount = batchResponse.SuccessCount;
                response.FailureCount = batchResponse.FailureCount;

                // Başarısız gönderimleri işleme
                for (int i = 0; i < batchResponse.Responses.Count; i++)
                {
                    var sendResponse = batchResponse.Responses[i];
                    if (sendResponse.IsSuccess)
                    {
                        response.MessageId = sendResponse.MessageId;
                    }
                    else
                    {
                        failedTokens.Add(messages[i].Token);
                        _logger.Error(sendResponse.Exception,
                            string.Format("Push notification gönderilemedi. Token: {0}", messages[i].Token));
                    }
                }

                response.IsSuccess = batchResponse.FailureCount == 0;
                response.FailedDeviceTokens = failedTokens;

                _logger.Information(string.Format("Push notification gönderildi. Başarılı: {0}, Başarısız: {1}",
                    batchResponse.SuccessCount, batchResponse.FailureCount));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Push notification gönderme hatası.");
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.FailedDeviceTokens = notification.DeviceTokens;
            }

            return response;
        }

        public async Task<PushNotificationResponse> SendToTopicAsync(string topic, PushNotification notification)
        {
            try
            {
                var message = new Message
                {
                    Topic = topic,
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = notification.Title,
                        Body = notification.Body
                    },
                    Data = notification.Data.Count > 0 ?
                        new Dictionary<string, string>(notification.Data) : null
                };

                var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                _logger.Information(string.Format("Push notification topic'e gönderildi. Topic: {0}, Message ID: {1}",
                    topic, messageId));

                return new PushNotificationResponse
                {
                    IsSuccess = true,
                    MessageId = messageId,
                    SuccessCount = 1
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format("Push notification topic'e gönderme hatası. Topic: {0}", topic));

                return new PushNotificationResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    FailureCount = 1
                };
            }
        }

        public async Task<bool> SubscribeToTopicAsync(string topic, List<string> deviceTokens)
        {
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(deviceTokens, topic);

                _logger.Information(string.Format("Cihazlar topic'e abone oldu. Topic: {0}, Başarılı: {1}, Başarısız: {2}",
                    topic, response.SuccessCount, response.FailureCount));

                return response.FailureCount == 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format("Cihazları topic'e abone etme hatası. Topic: {0}", topic));
                return false;
            }
        }

        public async Task<bool> UnsubscribeFromTopicAsync(string topic, List<string> deviceTokens)
        {
            try
            {
                var response = await FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(deviceTokens, topic);

                _logger.Information(string.Format("Cihazlar topic'ten aboneliği kaldırıldı. Topic: {0}, Başarılı: {1}, Başarısız: {2}",
                    topic, response.SuccessCount, response.FailureCount));

                return response.FailureCount == 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, string.Format("Cihazları topic'ten aboneliği kaldırma hatası. Topic: {0}", topic));
                return false;
            }
        }

        private Priority GetAndroidPriority(PushNotificationPriority priority)
        {
            return priority switch
            {
                PushNotificationPriority.High => Priority.High,
                PushNotificationPriority.Normal => Priority.Normal,
                _ => Priority.Normal // Low için Normal kullanılıyor
            };
        }

        private string GetApnsPriority(PushNotificationPriority priority)
        {
            return priority switch
            {
                PushNotificationPriority.High => "10",
                PushNotificationPriority.Normal => "5",
                _ => "5" // Low için Normal (5) kullanılıyor
            };
        }
    }
}