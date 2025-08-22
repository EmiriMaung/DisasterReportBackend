using DisasterReport.Services.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class NasaBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NasaBackgroundService> _logger;

        public NasaBackgroundService(IServiceProvider serviceProvider, ILogger<NasaBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // loop until service stops
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var nasaService = scope.ServiceProvider.GetRequiredService<IDisasterEventNasaService>();

                        _logger.LogInformation("Fetching NASA events at: {time}", DateTime.UtcNow);

                        await nasaService.FetchAndStoreDisastersAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while fetching NASA events");
                }

                // wait 5 minutes before next call
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
