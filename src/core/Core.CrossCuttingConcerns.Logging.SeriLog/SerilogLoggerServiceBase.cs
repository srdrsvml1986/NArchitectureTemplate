using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using PackageSerilog = Serilog;

namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Serilog;

public abstract class SerilogLoggerServiceBase : ILogger
{
    protected PackageSerilog.ILogger? Logger { get; set; }

    protected SerilogLoggerServiceBase(PackageSerilog.ILogger logger)
    {
        Logger = logger;
    }

    public void Critical(string message, params object[] args)
    {
        if (args?.Length > 0)
            Logger?.Fatal(message, args);
        else
            Logger?.Fatal(message);
    }

    public void Debug(string message, params object[] args)
    {
        if (args?.Length > 0)
            Logger?.Debug(message, args);
        else
            Logger?.Debug(message);
    }

    public void Error(string message, params object[] args)
    {
        if (args?.Length > 0)
            Logger?.Error(message, args);
        else
            Logger?.Error(message);
    }

    public void Error(Exception exception, string message, params object[] args)
    {
        if (args?.Length > 0)
            Logger?.Error(exception, message, args);
        else
            Logger?.Error(exception, message);
    }

    public void Information(string message, params object[] args)
    {
        if (args?.Length > 0)
            Logger?.Information(message, args);
        else
            Logger?.Information(message);
    }

    public void Trace(string message, params object[] args)
    {
        if (args?.Length > 0)
            Logger?.Verbose(message, args);
        else
            Logger?.Verbose(message);
    }

    public void Warning(string message, params object[] args)
    {
        if (args?.Length > 0)
            Logger?.Warning(message, args);
        else
            Logger?.Warning(message);
    }
}