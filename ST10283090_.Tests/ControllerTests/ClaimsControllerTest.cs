using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ST10283090_.Areas.Identity.Data;
using ST10283090_.Controllers;
using ST10283090_.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ST10283090_.Tests.ControllerTests
{
    public class ClaimsControllerTest : IDisposable
    {
        private readonly ClaimsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;

        public ClaimsControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            var userStore = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new IdentityUser { Id = "userId" });

            _controller = new ClaimsController(_context, _userManagerMock.Object);
        }

        [Fact]
        public async Task Approve()
        {
            // Arrange
            var claim = new Claims { ClaimID = 1, Status = "Pending", UserID = "userId" };
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Approve(claim.ClaimID) as RedirectToActionResult;

            // Assert
            var updatedClaim = await _context.Claims.FindAsync(claim.ClaimID);
            Assert.Equal("Approved", updatedClaim.Status);
            Assert.Equal(nameof(ClaimsController.Index), result.ActionName);
        }

        [Fact]
        public async Task Reject()
        {
            // Arrange
            var claim = new Claims { ClaimID = 2, Status = "Pending", UserID = "userId" };
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Reject(claim.ClaimID) as RedirectToActionResult;

            // Assert
            var updatedClaim = await _context.Claims.FindAsync(claim.ClaimID);
            Assert.Equal("Rejected", updatedClaim.Status);
            Assert.Equal(nameof(ClaimsController.Index), result.ActionName);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}




