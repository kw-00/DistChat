using DistChat.Node.Auth.Services;

namespace DistChat.Node.Auth.Application;

public interface ITokenCookieHandler
{
    TokenPair? GetTokenPair(HttpContext context);

    void SetTokenPair(HttpContext context, TokenPair tokens);
}