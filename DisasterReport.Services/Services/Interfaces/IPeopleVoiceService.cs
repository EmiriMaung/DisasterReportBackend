using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.PeopleVoiceDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IPeopleVoiceService
    {
        Task<IEnumerable<PeopleVoice>> GetAllPeopleVoicesAsync();
        Task<PeopleVoice?> GetPeopleVoiceByIdAsync(int id);
        Task<PeopleVoice> CreatePeopleVoiceAsync(CreatePeopleVoiceDto createDto);
        Task<PeopleVoice?> UpdatePeopleVoiceAsync(int id, UpdatePeopleVoiceDto updateDto);
        Task<bool> DeletePeopleVoiceAsync(int id);
    }
}
