using System;

namespace CommunityEvents.Models;

public class AppTask
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public int? AssignedMemberId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EffortPoints { get; set; } = 1;
    public decimal MoneyValue { get; set; } = 0m;
    public decimal Score { get; set; } = 0m;
    public AppTaskStatus Status { get; set; } = AppTaskStatus.NotStarted;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Issue? Issue { get; set; }
    public Member? AssignedMember { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public enum AppTaskStatus { NotStarted, WorkingOn, Completed }
