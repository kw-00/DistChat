using DistChat.Node.Domain;
using DistChat.Node.Domain.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistChat.Node.Infrastructure.Persistence.Chat;

public class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder
            .HasOne<Room>()
            .WithMany()
            .HasForeignKey(m => m.RoomId);
    }
}
