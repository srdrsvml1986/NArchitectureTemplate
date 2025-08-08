using Application.Services.AuthService;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Commands.CheckSuspiciousSessions;

public class CheckSuspiciousSessionsCommand : IRequest<CheckSuspiciousSessionsResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [Admin, Write];

    public class CheckSuspiciousSessionsCommandHandler : IRequestHandler<CheckSuspiciousSessionsCommand, CheckSuspiciousSessionsResponse>
    {
        private readonly IAuthService _authService;

        public CheckSuspiciousSessionsCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<CheckSuspiciousSessionsResponse> Handle(CheckSuspiciousSessionsCommand request, CancellationToken cancellationToken)
        {
            await _authService.FlagAndHandleSuspiciousSessionsAsync(request.UserId);

            return new CheckSuspiciousSessionsResponse
            {
                UserId = request.UserId,
                Message = "Suspicious sessions check completed"
            };
        }
    }
}
