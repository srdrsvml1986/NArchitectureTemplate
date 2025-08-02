using Application.Services.AuthService;
using Application.Services.UserSessions;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Queries.GetActiveSessionCount;
public class GetActiveSessionCountQuery : IRequest<GetMyActiveSessionCountResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetActiveSessionCountQueryHandler : IRequestHandler<GetActiveSessionCountQuery, GetMyActiveSessionCountResponse>
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IAuthService _authService;

        public GetActiveSessionCountQueryHandler(IUserSessionService userSessionService, IAuthService authService)
        {
            _userSessionService = userSessionService;
            _authService = authService;
        }

        public async Task<GetMyActiveSessionCountResponse> Handle(GetActiveSessionCountQuery request, CancellationToken cancellationToken)
        {
            var activeSessions = await _authService.GetActiveSessionsAsync(request.UserId);
            var count = activeSessions.Count();

            return new GetMyActiveSessionCountResponse
            {
                UserId = request.UserId,
                ActiveSessionCount = count
            };
        }
    }
}
