namespace WebAPI.Configrations;

public class WebApiConfiguration
{
    public string ApiDomain { get; set; } = "";
    public string BasePath { get; set; } = "/api";
    public string[] Origins { get; set; }

    public WebApiConfiguration()
    {
        ApiDomain = string.Empty;
        Origins = Array.Empty<string>();
    }

    public WebApiConfiguration(string apiDomain, string[] origins, string basePath)
    {
        ApiDomain = apiDomain;
        Origins = origins;
        BasePath = basePath;
    }
}