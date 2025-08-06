using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories
{
    public interface IImpactTypeRepo
    {
        Task<List<ImpactType>> GetAllAsync();
        Task<ImpactType?> GetByIdAsync(int id);
        Task<ImpactType> AddAsync(ImpactType impactType);
        Task<ImpactType?> UpdateAsync(ImpactType impactType);
        Task<bool> DeleteAsync(int id);
    }
}
