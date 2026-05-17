using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DistChat.Node.Auth.Options;
using DistChat.Node.Auth.Database;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DistChat.Node.Auth.Services;

public class TokenService : ITokenService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly TokenValidationParameters _validationParameters;
    private readonly SigningCredentials _signingCredentials;
    private readonly IAuthDbService _authDbService;
    private readonly IOptions<AuthOptions> _options;


    public TokenService(
        IAuthDbService authDbService,
        IOptions<AuthOptions> options
    )
    {
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.Value.AccessTokenSigningKey)
        );
        _signingCredentials = new SigningCredentials(
            signingKey, SecurityAlgorithms.HmacSha256
        );
        _validationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey
        };

        _authDbService = authDbService;
        _options = options;
    }

    public ClaimsPrincipal VerifyAccessToken(string accessToken)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = _tokenHandler
                .ValidateToken(accessToken, _validationParameters, out _);
        }
        catch (Exception ex)
        {
            throw new InvalidAccessTokenException("Access token failed validation.", ex);
        }

        try
        {
            Guid.Parse(
                principal
                    .Claims
                    .First(c => c.Type == JwtRegisteredClaimNames.Sub)
                    .Value
            );
        }
        catch (Exception ex)
        {
            throw new InvalidAccessTokenException(
                "Access token subject is not a valid Guid.",
                ex
            );
        }
        return principal;
    }

    public async Task<TokenPair> RefreshAsync(string refreshToken)
    {
        var tokenFormatValid = Guid.TryParse(refreshToken, out var tokenId);
        if (!tokenFormatValid) 
            throw new InvalidRefreshTokenException("Refresh token is not a valid Guid.");

        var rotationResult = await _authDbService.RotateRefreshTokenAsync(tokenId);
        if (rotationResult is RotationFailure failure)
        {
            string errorMessage;
            errorMessage = failure.FailureCause switch
            {
                RotationFailureCause.Expired => "Refresh token is expired.",
                RotationFailureCause.NotFound => "Refresh token not found.",
                RotationFailureCause.Reuse => "Refresh token has already been used.",
                _ => "Refresh token is invalid."
            };
            throw new InvalidRefreshTokenException(errorMessage);
        }

        if (rotationResult is RotationSuccess successResult)
        {
            var newRefreshTokenModel = successResult.NewRefreshToken;
            return new TokenPair(
                CreateAccessToken(newRefreshTokenModel.UserId), 
                newRefreshTokenModel.Id.ToString()
            );
        }
        throw new InvalidRefreshTokenException("Refresh token is invalid.");
    }

    public async Task<TokenPair> CreateTokenPairAsync(Guid userId)
    {
        var refreshToken = await _authDbService.CreateRefreshTokenAsync(userId);
        return new TokenPair(CreateAccessToken(userId), refreshToken.Id.ToString());
    } 

    private string CreateAccessToken(Guid userId)
    {
        var identity = new ClaimsIdentity([
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())
        ]);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Issuer = _options.Value.AccessTokenIssuer,
            Audience = _options.Value.AccessTokenAudience,
            Expires = DateTime.UtcNow + _options.Value.AccessTokenLifetime,
            SigningCredentials = _signingCredentials
        };
        return _tokenHandler.CreateEncodedJwt(descriptor);
    }
}