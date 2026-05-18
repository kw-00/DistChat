namespace DistChat.Node.Functionality.Database.Chat;

public static class RoomTable
{
    public const string TableName = "rooms";

    public static class Columns
    {
        public const string Id = "id";
        public const string Name = "name";
        public const string Type = "type";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_rooms";
        public const string RoomTypeCheck = "rooms_type_check";
    }    
}