using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Security.Hashing;
using static Application.Features.Users.Constants.UsersOperationClaims;
using static Domain.Entities.User;

namespace Application.Features.Users.Commands.UpdateStatus;

public class UpdateUserStatusCommand : IRequest<UpdatedUserStatusResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public UserStatus Status { get; set; }

    public DateTime? lastActivityDate { get; set; } = DateTime.Now;

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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public UpdateUserStatusCommandHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<UpdatedUserStatusResponse> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {            
            User? user = await _userRepository.GetAsync(
                predicate: u => u.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _userBusinessRules.UserShouldBeExistsWhenSelected(user);
            user = _mapper.Map(request, user);
            user.Status = request.Status;
            await _userRepository.UpdateAsync(user);

            UpdatedUserStatusResponse response = _mapper.Map<UpdatedUserStatusResponse>(user);
            return response;
        }
    }
}
