using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.ExceptionLogs.Commands.Create;

public class CreatedExceptionLogResponse : IResponse
{
    public Guid Id { get; set; }
}