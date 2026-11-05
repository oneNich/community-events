using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CommunityEvents.Models;

namespace CommunityEvents.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<AppTask> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public EventSettings? Settings { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }

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
