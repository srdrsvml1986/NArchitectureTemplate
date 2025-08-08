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

namespace Application.Features.RoleOperationClaims.Commands.Create;

public class CreateRoleOperationClaimCommand : IRequest<CreatedRoleOperationClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required int OperationClaimId { get; set; }
    public required int RoleId { get; set; }

    public string[] Roles => [Admin, Write, Constants.RoleOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoleOperationClaims"];

    public class CreateRoleClaimCommandHandler : IRequestHandler<CreateRoleOperationClaimCommand, CreatedRoleOperationClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleOperationClaimService _roleClaimService;
        private readonly RoleOperationClaimBusinessRules _roleClaimBusinessRules;

        public CreateRoleClaimCommandHandler(IMapper mapper, IRoleOperationClaimService roleClaimService,
                                         RoleOperationClaimBusinessRules roleClaimBusinessRules)
        {
            _mapper = mapper;
            _roleClaimService = roleClaimService;
            _roleClaimBusinessRules = roleClaimBusinessRules;
        }

        public async Task<CreatedRoleOperationClaimResponse> Handle(CreateRoleOperationClaimCommand request, CancellationToken cancellationToken)
        {
            RoleOperationClaim roleClaim = _mapper.Map<RoleOperationClaim>(request);

            await _roleClaimService.AddAsync(roleClaim);

            CreatedRoleOperationClaimResponse response = _mapper.Map<CreatedRoleOperationClaimResponse>(roleClaim);
            return response;
        }
    }
}