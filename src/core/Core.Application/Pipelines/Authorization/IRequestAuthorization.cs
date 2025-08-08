
namespace NArchitecture.Core.Application.Pipelines.Authorization;

public interface IRequestAuthorization
{
    public string[] Roles { get; }
    public string[] Permissions { get; }
    public string[] Groups { get; }
}

