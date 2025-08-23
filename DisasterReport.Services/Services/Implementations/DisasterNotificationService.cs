using DisasterReport.Services.Services.Interfaces;
using DisasterReport.Shared.SignalR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class DisasterNotificationService : IDisasterNotificationService
    {
        private readonly IHubContext<DisasterNotificationHub, INotificationClient> _hubContext;

        public DisasterNotificationService(IHubContext<DisasterNotificationHub, INotificationClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyAllAsync(string title, string message, string url)
        {
            // Broadcast to all connected clients
            await _hubContext.Clients.All.ReceiveDisasterAlert(title, message, url);
        }
    }
}
