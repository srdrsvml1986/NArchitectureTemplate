using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NArchitecture.Core.CrossCuttingConcerns.Logging.OAuth;
public class OAuthAuditLogger
{
    private readonly ILogger<OAuthAuditLogger> _logger;

    public OAuthAuditLogger(ILogger<OAuthAuditLogger> logger)
    {
        _logger = logger;
    }

    public Task LogOAuthEvent(string userId, string provider, string action)
    {
        _logger.LogInformation(
            "OAuth Event: {Action} for User {UserId} with Provider {Provider}",
            action,
            userId,
            provider
        );
        return Task.CompletedTask;
    }
}
