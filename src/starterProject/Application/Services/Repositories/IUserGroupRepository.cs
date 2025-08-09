using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IUserGroupRepository : IAsyncRepository<UserGroup, int>, IRepository<UserGroup, int>
{
}