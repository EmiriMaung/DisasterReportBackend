using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IDisasterEventNasaService
    {
        Task FetchAndStoreDisastersAsync();
        Task<IEnumerable<GetDisasterEventsNasa>> GetDisasterEventsAsync();
    }
}
