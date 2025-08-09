namespace NArchitectureTemplate.Core.Application.Pipelines.Caching;

public interface ICacheRemoverRequest
{
    bool BypassCache { get; }
    string? CacheKey { get; }
    string[]? CacheGroupKey { get; }
}
