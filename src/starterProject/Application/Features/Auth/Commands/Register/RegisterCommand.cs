using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.OperationClaims;
using Application.Services.UserGroups;
using Application.Services.UserOperationClaims;
using Application.Services.UserRoles;
using Application.Services.UsersService;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Dtos;
using NArchitectureTemplate.Core.Security.Hashing;
using NArchitectureTemplate.Core.Security.JWT;

namespace Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisteredResponse>
{
    public UserForRegisterDto UserForRegisterDto { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }

    public RegisterCommand()
    {
        UserForRegisterDto = null!;
        IpAddress = string.Empty;
        UserAgent = string.Empty;
    }

    public RegisterCommand(UserForRegisterDto userForRegisterDto, string ipAddress, string userAgent)
    {
        UserForRegisterDto = userForRegisterDto;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisteredResponse>
    {
        private readonly IUserService _userService;
        private readonly IUserOperationClaimService _userOperationClaimService;
        private readonly IUserGroupService _userGroupService;
        private readonly IOperationClaimService _operationClaimService;
        private readonly IUserRoleService _userRoleService;
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;

        public RegisterCommandHandler(
            IAuthService authService,
            AuthBusinessRules authBusinessRules,
            IUserService userService,
            IUserOperationClaimService userOperationClaimService,
            IUserGroupService userGroupService,
            IOperationClaimService operationClaimService,
            IUserRoleService userRoleService)
        {
            _authService = authService;
            _authBusinessRules = authBusinessRules;
            _userService = userService;
            _userOperationClaimService = userOperationClaimService;
            _userGroupService = userGroupService;
            _operationClaimService = operationClaimService;
            _userRoleService = userRoleService;
        }

        public async Task<RegisteredResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            await _authBusinessRules.UserEmailShouldBeNotExists(request.UserForRegisterDto.Email);

            HashingHelper.CreatePasswordHash(
                request.UserForRegisterDto.Password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
            );
            User newUser =
                new()
                {
                    Email = request.UserForRegisterDto.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                };
            User createdUser = await _userService.AddAsync(newUser);
            var allClaims = await _operationClaimService.GetListAsync();

            foreach (var claim in allClaims.Items)
            {
                if (claim.Name.Contains("Read"))
                {
                    await _userOperationClaimService.AddAsync(new UserOperationClaim
                    {
                        Id = new Guid(),
                        UserId = createdUser.Id,
                        OperationClaimId = claim.Id,
                        CreatedDate = DateTime.UtcNow,
                        User = createdUser
                    });
                }
            }

            await _userGroupService.AddAsync(new UserGroup
            {
                UserId = createdUser.Id,
                GroupId = 4
            });

            await _userRoleService.AddAsync(new UserRole
            {
                UserId = createdUser.Id,
                RoleId = 3
            });

            AccessToken createdAccessToken = await _authService.CreateAccessToken(createdUser);

            Domain.Entities.RefreshToken createdRefreshToken = await _authService.CreateRefreshToken(
                createdUser,
                request.IpAddress,
                request.UserAgent
            );
            Domain.Entities.RefreshToken addedRefreshToken = await _authService.AddRefreshToken(createdRefreshToken);

            RegisteredResponse registeredResponse = new() { AccessToken = createdAccessToken, RefreshToken = addedRefreshToken };
            return registeredResponse;
        }
    }
}
