using Application.Features.GroupRoles.Constants;
using Application.Features.GroupRoles.Rules;
using Application.Services.GroupRoles;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.GroupRoles.Constants.GroupRolesOperationClaims;

namespace Application.Features.GroupRoles.Commands.Create;

public class CreateGroupRoleCommand : IRequest<CreatedGroupRoleResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required int GroupId { get; set; }
    public required int RoleId { get; set; }

    public string[] Roles => [Admin, Write, GroupRolesOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupRoles"];

    public class CreateGroupRoleCommandHandler : IRequestHandler<CreateGroupRoleCommand, CreatedGroupRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupRoleService _groupRoleService;
        private readonly GroupRoleBusinessRules _groupRoleBusinessRules;

        public CreateGroupRoleCommandHandler(IMapper mapper, IGroupRoleService groupRoleService,
                                         GroupRoleBusinessRules groupRoleBusinessRules)
        {
            _mapper = mapper;
            _groupRoleService = groupRoleService;
            _groupRoleBusinessRules = groupRoleBusinessRules;
        }

        public async Task<CreatedGroupRoleResponse> Handle(CreateGroupRoleCommand request, CancellationToken cancellationToken)
        {
            GroupRole groupRole = _mapper.Map<GroupRole>(request);

            await _groupRoleService.AddAsync(groupRole);

            CreatedGroupRoleResponse response = _mapper.Map<CreatedGroupRoleResponse>(groupRole);
            return response;
        }
    }
}