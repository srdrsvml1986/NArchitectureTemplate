using MimeKit;

namespace NArchitectureTemplate.Core.Mailing;

public enum MailPriority
{
    Normal = 0,
    Low = 1,
    High = 2
}

public class Mail
{
    public string Subject { get; set; }
    public string TextBody { get; set; }
    public string HtmlBody { get; set; }
    public AttachmentCollection? Attachments { get; set; }
    public List<MailboxAddress> ToList { get; set; }
    public List<MailboxAddress>? CcList { get; set; }
    public List<MailboxAddress>? BccList { get; set; }
    public string? UnsubscribeLink { get; set; }
    public MailPriority Priority { get; set; } // Yeni özellik

    public Mail()
    {
        Subject = string.Empty;
        TextBody = string.Empty;
        HtmlBody = string.Empty;
        ToList = [];
        Priority = MailPriority.Normal; // Varsayılan değer
    }

    public Mail(string subject, string textBody, string htmlBody, List<MailboxAddress> toList, MailPriority priority = MailPriority.Normal)
    {
        Subject = subject;
        TextBody = textBody;
        HtmlBody = htmlBody;
        ToList = toList;
        Priority = priority;
    }
}