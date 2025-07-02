using Application.Features.RoleOperationClaims.Constants;
using Application.Features.RoleOperationClaims.Rules;
using Application.Services.RoleOperationClaims;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.RoleOperationClaims.Constants.RoleOperationClaims;

namespace Application.Features.RoleOperationClaims.Queries.GetById;

public class GetByIdRoleOperationClaimQuery : IRequest<GetByIdRoleOperationClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdRoleClaimQueryHandler : IRequestHandler<GetByIdRoleOperationClaimQuery, GetByIdRoleOperationClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleOperationClaimService _roleClaimService;
        private readonly RoleOperationClaimBusinessRules _roleClaimBusinessRules;

        public GetByIdRoleClaimQueryHandler(IMapper mapper, IRoleOperationClaimService roleClaimService, RoleOperationClaimBusinessRules roleClaimBusinessRules)
        {
            _mapper = mapper;
            _roleClaimService = roleClaimService;
            _roleClaimBusinessRules = roleClaimBusinessRules;
        }

        public async Task<GetByIdRoleOperationClaimResponse> Handle(GetByIdRoleOperationClaimQuery request, CancellationToken cancellationToken)
        {
            RoleOperationClaim? roleClaim = await _roleClaimService.GetAsync(predicate: rc => rc.Id == request.Id, cancellationToken: cancellationToken);
            await _roleClaimBusinessRules.RoleClaimShouldExistWhenSelected(roleClaim);

            GetByIdRoleOperationClaimResponse response = _mapper.Map<GetByIdRoleOperationClaimResponse>(roleClaim);
            return response;
        }
    }
}