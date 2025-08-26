using NArchitectureTemplate.Core.Persistence.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class DeviceToken: NArchitectureTemplate.Core.Security.Entities.DeviceToken<Guid,Guid>
{
    // Navigation property
    public virtual User User { get; set; }
}