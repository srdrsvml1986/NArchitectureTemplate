using Application.Features.Roles.Constants;
using Application.Features.Roles.Rules;
using Application.Services.Roles;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Roles.Constants.RolesOperationClaims;

namespace Application.Features.Roles.Queries.GetById;

public class GetByIdRoleQuery : IRequest<GetByIdRoleResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdRoleQueryHandler : IRequestHandler<GetByIdRoleQuery, GetByIdRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly RoleBusinessRules _roleBusinessRules;

        public GetByIdRoleQueryHandler(IMapper mapper, IRoleService roleService, RoleBusinessRules roleBusinessRules)
        {
            _mapper = mapper;
            _roleService = roleService;
            _roleBusinessRules = roleBusinessRules;
        }

        public async Task<GetByIdRoleResponse> Handle(GetByIdRoleQuery request, CancellationToken cancellationToken)
        {
            Role? role = await _roleService.GetAsync(predicate: r => r.Id == request.Id, cancellationToken: cancellationToken);
            await _roleBusinessRules.RoleShouldExistWhenSelected(role);

            GetByIdRoleResponse response = _mapper.Map<GetByIdRoleResponse>(role);
            return response;
        }
    }
}