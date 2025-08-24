namespace Application.Services.NotificationServices;
using System.Threading.Tasks;
using System.Collections.Generic;

public interface ISmsService
{
    SmsProvider Provider { get; } // Yeni property

    Task<SmsResponse> SendAsync(SmsMessage message);
    Task<SmsResponse> SendBulkAsync(List<SmsMessage> messages);
    Task<decimal> GetBalanceAsync();
    Task<SmsResponse> GetStatusAsync(string messageId);
}
public enum SmsPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Emergency = 3
}


public enum SmsProvider
{
    Vodafone,
    Turkcell,
    Twilio,
    None // Multi-provider için yeni değer
}



public class SmsMessage
{
    public string To { get; set; }
    public string Content { get; set; }
    public SmsPriority Priority { get; set; } = SmsPriority.Normal;
    public string? ReferenceId { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public string? Sender { get; set; }
    public SmsProvider? PreferredProvider { get; set; }
}

public class SmsResponse
{
    public bool IsSuccess { get; set; }
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ProviderResponse { get; set; }
    public SmsProvider Provider { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsFallback { get; set; } // Yeni property
}




