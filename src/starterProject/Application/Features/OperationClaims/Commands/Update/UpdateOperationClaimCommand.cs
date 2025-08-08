using Application.Features.OperationClaims.Constants;
using Application.Features.OperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.OperationClaims.Constants.OperationClaims;

namespace Application.Features.OperationClaims.Commands.Update;

public class UpdateOperationClaimCommand : IRequest<UpdateOperationClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }
    public string Name { get; set; }

    public UpdateOperationClaimCommand()
    {
        Name = string.Empty;
    }

    public UpdateOperationClaimCommand(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public string[] Roles => new[] { Admin, Write, Constants.OperationClaims.Update };

    public class UpdateClaimCommandHandler : IRequestHandler<UpdateOperationClaimCommand, UpdateOperationClaimResponse>
    {
        private readonly IOperationClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly OperationClaimBusinessRules _securityClaimBusinessRules;

        public UpdateClaimCommandHandler(
            IOperationClaimRepository securityClaimRepository,
            IMapper mapper,
            OperationClaimBusinessRules securityClaimBusinessRules
        )
        {
            _securityClaimRepository = securityClaimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = securityClaimBusinessRules;
        }

        public async Task<UpdateOperationClaimResponse> Handle(
            UpdateOperationClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            OperationClaim? claim = await _securityClaimRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _securityClaimBusinessRules.ClaimShouldExistWhenSelected(claim);
            await _securityClaimBusinessRules.ClaimNameShouldNotExistWhenUpdating(request.Id, request.Name);
            OperationClaim mappedClaim = _mapper.Map(request, destination: claim!);

            OperationClaim updatedClaim = await _securityClaimRepository.UpdateAsync(mappedClaim);

            UpdateOperationClaimResponse response = _mapper.Map<UpdateOperationClaimResponse>(updatedClaim);
            return response;
        }
    }
}
