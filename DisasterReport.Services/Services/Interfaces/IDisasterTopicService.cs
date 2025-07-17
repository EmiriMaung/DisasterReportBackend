using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IDisasterTopicService
    {
        Task<List<DisasterTopicDto>> GetAllAsync();
        Task<DisasterTopicDto?> GetByIdAsync(int id);
        Task<DisasterTopicDto> CreateAsync(CreateDisasterTopicDto dto);
        Task<bool> UpdateAsync(UpdateDisasterTopicDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
