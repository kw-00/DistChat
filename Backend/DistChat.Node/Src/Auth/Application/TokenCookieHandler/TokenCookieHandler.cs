using DistChat.Node.Auth.Services;

namespace DistChat.Node.Auth.Application;

public class TokenCookieHandler : ITokenCookieHandler
{
    public const string RefreshTokenKey = "refresh-token";
    public const string AccessTokenKey = "access-token";

    public TokenPair? GetTokenPair(HttpContext context)
    {
        context.Request.Cookies.TryGetValue(AccessTokenKey, out var accessToken);
        context.Request.Cookies.TryGetValue(RefreshTokenKey, out var refreshToken);

        if (accessToken is null || refreshToken is null) 
            return null;

        return new TokenPair(accessToken, refreshToken);
    }

    public void SetTokenPair(HttpContext context, TokenPair tokens)
    {
        context.Response.Cookies.Append(AccessTokenKey, tokens.AccessToken);
        context.Response.Cookies.Append(RefreshTokenKey, tokens.RefreshToken);
    }

    protected virtual CookieOptions GetAccessTokenOptions() => new()
    {
        Path = "/",
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        Secure = true
    };

    protected virtual CookieOptions GetRefreshTokenOptions() => new()
    {
        Path = "/auth",
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        Secure = true
    };
}