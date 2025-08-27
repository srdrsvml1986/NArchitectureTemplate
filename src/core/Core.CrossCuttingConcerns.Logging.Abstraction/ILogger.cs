namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;

public interface ILogger
{
    void Trace(string message, params object[] args);
    void Critical(string message, params object[] args);
    void Information(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Debug(string message, params object[] args);
    void Error(string message, params object[] args);
    void Error(Exception exception, string message, params object[] args);
}
