namespace DistChat.Node.Functionality.Database.Chat;

public static class RoleTable
{
    public const string TableName = "roles";

    public static class Columns
    {
        public const string Id = "id";
        public const string Level = "level";
    }
    public static class Role
    {
        public const string Member = "member";
        public const string Elder = "elder";
        public const string Owner = "owner";
    }
}