using Application.Features.Roles.Constants;
using Application.Features.Roles.Constants;
using Application.Features.Roles.Rules;
using Application.Services.Roles;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Roles.Constants.RolesOperationClaims;

namespace Application.Features.Roles.Commands.Delete;

public class DeleteRoleCommand : IRequest<DeletedRoleResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, RolesOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoles"];

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, DeletedRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly RoleBusinessRules _roleBusinessRules;

        public DeleteRoleCommandHandler(IMapper mapper, IRoleService roleService,
                                         RoleBusinessRules roleBusinessRules)
        {
            _mapper = mapper;
            _roleService = roleService;
            _roleBusinessRules = roleBusinessRules;
        }

        public async Task<DeletedRoleResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            Role? role = await _roleService.GetAsync(predicate: r => r.Id == request.Id, cancellationToken: cancellationToken);
            await _roleBusinessRules.RoleShouldExistWhenSelected(role);

            await _roleService.DeleteAsync(role!);

            DeletedRoleResponse response = _mapper.Map<DeletedRoleResponse>(role);
            return response;
        }
    }
}