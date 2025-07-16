using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Interfaces;

    public interface IImpactUrlRepo
    {
         Task<ImpactUrl?> GetByIdAsync(int id);
         Task AddAsync(ImpactUrl url);
         Task UpdateAsync(ImpactUrl url);
         Task DeleteAsync(int id);
         Task SaveChangesAsync();
    }

