using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.NotificationServices
{
    public class SmsServiceFactory : ISmsServiceFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Dictionary<SmsProvider, ISmsService> _services;

        public SmsServiceFactory(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
            _httpClientFactory = httpClientFactory;
            _services = new Dictionary<SmsProvider, ISmsService>();
        }

        public ISmsService GetService(SmsProvider provider)
        {
            if (_services.TryGetValue(provider, out var service))
            {
                return service;
            }

            switch (provider)
            {
                case SmsProvider.Vodafone:
                    service = new VodafoneSmsService(
                        _configuration,
                        _loggerFactory.CreateLogger<VodafoneSmsService>(),
                        _httpClientFactory.CreateClient("VodafoneSms"));
                    break;

                case SmsProvider.Turkcell:
                    service = new TurkcellSmsService(
                        _configuration,
                        _loggerFactory.CreateLogger<TurkcellSmsService>(),
                        _httpClientFactory.CreateClient("TurkcellSms"));
                    break;

                default:
                    throw new ArgumentException($"Desteklenmeyen SMS sağlayıcı: {provider}");
            }

            _services[provider] = service;
            return service;
        }

        public ISmsService GetDefaultService()
        {
            var defaultProvider = _configuration["Sms:DefaultProvider"] ?? "Vodafone";
            if (Enum.TryParse<SmsProvider>(defaultProvider, true, out var provider))
            {
                return GetService(provider);
            }

            return GetService(SmsProvider.Vodafone);
        }
    }

    public interface ISmsServiceFactory
    {
        ISmsService GetService(SmsProvider provider);
        ISmsService GetDefaultService();
    }
}