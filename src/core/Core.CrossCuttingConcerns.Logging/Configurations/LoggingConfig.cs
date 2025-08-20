namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Configurations;

public class LoggingConfig
{
    public string DefaultLogLevel { get; set; } = "Information";
    public Dictionary<string, string> Override { get; set; } = new Dictionary<string, string>();
    public string Target { get; set; } = "File";

    public LoggingConfig()
    {
        Override = new Dictionary<string, string>();
    }
}
