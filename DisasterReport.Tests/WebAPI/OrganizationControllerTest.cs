using DisasterReport.API.Controllers;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Tests.WebAPI
{
    public class OrganizationControllerTest
    {
        private readonly Mock<IOrganizationService> _mockService;
        private readonly OrganizationController _controller;

        public OrganizationControllerTest()
        {
            _mockService = new Mock<IOrganizationService>();
            _controller = new OrganizationController(_mockService.Object);

            // Setup User with a NameIdentifier claim for tests requiring auth user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithOrganizations()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<OrganizationDto> { new OrganizationDto { Id = 1, Name = "Org1" } });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orgs = Assert.IsAssignableFrom<IEnumerable<OrganizationDto>>(okResult.Value);
            Assert.Single(orgs);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            int orgId = 1;
            _mockService.Setup(s => s.GetByIdAsync(orgId))
                .ReturnsAsync(new OrganizationDto { Id = orgId, Name = "Org1" });

            var result = await _controller.GetById(orgId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var org = Assert.IsType<OrganizationDto>(okResult.Value);
            Assert.Equal(orgId, org.Id);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((OrganizationDto?)null);

            var result = await _controller.GetById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            var newOrgId = 1;
            var dto = new CreateOrganizationDto
            {
                Name = "New Org",
                OrganizationEmail = "email@example.com"
                // add other required properties here
            };

            _mockService.Setup(s => s.CreateOrganizationAsync(dto, It.IsAny<Guid>()))
                .ReturnsAsync(newOrgId);

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(newOrgId, ((dynamic)createdResult.Value).Id);
        }

        [Fact]
        public async Task Create_ThrowsInvalidOperationException_ReturnsBadRequest()
        {
            var dto = new CreateOrganizationDto { Name = "Test" };
            _mockService.Setup(s => s.CreateOrganizationAsync(dto, It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException("You already have an active organization"));

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("active organization", badRequest.Value.ToString()!);
        }

        [Fact]
        public async Task Update_MismatchedId_ReturnsBadRequest()
        {
            var dto = new UpdateOrganizationDto { Id = 2, Name = "Org" };

            var result = await _controller.Update(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Mismatched ID", badRequest.Value);
        }

        [Fact]
        public async Task Update_ValidId_ReturnsNoContent()
        {
            var dto = new UpdateOrganizationDto { Id = 1, Name = "Org" };
            _mockService.Setup(s => s.UpdateOrganizationAsync(dto)).ReturnsAsync(true);

            var result = await _controller.Update(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_NotFound_ReturnsNotFound()
        {
            var dto = new UpdateOrganizationDto { Id = 1, Name = "Org" };
            _mockService.Setup(s => s.UpdateOrganizationAsync(dto)).ReturnsAsync(false);

            var result = await _controller.Update(1, dto);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
