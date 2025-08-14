//using DisasterReport.Data.Domain;
//using DisasterReport.Data.Repositories;
//using DisasterReport.Data.Repositories.Interfaces;
//using DisasterReport.Services.Enums;
//using DisasterReport.Services.Models;
//using DisasterReport.Services.Services;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DisasterReport.Tests.Services
//{
//    public class OrganizationMemberServiceTest
//    {
//        private readonly Mock<IOrganizationRepo> _mockOrgRepo;
//        private readonly Mock<IOrganizationMemberRepo> _mockOrgMemberRepo;
//        private readonly Mock<IUserRepo> _mockUserRepo;
//        private readonly OrganizationMemberService _service;

//        public OrganizationMemberServiceTest()
//        {
//            _mockOrgRepo = new Mock<IOrganizationRepo>();
//            _mockOrgMemberRepo = new Mock<IOrganizationMemberRepo>();
//            _mockUserRepo = new Mock<IUserRepo>();

//            _service = new OrganizationMemberService(
//                _mockOrgRepo.Object,
//                _mockOrgMemberRepo.Object,
//                _mockUserRepo.Object
//            );
//        }
//        [Fact]
//        public async Task AcceptInvitationAsync_ThrowsException_WhenTokenInvalid()
//        {
//            // Arrange
//            var dto = new AcceptInvitationDto { Token = Guid.NewGuid() };
//            _mockOrgMemberRepo.Setup(r => r.GetByTokenAsync(dto.Token)).ReturnsAsync((OrganizationMember)null!);

//            // Act & Assert
//            var ex = await Assert.ThrowsAsync<Exception>(() =>
//                _service.AcceptInvitationAsync(dto, Guid.NewGuid()));

//            Assert.Equal("Invalid or expired invitation token.", ex.Message);
//        }
//        [Fact]
//        public async Task InviteMemberAsync_Throws_WhenOrgNotApproved()
//        {
//            // Arrange
//            var org = new Organization { Id = 1, Status = (int)Status.Pending };
//            _mockOrgRepo.Setup(r => r.GetByIdAsync(org.Id)).ReturnsAsync(org);

//            // Act & Assert
//            var ex = await Assert.ThrowsAsync<Exception>(() =>
//                _service.InviteMemberAsync(org.Id, new InviteMemberDto { InvitedEmail = "new@user.com" }, Guid.NewGuid()));

//            Assert.Equal("Organization not found or not approved.", ex.Message);
//        }
//        [Fact]
//        public async Task RemoveMemberAsync_Succeeds_WhenValid()
//        {
//            // Arrange
//            int orgId = 1;
//            var userId = Guid.NewGuid();

//            var member = new OrganizationMember { OrganizationId = orgId, UserId = userId };

//            _mockOrgMemberRepo.Setup(r => r.GetByOrgAndUserAsync(orgId, userId)).ReturnsAsync(member);
//            _mockOrgMemberRepo.Setup(r => r.RemoveAsync(member)).Returns(Task.CompletedTask);
//            _mockOrgMemberRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

//            // Act
//            var result = await _service.RemoveMemberAsync(orgId, userId);

//            // Assert
//            Assert.True(result);
//        }


//    }
//}
