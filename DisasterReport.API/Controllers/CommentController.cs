using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createDto)
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated");
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var result = await _commentService.CreateCommentAsync(createDto, userId);
            return Ok(result);
        }


        [HttpGet("disaster/{disasterReportId}")]
        public async Task<IActionResult> GetCommentsByDisaster(int disasterReportId)
        {
            var comments = await _commentService.GetCommentsByReportIdAsync(disasterReportId);
            return Ok(comments);
        }
    }

}
