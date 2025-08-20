using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public interface IDisasterEventNasaRepo
    {
        Task<bool> ExistsAsync(string eventId);
        Task AddAsync(DisasterEventNasa entity);
        Task SaveChangesAsync();
    }
}
