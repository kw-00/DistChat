using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DistChat.Node.Functionality.Database.Chat;

public class MembershipTable
{
    public const string TableName = "memberships";
    public static class Columns
    {
        public const string RoomId = "roomId";
        public const string UserId = "userId";
        public const string Role = "role";
    }

    public static class Constraints
    {
        public const string PrimaryKey = "pk_memberships";
        public const string FkMembershipUserId = "memberships_fk_userId";
        public const string FkMembershipRoomId = "memberships_fk_roomId";
        public const string MembershipRoleCheck = "memberships_role_check";
    }
}