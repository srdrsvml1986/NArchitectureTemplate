using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.Logs.Commands.Update;

public class UpdatedLogResponse : IResponse
{
    public Guid Id { get; set; }
}