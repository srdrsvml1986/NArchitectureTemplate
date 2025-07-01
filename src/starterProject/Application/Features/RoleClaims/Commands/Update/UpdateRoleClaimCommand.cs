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

namespace Application.Features.RoleClaims.Commands.Update;

public class UpdateRoleClaimCommand : IRequest<UpdatedRoleClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required int ClaimId { get; set; }
    public required int RoleId { get; set; }

    public string[] Roles => [Admin, Write, RoleClaimsOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoleClaims"];

    public class UpdateRoleClaimCommandHandler : IRequestHandler<UpdateRoleClaimCommand, UpdatedRoleClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleClaimService _roleClaimService;
        private readonly RoleClaimBusinessRules _roleClaimBusinessRules;

        public UpdateRoleClaimCommandHandler(IMapper mapper, IRoleClaimService roleClaimService,
                                         RoleClaimBusinessRules roleClaimBusinessRules)
        {
            _mapper = mapper;
            _roleClaimService = roleClaimService;
            _roleClaimBusinessRules = roleClaimBusinessRules;
        }

        public async Task<UpdatedRoleClaimResponse> Handle(UpdateRoleClaimCommand request, CancellationToken cancellationToken)
        {
            RoleClaim? roleClaim = await _roleClaimService.GetAsync(predicate: rc => rc.Id == request.Id, cancellationToken: cancellationToken);
            await _roleClaimBusinessRules.RoleClaimShouldExistWhenSelected(roleClaim);
            roleClaim = _mapper.Map(request, roleClaim);

            await _roleClaimService.UpdateAsync(roleClaim!);

            UpdatedRoleClaimResponse response = _mapper.Map<UpdatedRoleClaimResponse>(roleClaim);
            return response;
        }
    }
}