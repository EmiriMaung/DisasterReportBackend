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
            if (string.IsNullOrWhiteSpace(createDto.Content))
            {
                throw new ArgumentException("Comment content cannot be empty");
            }

            var entity = new Comment
            {
                UserId = userId,
                DisasterReportId = createDto.DisasterReportId,
                Content = createDto.Content.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(entity);
            await _context.SaveChangesAsync();

            return await GetCommentByIdAsync(entity.Id);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByReportIdAsync(int reportId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.DisasterReportId == reportId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    DisasterReportId = c.DisasterReportId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserName = c.User.Name,
                    UserProfilePictureUrl = c.User.ProfilePictureUrl
                })
                .ToListAsync();
        }

        public async Task<CommentDto> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {id} not found");
            }

            return MapToDto(comment);
        }

        public async Task<CommentDto> UpdateCommentAsync(int commentId, UpdateCommentDto updateDto, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(updateDto.Content))
            {
                throw new ArgumentException("Comment content cannot be empty");
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found");
            }

            if (comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only update your own comments");
            }

            comment.Content = updateDto.Content.Trim();
            comment.CreatedAt = DateTime.UtcNow; 

            await _context.SaveChangesAsync();

            return await GetCommentByIdAsync(comment.Id);
        }

        public async Task<bool> DeleteCommentAsync(int commentId, Guid userId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found");
            }

            if (comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own comments");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return true;
        }

        private CommentDto MapToDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                UserId = comment.UserId,
                DisasterReportId = comment.DisasterReportId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserName = comment.User?.Name ?? "Unknown",
                UserProfilePictureUrl = comment.User?.ProfilePictureUrl
            };
        }

    }

}
