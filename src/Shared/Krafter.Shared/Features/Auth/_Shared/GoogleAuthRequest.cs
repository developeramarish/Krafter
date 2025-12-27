namespace Krafter.Shared.Features.Auth;

/// <summary>
/// Request model for Google OAuth authentication.
/// </summary>
public class GoogleAuthRequest
{
    public string Code { get; set; } = string.Empty;
}
