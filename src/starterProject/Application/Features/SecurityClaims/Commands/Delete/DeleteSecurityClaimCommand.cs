using Application.Features.SecurityClaims.Constants;
using Application.Features.SecurityClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.SecurityClaims.Constants.SecurityClaims;

namespace Application.Features.SecurityClaims.Commands.Delete;

public class DeleteSecurityClaimCommand : IRequest<DeleteSecurityClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.SecurityClaims.Delete };

    public class DeleteClaimCommandHandler : IRequestHandler<DeleteSecurityClaimCommand, DeleteSecurityClaimResponse>
    {
        private readonly ISecurityClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly SecurityClaimBusinessRules _securityClaimBusinessRules;

        public DeleteClaimCommandHandler(
            ISecurityClaimRepository claimRepository,
            IMapper mapper,
            SecurityClaimBusinessRules claimBusinessRules
        )
        {
            _securityClaimRepository = claimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = claimBusinessRules;
        }

        public async Task<DeleteSecurityClaimResponse> Handle(
            DeleteSecurityClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            SecurityClaim? securityClaim = await _securityClaimRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _securityClaimBusinessRules.SecurityClaimShouldExistWhenSelected(securityClaim);

            await _securityClaimRepository.DeleteAsync(entity: securityClaim!);

            DeleteSecurityClaimResponse response = _mapper.Map<DeleteSecurityClaimResponse>(securityClaim);
            return response;
        }
    }
}
