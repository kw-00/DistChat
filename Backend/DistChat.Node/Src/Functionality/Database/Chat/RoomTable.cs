namespace DistChat.Node.Functionality.Database.Chat;

public static class RoomTable
{
    public const string TableName = "rooms";

    public static class Columns
    {
        public const string Id = "id";
        public const string Name = "name";
        public const string Type = "type";
        public const string DmUserHighId = "dmUserHighId";
        public const string DmUserLowId = "dmUserLowId";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_rooms";
        public const string RoomTypeCheck = "rooms_type_check";
        public const string UniqueDms = "rooms_unique_dms";
        public const string FkDmUserHighId = "rooms_fk_userHighId";
        public const string FkDmUserLowId = "rooms_fk_userLowId";
        public const string DmUserOrder = "rooms_dm_user_order";
    }

    public static class Type
    {
        public const string Group = "group";
        public const string Dm = "dm";
    }
}