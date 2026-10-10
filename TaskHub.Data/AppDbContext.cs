using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;

namespace TaskHub.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
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
        builder.Entity<TeamMember>()
            .Property(tm => tm.Role)
            .HasConversion<string>();

        //Team Configuring
        builder.Entity<Team>()
            .Property(t => t.Name)
            .HasMaxLength(128);
        builder.Entity<Team>()
            .Property(t => t.Description)
            .HasMaxLength(512);

        //RefreshToken Configuring
        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();
        builder.Entity<RefreshToken>()
            .Property(rt => rt.Token)
            .HasMaxLength(256);

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
            .HasOne(ti => ti.Team)
            .WithMany(t => t.Tasks)
            .HasForeignKey(ti => ti.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TaskItem>()
            .HasIndex(ti => new { ti.Status, ti.Priority });

        builder.Entity<TaskItem>()
            .Property(ti => ti.Status)
            .HasConversion<string>();
        builder.Entity<TaskItem>()
            .Property(ti => ti.Priority)
            .HasConversion<string>();
        builder.Entity<TaskItem>()
            .Property(ti => ti.Title)
            .HasMaxLength(256);
        builder.Entity<TaskItem>()
            .Property(ti => ti.Description)
            .HasMaxLength(1024);

        //User Configuring
        builder.Entity<User>(entity =>
        {
            entity
                .Property(u => u.FirstName)
                .HasMaxLength(50);
            entity
                .Property(u => u.LastName)
                .HasMaxLength(50);
        });

        base.OnModelCreating(builder);
    }
}