using DistChat.Node.Auth.Database;
using DistChat.Node.Auth.Services;
using DistChat.Node.Functionality.Database.Users;
using DistChat.Node.Functionality.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using MyApp.Infrastructure.Services;

namespace DistChat.Node.Auth.Application;

[ApiController]
[Route("[controller]")]
public class AuthController(
    ITokenService tokenService,
    TokenCookieHandler tokenCookies,
    IPasswordService passwordService,
    IRegistrationDbService registrationDbService,
    IUserDbService userDbService,
    IEmailService emailService
) : ControllerBase
{

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var tokens = tokenCookies.GetTokenPair(HttpContext);
        if (tokens is null) return BadRequest("Missing tokens.");

        var newTokens = await tokenService.RefreshAsync(tokens.RefreshToken);
        tokenCookies.SetTokenPair(HttpContext, newTokens);
        return Ok();
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(
        [FromBody] string? login, 
        [FromBody] string? email,
        [FromBody] string password
    )
    {
        if (login is not null)
        {
            var user = await VerifyLoginPass(login, password);
            if (user is null) return BadRequest("Invalid credentials.");
            await PrepareTokenCookies(user.Id);
            return Ok(new SelfUserDTO(user));
        }
        if (email is not null)
        {
            var user = await VerifyEmailPass(email, password);
            if (user is null) return BadRequest("Invalid credentials.");
            await PrepareTokenCookies(user.Id);
            return Ok(new SelfUserDTO(user));
        }
        return BadRequest("Missing login or email.");
    }

    [HttpPost("request-registration-link")]
    public async Task<IActionResult> RequestRegistrationLink(
        [FromBody] string login, 
        [FromBody] string email, 
        [FromBody] string password
    )
    {
        var pendingRegistration 
            = await registrationDbService.CreatePendingRegistrationAsync(
                login, email, password
            );
        await emailService.SendAsync(
            email, 
            "DistChat Registration",
            $"""
            <!DOCTYPE html>
            <html>
                <head>
                    <meta charset="UTF-8">
                    <meta 
                        name="viewport" 
                        content="width=device-width, initial-scale=1.0"
                    >
                    <title>DistChat Registration</title>
                </head>
                <body>
                    <h1>DistChat Registration</h1>
                    <p>
                        Click <a href="/register/{pendingRegistration.Id}">here</a> 
                        to register.
                    </p>
                </body>
            </html>
            """
        );
        return Ok();
    }

    [HttpPost("register/{token}")]
    public async Task<IActionResult> Register(string token)
    {
        var pendingRegistrationId = Guid.Parse(token);
        var registeredUser = await registrationDbService.RegisterUserAsync(
            pendingRegistrationId
        );
        await PrepareTokenCookies(registeredUser.Id);
        return Ok(new SelfUserDTO(registeredUser));

    }

    private async Task<User?> VerifyLoginPass(string login, string password)
    {
        var user = await userDbService.GetByLoginAsync(login);
        if (user is null) return null;
        var credentialsValid = passwordService.VerifyPassword(password, user.PasswordHash);
        return credentialsValid ? user : null;
    }

    private async Task<User?> VerifyEmailPass(string email, string password)
    {
        var user = await userDbService.GetByEmailAsync(email);
        if (user is null) return null;
        var credentialsValid = passwordService.VerifyPassword(password, user.PasswordHash);
        return credentialsValid ? user : null;
    }

    private async Task PrepareTokenCookies(Guid userId)
    {
        var tokens = await tokenService.CreateTokenPairAsync(userId);
        tokenCookies.SetTokenPair(HttpContext, tokens);
    }
}