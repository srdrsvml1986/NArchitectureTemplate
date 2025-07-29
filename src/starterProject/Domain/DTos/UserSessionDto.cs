using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTos;
public class UserSessionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime LoginTime { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsSuspicious { get; set; }
    public string? LocationInfo { get; set; }
}
