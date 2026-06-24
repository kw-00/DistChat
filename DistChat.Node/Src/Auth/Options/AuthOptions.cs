namespace DistChat.Node.Auth.Options;

public class AuthOptions
{
    public required string AccessTokenSigningKey { get; set; }
    public required TimeSpan AccessTokenLifetime { get; set; }
    public required string AccessTokenIssuer { get; set; }
    public required string AccessTokenAudience { get; set; }
    public required TimeSpan RefreshTokenLifetime { get; set; }

    public required TimeSpan PendingRegistrationLinkLifetime { get; set; }
}