using Application.Features.RoleClaims.Constants;
using Application.Features.RoleClaims.Constants;
using Application.Features.RoleClaims.Rules;
using Application.Services.RoleClaims;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.RoleClaims.Constants.RoleClaimsOperationClaims;

namespace Application.Features.RoleClaims.Commands.Delete;

public class DeleteRoleClaimCommand : IRequest<DeletedRoleClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, RoleClaimsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoleClaims"];

    public class DeleteRoleClaimCommandHandler : IRequestHandler<DeleteRoleClaimCommand, DeletedRoleClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleClaimService _roleClaimService;
        private readonly RoleClaimBusinessRules _roleClaimBusinessRules;

        public DeleteRoleClaimCommandHandler(IMapper mapper, IRoleClaimService roleClaimService,
                                         RoleClaimBusinessRules roleClaimBusinessRules)
        {
            _mapper = mapper;
            _roleClaimService = roleClaimService;
            _roleClaimBusinessRules = roleClaimBusinessRules;
        }

        public async Task<DeletedRoleClaimResponse> Handle(DeleteRoleClaimCommand request, CancellationToken cancellationToken)
        {
            RoleClaim? roleClaim = await _roleClaimService.GetAsync(predicate: rc => rc.Id == request.Id, cancellationToken: cancellationToken);
            await _roleClaimBusinessRules.RoleClaimShouldExistWhenSelected(roleClaim);

            await _roleClaimService.DeleteAsync(roleClaim!);

            DeletedRoleClaimResponse response = _mapper.Map<DeletedRoleClaimResponse>(roleClaim);
            return response;
        }
    }
}