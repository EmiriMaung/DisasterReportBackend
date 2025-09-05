using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services;
using DisasterReport.Services.Services.Implementations;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace DisasterReport.Tests.Services
{
    public class DisasterReportServiceTests
    {
        private readonly Mock<IPostRepo> _postRepoMock;
        private readonly Mock<ILocationRepo> _locationRepoMock;
        private readonly Mock<IImpactUrlRepo> _impactUrlRepoMock;
        private readonly Mock<ICloudinaryService> _cloudinaryServiceMock;
        private readonly Mock<IDisasterTopicService> _disasterTopicServiceMock;
        private readonly IMemoryCache _memoryCache;
        private readonly DisasterReportService _service;

        public DisasterReportServiceTests()
        {
            _postRepoMock = new Mock<IPostRepo>();
            _locationRepoMock = new Mock<ILocationRepo>();
            _impactUrlRepoMock = new Mock<IImpactUrlRepo>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _disasterTopicServiceMock = new Mock<IDisasterTopicService>();

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _service = new DisasterReportService(
                _postRepoMock.Object,
                _cloudinaryServiceMock.Object,
                _impactUrlRepoMock.Object,
                _locationRepoMock.Object,
                _memoryCache,
                _disasterTopicServiceMock.Object
            );
        }



        [Fact]
        public async Task GetReportByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _postRepoMock.Setup(r => r.GetPostByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((DisastersReport?)null);

            // Act
            var result = await _service.GetReportByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SoftDeleteAsync_ThrowsException_WhenNotFound()
        {
            // Arrange
            _postRepoMock.Setup(r => r.GetPostByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((DisastersReport?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_CallsRepoMethods_WhenFound()
        {
            // Arrange
            var report = new DisastersReport { Id = 1, ReporterId = Guid.NewGuid() };

            _postRepoMock.Setup(r => r.GetPostByIdAsync(1))
                .ReturnsAsync(report);

            // Act
            await _service.SoftDeleteAsync(1);

            // Assert
            _postRepoMock.Verify(r => r.SoftDeleteReportAsync(1), Times.Once);
            _postRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
