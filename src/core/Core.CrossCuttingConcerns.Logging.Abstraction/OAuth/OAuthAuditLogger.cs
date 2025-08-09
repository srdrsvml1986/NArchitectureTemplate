namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction.OAuth;
public class OAuthAuditLogger
{
    private readonly ILogger _logger;

    public OAuthAuditLogger(ILogger logger)
    {
        _logger = logger;
    }

    public async Task LogOAuthEvent(string userId, string provider, string action)
    {
        _logger.Information(
            $"OAuth Event: {action} for User {userId} with Provider {provider}"
        );
        await Task.CompletedTask;
    }
}
