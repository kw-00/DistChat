namespace DistChat.Node.Functionality.Database.Users;

public static class FriendshipTable
{
    public const string TableName = "friendships";
    public static class Columns
    {
        public const string UserId = "userId";
        public const string FriendId = "friendId";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_friendships";
        public const string FkUserId = "friendships_fk_userId";
        public const string FkFriendId = "friendships_fk_friendId";
    }
}