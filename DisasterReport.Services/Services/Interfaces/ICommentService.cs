using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface ICommentService
    {
        // Create
        Task<CommentDto> CreateCommentAsync(CreateCommentDto createDto, Guid userId);

        // Read
        Task<IEnumerable<CommentDto>> GetCommentsByReportIdAsync(int reportId);
        Task<CommentDto> GetCommentByIdAsync(int id);

        // Update
        Task<CommentDto> UpdateCommentAsync(int commentId, UpdateCommentDto updateDto, Guid userId);

        // Delete
        Task<bool> DeleteCommentAsync(int commentId, Guid userId);
    }

}
