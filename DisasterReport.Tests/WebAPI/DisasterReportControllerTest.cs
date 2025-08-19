using DisasterReport.API.Controllers;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Tests.WebAPI
{
    public class DisasterReportControllerTest
    {
        private readonly Mock<IDisasterReportService> _mockService;
        private readonly DisastersReportController _controller;

        public DisasterReportControllerTest()
        {
            _mockService = new Mock<IDisasterReportService>();
            //_controller = new DisastersReportController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllReportsAsync_ReturnsOkResult_WithReports()
        {
            // Arrange
            var fakeReports = new List<DisasterReportDto>
            {
                new DisasterReportDto { Id = 1, Title = "Flood" },
                new DisasterReportDto { Id = 2, Title = "Earthquake" }
            };

            _mockService.Setup(s => s.GetAllReportsAsync()).ReturnsAsync(fakeReports);

            // Act
            var result = await _controller.GetAllReportsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<DisasterReportDto>>(okResult.Value);
            Assert.Equal(2, ((List<DisasterReportDto>)returnValue).Count);
        }
    }
}
