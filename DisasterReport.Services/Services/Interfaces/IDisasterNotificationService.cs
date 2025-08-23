using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IDisasterNotificationService
    {
        Task NotifyAllAsync(string title, string message, string url);
    }
}
