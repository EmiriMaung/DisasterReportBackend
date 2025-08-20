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
        Task<EonetResponse> GetDisasterEventsAsync();
    }

    public class NasaService : INasaService
    {
        private readonly HttpClient _httpClient;

        public NasaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EonetResponse> GetDisasterEventsAsync()
        {
            var url = "https://eonet.gsfc.nasa.gov/api/v3/events?bbox=92,9,101,28&limit=20";
            // bbox = Myanmar roughly (Lon: 92–101, Lat: 9–28)

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<EonetResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data;
        }
    }
}
