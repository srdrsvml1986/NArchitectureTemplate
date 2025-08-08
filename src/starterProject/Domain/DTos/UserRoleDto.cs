namespace Domain.DTos;
public class UserRoleDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public DateTime AssignedAt { get; set; }
    public string AssignedByUserName { get; set; }
}