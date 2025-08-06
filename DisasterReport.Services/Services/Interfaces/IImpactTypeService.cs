using DisasterReport.Data.Domain;
using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IImpactTypeService
    {
        Task<List<ImpactTypeDto>> GetAllAsync();
        Task<ImpactTypeDto?> GetByIdAsync(int id);
        Task<ImpactTypeDto> AddAsync(ImpactTypeDto dto);
        Task<ImpactTypeDto?> UpdateAsync(ImpactTypeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
