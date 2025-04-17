using Application.Features.SecurityClaims.Constants;
using Application.Features.SecurityClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.SecurityClaims.Constants.SecurityClaims;

namespace Application.Features.SecurityClaims.Commands.Create;

public class CreateSecurityClaimCommand : IRequest<CreatedSecurityClaimResponse>, ISecuredRequest
{
    public string Name { get; set; }

    public CreateSecurityClaimCommand()
    {
        Name = string.Empty;
    }

    public CreateSecurityClaimCommand(string name)
    {
        Name = name;
    }

    public string[] Roles => new[] { Admin, Write, Constants.SecurityClaims.Create };

    public class CreateClaimCommandHandler : IRequestHandler<CreateSecurityClaimCommand, CreatedSecurityClaimResponse>
    {
        private readonly ISecurityClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly SecurityClaimBusinessRules _securityClaimBusinessRules;

        public CreateClaimCommandHandler(
            ISecurityClaimRepository claimRepository,
            IMapper mapper,
            SecurityClaimBusinessRules claimBusinessRules
        )
        {
            _securityClaimRepository = claimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = claimBusinessRules;
        }

        public async Task<CreatedSecurityClaimResponse> Handle(
            CreateSecurityClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            await _securityClaimBusinessRules.SecurityClaimNameShouldNotExistWhenCreating(request.Name);
            SecurityClaim mappedClaim = _mapper.Map<SecurityClaim>(request);

            SecurityClaim createdClaim = await _securityClaimRepository.AddAsync(mappedClaim);

            CreatedSecurityClaimResponse response = _mapper.Map<CreatedSecurityClaimResponse>(createdClaim);
            return response;
        }
    }
}
