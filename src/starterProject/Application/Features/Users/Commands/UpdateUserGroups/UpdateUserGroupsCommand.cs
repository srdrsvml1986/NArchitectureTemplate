using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Responses;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.UpdateUserGroups;

public class UpdateUserGroupsCommand : IRequest<UpdateUserGroupsResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public IList<int> GroupIds { get; set; }

    public string[] Roles => [Admin, Write];

    public class UpdateUserGroupsCommandHandler : IRequestHandler<UpdateUserGroupsCommand, UpdateUserGroupsResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public UpdateUserGroupsCommandHandler(
            IUserRepository userRepository,
            IUserGroupRepository userGroupRepository,
            IGroupRepository groupRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _groupRepository = groupRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<UpdateUserGroupsResponse> Handle(UpdateUserGroupsCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var existingUserGroups = await _userGroupRepository.GetListAsync(
                predicate: ug => ug.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var existingGroupIds = existingUserGroups.Items.Select(ug => ug.GroupId).ToList();

            // Yeni eklenecek gruplar
            var groupsToAdd = request.GroupIds
                .Except(existingGroupIds)
                .Select(groupId => new UserGroup { UserId = request.UserId, GroupId = groupId })
                .ToList();

            // Silinecek gruplar
            var groupsToRemove = existingUserGroups.Items
                .Where(ug => !request.GroupIds.Contains(ug.GroupId))
                .ToList();

            if (groupsToAdd.Any())
                await _userGroupRepository.AddRangeAsync(groupsToAdd, cancellationToken);

            if (groupsToRemove.Any())
                await _userGroupRepository.DeleteRangeAsync(groupsToRemove);

            var updatedGroups = _groupRepository.Query().Where(g => request.GroupIds.Contains(g.Id));

            return new UpdateUserGroupsResponse { Groups = updatedGroups };
        }
    }
}

public class UpdateUserGroupsResponse : IResponse
{
    public IQueryable<Group>? Groups { get; set; }
}
