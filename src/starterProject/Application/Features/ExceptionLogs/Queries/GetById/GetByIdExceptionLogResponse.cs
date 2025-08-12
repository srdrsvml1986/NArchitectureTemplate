using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.ExceptionLogs.Queries.GetById;

public class GetByIdExceptionLogResponse : IResponse
{
    public Guid Id { get; set; }
}