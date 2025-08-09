using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.AddUserRoles;

public class AddUserRolesCommand : IRequest<AddUserRolesResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public IList<int> RoleIds { get; set; }

    public string[] Roles => new[] { Admin, Write, UsersOperationClaims.Create };

    public class AddUserRolesCommandHandler : IRequestHandler<AddUserRolesCommand, AddUserRolesResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public AddUserRolesCommandHandler(
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<AddUserRolesResponse> Handle(AddUserRolesCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var existingUserRoles = await _userRoleRepository.GetListAsync(
                predicate: uc => uc.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var existingRoleIds = existingUserRoles.Items.Select(uc => uc.RoleId).ToList();

            // Sadece yeni olan role'leri ekle
            var newRolesToAdd = request.RoleIds
                .Except(existingRoleIds)
                .Select(roleId => new UserRole { UserId = request.UserId, RoleId = roleId })
                .ToList();

            if (newRolesToAdd.Any())
                await _userRoleRepository.AddRangeAsync(newRolesToAdd, true, cancellationToken);

            var addedRoles = _roleRepository.Query().Where(c => newRolesToAdd.Select(nc => nc.RoleId).Contains(c.Id));

            return new AddUserRolesResponse { Roles = addedRoles };
        }
    }
}
