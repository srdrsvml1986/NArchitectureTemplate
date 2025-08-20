namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Configurations;

// DatabaseLogConfiguration.cs
public class DatabaseLogConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public string TableName { get; set; } = "Logs";
    public bool AutoCreateSqlTable { get; set; } = true;
}