using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NArchitectureTemplate.Core.Application.Dtos;

public class UserForLoginDto : IDto
{
    /// <summary>
    /// Kullanıcı e-posta adresi
    /// </summary>
    /// <example>user@example.com</example>
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// Kullanıcı şifresi
    /// </summary>
    /// <example>Password123!</example>
    [Required]
    public required string Password { get; set; }

    /// <summary>
    /// İki faktörlü kimlik doğrulama kodu (eğer etkinse)
    /// </summary>
    /// <example>123456</example>
    public string? AuthenticatorCode { get; set; }
}
