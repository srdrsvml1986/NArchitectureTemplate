using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using Application.Services.UsersService;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.Users.Constants.UsersOperationClaims;
using static Domain.Entities.User;

namespace Application.Features.Users.Commands.UpdateStatus;

public class UpdateUserStatusCommand : IRequest<UpdatedUserStatusResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    /// <summary>
    /// Kullanýcýya atanacak yeni durum. 
    /// Olasý deðerler: Active, Unverified, Inactive, Suspended, Deleted
    /// </summary>
    /// <example>Active</example>
    public UserStatus Status { get; set; }

    public UpdateUserStatusCommand()
    {
        Status = UserStatus.Inactive;
    }

    public UpdateUserStatusCommand(Guid id, string email, UserStatus status = UserStatus.Inactive)
    {
        Id = id;
        Status = status;
    }

    public string[] Roles => new[] { Admin };

    public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, UpdatedUserStatusResponse>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public UpdateUserStatusCommandHandler(IMapper mapper, UserBusinessRules userBusinessRules, IUserService userService)
        {
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _userService = userService;
        }

        public async Task<UpdatedUserStatusResponse> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {            
            User? user = await _userService.GetAsync(
                predicate: u => u.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _userBusinessRules.UserShouldBeExistsWhenSelected(user);
            user = _mapper.Map(request, user);
            user.Status = request.Status;
            await _userService.UpdateAsync(user);

            UpdatedUserStatusResponse response = _mapper.Map<UpdatedUserStatusResponse>(user);
            return response;
        }
    }
}
