using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using DisasterReport.API.Controllers;
using DisasterReport.Services.Services.Interfaces;
using DisasterReport.Services.Models;
using DisasterReport.Services.Models.Common;
using DisasterReport.Data.Domain;

namespace DisasterReport.Tests.Controllers
{
    public class DisastersReportControllerTests
    {
        private readonly Mock<IDisasterReportService> _serviceMock;
        private readonly Mock<IHubContext<DisasterReportHub>> _hubMock;
        private readonly DisastersReportController _controller;

        public DisastersReportControllerTests()
        {
            _serviceMock = new Mock<IDisasterReportService>();
            _hubMock = new Mock<IHubContext<DisasterReportHub>>();
            _controller = new DisastersReportController(_serviceMock.Object, _hubMock.Object);

            // Setup mock user for authorized endpoints
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetAllReportsAsync_ReturnsOk()
        {
            var paged = new PagedResponse<DisasterReportDto>(new List<DisasterReportDto>(), 1, 10, 0);
            _serviceMock.Setup(s => s.GetAllReportsAsync(1, 10)).ReturnsAsync(paged);

            var result = await _controller.GetAllReportsAsync();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(paged, ok.Value);
        }

        [Fact]
        public async Task SearchReportsAsync_ReturnsOk()
        {
            var paged = new PagedResponse<DisasterReportDto>(new List<DisasterReportDto>(), 1, 10, 0);
            _serviceMock.Setup(s => s.SearchReportsAsync(null, null, null, null, null, null, 1, 10))
                        .ReturnsAsync(paged);

            var result = await _controller.SearchReportsAsync(null, null, null, null, null, null);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(paged, ok.Value);
        }

        [Fact]
        public async Task GetUrgentReportsAsync_ReturnsOk()
        {
            var data = new List<DisasterReportDto> { new DisasterReportDto { Id = 1 } };
            _serviceMock.Setup(s => s.GetUrgentReportsAsync()).ReturnsAsync(data);

            var result = await _controller.GetUrgentReportsAsync();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(data, ok.Value);
        }

        [Fact]
        public async Task GetMyReportsAsync_ReturnsOk()
        {
            var data = new List<DisasterReportDto> { new DisasterReportDto { Id = 1 } };
            _serviceMock.Setup(s => s.GetMyReportsAsync(It.IsAny<Guid>())).ReturnsAsync(data);

            var result = await _controller.GetMyReportsAsync();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(data, ok.Value);
        }

        [Fact]
        public async Task GetReportByIdAsync_WhenNotFound_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetReportByIdAsync(99)).ReturnsAsync((DisasterReportDto?)null);

            var result = await _controller.GetReportByIdAsync(99);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddReportAsync_ReturnsOk()
        {
            var dto = new AddDisasterReportDto { Title = "Test" };

            var result = await _controller.AddReportAsync(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Report added successfully.", ok.Value);
        }

        [Fact]
        public async Task UpdateReportAsync_ReturnsOk()
        {
            var dto = new UpdateDisasterReportDto { Title = "Updated" };

            var result = await _controller.UpdateReportAsync(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Report updated successfully.", ok.Value);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsOk()
        {
            var result = await _controller.SoftDeleteAsync(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("SoftDeleted successfully.", ok.Value);
        }

        [Fact]
        public async Task RestoreAsync_ReturnsOk()
        {
            var result = await _controller.RestoreAsync(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("restore successfully.", ok.Value);
        }

        [Fact]
        public async Task HardDeleteAsync_ReturnsNoContent()
        {
            var result = await _controller.HardDeleteAsync(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ApproveReportAsync_ReturnsOk()
        {
            var dto = new ApproveWithTopicDto();

            var result = await _controller.ApproveReportAsync(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Report approved successfully.", ok.Value?.GetType().GetProperty("message")?.GetValue(ok.Value));
        }

        [Fact]
        public async Task RejectReportAsync_ReturnsOk()
        {
            var result = await _controller.RejectReportAsync(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Report rejected successfully.", ok.Value?.GetType().GetProperty("message")?.GetValue(ok.Value));
        }
    }
}
