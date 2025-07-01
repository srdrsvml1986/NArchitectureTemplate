using Application.Features.Claims.Constants;
using Application.Features.Claims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.Claims.Constants.Claims;

namespace Application.Features.Claims.Commands.Delete;

public class DeleteClaimCommand : IRequest<DeleteClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.Claims.Delete };

    public class DeleteClaimCommandHandler : IRequestHandler<DeleteClaimCommand, DeleteClaimResponse>
    {
        private readonly IClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly ClaimBusinessRules _securityClaimBusinessRules;

        public DeleteClaimCommandHandler(
            IClaimRepository claimRepository,
            IMapper mapper,
            ClaimBusinessRules claimBusinessRules
        )
        {
            _securityClaimRepository = claimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = claimBusinessRules;
        }

        public async Task<DeleteClaimResponse> Handle(
            DeleteClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            Claim? securityClaim = await _securityClaimRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _securityClaimBusinessRules.ClaimShouldExistWhenSelected(securityClaim);

            await _securityClaimRepository.DeleteAsync(entity: securityClaim!);

            DeleteClaimResponse response = _mapper.Map<DeleteClaimResponse>(securityClaim);
            return response;
        }
    }
}
