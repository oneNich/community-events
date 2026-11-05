using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CommunityEvents.Models;

namespace CommunityEvents.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Issue> Issues { get; set; } = null!;
    public DbSet<AppTask> Tasks { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<EventSettings> EventSettings { get; set; } = null!;
    public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>()
            .HasOne(m => m.Event)
            .WithMany(e => e.Members)
            .HasForeignKey(m => m.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Issue>()
            .HasOne(i => i.Event)
            .WithMany(e => e.Issues)
            .HasForeignKey(i => i.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppTask>()
            .HasOne(t => t.Issue)
            .WithMany(i => i.Tasks)
            .HasForeignKey(t => t.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ActivityLog>()
            .HasOne(al => al.Event)
            .WithMany(e => e.ActivityLogs)
            .HasForeignKey(al => al.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
