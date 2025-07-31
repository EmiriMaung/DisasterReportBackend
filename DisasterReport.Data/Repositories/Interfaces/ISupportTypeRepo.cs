using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories
{
    public interface ISupportTypeRepo
    {
        Task<List<SupportType>> GetAllAsync();
        Task<SupportType?> GetByIdAsync(int id);
        Task<SupportType> AddAsync(SupportType supportType);
        Task<SupportType?> UpdateAsync(SupportType supportType);
        Task<bool> DeleteAsync(int id);
    }
}
