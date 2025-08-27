using Microsoft.Extensions.Configuration;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NArchitectureTemplate.Core.Notification.Services
{
    public class MultiProviderSmsService : ISmsService
    {
        private readonly ISmsServiceFactory _serviceFactory;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public MultiProviderSmsService(
            ISmsServiceFactory serviceFactory,
            ILogger logger,
            IConfiguration configuration)
        {
            _serviceFactory = serviceFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public SmsProvider Provider => SmsProvider.None; // Multi-provider olduğu için None

        public async Task<SmsResponse> SendAsync(SmsMessage message)
        {
            // Öncelikle tercih edilen sağlayıcıyı kullan
            var preferredProvider = message.PreferredProvider ??
                                  GetDefaultProvider();

            var service = _serviceFactory.GetService(preferredProvider);
            var response = await service.SendAsync(message);
            response.Provider = preferredProvider;

            // Eğer başarısızsa, yedek sağlayıcıyı dene
            if (!response.IsSuccess)
            {
                _logger.Warning(string.Format("Birincil SMS sağlayıcı ({Provider}) başarısız oldu, yedek sağlayıcı deneniyor",
                    preferredProvider));

                var fallbackProvider = GetFallbackProvider(preferredProvider);
                var fallbackService = _serviceFactory.GetService(fallbackProvider);

                var fallbackResponse = await fallbackService.SendAsync(message);
                fallbackResponse.Provider = fallbackProvider;
                fallbackResponse.IsFallback = true;

                return fallbackResponse;
            }

            return response;
        }

        public async Task<SmsResponse> SendBulkAsync(List<SmsMessage> messages)
        {
            // Tüm mesajlar için aynı sağlayıcıyı kullan
            var provider = messages.Count > 0 && messages[0].PreferredProvider.HasValue
                ? messages[0].PreferredProvider.Value
                : GetDefaultProvider();

            var service = _serviceFactory.GetService(provider);
            var response = await service.SendBulkAsync(messages);
            response.Provider = provider;

            // Eğer başarısızsa, yedek sağlayıcıyı dene
            if (!response.IsSuccess)
            {
                _logger.Warning(string.Format("Birincil SMS sağlayıcı ({Provider}) başarısız oldu, yedek sağlayıcı deneniyor",
                    provider));

                var fallbackProvider = GetFallbackProvider(provider);
                var fallbackService = _serviceFactory.GetService(fallbackProvider);

                var fallbackResponse = await fallbackService.SendBulkAsync(messages);
                fallbackResponse.Provider = fallbackProvider;
                fallbackResponse.IsFallback = true;

                return fallbackResponse;
            }

            return response;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            // Varsayılan sağlayıcının bakiyesini döndür
            var defaultService = _serviceFactory.GetDefaultService();
            return await defaultService.GetBalanceAsync();
        }

        public async Task<SmsResponse> GetStatusAsync(string messageId)
        {
            // Bu implementasyon için mesaj ID'sinden sağlayıcıyı çıkarmamız gerekebilir
            // Şimdilik varsayılan sağlayıcıyı kullanıyoruz
            var defaultService = _serviceFactory.GetDefaultService();
            return await defaultService.GetStatusAsync(messageId);
        }

        private SmsProvider GetDefaultProvider()
        {
            var defaultProvider = _configuration["Sms:DefaultProvider"] ?? "Vodafone";
            if (Enum.TryParse<SmsProvider>(defaultProvider, true, out var provider))
            {
                return provider;
            }

            return SmsProvider.Vodafone;
        }

        private SmsProvider GetFallbackProvider(SmsProvider primaryProvider)
        {
            var fallbackConfig = _configuration["Sms:FallbackProvider"];
            if (!string.IsNullOrEmpty(fallbackConfig) && Enum.TryParse<SmsProvider>(fallbackConfig, true, out var fallback))
            {
                return fallback;
            }

            return primaryProvider == SmsProvider.Vodafone ? SmsProvider.Turkcell : SmsProvider.Vodafone;
        }
    }
}