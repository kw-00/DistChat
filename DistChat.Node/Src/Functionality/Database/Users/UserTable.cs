namespace DistChat.Node.Functionality.Database.Users;

public static class UserTable
{
    public const string TableName = "users";
    public static class Columns
    {
        public const string Id = "id";
        public const string Login = "login";
        public const string Email = "email";
        public const string PasswordHash = "passwordHash";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_users";
        public const string UniqueLogin = "users_login_unique";
        public const string UniqueEmail = "users_email_unique";
    }
}