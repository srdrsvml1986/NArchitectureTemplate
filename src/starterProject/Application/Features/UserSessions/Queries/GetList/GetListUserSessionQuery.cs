using Application.Features.UserSessions.Constants;
using Application.Services.UserSessions;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using MediatR;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Queries.GetList;

public class GetListUserSessionQuery : IRequest<GetListResponse<GetListUserSessionListItemDto>>, ISecuredRequest, ICachableRequest
{
    public required PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListUserSessions({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetUserSessions";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListUserSessionQueryHandler : IRequestHandler<GetListUserSessionQuery, GetListResponse<GetListUserSessionListItemDto>>
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IMapper _mapper;

        public GetListUserSessionQueryHandler(IUserSessionService userSessionService, IMapper mapper)
        {
            _userSessionService = userSessionService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserSessionListItemDto>> Handle(GetListUserSessionQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserSession>? userSessions = await _userSessionService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListUserSessionListItemDto> response;
            if (userSessions == null || userSessions.Items == null || !userSessions.Items.Any())
            {
                response = new GetListResponse<GetListUserSessionListItemDto>
                {
                    Items = new List<GetListUserSessionListItemDto>(),
                    Index = 0,
                    Size = 0,
                    Count = 0,
                    Pages = 0
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListUserSessionListItemDto>>(userSessions);
            }

            return response;
        }
    }
}