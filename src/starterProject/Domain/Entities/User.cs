using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User : NArchitecture.Core.Security.Entities.User<Guid>
{
    public virtual ICollection<UserClaim> UserClaims { get; set; } = default!;
    public virtual ICollection<UserGroup> UserGroups { get; set; } = default!;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!;
    public virtual ICollection<OtpAuthenticator> OtpAuthenticators { get; set; } = default!;
    public virtual ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = default!;
    public bool Status { get; set; } = true;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Display(Name = "Full Name")]
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }
    public string? PhotoURL { get; set; }
    public bool? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? lastActivityDate { get; set; } = DateTime.Now;
    public string? ExternalAuthProvider { get; set; }
    public string? ExternalAuthId { get; set; }
}
