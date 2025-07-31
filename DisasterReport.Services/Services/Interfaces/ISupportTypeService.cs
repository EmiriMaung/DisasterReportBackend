using DisasterReport.Data.Domain;
using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface ISupportTypeService
    {
        Task<List<SupportTypeDto>> GetAllAsync();
        Task<SupportTypeDto?> GetByIdAsync(int id);
        Task<SupportTypeDto> AddAsync(SupportTypeDto dto);
        Task<SupportTypeDto?> UpdateAsync(SupportTypeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
