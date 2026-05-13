using DistChat.Node.Domain.Friends;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistChat.Node.Infrastructure.Persistence.Friends;

public class FriendRequestConfig : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        builder.HasKey(f => new { f.FromUserId, f.ToUserId });
        builder.ToTable(t =>
        {
           t.HasCheckConstraint("CK_FriendRequest_NoSelfRequest", "\"FromUserId\" <> \"ToUserId\"");
        });
    }
}