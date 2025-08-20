using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class DisasterEventNasaService : IDisasterEventNasaService
    {
        private readonly INasaService _nasaService;
        private readonly IDisasterEventNasaRepo _repo;

        public DisasterEventNasaService(INasaService nasaService, IDisasterEventNasaRepo repo)
        {
            _nasaService = nasaService;
            _repo = repo;
        }

        public async Task FetchAndStoreDisastersAsync()
        {
            var response = await _nasaService.GetDisasterEventsAsync();

            foreach (var ev in response.Events)
            {
                // check if EventId already exists
                bool exists = await _repo.ExistsAsync(ev.Id);
                if (!exists)
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
                    await _repo.SaveChangesAsync();
                }
            }
        }
    }
}
