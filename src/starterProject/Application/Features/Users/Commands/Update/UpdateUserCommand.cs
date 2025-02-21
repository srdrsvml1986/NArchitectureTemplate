using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Security.Hashing;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserCommand : IRequest<UpdatedUserResponse>, ISecuredRequest
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public bool? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? lastActivityDate { get; set; } = DateTime.Now;
    public bool Status { get; set; } = true;

    public UpdateUserCommand()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
    }

    public UpdateUserCommand(Guid id, string firstName, string lastName, string email, string password)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public UpdateUserCommand(Guid id, string? firstName, string? lastName, string email, string password,bool? gender, string? phoneNumber, string? notes, DateTime? birthDate, DateTime? lastActivityDate) : this(id, firstName, lastName, email, password)
    {
        Gender = gender;
        PhoneNumber = phoneNumber;
        Notes = notes;
        BirthDate = birthDate;
        this.lastActivityDate = lastActivityDate;
    }

    public string[] Roles => new[] { Admin, Write, UsersOperationClaims.Update };

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdatedUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<UpdatedUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {            
            User? user = await _userRepository.GetAsync(
                predicate: u => u.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _userBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(user!.Id, user.Email);
            user = _mapper.Map(request, user);

            await _userRepository.UpdateAsync(user);

            UpdatedUserResponse response = _mapper.Map<UpdatedUserResponse>(user);
            return response;
        }
    }
}
