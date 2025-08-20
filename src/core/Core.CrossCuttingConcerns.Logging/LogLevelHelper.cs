namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging;
public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical
}

public static class LogLevelHelper
{
    public static bool ShouldLog(LogLevel configuredLevel, LogLevel messageLevel)
    {
        return messageLevel >= configuredLevel;
    }
}