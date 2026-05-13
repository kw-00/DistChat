using DistChat.Node.Domain;
using DistChat.Node.Domain.Chat;
using DistChat.Node.Domain.Users;
using DistChat.Node.Infrastructure.Persistence.Chat;
using DistChat.Node.Infrastructure.Persistence.Friends;
using DistChat.Node.Infrastructure.Persistence.Users;
using Microsoft.EntityFrameworkCore;

namespace DistChat.Node.Infrastructure.Persistence;

public class DistChatDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfig());

        modelBuilder.ApplyConfiguration(new FriendshipConfig());
        modelBuilder.ApplyConfiguration(new FriendRequestConfig());

        modelBuilder.ApplyConfiguration(new RoomConfig());
        modelBuilder.ApplyConfiguration(new MembershipConfig());
        modelBuilder.ApplyConfiguration(new MessageConfig());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
