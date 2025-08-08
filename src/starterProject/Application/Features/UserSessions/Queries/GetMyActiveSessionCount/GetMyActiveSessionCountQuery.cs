using Application.Features.UserSessions.Queries.GetActiveSessionCount;
using Application.Services.UserSessions;
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
        private readonly IUserSessionService _userSessionService;

        public GetMyActiveSessionCountQueryHandler(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
        }

        public async Task<GetMyActiveSessionCountResponse> Handle(GetMyActiveSessionCountQuery request, CancellationToken cancellationToken)
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