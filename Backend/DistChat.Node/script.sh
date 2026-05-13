#!/usr/bin/env bash
set -euo pipefail

ROOT="./Src"

echo "== Restoring EF Core persistence layer =="

mkdir -p "$ROOT/Infrastructure/Persistence/Config"

# -------------------------
# DbContext
# -------------------------
cat > "$ROOT/Infrastructure/Persistence/DistChatDbContext.cs" << 'EOF'
using DistChat.Node.Domain;
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
        modelBuilder.ApplyConfiguration(new RoomConfig());
        modelBuilder.ApplyConfiguration(new MembershipConfig());
        modelBuilder.ApplyConfiguration(new MessageConfig());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
    }
}
EOF

# -------------------------
# UserConfig
# -------------------------
cat > "$ROOT/Infrastructure/Persistence/Config/UserConfig.cs" << 'EOF'
using DistChat.Node.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistChat.Node.Infrastructure.Persistence;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> modelBuilder)
    {
        modelBuilder.HasKey(u => u.Id);

        modelBuilder
            .HasIndex(m => m.Email)
            .IsUnique();
    }
}
EOF

# -------------------------
# RoomConfig
# -------------------------
cat > "$ROOT/Infrastructure/Persistence/Config/RoomConfig.cs" << 'EOF'
using DistChat.Node.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistChat.Node.Infrastructure.Persistence;

public class RoomConfig : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> modelBuilder)
    {
        modelBuilder.HasKey(r => r.Id);
    }
}
EOF

# -------------------------
# MessageConfig
# -------------------------
cat > "$ROOT/Infrastructure/Persistence/Config/MessageConfig.cs" << 'EOF'
using DistChat.Node.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistChat.Node.Infrastructure.Persistence;

public class MessageConfig : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> modelBuilder)
    {
        modelBuilder.HasKey(m => m.Id);

        modelBuilder
            .HasOne<Room>()
            .WithMany()
            .HasForeignKey(m => m.RoomId);
    }
}
EOF

# -------------------------
# MembershipConfig
# -------------------------
cat > "$ROOT/Infrastructure/Persistence/Config/MembershipConfig.cs" << 'EOF'
using DistChat.Node.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistChat.Node.Infrastructure.Persistence;

public class MembershipConfig : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> modelBuilder)
    {
        modelBuilder.HasKey(m => new { m.RoomId, m.UserId });

        modelBuilder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserId);

        modelBuilder
            .HasOne<Room>()
            .WithMany()
            .HasForeignKey(m => m.RoomId);
    }
}
EOF

echo "DONE"