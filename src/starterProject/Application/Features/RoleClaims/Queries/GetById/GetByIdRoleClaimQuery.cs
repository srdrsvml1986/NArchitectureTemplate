using Application.Features.RoleClaims.Constants;
using Application.Features.RoleClaims.Rules;
using Application.Services.RoleClaims;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.RoleClaims.Constants.RoleClaimsOperationClaims;

namespace Application.Features.RoleClaims.Queries.GetById;

public class GetByIdRoleClaimQuery : IRequest<GetByIdRoleClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdRoleClaimQueryHandler : IRequestHandler<GetByIdRoleClaimQuery, GetByIdRoleClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleClaimService _roleClaimService;
        private readonly RoleClaimBusinessRules _roleClaimBusinessRules;

        public GetByIdRoleClaimQueryHandler(IMapper mapper, IRoleClaimService roleClaimService, RoleClaimBusinessRules roleClaimBusinessRules)
        {
            _mapper = mapper;
            _roleClaimService = roleClaimService;
            _roleClaimBusinessRules = roleClaimBusinessRules;
        }

        public async Task<GetByIdRoleClaimResponse> Handle(GetByIdRoleClaimQuery request, CancellationToken cancellationToken)
        {
            RoleClaim? roleClaim = await _roleClaimService.GetAsync(predicate: rc => rc.Id == request.Id, cancellationToken: cancellationToken);
            await _roleClaimBusinessRules.RoleClaimShouldExistWhenSelected(roleClaim);

            GetByIdRoleClaimResponse response = _mapper.Map<GetByIdRoleClaimResponse>(roleClaim);
            return response;
        }
    }
}