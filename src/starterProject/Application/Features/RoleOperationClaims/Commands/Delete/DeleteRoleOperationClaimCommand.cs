using Application.Features.RoleOperationClaims.Constants;
using Application.Features.RoleOperationClaims.Rules;
using Application.Services.RoleOperationClaims;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.RoleOperationClaims.Constants.RoleOperationClaims;

namespace Application.Features.RoleOperationClaims.Commands.Delete;

public class DeleteRoleOperationClaimCommand : IRequest<DeletedRoleOperationClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, Constants.RoleOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoleOperationClaims"];

    public class DeleteRoleClaimCommandHandler : IRequestHandler<DeleteRoleOperationClaimCommand, DeletedRoleOperationClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleOperationClaimService _roleClaimService;
        private readonly RoleOperationClaimBusinessRules _roleClaimBusinessRules;

        public DeleteRoleClaimCommandHandler(IMapper mapper, IRoleOperationClaimService roleClaimService,
                                         RoleOperationClaimBusinessRules roleClaimBusinessRules)
        {
            _mapper = mapper;
            _roleClaimService = roleClaimService;
            _roleClaimBusinessRules = roleClaimBusinessRules;
        }

        public async Task<DeletedRoleOperationClaimResponse> Handle(DeleteRoleOperationClaimCommand request, CancellationToken cancellationToken)
        {
            RoleOperationClaim? roleClaim = await _roleClaimService.GetAsync(predicate: rc => rc.Id == request.Id, cancellationToken: cancellationToken);
            await _roleClaimBusinessRules.RoleClaimShouldExistWhenSelected(roleClaim);

            await _roleClaimService.DeleteAsync(roleClaim!);

            DeletedRoleOperationClaimResponse response = _mapper.Map<DeletedRoleOperationClaimResponse>(roleClaim);
            return response;
        }
    }
}