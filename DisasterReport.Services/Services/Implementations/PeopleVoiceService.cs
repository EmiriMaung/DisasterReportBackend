using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.PeopleVoiceDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class PeopleVoiceService : IPeopleVoiceService
    {
        private readonly ApplicationDBContext _context;

        public PeopleVoiceService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PeopleVoice>> GetAllPeopleVoicesAsync()
        {
            return await _context.PeopleVoices
                .OrderByDescending(pv => pv.CreatedAt)
                .ToListAsync();
        }

        public async Task<PeopleVoice?> GetPeopleVoiceByIdAsync(int id)
        {
            return await _context.PeopleVoices.FindAsync(id);
        }

        public async Task<PeopleVoice> CreatePeopleVoiceAsync(CreatePeopleVoiceDto createDto)
        {
            var peopleVoice = new PeopleVoice
            {
                FullName = createDto.FullName,
                Email = createDto.Email,
                Phone = createDto.Phone,
                Message = createDto.Message,
                CreatedAt = DateTime.UtcNow
            };

            _context.PeopleVoices.Add(peopleVoice);
            await _context.SaveChangesAsync();

            return peopleVoice;
        }

        public async Task<PeopleVoice?> UpdatePeopleVoiceAsync(int id, UpdatePeopleVoiceDto updateDto)
        {
            var existingPeopleVoice = await _context.PeopleVoices.FindAsync(id);
            if (existingPeopleVoice == null)
            {
                return null;
            }

            if (updateDto.FullName != null)
                existingPeopleVoice.FullName = updateDto.FullName;

            if (updateDto.Email != null)
                existingPeopleVoice.Email = updateDto.Email;

            if (updateDto.Phone != null)
                existingPeopleVoice.Phone = updateDto.Phone;

            if (updateDto.Message != null)
                existingPeopleVoice.Message = updateDto.Message;

            await _context.SaveChangesAsync();

            return existingPeopleVoice;
        }

        public async Task<bool> DeletePeopleVoiceAsync(int id)
        {
            var peopleVoice = await _context.PeopleVoices.FindAsync(id);
            if (peopleVoice == null)
            {
                return false;
            }

            _context.PeopleVoices.Remove(peopleVoice);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
