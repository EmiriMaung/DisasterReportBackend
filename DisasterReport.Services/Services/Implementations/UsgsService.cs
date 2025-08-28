using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    // File: DisasterReport.Services.Services.Implementations/UsgsService.cs
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using DisasterReport.Services.Models;
    using DisasterReport.Services.Services.Interfaces;

    public interface IUsgsService
    {
        Task<List<UsgsEvent>> GetEarthquakeEventsAsync();
    }

    public class UsgsService : IUsgsService
    {
        private readonly HttpClient _httpClient;

        public UsgsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UsgsEvent>> GetEarthquakeEventsAsync()
        {
            // Ensure the date range is always valid
            var startTime = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");
            var endTime = DateTime.UtcNow.ToString("yyyy-MM-dd");

          
            string url = $"https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime={startTime}&endtime={endTime}&minmagnitude=2.5&minlatitude=9&maxlatitude=28&minlongitude=92&maxlongitude=101";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<UsgsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data?.Features ?? new List<UsgsEvent>();
        }
    }
}
