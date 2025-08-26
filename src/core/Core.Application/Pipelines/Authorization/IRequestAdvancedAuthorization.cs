
namespace NArchitectureTemplate.Core.Application.Pipelines.Authorization;

public interface IRequestAdvancedAuthorization
{
    public string[] Roles { get; }
    public string[] Permissions { get; }
    public string[] Groups { get; }
}

