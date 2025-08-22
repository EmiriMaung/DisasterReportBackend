using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Services.Implementations;
using DisasterReport.Services.Services.Interfaces;
using NETCore.MailKit.Core;

public class DisasterEventNasaService : IDisasterEventNasaService
{
    private readonly INasaService _nasaService;
    private readonly IDisasterEventNasaRepo _repo;
    private readonly IUserRepo _userRepo;
    private readonly IEmailServices _emailService;

    public DisasterEventNasaService(
        INasaService nasaService,
        IDisasterEventNasaRepo repo,
        IUserRepo userRepo,
       IEmailServices emailService)
    {
        _nasaService = nasaService;
        _repo = repo;
        _userRepo = userRepo;
        _emailService = emailService;
    }

    public async Task FetchAndStoreDisastersAsync()
    {
        var response = await _nasaService.GetDisasterEventsAsync();

        if (response?.Events == null || !response.Events.Any())
            return; // no events from API

        // Get all EventIds from API response
        var apiEventIds = response.Events.Select(e => e.Id).ToList();

        // Get existing EventIds from database
        var existingEventIds = await _repo.GetExistingEventIdsAsync(apiEventIds);

        // Only take events that do NOT exist yet
        var newEvents = response.Events
            .Where(ev => !existingEventIds.Contains(ev.Id))
            .ToList();

        if (!newEvents.Any())
            return; // all events already exist

        // Get all active users once
        var (users, _) = await _userRepo.GetPaginatedActiveUsersAsync(1, 1000, null, null, null);

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

            await _repo.AddAsync(newEvent);

          //  Send email to all active users about this new event
            foreach (var user in users)
            {
                string subject = $"New Disaster Alert: {newEvent.Title}";
    string body = $"Hello {user.Name},<br>A new disaster has been reported:<br>" +
                  $"<b>{newEvent.Title}</b><br>" +
                  $"Category: {newEvent.Category}<br>" +
                  $"Date: {newEvent.EventDate:yyyy-MM-dd}<br>" +
                  $"More info: <a href='{newEvent.SourceUrl}'>Click Here</a>";

    await _emailService.SendEmailAsync(user.Email, subject, body);
}
        }

        // Save all new events at once
        await _repo.SaveChangesAsync();
    }


}