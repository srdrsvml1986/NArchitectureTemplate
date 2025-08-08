using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IUserGroupRepository : IAsyncRepository<UserGroup, int>, IRepository<UserGroup, int>
{
}