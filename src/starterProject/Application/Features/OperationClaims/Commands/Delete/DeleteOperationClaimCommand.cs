using Application.Features.OperationClaims.Constants;
using Application.Features.OperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.OperationClaims.Constants.OperationClaims;

namespace Application.Features.OperationClaims.Commands.Delete;

public class DeleteOperationClaimCommand : IRequest<DeleteOperationClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.OperationClaims.Delete };

    public class DeleteClaimCommandHandler : IRequestHandler<DeleteOperationClaimCommand, DeleteOperationClaimResponse>
    {
        private readonly IOperationClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly OperationClaimBusinessRules _securityClaimBusinessRules;

        public DeleteClaimCommandHandler(
            IOperationClaimRepository claimRepository,
            IMapper mapper,
            OperationClaimBusinessRules claimBusinessRules
        )
        {
            _securityClaimRepository = claimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = claimBusinessRules;
        }

        public async Task<DeleteOperationClaimResponse> Handle(
            DeleteOperationClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            OperationClaim? securityClaim = await _securityClaimRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _securityClaimBusinessRules.ClaimShouldExistWhenSelected(securityClaim);

            await _securityClaimRepository.DeleteAsync(entity: securityClaim!);

            DeleteOperationClaimResponse response = _mapper.Map<DeleteOperationClaimResponse>(securityClaim);
            return response;
        }
    }
}
