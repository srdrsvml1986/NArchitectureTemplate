﻿namespace NArchitecture.Core.Security.OAuth.Models;
public class ExternalAuthUser
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}
