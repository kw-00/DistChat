using System.Security.Claims;

namespace DistChat.Node.Auth.Services;

public interface ITokenService
{
    ClaimsPrincipal VerifyAccessToken(string accessToken);

    Task<TokenPair> RefreshAsync(string refreshToken);

    Task<TokenPair> CreateTokenPairAsync(Guid userId);
}