using DisasterReport.Data.Domain;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class DisasterTopicService : IDisasterTopicService
    {
        private readonly ApplicationDBContext _context;

        public DisasterTopicService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<DisasterTopicDto>> GetAllAsync()
        {
            return await _context.DisasterTopics
                .Select(t => new DisasterTopicDto
                {
                    Id = t.Id,
                    TopicName = t.TopicName,
                    AdminId = t.AdminId
                })
                .ToListAsync();
        }

        public async Task<DisasterTopicDto?> GetByIdAsync(int id)
        {
            var topic = await _context.DisasterTopics.FindAsync(id);
            if (topic == null) return null;

            return new DisasterTopicDto
            {
                Id = topic.Id,
                TopicName = topic.TopicName,
                AdminId = topic.AdminId
            };
        }

        public async Task<DisasterTopicDto> CreateAsync(CreateDisasterTopicDto dto)
        {
            var topic = new DisasterTopic
            {
                TopicName = dto.TopicName,
                AdminId = dto.AdminId,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            _context.DisasterTopics.Add(topic);
            await _context.SaveChangesAsync();

            return new DisasterTopicDto
            {
                Id = topic.Id,
                TopicName = topic.TopicName,
                AdminId = topic.AdminId
            };
        }

        public async Task<bool> UpdateAsync(UpdateDisasterTopicDto dto)
        {
            var topic = await _context.DisasterTopics.FindAsync(dto.Id);
            if (topic == null) return false;

            topic.TopicName = dto.TopicName;
            if (dto.UpdatedAdminId.HasValue)
                topic.UpdatedAdminId = dto.UpdatedAdminId.Value;

            topic.UpdateAt = DateTime.UtcNow;

            _context.DisasterTopics.Update(topic);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var topic = await _context.DisasterTopics.FindAsync(id);
            if (topic == null) return false;

            _context.DisasterTopics.Remove(topic);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
