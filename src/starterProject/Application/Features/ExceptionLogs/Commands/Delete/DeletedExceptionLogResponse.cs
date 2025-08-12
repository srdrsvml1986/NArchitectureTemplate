using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.ExceptionLogs.Commands.Delete;

public class DeletedExceptionLogResponse : IResponse
{
    public Guid Id { get; set; }
}