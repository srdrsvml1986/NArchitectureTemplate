namespace Persistence;

public class DatabaseSettings
{
    public string SelectedProvider { get; set; }
    public DatabaseProviderConfig PostgreConfiguration { get; set; }
    public DatabaseProviderConfig MsSqlConfiguration { get; set; }
    public DatabaseProviderConfig OracleConfiguration { get; set; }
    public DatabaseProviderConfig MongoDbConfiguration { get; set; }
    public DatabaseProviderConfig InMemoryConfiguration { get; set; }

    public DatabaseProviderConfig? GetSelectedProviderConfig()
    {
        return SelectedProvider?.ToLower() switch
        {
            "postgresql" => PostgreConfiguration,
            "sqlserver" => MsSqlConfiguration,
            "oracle" => OracleConfiguration,
            "mongodb" => MongoDbConfiguration,
            "ınmemory" => InMemoryConfiguration,
            _ => throw new InvalidOperationException($"Desteklenmeyen veritabanı sağlayıcı: {SelectedProvider}")
        };
    }
}
