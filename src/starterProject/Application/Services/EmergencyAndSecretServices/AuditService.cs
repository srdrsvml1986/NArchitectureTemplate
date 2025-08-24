using System.Text.Json;

namespace Application.Services.EmergencyAndSecretServices;
// AuditService.cs
public class AuditService
{
    private readonly string _auditLogPath;
    private readonly object _lock = new object();

    public AuditService(string logDirectory = "C:\\App\\Logs\\Security")
    {
        _auditLogPath = Path.Combine(logDirectory, "secrets_audit.log");
        Directory.CreateDirectory(logDirectory);
    }

    public void LogAccess(string key, string action, string user, string ipAddress, string details = "")
    {
        var logEntry = new
        {
            Timestamp = DateTime.UtcNow,
            User = user,
            IP = ipAddress,
            Action = action,
            Key = key,
            Details = details,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        };

        var logMessage = JsonSerializer.Serialize(logEntry);

        lock (_lock)
            File.AppendAllText(_auditLogPath, logMessage + Environment.NewLine);
    }

    public IEnumerable<string> GetRecentAuditLogs(int maxEntries = 100)
    {
        if (!File.Exists(_auditLogPath))
            return Enumerable.Empty<string>();

        lock (_lock)
            return File.ReadLines(_auditLogPath).TakeLast(maxEntries);
    }
}
