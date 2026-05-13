using DistChat.Node.Domain.Chat;
using DistChat.Node.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistChat.Node.Infrastructure.Persistence.Chat;

public class MembershipConfig : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.HasKey(m => new { m.RoomId, m.UserId });

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserId);

        builder
            .HasOne<Room>()
            .WithMany()
            .HasForeignKey(m => m.RoomId);
    }
}
