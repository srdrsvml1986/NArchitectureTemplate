using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.Repositories;
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
        private readonly IUserRepository _userRepository;
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IOperationClaimRepository _operationClaimRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IAuthService authService,
            AuthBusinessRules authBusinessRules,
            IOperationClaimRepository operationClaimRepository,
            IUserOperationClaimRepository userOperationClaimRepository
,
            IUserGroupRepository userGroupRepository,
            IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _authService = authService;
            _authBusinessRules = authBusinessRules;
            _operationClaimRepository = operationClaimRepository;
            _userOperationClaimRepository = userOperationClaimRepository;
            _userGroupRepository = userGroupRepository;
            _userRoleRepository = userRoleRepository;
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
            User createdUser = await _userRepository.AddAsync(newUser);
            var allClaims = _operationClaimRepository.Query().ToList();

            foreach (var claim in allClaims)
            {
                if (claim.Name.Contains("Read"))
                {
                    await _userOperationClaimRepository.AddAsync(new UserOperationClaim
                    {
                        Id = new Guid(),
                        UserId = createdUser.Id,
                        OperationClaimId = claim.Id,
                        CreatedDate = DateTime.UtcNow,
                        User = createdUser
                    });
                }
            }

            await _userGroupRepository.AddAsync(new UserGroup
            {
                UserId = createdUser.Id,
                GroupId = 4
            });

            await _userRoleRepository.AddAsync(new UserRole
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
