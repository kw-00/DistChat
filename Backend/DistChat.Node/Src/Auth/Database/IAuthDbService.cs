using DistChat.Node.Auth.Models;

namespace DistChat.Node.Auth.Database;

public interface IAuthDbService
{
    Task<RotationResult> RotateRefreshTokenAsync(
        Guid currentRefreshTokenId
    );

    Task<RefreshToken> CreateRefreshTokenAsync(Guid userId);
}

public record RotationResult;

public record RotationSuccess(
    RefreshToken OldRefreshToken,
    RefreshToken NewRefreshToken
) : RotationResult;

public record RotationFailure(
    RefreshToken? OldRefreshToken,
    RotationFailureCause FailureCause
) : RotationResult;

public enum RotationFailureCause
{
    NotFound,
    Expired,
    Reuse
}