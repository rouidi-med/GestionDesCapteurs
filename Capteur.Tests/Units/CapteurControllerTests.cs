using Moq;
using Microsoft.AspNetCore.Mvc;
using Capteurs.Services.Interfaces;
using Capteurs.Controllers;

namespace YourNamespace.Tests
{
    public class SensorControllerTests
    {
        private readonly Mock<ICapteurService> _capteurServiceMock;
        private readonly CapteurController _capteurController;

        public SensorControllerTests()
        {
            // Mock the ICapteurService
            _capteurServiceMock = new Mock<ICapteurService>();
            _capteurController = new CapteurController(_capteurServiceMock.Object);
        }

        [Fact]
        public async Task Delete_CapteurExists_ReturnsNoContent()
        {
            // Arrange
            var capteurId = 1;
            _capteurServiceMock.Setup(s => s.DeleteCapteurAsync(capteurId))
                .ReturnsAsync(true);  // Simulate successful deletion

            // Act
            var result = await _capteurController.Delete(capteurId);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_CapteurDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var capteurId = 1;
            _capteurServiceMock.Setup(s => s.DeleteCapteurAsync(capteurId))
                .ReturnsAsync(false);  // Simulate sensor not found

            // Act
            var result = await _capteurController.Delete(capteurId);

            // Assert
            var actionResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WhenDeleteFails_ReturnsBadRequest()
        {
            // Arrange
            var invalidCapteurId = -1;
            // Act
            var result = await _capteurController.Delete(invalidCapteurId);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
            var badRequestResponse = actionResult.Value as string;
            Assert.Equal("Invalid sensor id provided.", badRequestResponse);
        }

        [Fact]
        public async Task Delete_WhenDeleteFails_ReturnsInternalServerError()
        {
            // Arrange
            var capteurId = 1;
            _capteurServiceMock.Setup(s => s.DeleteCapteurAsync(capteurId))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _capteurController.Delete(capteurId);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, actionResult.StatusCode);
        }
    }
}