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
        private readonly IClaimRepository _operationClaimRepository;
        private readonly IMapper _mapper;
        private readonly ClaimBusinessRules _claimBusinessRules;

        public CreateClaimCommandHandler(
            IClaimRepository claimRepository,
            IMapper mapper,
            ClaimBusinessRules claimBusinessRules
        )
        {
            _operationClaimRepository = claimRepository;
            _mapper = mapper;
            _claimBusinessRules = claimBusinessRules;
        }

        public async Task<CreatedClaimResponse> Handle(
            CreateClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            await _claimBusinessRules.OperationClaimNameShouldNotExistWhenCreating(request.Name);
            Claim mappedClaim = _mapper.Map<Claim>(request);

            Claim createdClaim = await _operationClaimRepository.AddAsync(mappedClaim);

            CreatedClaimResponse response = _mapper.Map<CreatedClaimResponse>(createdClaim);
            return response;
        }
    }
}
