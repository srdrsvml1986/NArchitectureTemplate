using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IGroupRepository : IAsyncRepository<Group, int>, IRepository<Group, int>
{
}