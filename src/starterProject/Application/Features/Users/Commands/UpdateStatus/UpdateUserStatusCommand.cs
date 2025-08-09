using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Security.Hashing;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.UpdateStatus;

public class UpdateUserStatusCommand : IRequest<UpdatedUserStatusResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public bool Status { get; set; }

    public DateTime? lastActivityDate { get; set; } = DateTime.Now;

    public UpdateUserStatusCommand()
    {
        Email = string.Empty;
        Status = false;
    }

    public UpdateUserStatusCommand(Guid id, string email,bool status = false)
    {
        Id = id;
        Email = email;
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
            await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(user!.Id, user.Email);
            user = _mapper.Map(request, user);

            await _userRepository.UpdateAsync(user);

            UpdatedUserStatusResponse response = _mapper.Map<UpdatedUserStatusResponse>(user);
            return response;
        }
    }
}
