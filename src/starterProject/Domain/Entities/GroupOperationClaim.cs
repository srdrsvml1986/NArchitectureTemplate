using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Domain.Entities;

public class GroupOperationClaim : NArchitectureTemplate.Core.Security.Entities.GroupOperationClaim<int,int,int>
{
    public virtual OperationClaim OperationClaim { get; set; }
    public virtual Group Group { get; set; }
}