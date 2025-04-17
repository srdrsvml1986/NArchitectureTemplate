using Application.Features.SecurityClaims.Constants;
using Application.Features.SecurityClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.SecurityClaims.Constants.SecurityClaims;

namespace Application.Features.SecurityClaims.Commands.Update;

public class UpdateSecurityClaimCommand : IRequest<UpdateSecurityClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }
    public string Name { get; set; }

    public UpdateSecurityClaimCommand()
    {
        Name = string.Empty;
    }

    public UpdateSecurityClaimCommand(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public string[] Roles => new[] { Admin, Write, Constants.SecurityClaims.Update };

    public class UpdateClaimCommandHandler : IRequestHandler<UpdateSecurityClaimCommand, UpdateSecurityClaimResponse>
    {
        private readonly ISecurityClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly SecurityClaimBusinessRules _securityClaimBusinessRules;

        public UpdateClaimCommandHandler(
            ISecurityClaimRepository securityClaimRepository,
            IMapper mapper,
            SecurityClaimBusinessRules securityClaimBusinessRules
        )
        {
            _securityClaimRepository = securityClaimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = securityClaimBusinessRules;
        }

        public async Task<UpdateSecurityClaimResponse> Handle(
            UpdateSecurityClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            SecurityClaim? claim = await _securityClaimRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _securityClaimBusinessRules.SecurityClaimShouldExistWhenSelected(claim);
            await _securityClaimBusinessRules.SecurityClaimNameShouldNotExistWhenUpdating(request.Id, request.Name);
            SecurityClaim mappedClaim = _mapper.Map(request, destination: claim!);

            SecurityClaim updatedClaim = await _securityClaimRepository.UpdateAsync(mappedClaim);

            UpdateSecurityClaimResponse response = _mapper.Map<UpdateSecurityClaimResponse>(updatedClaim);
            return response;
        }
    }
}
