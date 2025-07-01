using Application.Features.GroupRoles.Constants;
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

namespace Application.Features.GroupRoles.Commands.Delete;

public class DeleteGroupRoleCommand : IRequest<DeletedGroupRoleResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, GroupRolesOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupRoles"];

    public class DeleteGroupRoleCommandHandler : IRequestHandler<DeleteGroupRoleCommand, DeletedGroupRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupRoleService _groupRoleService;
        private readonly GroupRoleBusinessRules _groupRoleBusinessRules;

        public DeleteGroupRoleCommandHandler(IMapper mapper, IGroupRoleService groupRoleService,
                                         GroupRoleBusinessRules groupRoleBusinessRules)
        {
            _mapper = mapper;
            _groupRoleService = groupRoleService;
            _groupRoleBusinessRules = groupRoleBusinessRules;
        }

        public async Task<DeletedGroupRoleResponse> Handle(DeleteGroupRoleCommand request, CancellationToken cancellationToken)
        {
            GroupRole? groupRole = await _groupRoleService.GetAsync(predicate: gr => gr.Id == request.Id, cancellationToken: cancellationToken);
            await _groupRoleBusinessRules.GroupRoleShouldExistWhenSelected(groupRole);

            await _groupRoleService.DeleteAsync(groupRole!);

            DeletedGroupRoleResponse response = _mapper.Map<DeletedGroupRoleResponse>(groupRole);
            return response;
        }
    }
}