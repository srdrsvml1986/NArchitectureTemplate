using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Responses;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.UpdateUserRoles;

public class UpdateUserRolesCommand : IRequest<UpdateUserRolesResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public IList<int> RoleIds { get; set; }

    public string[] Roles => [Admin, Write];

    public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, UpdateUserRolesResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _groupRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public UpdateUserRolesCommandHandler(
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository groupRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _groupRepository = groupRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<UpdateUserRolesResponse> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var existingUserRoles = await _userRoleRepository.GetListAsync(
                predicate: ug => ug.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var existingRoleIds = existingUserRoles.Items.Select(ug => ug.RoleId).ToList();

            // Yeni eklenecek gruplar
            var groupsToAdd = request.RoleIds
                .Except(existingRoleIds)
                .Select(groupId => new UserRole { UserId = request.UserId, RoleId = groupId })
                .ToList();

            // Silinecek gruplar
            var groupsToRemove = existingUserRoles.Items
                .Where(ug => !request.RoleIds.Contains(ug.RoleId))
                .ToList();

            if (groupsToAdd.Any())
                await _userRoleRepository.AddRangeAsync(groupsToAdd, cancellationToken);

            if (groupsToRemove.Any())
                await _userRoleRepository.DeleteRangeAsync(groupsToRemove);

            var updatedRoles = _groupRepository.Query().Where(g => request.RoleIds.Contains(g.Id));

            return new UpdateUserRolesResponse { Roles = updatedRoles };
        }
    }
}

public class UpdateUserRolesResponse : IResponse
{
    public IQueryable<Role>? Roles { get; set; }
}
