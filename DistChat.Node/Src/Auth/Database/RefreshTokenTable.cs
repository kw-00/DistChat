namespace DistChat.Node.Auth.Database;

public static class RefreshTokenTable
{
    public const string TableName = "refreshTokens";

    public static class Columns
    {
        public const string Id = "id";
        public const string UserId = "userId";
        public const string IsUsed = "isUsed";
        public const string ExpiresAt = "expiresAt";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_refreshTokens";
        public const string FkUserId = "refreshTokens_fk_userId";
    }
}