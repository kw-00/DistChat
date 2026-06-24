namespace DistChat.Node.Auth.Application;

public class DevTokenCookieHandler : TokenCookieHandler
{
    protected override CookieOptions GetAccessTokenOptions()
    {
        return DisableSecure(base.GetAccessTokenOptions());
    }

    protected override CookieOptions GetRefreshTokenOptions()
    {
        return DisableSecure(base.GetRefreshTokenOptions());
    }

    private static CookieOptions DisableSecure(CookieOptions options)
    {
        options.Secure = false;
        return options;
    }
}