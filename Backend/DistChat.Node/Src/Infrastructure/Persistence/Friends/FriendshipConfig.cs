using DistChat.Node.Domain.Friends;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistChat.Node.Infrastructure.Persistence.Friends;

public class FriendshipConfig : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(f => new { f.FirstUserId, f.SecondUserId });
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Friendship_NoSelfFriendship", "\"FirstUserId\" <> \"SecondUserId\"");
            t.HasCheckConstraint("CK_Friendship_FriendOrder", "\"FirstUserId\" < \"SecondUserId\"");
        });
    }
}
