using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Implementations;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Implementations;
using DisasterReport.Services.Services.Interfaces;

public class DisasterEvenNasaService : IDisasterEventNasaService
{
    private readonly INasaService _nasaService;
    private readonly IUsgsService _usgsService; // New service dependency
    private readonly IDisasterEventRepo _repo; // Updated repository
    private readonly IUserRepo _userRepo;
    private readonly IEmailServices _emailService;
    private readonly IDisasterNotificationService _notificationService;

    public DisasterEvenNasaService(
        INasaService nasaService,
        IUsgsService usgsService,
        IDisasterEventRepo repo,
        IUserRepo userRepo,
        IEmailServices emailService,
        IDisasterNotificationService notificationService)
    {
        _nasaService = nasaService;
        _usgsService = usgsService;
        _repo = repo;
        _userRepo = userRepo;
        _emailService = emailService;
        _notificationService = notificationService;
    }

    //public async Task FetchAndStoreDisastersAsync()
    //{
    //    // 1. Fetch data from both APIs
    //    var nasaEvents = await _nasaService.GetAllDisasterEventsAsync();
    //    var usgsEvents = await _usgsService.GetEarthquakeEventsAsync();

    //    // 2. Consolidate new events from both sources
    //    var allApiEventIds = nasaEvents.Select(e => e.Id)
    //                                   .Concat(usgsEvents.Select(e => e.Id))
    //                                   .ToList();
    //    var existingEventIds = await _repo.GetExistingEventIdsAsync(allApiEventIds);

    //    var newNasaEvents = nasaEvents.Where(ev => !existingEventIds.Contains(ev.Id)).ToList();
    //    var newUsgsEvents = usgsEvents.Where(ev => !existingEventIds.Contains(ev.Id)).ToList();

    //    if (!newNasaEvents.Any() && !newUsgsEvents.Any())
    //        return;

    //    var newEventEntities = new List<DisasterEventNasa>();

    //    // 3. Map new EONET events to the unified model
    //    foreach (var ev in newNasaEvents)
    //    {
    //        newEventEntities.Add(new DisasterEventNasa
    //        {
    //            EventId = ev.Id,
    //            Source = "EONET",
    //            Title = ev.Title,
    //            Description = ev.Description,
    //            Category = ev.Categories.FirstOrDefault()?.Title,
    //            SourceUrl = ev.Sources.FirstOrDefault()?.Url,
    //            EventDate = ev.Geometry.FirstOrDefault()?.Date ?? DateTime.UtcNow,
    //            Latitude = ev.Geometry.FirstOrDefault()?.Coordinates?[1],
    //            Longitude = ev.Geometry.FirstOrDefault()?.Coordinates?[0],
    //            CreatedAt = DateTime.UtcNow
    //        });
    //    }

    //    // 4. Map new USGS events to the unified model
    //    foreach (var ev in newUsgsEvents)
    //    {
    //        newEventEntities.Add(new DisasterEventNasa
    //        {
    //            EventId = ev.Id,
    //            Source = "USGS",
    //            Title = ev.Properties.Place,
    //            Description = ev.Properties.Place,
    //            Category = "Earthquake",
    //            SourceUrl = ev.Properties.Url,
    //            EventDate = DateTimeOffset.FromUnixTimeMilliseconds(ev.Properties.Time).UtcDateTime,
    //            Latitude = ev.Geometry.Coordinates?[1],
    //            Longitude = ev.Geometry.Coordinates?[0],
    //            CreatedAt = DateTime.UtcNow,
    //            Magnitude = ev.Properties.Mag
    //        });
    //    }

    //    if (newEventEntities.Any())
    //    {
    //        await _repo.AddRangeAsync(newEventEntities);
    //        await _repo.SaveChangesAsync();

    //        // 5. Send notifications for new events
    //        foreach (var ev in newEventEntities)
    //        {
    //            string title = $"🚨 New Disaster Alert from {ev.Source}: {ev.Title}";
    //            string message = $"{ev.Category} on {ev.EventDate:yyyy-MM-dd}";
    //            if (ev.Magnitude.HasValue)
    //            {
    //                message += $" (Magnitude: {ev.Magnitude.Value})";
    //            }
    //            string url = ev.SourceUrl ?? "#";

    //            await _notificationService.NotifyAllAsync(title, message, url);
    //        }

    //        // 6. Send emails for new events
    //        var (users, _) = await _userRepo.GetPaginatedActiveUsersAsync(1, 1000, null, null, null);
    //        foreach (var ev in newEventEntities)
    //        {
    //            foreach (var user in users)
    //            {
    //                try
    //                {
    //                    string subject = $"New Disaster Alert: {ev.Title}";
    //                    string body = $"Hello {user.Name},<br>A new disaster has been reported:<br>" +
    //                                  $"<b>{ev.Title}</b><br>" +
    //                                  $"Source: {ev.Source}<br>" +
    //                                  $"Category: {ev.Category}<br>" +
    //                                  $"Date: {ev.EventDate:yyyy-MM-dd}<br>";
    //                    if (ev.Magnitude.HasValue)
    //                    {
    //                        body += $"Magnitude: {ev.Magnitude.Value}<br>";
    //                    }
    //                    body += $"More info: <a href='{ev.SourceUrl}'>Click Here</a>";

    //                    await _emailService.SendEmailAsync(user.Email, subject, body);
    //                    await Task.Delay(500);
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine($"Email to {user.Email} failed: {ex.Message}");
    //                }
    //            }
    //        }
    //    }
    //}
    public async Task FetchAndStoreDisastersAsync()
    {
        // 1. Fetch data only from USGS
        var usgsEvents = await _usgsService.GetEarthquakeEventsAsync();

        // 2. Check existing events
        var allApiEventIds = usgsEvents.Select(e => e.Id).ToList();
        var existingEventIds = await _repo.GetExistingEventIdsAsync(allApiEventIds);

        var newUsgsEvents = usgsEvents
            .Where(ev => !existingEventIds.Contains(ev.Id))
            .ToList();

        if (!newUsgsEvents.Any())
            return;

        var newEventEntities = new List<DisasterEventNasa>();

        // 3. Map new USGS events to the unified model
        foreach (var ev in newUsgsEvents)
        {
            newEventEntities.Add(new DisasterEventNasa
            {
                EventId = ev.Id,
                Source = "USGS",
                Title = ev.Properties.Place,
                Description = ev.Properties.Place,
                Category = "Earthquake",
                SourceUrl = ev.Properties.Url,
                EventDate = DateTimeOffset.FromUnixTimeMilliseconds(ev.Properties.Time).UtcDateTime,
                Latitude = ev.Geometry.Coordinates?[1],
                Longitude = ev.Geometry.Coordinates?[0],
                CreatedAt = DateTime.UtcNow,
                Magnitude = ev.Properties.Mag
            });
        }

        // 4. Save new events
        if (newEventEntities.Any())
        {
            await _repo.AddRangeAsync(newEventEntities);
            await _repo.SaveChangesAsync();

            // 5. Send notifications for new events
            foreach (var ev in newEventEntities)
            {
                string title = $"🚨 New Earthquake Alert: {ev.Title}";
                string message = $"{ev.Category} on {ev.EventDate:yyyy-MM-dd}";
                if (ev.Magnitude.HasValue)
                {
                    message += $" (Magnitude: {ev.Magnitude.Value})";
                }
                string url = ev.SourceUrl ?? "#";

                await _notificationService.NotifyAllAsync(title, message, url);
            }

            // 6. Send emails for new events
            var (users, _) = await _userRepo.GetPaginatedActiveUsersAsync(1, 1000, null, null, null);
            foreach (var ev in newEventEntities)
            {
                foreach (var user in users)
                {
                    try
                    {
                        string subject = $"New Earthquake Alert: {ev.Title}";
                        string body = $"Hello {user.Name},<br>A new earthquake has been reported:<br>" +
                                      $"<b>{ev.Title}</b><br>" +
                                      $"Source: {ev.Source}<br>" +
                                      $"Category: {ev.Category}<br>" +
                                      $"Date: {ev.EventDate:yyyy-MM-dd}<br>";
                        if (ev.Magnitude.HasValue)
                        {
                            body += $"Magnitude: {ev.Magnitude.Value}<br>";
                        }
                        body += $"More info: <a href='{ev.SourceUrl}'>Click Here</a>";

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
    }

    public async Task<IEnumerable<GetDisasterEventsNasa>> GetDisasterEventsAsync()
    {
        var events = await _repo.GetAllAsync();
        return events.Select(e => new GetDisasterEventsNasa
        {
            Id = e.Id,
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

