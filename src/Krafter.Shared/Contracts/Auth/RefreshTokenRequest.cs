namespace Krafter.Shared.Contracts.Auth;

/// <summary>
/// Request model for refreshing an authentication token.
/// </summary>
public record RefreshTokenRequest(string Token, string RefreshToken);
