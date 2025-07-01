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

namespace Application.Features.Roles.Commands.Create;

public class CreateRoleCommand : IRequest<CreatedRoleResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }

    public string[] Roles => [Admin, Write, RolesOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetRoles"];

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CreatedRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly RoleBusinessRules _roleBusinessRules;

        public CreateRoleCommandHandler(IMapper mapper, IRoleService roleService,
                                         RoleBusinessRules roleBusinessRules)
        {
            _mapper = mapper;
            _roleService = roleService;
            _roleBusinessRules = roleBusinessRules;
        }

        public async Task<CreatedRoleResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            Role role = _mapper.Map<Role>(request);

            await _roleService.AddAsync(role);

            CreatedRoleResponse response = _mapper.Map<CreatedRoleResponse>(role);
            return response;
        }
    }
}