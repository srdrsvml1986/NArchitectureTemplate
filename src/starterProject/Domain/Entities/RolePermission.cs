using NArchitecture.Core.Persistence.Repositories;

public class RolePermission : Entity<int>
{
    public int RoleId { get; set; }
    public string PermissionName { get; set; }

    // Navigation Property
    public virtual Role Role { get; set; }
}
