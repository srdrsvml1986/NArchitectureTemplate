using Application.Features.Claims.Constants;
using Application.Features.Claims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.Claims.Constants.Claims;

namespace Application.Features.Claims.Commands.Create;

public class CreateClaimCommand : IRequest<CreatedClaimResponse>, ISecuredRequest
{
    public string Name { get; set; }

    public CreateClaimCommand()
    {
        Name = string.Empty;
    }

    public CreateClaimCommand(string name)
    {
        Name = name;
    }

    public string[] Roles => new[] { Admin, Write, Constants.Claims.Create };

    public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, CreatedClaimResponse>
    {
        private readonly IClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly ClaimBusinessRules _securityClaimBusinessRules;

        public CreateClaimCommandHandler(
            IClaimRepository claimRepository,
            IMapper mapper,
            ClaimBusinessRules claimBusinessRules
        )
        {
            _securityClaimRepository = claimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = claimBusinessRules;
        }

        public async Task<CreatedClaimResponse> Handle(
            CreateClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            await _securityClaimBusinessRules.ClaimNameShouldNotExistWhenCreating(request.Name);
            Claim mappedClaim = _mapper.Map<Claim>(request);

            Claim createdClaim = await _securityClaimRepository.AddAsync(mappedClaim);

            CreatedClaimResponse response = _mapper.Map<CreatedClaimResponse>(createdClaim);
            return response;
        }
    }
}
