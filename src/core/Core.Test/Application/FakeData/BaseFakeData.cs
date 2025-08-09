using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Test.Application.FakeData;

public abstract class BaseFakeData<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>, new()
{
    public List<TEntity> Data => CreateFakeData();
    public abstract List<TEntity> CreateFakeData();
}
