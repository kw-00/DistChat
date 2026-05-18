namespace DistChat.Node.Functionality.Database.Users;

public static class FriendRequestTable
{
    public const string TableName = "friendRequests";
    public static class Columns
    {
        public const string RequestingUserId = "requestingUserId";
        public const string TargetUserId = "targetUserId";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_friendRequests";

        public const string FkRequestingUserId = "friendRequests_fk_requestingUserId";
        public const string FkTargetUserId = "friendRequests_fk_targetUserId";

        public const string NoMutualRequests = "friendRequests_no_mutual_requests";
    }
}