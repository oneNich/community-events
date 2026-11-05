using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CommunityEvents.Data;
using CommunityEvents.Models;

namespace CommunityEvents.Services;

public class SeedDataService
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public SeedDataService(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        if (await _db.Events.AnyAsync()) return;

        var users = new List<IdentityUser>();
        for (int i = 1; i <= 5; i++)
        {
            var user = new IdentityUser { UserName = $"user{i}", Email = $"user{i}@example.com" };
            await _userManager.CreateAsync(user, "Password123!");
            users.Add(user);
        }
        await _db.SaveChangesAsync();

        var evt = new Event
        {
            Name = "Tech Conference 2024",
            Description = "Annual tech conference planning",
            EventDate = DateTime.Now.AddMonths(3),
            Budget = 50000m,
            InviteCode = "TECH2024"
        };
        _db.Events.Add(evt);
        await _db.SaveChangesAsync();

        var organizer = new Member
        {
            EventId = evt.Id,
            UserId = users[0].Id,
            Name = "Alice Organizer",
            Email = users[0].Email!,
            Role = MemberRole.Organizer,
            Skills = "Project Management",
            Availability = "Full-time"
        };
        _db.Members.Add(organizer);

        for (int i = 1; i < 5; i++)
        {
            _db.Members.Add(new Member
            {
                EventId = evt.Id,
                UserId = users[i].Id,
                Name = $"Member {i}",
                Email = users[i].Email!,
                Role = MemberRole.Member,
                Skills = $"Skill Set {i}",
                Availability = "Part-time"
            });
        }
        await _db.SaveChangesAsync();

        var issues = new[]
        {
            new Issue { EventId = evt.Id, Title = "Venue Setup", Description = "Book and organize venue" },
            new Issue { EventId = evt.Id, Title = "Speaker Coordination", Description = "Reach out to speakers" },
            new Issue { EventId = evt.Id, Title = "Marketing", Description = "Marketing campaign" },
            new Issue { EventId = evt.Id, Title = "Registration", Description = "Setup registration system" }
        };
        foreach (var issue in issues) _db.Issues.Add(issue);
        await _db.SaveChangesAsync();

        var taskDescs = new[] { "Call venues", "Get quotes", "Book location", "Confirm details" };
        foreach (var issue in issues)
        {
            for (int i = 0; i < 4; i++)
            {
                _db.Tasks.Add(new AppTask
                {
                    IssueId = issue.Id,
                    Title = $"{issue.Title} - {taskDescs[i]}",
                    Description = $"Task for {issue.Title}",
                    EffortPoints = (i % 5) + 1,
                    MoneyValue = (i + 1) * 100m
                });
            }
        }
        await _db.SaveChangesAsync();

        var settings = new EventSettings { EventId = evt.Id, EffortWeight = 0.5m, MoneyWeight = 0.5m };
        _db.EventSettings.Add(settings);
        await _db.SaveChangesAsync();
    }
}
