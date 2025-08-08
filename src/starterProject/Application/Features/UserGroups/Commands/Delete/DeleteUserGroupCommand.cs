using Application.Features.UserGroups.Constants;
using Application.Features.UserGroups.Constants;
using Application.Features.UserGroups.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.UserGroups.Constants.UserGroupsOperationClaims;

namespace Application.Features.UserGroups.Commands.Delete;

public class DeleteUserGroupCommand : IRequest<DeletedUserGroupResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, UserGroupsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserGroups"];

    public class DeleteUserGroupCommandHandler : IRequestHandler<DeleteUserGroupCommand, DeletedUserGroupResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly UserGroupBusinessRules _userGroupBusinessRules;

        public DeleteUserGroupCommandHandler(IMapper mapper, IUserGroupRepository userGroupRepository,
                                         UserGroupBusinessRules userGroupBusinessRules)
        {
            _mapper = mapper;
            _userGroupRepository = userGroupRepository;
            _userGroupBusinessRules = userGroupBusinessRules;
        }

        public async Task<DeletedUserGroupResponse> Handle(DeleteUserGroupCommand request, CancellationToken cancellationToken)
        {
            UserGroup? userGroup = await _userGroupRepository.GetAsync(predicate: ug => ug.Id == request.Id, cancellationToken: cancellationToken);
            await _userGroupBusinessRules.UserGroupShouldExistWhenSelected(userGroup);

            await _userGroupRepository.DeleteAsync(userGroup!);

            DeletedUserGroupResponse response = _mapper.Map<DeletedUserGroupResponse>(userGroup);
            return response;
        }
    }
}