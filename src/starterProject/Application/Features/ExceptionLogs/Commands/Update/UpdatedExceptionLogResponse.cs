using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.ExceptionLogs.Commands.Update;

public class UpdatedExceptionLogResponse : IResponse
{
    public Guid Id { get; set; }
}