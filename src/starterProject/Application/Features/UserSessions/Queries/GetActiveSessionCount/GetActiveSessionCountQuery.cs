using Application.Services.UserSessions;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Queries.GetActiveSessionCount;
public class GetActiveSessionCountQuery : IRequest<GetMyActiveSessionCountResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetActiveSessionCountQueryHandler : IRequestHandler<GetActiveSessionCountQuery, GetMyActiveSessionCountResponse>
    {
        private readonly IUserSessionService _userSessionService;

        public GetActiveSessionCountQueryHandler(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
        }

        public async Task<GetMyActiveSessionCountResponse> Handle(GetActiveSessionCountQuery request, CancellationToken cancellationToken)
        {
            var activeSessions = await _userSessionService.GetUserSessionsAsync(request.UserId);
            var count = activeSessions.Count();

            return new GetMyActiveSessionCountResponse
            {
                UserId = request.UserId,
                ActiveSessionCount = count
            };
        }
    }
}
