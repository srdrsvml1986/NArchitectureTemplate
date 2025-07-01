using Application.Features.GroupRoles.Constants;
using Application.Services.GroupRoles;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.GroupRoles.Constants.GroupRolesOperationClaims;

namespace Application.Features.GroupRoles.Queries.GetList;

public class GetListGroupRoleQuery : IRequest<GetListResponse<GetListGroupRoleListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListGroupRoles({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetGroupRoles";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListGroupRoleQueryHandler : IRequestHandler<GetListGroupRoleQuery, GetListResponse<GetListGroupRoleListItemDto>>
    {
        private readonly IGroupRoleService _groupRoleService;
        private readonly IMapper _mapper;

        public GetListGroupRoleQueryHandler(IGroupRoleService groupRoleService, IMapper mapper)
        {
            _groupRoleService = groupRoleService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListGroupRoleListItemDto>> Handle(GetListGroupRoleQuery request, CancellationToken cancellationToken)
        {
            IPaginate<GroupRole> groupRoles = await _groupRoleService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListGroupRoleListItemDto> response;
            if (groupRoles.Items == null || !groupRoles.Items.Any())
            {
                response = new GetListResponse<GetListGroupRoleListItemDto>
                {
                    Items = new List<GetListGroupRoleListItemDto>(),
                    Index = groupRoles.Index,
                    Size = groupRoles.Size,
                    Count = groupRoles.Count,
                    Pages = groupRoles.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListGroupRoleListItemDto>>(groupRoles);
            }

            return response;
        }
    }
}