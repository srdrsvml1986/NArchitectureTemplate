using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.Logs.Commands.Create;

public class CreatedLogResponse : IResponse
{
    public Guid Id { get; set; }
}