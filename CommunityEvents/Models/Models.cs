using System;

namespace CommunityEvents.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal Budget { get; set; }
    public string InviteCode { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Member> Members { get; set; } = new List<Member>();
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public EventSettings? Settings { get; set; }
}

public class Member
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string Availability { get; set; } = string.Empty;
    public MemberRole Role { get; set; } = MemberRole.Member;

    public Event? Event { get; set; }
    public Microsoft.AspNetCore.Identity.IdentityUser? User { get; set; }
    public ICollection<AppTask> Tasks { get; set; } = new List<AppTask>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public enum MemberRole { Organizer, Member }

public class Issue
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueStatus Status { get; set; } = IssueStatus.NotStarted;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Event? Event { get; set; }
    public ICollection<AppTask> Tasks { get; set; } = new List<AppTask>();
}

public enum IssueStatus { NotStarted, WorkingOn, Completed }

public class Comment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int AuthorId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppTask? Task { get; set; }
    public Member? Author { get; set; }
    public Comment? ParentComment { get; set; }
}

public class EventSettings
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public decimal EffortWeight { get; set; } = 0.5m;
    public decimal MoneyWeight { get; set; } = 0.5m;

    public Event? Event { get; set; }
}

public class ActivityLog
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int? RelatedMemberId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Event? Event { get; set; }
    public Member? RelatedMember { get; set; }
}
