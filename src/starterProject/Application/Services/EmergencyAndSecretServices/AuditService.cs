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

    public void LogAccess(string key, string action, string user, string ipAddress, string t="")
    {
        var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {user} | {ipAddress} | {action} | {key}";

        lock (_lock)
            File.AppendAllText(_auditLogPath, logEntry + Environment.NewLine);
    }

    public IEnumerable<string> GetRecentAuditLogs(int maxEntries = 100)
    {
        if (!File.Exists(_auditLogPath))
            return Enumerable.Empty<string>();

        lock (_lock)
            return File.ReadLines(_auditLogPath).TakeLast(maxEntries);
    }
}
