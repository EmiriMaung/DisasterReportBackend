using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Implementations;
using DisasterReport.Services.Services.Interfaces;
using NETCore.MailKit.Core;

public class DisasterEventNasaService : IDisasterEventNasaService
{
    private readonly INasaService _nasaService;
    private readonly IDisasterEventNasaRepo _repo;
    private readonly IUserRepo _userRepo;
    private readonly IEmailServices _emailService;
    private readonly IDisasterNotificationService _notificationService;

    public DisasterEventNasaService(
        INasaService nasaService,
        IDisasterEventNasaRepo repo,
        IUserRepo userRepo,
        IEmailServices emailService,
        IDisasterNotificationService notificationService)
    {
        _nasaService = nasaService;
        _repo = repo;
        _userRepo = userRepo;
        _emailService = emailService;
        _notificationService = notificationService;
    }

    public async Task FetchAndStoreDisastersAsync()
    {
        var events = await _nasaService.GetAllDisasterEventsAsync();

        if (events == null || !events.Any())
            return;

        var apiEventIds = events.Select(e => e.Id).ToList();
        var existingEventIds = await _repo.GetExistingEventIdsAsync(apiEventIds);

        var newEvents = events
            .Where(ev => !existingEventIds.Contains(ev.Id))
            .ToList();

        if (!newEvents.Any())
            return;

        var newEventEntities = new List<DisasterEventNasa>();
        foreach (var ev in newEvents)
        {
            var newEvent = new DisasterEventNasa
            {
                EventId = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Category = ev.Categories.FirstOrDefault()?.Title,
                SourceUrl = ev.Sources.FirstOrDefault()?.Url,
                EventDate = ev.Geometry.FirstOrDefault()?.Date ?? DateTime.UtcNow,
                Latitude = ev.Geometry.FirstOrDefault()?.Coordinates?[1],
                Longitude = ev.Geometry.FirstOrDefault()?.Coordinates?[0],
                CreatedAt = DateTime.UtcNow
            };

            newEventEntities.Add(newEvent);
            await _repo.AddAsync(newEvent);
        }

        await _repo.SaveChangesAsync();

        // ✅ Push notification to ALL connected clients
        foreach (var ev in newEventEntities)
        {
            string title = $"🚨 New Disaster Alert: {ev.Title}";
            string message = $"{ev.Category} on {ev.EventDate:yyyy-MM-dd}";
            string url = ev.SourceUrl ?? "#";

            await _notificationService.NotifyAllAsync(title, message, url);
        }

        // Optional: Email sending (your existing code)
        var (users, _) = await _userRepo.GetPaginatedActiveUsersAsync(1, 1000, null, null, null);

        foreach (var ev in newEventEntities)
        {
            foreach (var user in users)
            {
                try
                {
                    string subject = $"New Disaster Alert: {ev.Title}";
                    string body = $"Hello {user.Name},<br>A new disaster has been reported:<br>" +
                                  $"<b>{ev.Title}</b><br>" +
                                  $"Category: {ev.Category}<br>" +
                                  $"Date: {ev.EventDate:yyyy-MM-dd}<br>" +
                                  $"More info: <a href='{ev.SourceUrl}'>Click Here</a>";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email to {user.Email} failed: {ex.Message}");
                }
            }
        }
    }

    public async Task<IEnumerable<GetDisasterEventsNasa>> GetDisasterEventsAsync()
    {
        var events = await _repo.GetAllAsync();

        return events.Select(e => new GetDisasterEventsNasa
        {
            Id=e.Id,
            EventId = e.EventId,
            Title = e.Title,
            Description = e.Description,
            Category = e.Category,
            SourceUrl = e.SourceUrl,
            EventDate = e.EventDate,
            Latitude = e.Latitude,
            Longitude = e.Longitude
        }).ToList();
    }
}
