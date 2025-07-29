using Application.Services.UserSessions;
using AutoMapper;
using Domain.DTos;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;


namespace Application.Features.UserSessions.Queries.GetActiveSessions;

public class GetActiveSessionsQuery : IRequest<GetActiveSessionsResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetActiveSessionsQueryHandler : IRequestHandler<GetActiveSessionsQuery, GetActiveSessionsResponse>
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IMapper _mapper;

        public GetActiveSessionsQueryHandler(IUserSessionService userSessionService, IMapper mapper)
        {
            _userSessionService = userSessionService;
            _mapper = mapper;
        }

        public async Task<GetActiveSessionsResponse> Handle(GetActiveSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _userSessionService.GetActiveSessionsAsync(request.UserId);

            return new GetActiveSessionsResponse
            {
                UserId = request.UserId,
                Sessions = _mapper.Map<List<UserSessionDto>>(sessions)
            };
        }
    }
}
