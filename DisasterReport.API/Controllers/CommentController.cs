using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DisasterReport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication by default
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(
            ICommentService commentService,
            ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? User.FindFirst("sub")
                           ?? User.FindFirst("id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Unable to parse user ID from claims");
                throw new UnauthorizedAccessException("Invalid user identification");
            }

            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _commentService.CreateCommentAsync(createDto, userId);
                return CreatedAtAction(
                    nameof(GetCommentById),
                    new { id = result.Id },
                    result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid comment creation request");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, "An error occurred while creating the comment");
            }
        }

        [HttpGet("disaster/{disasterReportId}")]
        [AllowAnonymous] // Optional: Allow unauthenticated access to view comments
        public async Task<IActionResult> GetCommentsByDisaster(int disasterReportId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByReportIdAsync(disasterReportId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving comments for disaster {disasterReportId}");
                return StatusCode(500, "An error occurred while retrieving comments");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Optional: Allow unauthenticated access to view single comment
        public async Task<IActionResult> GetCommentById(int id)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(id);
                return Ok(comment);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving comment {id}");
                return StatusCode(500, "An error occurred while retrieving the comment");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto updateDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _commentService.UpdateCommentAsync(id, updateDto, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating comment {id}");
                return StatusCode(500, "An error occurred while updating the comment");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _commentService.DeleteCommentAsync(id, userId);
                return success ? NoContent() : StatusCode(500);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting comment {id}");
                return StatusCode(500, "An error occurred while deleting the comment");
            }
        }
    }
}