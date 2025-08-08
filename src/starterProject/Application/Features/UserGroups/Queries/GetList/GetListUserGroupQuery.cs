using Application.Features.UserGroups.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.UserGroups.Constants.UserGroupsOperationClaims;

namespace Application.Features.UserGroups.Queries.GetList;

public class GetListUserGroupQuery : IRequest<GetListResponse<GetListUserGroupListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListUserGroups({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetUserGroups";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListUserGroupQueryHandler : IRequestHandler<GetListUserGroupQuery, GetListResponse<GetListUserGroupListItemDto>>
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IMapper _mapper;

        public GetListUserGroupQueryHandler(IUserGroupRepository userGroupRepository, IMapper mapper)
        {
            _userGroupRepository = userGroupRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserGroupListItemDto>> Handle(GetListUserGroupQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserGroup> userGroups = await _userGroupRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListUserGroupListItemDto> response;
            if (userGroups.Items == null || !userGroups.Items.Any())
            {
                response = new GetListResponse<GetListUserGroupListItemDto>
                {
                    Items = new List<GetListUserGroupListItemDto>(),
                    Index = userGroups.Index,
                    Size = userGroups.Size,
                    Count = userGroups.Count,
                    Pages = userGroups.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListUserGroupListItemDto>>(userGroups);
            }

            return response;
        }
    }
}