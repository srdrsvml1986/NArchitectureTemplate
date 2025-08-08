using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserSessions.Commands.Delete;

public class DeletedUserSessionResponse : IResponse
{
    public Guid Id { get; set; }
}