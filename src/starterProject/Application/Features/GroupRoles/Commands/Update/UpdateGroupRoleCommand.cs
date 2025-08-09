using Application.Features.GroupRoles.Constants;
using Application.Features.GroupRoles.Rules;
using Application.Services.GroupRoles;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.GroupRoles.Constants.GroupRolesOperationClaims;

namespace Application.Features.GroupRoles.Commands.Update;

public class UpdateGroupRoleCommand : IRequest<UpdatedGroupRoleResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required int GroupId { get; set; }
    public required int RoleId { get; set; }

    public string[] Roles => [Admin, Write, GroupRolesOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupRoles"];

    public class UpdateGroupRoleCommandHandler : IRequestHandler<UpdateGroupRoleCommand, UpdatedGroupRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupRoleService _groupRoleService;
        private readonly GroupRoleBusinessRules _groupRoleBusinessRules;

        public UpdateGroupRoleCommandHandler(IMapper mapper, IGroupRoleService groupRoleService,
                                         GroupRoleBusinessRules groupRoleBusinessRules)
        {
            _mapper = mapper;
            _groupRoleService = groupRoleService;
            _groupRoleBusinessRules = groupRoleBusinessRules;
        }

        public async Task<UpdatedGroupRoleResponse> Handle(UpdateGroupRoleCommand request, CancellationToken cancellationToken)
        {
            GroupRole? groupRole = await _groupRoleService.GetAsync(predicate: gr => gr.Id == request.Id, cancellationToken: cancellationToken);
            await _groupRoleBusinessRules.GroupRoleShouldExistWhenSelected(groupRole);
            groupRole = _mapper.Map(request, groupRole);

            await _groupRoleService.UpdateAsync(groupRole!);

            UpdatedGroupRoleResponse response = _mapper.Map<UpdatedGroupRoleResponse>(groupRole);
            return response;
        }
    }
}