using Application.Features.Roles.Constants;
using Application.Features.Roles.Rules;
using Application.Services.Roles;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Roles.Constants.RolesOperationClaims;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleCommand : IRequest<UpdatedRoleResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    public string[] Roles => [Admin, Write, RolesOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoles"];

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, UpdatedRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly RoleBusinessRules _roleBusinessRules;

        public UpdateRoleCommandHandler(IMapper mapper, IRoleService roleService,
                                         RoleBusinessRules roleBusinessRules)
        {
            _mapper = mapper;
            _roleService = roleService;
            _roleBusinessRules = roleBusinessRules;
        }

        public async Task<UpdatedRoleResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            Role? role = await _roleService.GetAsync(predicate: r => r.Id == request.Id, cancellationToken: cancellationToken);
            await _roleBusinessRules.RoleShouldExistWhenSelected(role);
            role = _mapper.Map(request, role);

            await _roleService.UpdateAsync(role!);

            UpdatedRoleResponse response = _mapper.Map<UpdatedRoleResponse>(role);
            return response;
        }
    }
}