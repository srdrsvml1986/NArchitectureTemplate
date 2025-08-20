using System.ComponentModel.DataAnnotations;

namespace WebAPI.Extensions;
using System.ComponentModel.DataAnnotations;

public class EmergencyTokenRequest
{
    [Required]
    public string AdminPassword { get; set; }
}

public class EmergencyAccessRequest
{
    [Required]
    public string Token { get; set; }
}

public class EmergencyNotificationRequest
{
    [Required]
    public string Message { get; set; }

    public string Severity { get; set; } = "HIGH";
}

public class SecretRequest
{
    [Required]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }
}

public class UpdateSecretRequest
{
    [Required]
    public string NewValue { get; set; }
}

public class LoginRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}