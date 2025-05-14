using Application.Features.SecurityClaims.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.SecurityClaims.Queries.GetList;

public class GetListSecurityClaimQuery : IRequest<GetListResponse<GetListSecurityClaimListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Constants.SecurityClaims.Read];

    public GetListSecurityClaimQuery()
    {
        PageRequest = new PageRequest { Index = 0, Size = 10 };
    }

    public GetListSecurityClaimQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListClaimQueryHandler
        : IRequestHandler<GetListSecurityClaimQuery, GetListResponse<GetListSecurityClaimListItemDto>>
    {
        private readonly ISecurityClaimRepository _claimRepository;
        private readonly IMapper _mapper;

        public GetListClaimQueryHandler(ISecurityClaimRepository claimRepository, IMapper mapper)
        {
            _claimRepository = claimRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListSecurityClaimListItemDto>> Handle(
            GetListSecurityClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<SecurityClaim> operationClaims = await _claimRepository.GetListAsync(
                index: request.PageRequest.Index,
                size: request.PageRequest.Size,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListSecurityClaimListItemDto> response = _mapper.Map<
                GetListResponse<GetListSecurityClaimListItemDto>
            >(operationClaims);
            return response;
        }
    }
}
