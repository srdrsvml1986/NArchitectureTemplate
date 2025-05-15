using Application.Features.GroupClaims.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.GroupClaims.Constants.GroupClaimsOperationClaims;

namespace Application.Features.GroupClaims.Queries.GetList;

public class GetListGroupClaimQuery : IRequest<GetListResponse<GetListGroupClaimListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListGroupClaims({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetGroupClaims";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListGroupClaimQueryHandler : IRequestHandler<GetListGroupClaimQuery, GetListResponse<GetListGroupClaimListItemDto>>
    {
        private readonly IGroupClaimRepository _groupClaimRepository;
        private readonly IMapper _mapper;

        public GetListGroupClaimQueryHandler(IGroupClaimRepository groupClaimRepository, IMapper mapper)
        {
            _groupClaimRepository = groupClaimRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListGroupClaimListItemDto>> Handle(GetListGroupClaimQuery request, CancellationToken cancellationToken)
        {
            IPaginate<GroupClaim> groupClaims = await _groupClaimRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListGroupClaimListItemDto> response;
            if (groupClaims.Items == null || !groupClaims.Items.Any())
            {
                response = new GetListResponse<GetListGroupClaimListItemDto>
                {
                    Items = new List<GetListGroupClaimListItemDto>(),
                    Index = groupClaims.Index,
                    Size = groupClaims.Size,
                    Count = groupClaims.Count,
                    Pages = groupClaims.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListGroupClaimListItemDto>>(groupClaims);
            }

            return response;
        }
    }
}