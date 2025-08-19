using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.Logs.Queries.GetById;

public class GetByIdLogResponse : IResponse
{
    public Guid Id { get; set; }
}