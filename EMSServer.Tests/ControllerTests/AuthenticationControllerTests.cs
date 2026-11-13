using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Responses;
using EMSServer.Controllers;
using EMSServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EMSServer.Tests.ControllerTests
{
    public class AuthenticationControllerTests
    {
        [Fact]
        public async Task CreateAsync_WhenUserIsValid_ReturnsOkObjectResult_WithMockData()
        {
            var registerUser = new Register
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };
            var expectedResponse = new GeneralResponse(true, "Registration successful");
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.CreateAsync(registerUser)).ReturnsAsync(expectedResponse);

            var controller = new AuthenticationController(repository.Object);

            var result = await controller.CreateAsync(registerUser);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.CreateAsync(registerUser), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenUserIsNull_ReturnsBadRequest()
        {
            var repository = new Mock<IAccountRepository>();
            var controller = new AuthenticationController(repository.Object);

            var result = await controller.CreateAsync(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Model is empty!", badRequest.Value);
            repository.Verify(r => r.CreateAsync(It.IsAny<Register>()), Times.Never);
        }

        [Fact]
        public async Task SignInAsync_WhenUserIsValid_ReturnsOkObjectResult_WithMockData()
        {
            var loginUser = new Login
            {
                Email = "alice@example.com",
                Password = "Password123"
            };
            var expectedResponse = new LoginResponse(true, "Login successful", "token-123", "refresh-token-456");
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.SignInAsync(loginUser)).ReturnsAsync(expectedResponse);

            var controller = new AuthenticationController(repository.Object);

            var result = await controller.SignInAsync(loginUser);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.SignInAsync(loginUser), Times.Once);
        }

        [Fact]
        public async Task SignInAsync_WhenUserIsNull_ReturnsBadRequest()
        {
            var repository = new Mock<IAccountRepository>();
            var controller = new AuthenticationController(repository.Object);

            var result = await controller.SignInAsync(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Model is empty!", badRequest.Value);
            repository.Verify(r => r.SignInAsync(It.IsAny<Login>()), Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenIsValid_ReturnsOkObjectResult_WithMockData()
        {
            var refreshToken = new RefreshToken { Token = "valid-refresh-token" };
            var expectedResponse = new LoginResponse(true, "Token refreshed successfully", "new-token-123", "new-refresh-token-456");
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.RefreshTokenAsync(refreshToken)).ReturnsAsync(expectedResponse);

            var controller = new AuthenticationController(repository.Object);

            var result = await controller.RefreshTokenAsync(refreshToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.RefreshTokenAsync(refreshToken), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenIsNull_ReturnsBadRequest()
        {
            var repository = new Mock<IAccountRepository>();
            var controller = new AuthenticationController(repository.Object);

            var result = await controller.RefreshTokenAsync(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Model is empty!", badRequest.Value);
            repository.Verify(r => r.RefreshTokenAsync(It.IsAny<RefreshToken>()), Times.Never);
        }
    }
}
