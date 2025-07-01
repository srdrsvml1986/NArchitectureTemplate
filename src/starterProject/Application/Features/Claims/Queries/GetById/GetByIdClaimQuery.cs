using Application.Features.Claims.Constants;
using Application.Features.Claims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;

namespace Application.Features.Claims.Queries.GetById;

public class GetByIdClaimQuery : IRequest<GetByIdClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Constants.Claims.Read];

    public class GetByIdClaimQueryHandler : IRequestHandler<GetByIdClaimQuery, GetByIdClaimResponse>
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        private readonly ClaimBusinessRules _claimBusinessRules;

        public GetByIdClaimQueryHandler(
            IClaimRepository claimRepository,
            IMapper mapper,
            ClaimBusinessRules claimBusinessRules
        )
        {
            _claimRepository = claimRepository;
            _mapper = mapper;
            _claimBusinessRules = claimBusinessRules;
        }

        public async Task<GetByIdClaimResponse> Handle(
            GetByIdClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            Claim? operationClaim = await _claimRepository.GetAsync(
                predicate: b => b.Id == request.Id,
                cancellationToken: cancellationToken,
                enableTracking: false
            );
            await _claimBusinessRules.ClaimShouldExistWhenSelected(operationClaim);

            GetByIdClaimResponse response = _mapper.Map<GetByIdClaimResponse>(operationClaim);
            return response;
        }
    }
}
