using Microsoft.EntityFrameworkCore;
using CommunityEvents.Data;
using CommunityEvents.Models;

namespace CommunityEvents.Services;

public class EventService
{
    private readonly AppDbContext _db;

    public EventService(AppDbContext db) => _db = db;

    public async Task<Event?> GetEventAsync(int id) =>
        await _db.Events.Include(e => e.Members).Include(e => e.Settings)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Event> CreateEventAsync(Event evt, Member organizer)
    {
        evt.Settings = new EventSettings { EffortWeight = 0.5m, MoneyWeight = 0.5m };
        _db.Events.Add(evt);
        await _db.SaveChangesAsync();

        organizer.EventId = evt.Id;
        organizer.Role = MemberRole.Organizer;
        _db.Members.Add(organizer);
        await _db.SaveChangesAsync();

        return evt;
    }

    public async Task UpdateEventAsync(Event evt)
    {
        _db.Events.Update(evt);
        await _db.SaveChangesAsync();
    }

    public async Task<Event?> JoinEventAsync(int eventId, string inviteCode, Member member)
    {
        var evt = await _db.Events.FirstOrDefaultAsync(e => e.Id == eventId && e.InviteCode == inviteCode);
        if (evt == null) return null;

        member.EventId = eventId;
        member.Role = MemberRole.Member;
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        return evt;
    }

    public async Task UpdateSettingsAsync(int eventId, decimal effortWeight, decimal moneyWeight)
    {
        var settings = await _db.EventSettings.FirstOrDefaultAsync(es => es.EventId == eventId);
        if (settings == null)
        {
            settings = new EventSettings { EventId = eventId, EffortWeight = effortWeight, MoneyWeight = moneyWeight };
            _db.EventSettings.Add(settings);
        }
        else
        {
            settings.EffortWeight = effortWeight;
            settings.MoneyWeight = moneyWeight;
        }
        await _db.SaveChangesAsync();
    }
}
