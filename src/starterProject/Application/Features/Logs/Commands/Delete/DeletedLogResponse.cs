using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.Logs.Commands.Delete;

public class DeletedLogResponse : IResponse
{
    public Guid Id { get; set; }
}