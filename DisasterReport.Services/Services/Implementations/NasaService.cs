using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public interface INasaService
    {
        Task<List<EonetEvent>> GetAllDisasterEventsAsync();
    }

    public class NasaService : INasaService
    {
        private readonly HttpClient _httpClient;

        public NasaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EonetEvent>> GetAllDisasterEventsAsync()
        {
            var allEvents = new List<EonetEvent>();
            string url = "https://eonet.gsfc.nasa.gov/api/v3/events?bbox=92,9,101,28&status=open&limit=50&days=30";
            while (!string.IsNullOrEmpty(url))
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<EonetResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Events != null)
                    allEvents.AddRange(data.Events);

                url = data?.Links?.Next;
            }

            return allEvents;
        }
    }
}
