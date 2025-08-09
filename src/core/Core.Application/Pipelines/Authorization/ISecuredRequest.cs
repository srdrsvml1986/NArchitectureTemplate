namespace NArchitectureTemplate.Core.Application.Pipelines.Authorization;

public interface ISecuredRequest
{
    public string[] Roles { get; }
}
