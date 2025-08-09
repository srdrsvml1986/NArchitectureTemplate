using Application.Features.GroupRoles.Constants;
using Application.Features.GroupRoles.Rules;
using Application.Services.GroupRoles;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.GroupRoles.Constants.GroupRolesOperationClaims;

namespace Application.Features.GroupRoles.Queries.GetById;

public class GetByIdGroupRoleQuery : IRequest<GetByIdGroupRoleResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdGroupRoleQueryHandler : IRequestHandler<GetByIdGroupRoleQuery, GetByIdGroupRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupRoleService _groupRoleService;
        private readonly GroupRoleBusinessRules _groupRoleBusinessRules;

        public GetByIdGroupRoleQueryHandler(IMapper mapper, IGroupRoleService groupRoleService, GroupRoleBusinessRules groupRoleBusinessRules)
        {
            _mapper = mapper;
            _groupRoleService = groupRoleService;
            _groupRoleBusinessRules = groupRoleBusinessRules;
        }

        public async Task<GetByIdGroupRoleResponse> Handle(GetByIdGroupRoleQuery request, CancellationToken cancellationToken)
        {
            GroupRole? groupRole = await _groupRoleService.GetAsync(predicate: gr => gr.Id == request.Id, cancellationToken: cancellationToken);
            await _groupRoleBusinessRules.GroupRoleShouldExistWhenSelected(groupRole);

            GetByIdGroupRoleResponse response = _mapper.Map<GetByIdGroupRoleResponse>(groupRole);
            return response;
        }
    }
}