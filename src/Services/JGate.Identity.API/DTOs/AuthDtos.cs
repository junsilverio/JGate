namespace JGate.Identity.API.DTOs;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string UserId,
    string Email,
    string FullName,
    string Role);

public record RefreshTokenRequest(string RefreshToken);
