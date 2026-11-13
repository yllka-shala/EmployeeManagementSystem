using EMSServer.Controllers;
using EMSServer.Repositories.Interfaces;
using EMSBaseLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EMSServer.Tests.ControllerTests
{
    public class GenericControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkObjectResult_WithMockData()
        {
            var expected = new List<string> { "Alice", "Bob" };
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            repository.Setup(r => r.GetAll()).ReturnsAsync(expected);

            var controller = new GenericController<string>(repository.Object);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, okResult.Value);
            repository.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenServiceFails_ThrowsException()
        {
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            repository.Setup(r => r.GetAll())
                .ThrowsAsync(new InvalidOperationException("Service unavailable"));

            var controller = new GenericController<string>(repository.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.GetAll());
            repository.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenIdIsValid_ReturnsOkObjectResult_WithMockData()
        {
            const int id = 7;
            const string expected = "Employee-7";
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            repository.Setup(r => r.GetById(id)).ReturnsAsync(expected);

            var controller = new GenericController<string>(repository.Object);

            var result = await controller.GetById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, okResult.Value);
            repository.Verify(r => r.GetById(id), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_WhenIdIsInvalid_ReturnsBadRequest(int id)
        {
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            var controller = new GenericController<string>(repository.Object);

            var result = await controller.GetById(id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid Request Sent", badRequest.Value);
            repository.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenModelIsValid_ReturnsOkObjectResult_WithMockData()
        {
            const string model = "New Employee";
            var expectedResponse = new GeneralResponse(true, "Added");
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            repository.Setup(r => r.Insert(model)).ReturnsAsync(expectedResponse);

            var controller = new GenericController<string>(repository.Object);

            var result = await controller.Add(model);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.Insert(model), Times.Once);
        }

        [Fact]
        public async Task Add_WhenModelIsNull_ReturnsBadRequest()
        {
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            var controller = new GenericController<string>(repository.Object);

            var result = await controller.Add(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request Made!", badRequest.Value);
            repository.Verify(r => r.Insert(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenModelIsValid_ReturnsOkObjectResult_WithMockData()
        {
            const string model = "Updated Employee";
            var expectedResponse = new GeneralResponse(true, "Updated");
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            repository.Setup(r => r.Update(model)).ReturnsAsync(expectedResponse);

            var controller = new GenericController<string>(repository.Object);

            var result = await controller.Update(model);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.Update(model), Times.Once);
        }

        [Fact]
        public async Task Update_WhenModelIsNull_ReturnsBadRequest()
        {
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            var controller = new GenericController<string>(repository.Object);

            var result = await controller.Update(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request Made", badRequest.Value);
            repository.Verify(r => r.Update(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenIdIsValid_ReturnsOkObjectResult_WithMockData()
        {
            const int id = 12;
            var expectedResponse = new GeneralResponse(true, "Deleted");
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            repository.Setup(r => r.DeleteById(id)).ReturnsAsync(expectedResponse);

            var controller = new GenericController<string>(repository.Object);

            var result = await controller.Delete(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            repository.Verify(r => r.DeleteById(id), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task Delete_WhenIdIsInvalid_ReturnsBadRequest(int id)
        {
            var repository = new Mock<IGenericRepositoryInterface<string>>();
            var controller = new GenericController<string>(repository.Object);

            var result = await controller.Delete(id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid Request Sent!", badRequest.Value);
            repository.Verify(r => r.DeleteById(It.IsAny<int>()), Times.Never);
        }
    }
}
