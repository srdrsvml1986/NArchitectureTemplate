using Application.Features.UserSessions.Queries.GetActiveSessionCount;
using Application.Services.AuthService;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Queries.GetMyActiveSessionCount;
public class GetMyActiveSessionCountQuery : IRequest<GetMyActiveSessionCountResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetMyActiveSessionCountQueryHandler : IRequestHandler<GetMyActiveSessionCountQuery, GetMyActiveSessionCountResponse>
    {
        private readonly IAuthService _authService;

        public GetMyActiveSessionCountQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<GetMyActiveSessionCountResponse> Handle(GetMyActiveSessionCountQuery request, CancellationToken cancellationToken)
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