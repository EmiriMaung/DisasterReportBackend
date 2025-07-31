using DisasterReport.Data.Domain;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDBContext _context;

        public CommentService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createDto, Guid userId)
        {
            var entity = new Comment
            {
                UserId = userId,
                DisasterReportId = createDto.DisasterReportId,
                Content = createDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(entity);
            await _context.SaveChangesAsync();

            return new CommentDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                DisasterReportId = entity.DisasterReportId,
                Content = entity.Content,
                CreatedAt = entity.CreatedAt
            };
        }


        public async Task<IEnumerable<CommentDto>> GetCommentsByReportIdAsync(int reportId)
        {
            return await _context.Comments
                .Where(c => c.DisasterReportId == reportId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    DisasterReportId = c.DisasterReportId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }
    }

}
