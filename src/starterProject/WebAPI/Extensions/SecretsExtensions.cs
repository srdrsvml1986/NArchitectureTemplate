namespace WebAPI.Extensions;

using Application.Services;
using Application.Services.EmergencyAndSecretServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class SecretsSeedExtensions
{
    // Logger için ayrı bir non-static sınıf oluşturalım
    private class SecretsSeedLogger
    {
        // Bu sadece logger için kullanılacak boş bir sınıf
    }

    public static void SeedDefaultSecrets(IConfiguration configuration, ILocalSecretsManager secretsManager, bool forceUpdate = false)
    {     


        var defaultSecrets = new Dictionary<string, string>
        {
            // Database Connection Strings
            ["DatabaseSettings:PostgreConfiguration:ConnectionString"] =
        GetSecretValue(configuration, "DatabaseSettings:PostgreConfiguration:ConnectionString",
        "Host=localhost;Port=5432;Database=DevDb;Username=postgres;Password=devpass;"),

            ["DatabaseSettings:MsSqlConfiguration:ConnectionString"] =
        GetSecretValue(configuration, "DatabaseSettings:MsSqlConfiguration:ConnectionString",
        "Server=localhost;Database=DevDb;User Id=sa;Password=devpass;"),

            ["DatabaseSettings:OracleConfiguration:ConnectionString"] =
        GetSecretValue(configuration, "DatabaseSettings:OracleConfiguration:ConnectionString",
        "Data Source=localhost:1521;User Id=SYSTEM;Password=devpass;"),

            ["DatabaseSettings:MongoDbConfiguration:ConnectionString"] =
        GetSecretValue(configuration, "DatabaseSettings:MongoDbConfiguration:ConnectionString",
        "mongodb://localhost:27017/devdb?readPreference=primary&ssl=false"),

            // Token
            ["TokenOptions:SecurityKey"] =
        GetSecretValue(configuration, "TokenOptions:SecurityKey",
        "dev-super-secure-jwt-secret-key-minimum-64-characters-long-1234567890"),

            // Security
            ["Security:EncryptionKey"] =
        GetSecretValue(configuration, "Security:EncryptionKey",
        "dev-32-character-encryption-key!"),

            ["Backup:EncryptionKey"] =
        GetSecretValue(configuration, "Backup:EncryptionKey",
        "dev-backup-encryption-key-2024"),

            // Cloudinary
            ["CloudinaryAccount:ApiSecret"] =
        GetSecretValue(configuration, "CloudinaryAccount:ApiSecret",
        "dev-cloudinary-api-secret"),

            // MailSettings
            ["MailSettings:Password"] =
        GetSecretValue(configuration, "MailSettings:Password",
        "dev-email-password"),

            ["MailSettings:DkimPrivateKey"] =
        GetSecretValue(configuration, "MailSettings:DkimPrivateKey",
        "dev-dkim-private-key"),

            ["MailSettings:DomainName"] =
        GetSecretValue(configuration, "MailSettings:DomainName",
        "dev.example.com"),

            ["MailSettings:SenderEmail"] =
        GetSecretValue(configuration, "MailSettings:SenderEmail",
        "dev@example.com"),

            ["MailSettings:Server"] =
        GetSecretValue(configuration, "MailSettings:Server",
        "smtp.dev.example.com"),

            ["MailSettings:UserName"] =
        GetSecretValue(configuration, "MailSettings:UserName",
        "dev@example.com"),

            // Google Authentication
            ["Authentication:Google:ClientId"] =
        GetSecretValue(configuration, "Authentication:Google:ClientId",
        "dev-google-client-id.apps.googleusercontent.com"),

            ["Authentication:Google:ClientSecret"] =
        GetSecretValue(configuration, "Authentication:Google:ClientSecret",
        "dev-google-client-secret"),

            ["Authentication:Google:RedirectUri"] =
        GetSecretValue(configuration, "Authentication:Google:RedirectUri",
        "https://localhost:5001/signin-google"),

            // Facebook Authentication
            ["Authentication:Facebook:AppId"] =
        GetSecretValue(configuration, "Authentication:Facebook:AppId",
        "dev-facebook-app-id"),

            ["Authentication:Facebook:AppSecret"] =
        GetSecretValue(configuration, "Authentication:Facebook:AppSecret",
        "dev-facebook-app-secret"),

            ["Authentication:Facebook:RedirectUri"] =
        GetSecretValue(configuration, "Authentication:Facebook:RedirectUri",
        "https://localhost:5001/signin-facebook"),

            // Emergency Settings
            ["Emergency:SmtpServer"] =
        GetSecretValue(configuration, "Emergency:SmtpServer",
        "smtp.dev.example.com"),

            ["Emergency:NotificationRecipients"] =
        GetSecretValue(configuration, "Emergency:NotificationRecipients",
        "bilgi@serdarsevimli.tr"),

            ["Emergency:SmtpUser"] =
        GetSecretValue(configuration, "Emergency:SmtpUser",
        "dev@example.com"),

            ["Emergency:SmtpPass"] =
        GetSecretValue(configuration, "Emergency:SmtpPass",
        "dev-email-password"),

            ["Emergency:SmsApiUrl"] =
        GetSecretValue(configuration, "Emergency:SmsApiUrl",
        "https://dev-api.smsprovider.com/send"),

            ["Emergency:SmsApiKey"] =
        GetSecretValue(configuration, "Emergency:SmsApiKey",
        "dev-sms-api-key")
        };

        foreach (var (key, defaultValue) in defaultSecrets)
            try
            {
                var existingValue = secretsManager.GetSecret(key);

                if (forceUpdate || string.IsNullOrEmpty(existingValue))
                {
                    secretsManager.SetSecret(key, defaultValue);
                }
                else
                {
                    configuration[key] = existingValue;
                }
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, "Secret eklenirken/güncellenirken hata oluştu: {Key}", key);
            }

        
    }

    private static string GetSecretValue(IConfiguration configuration, string key, string defaultValue)
    {
        var value = configuration[key];
        return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }

    public static IServiceCollection AddSecretsManagement(this IServiceCollection services, IConfiguration configuration)
    {
        // Master key'i ortam değişkenlerinden al
        var masterKey = Environment.GetEnvironmentVariable("MASTER_KEY");
        if (string.IsNullOrEmpty(masterKey))
            // Development ortamında default bir master key kullan
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Staging")
            {
                masterKey = "dev-master-key-for-secrets-management-2024";
                Console.WriteLine("GELİŞTİRME MODU: Varsayılan master key kullanılıyor. Production'da MASTER_KEY ortam değişkeni ayarlayın.");
            }
            else
                throw new Exception("MASTER_KEY ortam değişkeni tanımlanmalı");


        // Servis kayıtları
        services.AddSingleton<IEncryptionService>(new EncryptionService(masterKey));
        services.AddSingleton<ILocalSecretsManager>(provider =>
            new LocalSecretsManager(masterKey));

        SeedDefaultSecrets(configuration, services.BuildServiceProvider().GetRequiredService<ILocalSecretsManager>());
        return services;
    }
}


