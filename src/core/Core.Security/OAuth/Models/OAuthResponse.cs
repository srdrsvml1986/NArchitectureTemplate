namespace NArchitectureTemplate.Core.Security.OAuth.Models;
public class OAuthResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public ExternalAuthUser? User { get; set; }
}
