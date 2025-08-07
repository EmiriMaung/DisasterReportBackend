using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Services;
using Moq;


namespace DisasterReport.Tests.Services
{
    public class OrganizationServiceTest
    {
        private readonly Mock<IOrganizationRepo> _mockOrgRepo;
        private readonly Mock<IOrganizationDocRepo> _mockOrgDocRepo;
        private readonly Mock<IOrganizationMemberRepo> _mockOrgMemberRepo;
        private readonly Mock<ICloudinaryService> _mockCloudService;

        private readonly OrganizationService _service;

        public OrganizationServiceTest()
        {
            _mockOrgRepo = new Mock<IOrganizationRepo>();
            _mockOrgDocRepo = new Mock<IOrganizationDocRepo>();
            _mockOrgMemberRepo = new Mock<IOrganizationMemberRepo>();
            _mockCloudService = new Mock<ICloudinaryService>();

            _service = new OrganizationService(
                _mockOrgRepo.Object,
                _mockOrgDocRepo.Object,
                _mockOrgMemberRepo.Object,
                _mockCloudService.Object);
        }

        [Fact]
        public async Task ApproveOrganizationAsync_ReturnsFalse_WhenOrganizationNotFound()
        {
            // Arrange
            int orgId = 1;
            Guid adminUserId = Guid.NewGuid();

            _mockOrgRepo.Setup(r => r.GetByIdAsync(orgId)).ReturnsAsync((Organization)null!);

            // Act
            var result = await _service.ApproveOrganizationAsync(orgId, adminUserId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ApproveOrganizationAsync_UpdatesAndReturnsTrue_WhenOrgFound()
        {
            // Arrange
            int orgId = 1;
            Guid adminUserId = Guid.NewGuid();
            var org = new Organization { Id = orgId };

            _mockOrgRepo.Setup(r => r.GetByIdAsync(orgId)).ReturnsAsync(org);
            _mockOrgRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _service.ApproveOrganizationAsync(orgId, adminUserId);

            // Assert
            Assert.True(result);
            Assert.Equal((int)Status.Approved, org.Status);
            Assert.Equal(adminUserId, org.ApprovedBy);

            _mockOrgRepo.Verify(r => r.Update(org), Times.Once);
            _mockOrgRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
