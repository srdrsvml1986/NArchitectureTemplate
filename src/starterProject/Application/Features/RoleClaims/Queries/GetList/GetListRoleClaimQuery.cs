using Application.Features.RoleClaims.Constants;
using Application.Services.RoleClaims;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.RoleClaims.Constants.RoleClaimsOperationClaims;

namespace Application.Features.RoleClaims.Queries.GetList;

public class GetListRoleClaimQuery : IRequest<GetListResponse<GetListRoleClaimListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListRoleClaims({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetRoleClaims";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListRoleClaimQueryHandler : IRequestHandler<GetListRoleClaimQuery, GetListResponse<GetListRoleClaimListItemDto>>
    {
        private readonly IRoleClaimService _roleClaimService;
        private readonly IMapper _mapper;

        public GetListRoleClaimQueryHandler(IRoleClaimService roleClaimService, IMapper mapper)
        {
            _roleClaimService = roleClaimService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListRoleClaimListItemDto>> Handle(GetListRoleClaimQuery request, CancellationToken cancellationToken)
        {
            IPaginate<RoleClaim> roleClaims = await _roleClaimService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListRoleClaimListItemDto> response;
            if (roleClaims.Items == null || !roleClaims.Items.Any())
            {
                response = new GetListResponse<GetListRoleClaimListItemDto>
                {
                    Items = new List<GetListRoleClaimListItemDto>(),
                    Index = roleClaims.Index,
                    Size = roleClaims.Size,
                    Count = roleClaims.Count,
                    Pages = roleClaims.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListRoleClaimListItemDto>>(roleClaims);
            }

            return response;
        }
    }
}