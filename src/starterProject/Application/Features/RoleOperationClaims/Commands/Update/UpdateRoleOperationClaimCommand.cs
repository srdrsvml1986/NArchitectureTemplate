using Application.Features.RoleOperationClaims.Constants;
using Application.Features.RoleOperationClaims.Rules;
using Application.Services.RoleOperationClaims;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.RoleOperationClaims.Constants.RoleOperationClaims;

namespace Application.Features.RoleOperationClaims.Commands.Update;

public class UpdateRoleOperationClaimCommand : IRequest<UpdatedRoleOperationClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required int OperationClaimId { get; set; }
    public required int RoleId { get; set; }

    public string[] Roles => [Admin, Write, Constants.RoleOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoleClaims"];

    public class UpdateRoleClaimCommandHandler : IRequestHandler<UpdateRoleOperationClaimCommand, UpdatedRoleOperationClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleOperationClaimService _roleClaimService;
        private readonly RoleOperationClaimBusinessRules _roleClaimBusinessRules;

        public UpdateRoleClaimCommandHandler(IMapper mapper, IRoleOperationClaimService roleClaimService,
                                         RoleOperationClaimBusinessRules roleClaimBusinessRules)
        {
            _mapper = mapper;
            _roleClaimService = roleClaimService;
            _roleClaimBusinessRules = roleClaimBusinessRules;
        }

        public async Task<UpdatedRoleOperationClaimResponse> Handle(UpdateRoleOperationClaimCommand request, CancellationToken cancellationToken)
        {
            RoleOperationClaim? roleClaim = await _roleClaimService.GetAsync(predicate: rc => rc.Id == request.Id, cancellationToken: cancellationToken);
            await _roleClaimBusinessRules.RoleClaimShouldExistWhenSelected(roleClaim);
            roleClaim = _mapper.Map(request, roleClaim);

            await _roleClaimService.UpdateAsync(roleClaim!);

            UpdatedRoleOperationClaimResponse response = _mapper.Map<UpdatedRoleOperationClaimResponse>(roleClaim);
            return response;
        }
    }
}