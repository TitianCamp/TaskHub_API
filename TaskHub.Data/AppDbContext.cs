using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;

namespace TaskHub.Data;

public class AppDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //TeamMember Configuring
        builder.Entity<TeamMember>()
            .HasKey(tm => new { tm.UserId, tm.TeamId });

        builder.Entity<TeamMember>()
            .HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<TeamMember>()
            .HasOne(tm => tm.Team)
            .WithMany(t => t.TeamMembers)
            .HasForeignKey(tm => tm.TeamId);

        //Team Configuring
        builder.Entity<Team>()
            .HasOne(t => t.Owner)
            .WithMany(u => u.OwnedTeams)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        //RefreshToken Configuring
        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        //TaskItem Configuring
        builder.Entity<TaskItem>()
            .HasOne(ti => ti.Author)
            .WithMany(u => u.AuthoredTasks)
            .HasForeignKey(ti => ti.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<TaskItem>()
            .HasOne(ti => ti.AssignedUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(ti => ti.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TaskItem>()
            .HasIndex(ti => new {ti.Status, ti.Priority});

        base.OnModelCreating(builder);
    }
}
