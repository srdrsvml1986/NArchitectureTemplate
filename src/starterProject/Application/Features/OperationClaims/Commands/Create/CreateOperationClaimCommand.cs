using Application.Features.OperationClaims.Constants;
using Application.Features.OperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.OperationClaims.Constants.OperationClaims;

namespace Application.Features.OperationClaims.Commands.Create;

public class CreateOperationClaimCommand : IRequest<CreateOperationClaimResponse>, ISecuredRequest
{
    public string Name { get; set; }

    public CreateOperationClaimCommand()
    {
        Name = string.Empty;
    }

    public CreateOperationClaimCommand(string name)
    {
        Name = name;
    }

    public string[] Roles => new[] { Admin, Write, Constants.OperationClaims.Create };

    public class CreateClaimCommandHandler : IRequestHandler<CreateOperationClaimCommand, CreateOperationClaimResponse>
    {
        private readonly IOperationClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly OperationClaimBusinessRules _securityClaimBusinessRules;

        public CreateClaimCommandHandler(
            IOperationClaimRepository claimRepository,
            IMapper mapper,
            OperationClaimBusinessRules claimBusinessRules
        )
        {
            _securityClaimRepository = claimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = claimBusinessRules;
        }

        public async Task<CreateOperationClaimResponse> Handle(
            CreateOperationClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            await _securityClaimBusinessRules.ClaimNameShouldNotExistWhenCreating(request.Name);
            OperationClaim mappedClaim = _mapper.Map<OperationClaim>(request);

            OperationClaim createdClaim = await _securityClaimRepository.AddAsync(mappedClaim);

            CreateOperationClaimResponse response = _mapper.Map<CreateOperationClaimResponse>(createdClaim);
            return response;
        }
    }
}
