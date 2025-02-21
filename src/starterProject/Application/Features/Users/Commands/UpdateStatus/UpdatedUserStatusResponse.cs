using NArchitecture.Core.Application.Responses;

namespace Application.Features.Users.Commands.UpdateStatus;

public class UpdatedUserStatusResponse : IResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public bool Status { get; set; }

    public UpdatedUserStatusResponse()
    {
        Email = string.Empty;
    }

    public UpdatedUserStatusResponse(Guid id,string email, bool status)
    {
        Id = id;
        Email = email;
        Status = status;
    }
}
