using Application.Features.RoleOperationClaims.Constants;
using Application.Services.RoleOperationClaims;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using MediatR;
using static Application.Features.RoleOperationClaims.Constants.RoleOperationClaims;

namespace Application.Features.RoleOperationClaims.Queries.GetList;

public class GetListRoleClaimQuery : IRequest<GetListResponse<GetListRoleOperationClaimListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListRoleClaims({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetRoleClaims";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListRoleClaimQueryHandler : IRequestHandler<GetListRoleClaimQuery, GetListResponse<GetListRoleOperationClaimListItemDto>>
    {
        private readonly IRoleOperationClaimService _roleClaimService;
        private readonly IMapper _mapper;

        public GetListRoleClaimQueryHandler(IRoleOperationClaimService roleClaimService, IMapper mapper)
        {
            _roleClaimService = roleClaimService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListRoleOperationClaimListItemDto>> Handle(GetListRoleClaimQuery request, CancellationToken cancellationToken)
        {
            IPaginate<RoleOperationClaim> roleClaims = await _roleClaimService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListRoleOperationClaimListItemDto> response;
            if (roleClaims.Items == null || !roleClaims.Items.Any())
            {
                response = new GetListResponse<GetListRoleOperationClaimListItemDto>
                {
                    Items = new List<GetListRoleOperationClaimListItemDto>(),
                    Index = roleClaims.Index,
                    Size = roleClaims.Size,
                    Count = roleClaims.Count,
                    Pages = roleClaims.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListRoleOperationClaimListItemDto>>(roleClaims);
            }

            return response;
        }
    }
}