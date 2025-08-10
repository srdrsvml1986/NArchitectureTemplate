using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Responses;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.AddUserGroups;

public class AddUserGroupsCommand : IRequest<AddUserGroupsResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public IList<int> GroupIds { get; set; }

    public string[] Roles => [Admin, Write];

    public class AddUserGroupsCommandHandler : IRequestHandler<AddUserGroupsCommand, AddUserGroupsResponse>
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly UserBusinessRules _userBusinessRules;

        public AddUserGroupsCommandHandler(
            IUserGroupRepository userGroupRepository,
            IGroupRepository groupRepository,
            UserBusinessRules userBusinessRules)
        {
            _userGroupRepository = userGroupRepository;
            _groupRepository = groupRepository;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<AddUserGroupsResponse> Handle(AddUserGroupsCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var existingUserGroups = await _userGroupRepository.GetListAsync(
                predicate: ug => ug.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var existingGroupIds = existingUserGroups.Items.Select(ug => ug.GroupId).ToList();

            var newGroupsToAdd = request.GroupIds
                .Except(existingGroupIds)
                .Select(groupId => new UserGroup { UserId = request.UserId, GroupId = groupId })
                .ToList();

            if (newGroupsToAdd.Any())
                await _userGroupRepository.AddRangeAsync(newGroupsToAdd, true, cancellationToken);

            var addedGroups = _groupRepository.Query().Where(g => newGroupsToAdd.Select(ng => ng.GroupId).Contains(g.Id));

            return new AddUserGroupsResponse { Groups = addedGroups };
        }
    }
}

public class AddUserGroupsResponse : IResponse
{
    public IQueryable<Group>? Groups { get; set; }
}
