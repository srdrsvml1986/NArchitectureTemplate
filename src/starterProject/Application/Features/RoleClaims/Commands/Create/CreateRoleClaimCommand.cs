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

namespace Application.Features.RoleClaims.Commands.Create;

public class CreateRoleClaimCommand : IRequest<CreatedRoleClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required int ClaimId { get; set; }
    public required int RoleId { get; set; }

    public string[] Roles => [Admin, Write, RoleClaimsOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoleClaims"];

    public class CreateRoleClaimCommandHandler : IRequestHandler<CreateRoleClaimCommand, CreatedRoleClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleClaimService _roleClaimService;
        private readonly RoleClaimBusinessRules _roleClaimBusinessRules;

        public CreateRoleClaimCommandHandler(IMapper mapper, IRoleClaimService roleClaimService,
                                         RoleClaimBusinessRules roleClaimBusinessRules)
        {
            _mapper = mapper;
            _roleClaimService = roleClaimService;
            _roleClaimBusinessRules = roleClaimBusinessRules;
        }

        public async Task<CreatedRoleClaimResponse> Handle(CreateRoleClaimCommand request, CancellationToken cancellationToken)
        {
            RoleClaim roleClaim = _mapper.Map<RoleClaim>(request);

            await _roleClaimService.AddAsync(roleClaim);

            CreatedRoleClaimResponse response = _mapper.Map<CreatedRoleClaimResponse>(roleClaim);
            return response;
        }
    }
}