using Application.Features.GroupOperationClaims.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.GroupOperationClaims.Constants.GroupOperationClaims;

namespace Application.Features.GroupOperationClaims.Queries.GetList;

public class GetListGroupOperationClaimQuery : IRequest<GetListResponse<GetListGroupOperationClaimListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListGroupClaims({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetGroupClaims";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListGroupClaimQueryHandler : IRequestHandler<GetListGroupOperationClaimQuery, GetListResponse<GetListGroupOperationClaimListItemDto>>
    {
        private readonly IGroupOperationClaimRepository _groupClaimRepository;
        private readonly IMapper _mapper;

        public GetListGroupClaimQueryHandler(IGroupOperationClaimRepository groupClaimRepository, IMapper mapper)
        {
            _groupClaimRepository = groupClaimRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListGroupOperationClaimListItemDto>> Handle(GetListGroupOperationClaimQuery request, CancellationToken cancellationToken)
        {
            IPaginate<GroupOperationClaim> groupClaims = await _groupClaimRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListGroupOperationClaimListItemDto> response;
            if (groupClaims.Items == null || !groupClaims.Items.Any())
            {
                response = new GetListResponse<GetListGroupOperationClaimListItemDto>
                {
                    Items = new List<GetListGroupOperationClaimListItemDto>(),
                    Index = groupClaims.Index,
                    Size = groupClaims.Size,
                    Count = groupClaims.Count,
                    Pages = groupClaims.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListGroupOperationClaimListItemDto>>(groupClaims);
            }

            return response;
        }
    }
}