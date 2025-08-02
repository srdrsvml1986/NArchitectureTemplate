using Application.Services.AuthService;
using Application.Services.UserSessions;
using AutoMapper;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Commands.RevokeAllOtherSessions;
public class RevokeAllOtherSessionsCommand : IRequest<RevokeAllOtherSessionsResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public Guid CurrentSessionId { get; set; }

    public string[] Roles => [Admin, Write];

    public class RevokeAllOtherSessionsCommandHandler : IRequestHandler<RevokeAllOtherSessionsCommand, RevokeAllOtherSessionsResponse>
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public RevokeAllOtherSessionsCommandHandler(
            IUserSessionService userSessionService,
            IMapper mapper,
            IAuthService authService)
        {
            _userSessionService = userSessionService;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<RevokeAllOtherSessionsResponse> Handle(RevokeAllOtherSessionsCommand request, CancellationToken cancellationToken)
        {
            var activeSessions = await _authService.GetActiveSessionsAsync(request.UserId);
            var otherSessions = activeSessions.Where(s => s.Id != request.CurrentSessionId).ToList();

            foreach (var session in otherSessions)
            {
                session.IsRevoked = true;
                await _userSessionService.UpdateAsync(session);
            }

            return new RevokeAllOtherSessionsResponse
            {
                UserId = request.UserId,
                RevokedSessionCount = otherSessions.Count,
                Message = $"{otherSessions.Count} sessions revoked successfully"
            };
        }
    }
}
