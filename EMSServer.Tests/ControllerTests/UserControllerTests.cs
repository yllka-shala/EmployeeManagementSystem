using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using EMSServer.Controllers;
using EMSServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EMSServer.Tests.ControllerTests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetUsersAsync_WhenUsersExist_ReturnsOkResult_WithMockData()
        {
            var expectedUsers = new List<User>
            {
                new() { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@test.com", Password = "Password123", RoleId = 1 },
                new() { Id = 2, FirstName = "Bob", LastName = "Smith", Email = "bob@test.com", Password = "Password123", RoleId = 2 }
            };
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetUsers()).ReturnsAsync(expectedUsers);

            var controller = new UserController(repository.Object);

            var result = await controller.GetUsersAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedUsers, okResult.Value);
            repository.Verify(r => r.GetUsers(), Times.Once);
        }

        [Fact]
        public async Task GetUsersAsync_WhenUsersAreNotFound_ReturnsNotFound()
        {
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetUsers()).ReturnsAsync((List<User>)null!);

            var controller = new UserController(repository.Object);

            var result = await controller.GetUsersAsync();

            Assert.IsType<NotFoundResult>(result);
            repository.Verify(r => r.GetUsers(), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WhenUserIsValid_ReturnsOkResult_WithMockData()
        {
            var user = new User
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@test.com",
                Password = "Password123",
                RoleId = 1,
                IsActive = true
            };
            var expectedResponse = new GeneralResponse(true, "User updated successfully");
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.UpdateUser(user)).ReturnsAsync(expectedResponse);

            var controller = new UserController(repository.Object);

            var result = await controller.UpdateUser(user);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.UpdateUser(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WhenUserIsNull_ReturnsBadRequest()
        {
            var repository = new Mock<IUserRepository>();
            var controller = new UserController(repository.Object);

            var result = await controller.UpdateUser(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request Made", badRequest.Value);
            repository.Verify(r => r.UpdateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_WhenIdIsValid_ReturnsOkResult_WithMockData()
        {
            const int id = 7;
            var expectedResponse = new GeneralResponse(true, "User deleted successfully");
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.DeleteUser(id)).ReturnsAsync(expectedResponse);

            var controller = new UserController(repository.Object);

            var result = await controller.DeleteUser(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.DeleteUser(id), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteUser_WhenIdIsInvalid_ReturnsBadRequest(int id)
        {
            var repository = new Mock<IUserRepository>();
            var controller = new UserController(repository.Object);

            var result = await controller.DeleteUser(id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid Request Sent!", badRequest.Value);
            repository.Verify(r => r.DeleteUser(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetUserImage_WhenImageExists_ReturnsOkResult_WithMockData()
        {
            const int id = 5;
            const string expectedImage = "data:image/png;base64,abc123";
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetUserImage(id)).ReturnsAsync(expectedImage);

            var controller = new UserController(repository.Object);

            var result = await controller.GetUserImage(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedImage, okResult.Value);
            repository.Verify(r => r.GetUserImage(id), Times.Once);
        }

        [Fact]
        public async Task GetUserImage_WhenImageDoesNotExist_ReturnsNotFound()
        {
            const int id = 99;
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetUserImage(id)).ReturnsAsync((string)null!);

            var controller = new UserController(repository.Object);

            var result = await controller.GetUserImage(id);

            Assert.IsType<NotFoundResult>(result);
            repository.Verify(r => r.GetUserImage(id), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_WhenProfileIsValid_ReturnsOkResult_WithMockData()
        {
            var profile = new UserProfile
            {
                Id = 1,
                Name = "Alice Johnson",
                Role = "Admin",
                Email = "alice@test.com",
                ProfilePictureFileName = "profile.png",
                Base64ImageData = "iVBORw0KGgoAAAANSUhEUg=="
            };
            var expectedResponse = new GeneralResponse(true, "Profile updated successfully");
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.UpdateProfile(profile)).ReturnsAsync(expectedResponse);

            var controller = new UserController(repository.Object);

            var result = await controller.UpdateProfile(profile);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.UpdateProfile(profile), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_WhenProfileIsInvalidOrUpdateFails_ReturnsOkResult_WithFailureResponse()
        {
            UserProfile profile = null!;
            var expectedResponse = new GeneralResponse(false, "Profile update failed");
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.UpdateProfile(It.IsAny<UserProfile>())).ReturnsAsync(expectedResponse);

            var controller = new UserController(repository.Object);

            var result = await controller.UpdateProfile(profile);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.UpdateProfile(It.IsAny<UserProfile>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProfilePicture_WhenIdIsValid_ReturnsOkResult_WithMockData()
        {
            const int id = 8;
            var expectedResponse = new GeneralResponse(true, "Profile picture deleted");
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.DeleteProfilePicture(id)).ReturnsAsync(expectedResponse);

            var controller = new UserController(repository.Object);

            var result = await controller.DeleteProfilePicture(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.DeleteProfilePicture(id), Times.Once);
        }

        [Fact]
        public async Task DeleteProfilePicture_WhenIdIsInvalidOrDeleteFails_ReturnsOkResult_WithFailureResponse()
        {
            const int id = -1;
            var expectedResponse = new GeneralResponse(false, "Profile picture delete failed");
            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.DeleteProfilePicture(id)).ReturnsAsync(expectedResponse);

            var controller = new UserController(repository.Object);

            var result = await controller.DeleteProfilePicture(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.DeleteProfilePicture(id), Times.Once);
        }
    }
}
