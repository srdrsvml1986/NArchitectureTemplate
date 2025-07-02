using NArchitecture.Core.Application.Responses;
using static Domain.Entities.User;

namespace Application.Features.Users.Commands.UpdateStatus;

public class UpdatedUserStatusResponse : IResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public UserStatus Status { get; set; }

    public UpdatedUserStatusResponse()
    {
        Email = string.Empty;
    }

    public UpdatedUserStatusResponse(Guid id,string email, UserStatus status)
    {
        Id = id;
        Email = email;
        Status = status;
    }
}
