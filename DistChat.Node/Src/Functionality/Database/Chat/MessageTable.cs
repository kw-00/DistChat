namespace DistChat.Node.Functionality.Database.Chat;

public static class MessageTable
{
    public const string TableName = "messages";
    public static class Columns
    {
        public const string Id = "id";
        public const string UserId = "userId";
        public const string RoomId = "roomId";
        public const string Content = "content";
        public const string CreatedAt = "createdAt";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_messages";
        public const string FkUserId = "messages_fk_userId";
        public const string FkRoomId = "messages_fk_roomId";
    }
}