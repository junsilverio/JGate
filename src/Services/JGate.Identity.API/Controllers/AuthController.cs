using JGate.Identity.API.DTOs;
using JGate.Identity.API.Models;
using JGate.Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JGate.Identity.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            return Conflict(new { message = "Email already registered." });

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        await userManager.AddToRoleAsync(user, request.Role);
        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
            return Unauthorized(new { message = "Invalid credentials." });

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials." });

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        return Ok(new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            user.Id,
            user.Email ?? string.Empty,
            $"{user.FirstName} {user.LastName}",
            roles.FirstOrDefault() ?? string.Empty));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var users = userManager.Users.Where(u => u.RefreshToken == request.RefreshToken).ToList();
        var user = users.FirstOrDefault();

        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        var roles = await userManager.GetRolesAsync(user);
        var newAccessToken = tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        return Ok(new AuthResponse(
            newAccessToken,
            newRefreshToken,
            DateTime.UtcNow.AddHours(1),
            user.Id,
            user.Email ?? string.Empty,
            $"{user.FirstName} {user.LastName}",
            roles.FirstOrDefault() ?? string.Empty));
    }
}
