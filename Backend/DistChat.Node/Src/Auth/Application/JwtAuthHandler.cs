using System.Text.Encodings.Web;
using DistChat.Node.Auth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DistChat.Node.Auth.Application;

public class JwtAuthHandler<TOptions>(
    ITokenService tokenService,
    ITokenCookieHandler cookie,
    IOptionsMonitor<TOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
)
    : AuthenticationHandler<TOptions>(options, logger, encoder) 
    where TOptions : AuthenticationSchemeOptions, new()
{

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var tokens = cookie.GetTokenPair(Context);
        if (tokens is null) return AuthenticateResult.NoResult();
        try
        {
            var principal = tokenService.VerifyAccessToken(tokens.AccessToken);
            return AuthenticateResult
                .Success(new AuthenticationTicket(principal, Scheme.Name));
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex);
        }
    }
}
