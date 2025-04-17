using Application.Features.SecurityClaims.Constants;
using Application.Features.SecurityClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;

namespace Application.Features.SecurityClaims.Queries.GetById;

public class GetByIdSecurityClaimQuery : IRequest<GetByIdSecurityClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Constants.SecurityClaims.Read];

    public class GetByIdClaimQueryHandler : IRequestHandler<GetByIdSecurityClaimQuery, GetByIdSecurityClaimResponse>
    {
        private readonly ISecurityClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        private readonly SecurityClaimBusinessRules _claimBusinessRules;

        public GetByIdClaimQueryHandler(
            ISecurityClaimRepository claimRepository,
            IMapper mapper,
            SecurityClaimBusinessRules claimBusinessRules
        )
        {
            _claimRepository = claimRepository;
            _mapper = mapper;
            _claimBusinessRules = claimBusinessRules;
        }

        public async Task<GetByIdSecurityClaimResponse> Handle(
            GetByIdSecurityClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            SecurityClaim? operationClaim = await _claimRepository.GetAsync(
                predicate: b => b.Id == request.Id,
                cancellationToken: cancellationToken,
                enableTracking: false
            );
            await _claimBusinessRules.SecurityClaimShouldExistWhenSelected(operationClaim);

            GetByIdSecurityClaimResponse response = _mapper.Map<GetByIdSecurityClaimResponse>(operationClaim);
            return response;
        }
    }
}
