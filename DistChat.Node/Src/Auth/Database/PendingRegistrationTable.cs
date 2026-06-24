namespace DistChat.Node.Auth.Database;

public static class PendingRegistrationTable
{
    public const string TableName = "pendingRegistrations";

    public static class Columns
    {
        public const string Id = "id";
        public const string Login = "login";
        public const string Email = "email";
        public const string PasswordHash = "passwordHash";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_pendingRegistrations";
        public const string UniqueLogin = "pendingRegistrations_login_unique";
        public const string UniqueEmail = "pendingRegistrations_email_unique";
    }
}