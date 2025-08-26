using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public partial class User : NArchitectureTemplate.Core.Security.Entities.User<Guid>
{
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = default!;
    public virtual ICollection<UserGroup> UserGroups { get; set; } = default!;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!;
    public virtual ICollection<OtpAuthenticator> OtpAuthenticators { get; set; } = default!;
    public virtual ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = default!;
    // Navigation Properties
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<UserSession> UserSessions { get; set; }
    public virtual ICollection<Log> Logs { get; set; } = default!;
    public virtual ICollection<ExceptionLog> ExceptionLogs { get; set; } = default!;
    public virtual ICollection<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();


    // Önerilen
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>
    /// Kullanıcı durumunu belirten enum
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// Kullanıcı aktif ve hesabını kullanabilir durumda
        /// </summary>
        Active,

        /// <summary>
        /// Kullanıcı e-posta doğrulamasını tamamlamamış
        /// </summary>
        Unverified,

        /// <summary>
        /// Kullanıcı pasif durumda (geçici olarak devre dışı)
        /// </summary>
        Inactive,

        /// <summary>
        /// Kullanıcı yasaklanmış durumda
        /// </summary>
        Suspended,

        /// <summary>
        /// Kullanıcı hesabı silinmiş
        /// </summary>
        Deleted
    }

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
