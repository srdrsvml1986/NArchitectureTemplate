using Application.Features.UserSessions.Queries.GetList;
using Application.Services.UserSessions;
using AutoMapper;
using Domain.DTos;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;


namespace Application.Features.UserSessions.Queries.GetActiveSessions;

public class GetActiveSessionsQuery : IRequest<GetListResponse<GetListUserSessionListItemDto>>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetActiveSessionsQueryHandler : IRequestHandler<GetActiveSessionsQuery, GetListResponse<GetListUserSessionListItemDto>>
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IMapper _mapper;

        public GetActiveSessionsQueryHandler(IUserSessionService userSessionService, IMapper mapper)
        {
            _userSessionService = userSessionService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserSessionListItemDto>> Handle(GetActiveSessionsQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserSession>? userSessions = await _userSessionService.GetListAsync(
              predicate: us => us.UserId == request.UserId,
              index: request.PageRequest.PageIndex,
              size: request.PageRequest.PageSize,
              cancellationToken: cancellationToken
          );
            if (userSessions == null || userSessions.Items == null || !userSessions.Items.Any())
            {
                return new GetListResponse<GetListUserSessionListItemDto>
                {
                    Items = new List<GetListUserSessionListItemDto>(),
                    Index = 0,
                    Size = 0,
                    Count = 0,
                    Pages = 0
                };
            }
            GetListResponse<GetListUserSessionListItemDto> response = _mapper.Map<GetListResponse<GetListUserSessionListItemDto>>(userSessions);
            return response;
        }
    }
}
