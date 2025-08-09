using Application.Features.Roles.Constants;
using Application.Services.Roles;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Roles.Constants.RolesOperationClaims;

namespace Application.Features.Roles.Queries.GetList;

public class GetListRoleQuery : IRequest<GetListResponse<GetListRoleListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListRoles({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetRoles";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListRoleQueryHandler : IRequestHandler<GetListRoleQuery, GetListResponse<GetListRoleListItemDto>>
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public GetListRoleQueryHandler(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListRoleListItemDto>> Handle(GetListRoleQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Role> roles = await _roleService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListRoleListItemDto> response;
            if (roles.Items == null || !roles.Items.Any())
            {
                response = new GetListResponse<GetListRoleListItemDto>
                {
                    Items = new List<GetListRoleListItemDto>(),
                    Index = roles.Index,
                    Size = roles.Size,
                    Count = roles.Count,
                    Pages = roles.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListRoleListItemDto>>(roles);
            }

            return response;
        }
    }
}