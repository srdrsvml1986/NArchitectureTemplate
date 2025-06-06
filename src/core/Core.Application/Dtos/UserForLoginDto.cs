﻿using System.Text.Json.Serialization;

namespace NArchitecture.Core.Application.Dtos;

public class UserForLoginDto : IDto
{
    public required string Email { get; set; }

    public string Password { get; set; }

    public string? AuthenticatorCode { get; set; }

    public UserForLoginDto()
    {
        Email = string.Empty;
        Password = string.Empty;
    }

    public UserForLoginDto(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
