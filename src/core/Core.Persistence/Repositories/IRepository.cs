using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using NArchitectureTemplate.Core.Persistence.Dynamic;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace NArchitectureTemplate.Core.Persistence.Repositories;

public interface IRepository<TEntity, TEntityId> : IQuery<TEntity>
    where TEntity : Entity<TEntityId>
{
    TEntity? Get(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true
    );

    IPaginate<TEntity> GetList(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true
    );

    IPaginate<TEntity> GetListByDynamic(
        DynamicQuery dynamic,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true
    );

    bool Any(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false
    );
    int Count(
      Expression<Func<TEntity, bool>>? predicate = null,
      bool withDeleted = false
    );
    TEntity Add(TEntity entity, bool enableTracking = true);

    ICollection<TEntity> AddRange(ICollection<TEntity> entities, bool enableTracking = true);

    TEntity Update(TEntity entity, bool enableTracking = true);

    ICollection<TEntity> UpdateRange(ICollection<TEntity> entities, bool enableTracking = true);

    TEntity Delete(TEntity entity, bool permanent = false);

    ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false);
}
