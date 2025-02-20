using Application.Features.Claims.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Claims.Queries.GetList;

public class GetListClaimQuery : IRequest<GetListResponse<GetListClaimListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Constants.Claims.Read];

    public GetListClaimQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListClaimQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListClaimQueryHandler
        : IRequestHandler<GetListClaimQuery, GetListResponse<GetListClaimListItemDto>>
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IMapper _mapper;

        public GetListClaimQueryHandler(IClaimRepository claimRepository, IMapper mapper)
        {
            _claimRepository = claimRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListClaimListItemDto>> Handle(
            GetListClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<Claim> operationClaims = await _claimRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListClaimListItemDto> response = _mapper.Map<
                GetListResponse<GetListClaimListItemDto>
            >(operationClaims);
            return response;
        }
    }
}
