using GetInitialFunctions.Dtos;
using GetInitialFunctions.Functions;
using GetInitialFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;

namespace Initials.Tests
{
    public class SubmitFunctionTests
    {
        private readonly Mock<IInitialsSerivice> _mockInitialsService;
        private readonly Mock<IQueueService> _mockQueueService;
        private readonly Mock<ILogger<SubmitInitialsInfoFunction>> _mockLogger;
        private readonly SubmitInitialsInfoFunction _function;

        public SubmitFunctionTests()
        {
            _mockInitialsService = new Mock<IInitialsSerivice>();
            _mockQueueService = new Mock<IQueueService>();
            _mockLogger = new Mock<ILogger<SubmitInitialsInfoFunction>>();

            _function = new SubmitInitialsInfoFunction(
                _mockLogger.Object,
                _mockInitialsService.Object, _mockQueueService.Object);

        }


        [Fact]
        public async Task Should_Return_1_If_Insert_Failed()
        {
            // Arrange
            var initialsDto = new InitialsDto
            {
                FirstName = "Reza",
                LastName = "Shirazi"
            };

            // Mock the service to return 1 (insert succeed)
            _mockInitialsService
                .Setup(x => x.InsertInitialsAsync(It.IsAny<InitialsDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);



            _mockQueueService
                .Setup(x => x.PublishToQueueAsync(It.IsAny<string>(), It.IsAny<InitialsDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var mockRequest = CreateMockHttpRequest(initialsDto);

            // Act
            var result = await _function.Run(mockRequest, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Success", okResult.Value);

        }

        [Fact]
        public async Task Should_Return_Zero_If_Insert_Failed()
        {
            // Arrange
            var initialsDto = new InitialsDto
            {
                FirstName = "Reza",
                LastName = "Shirazi"
            };

            // Mock the service to return 0 (insert failed)
            _mockInitialsService
                .Setup(x => x.InsertInitialsAsync(It.IsAny<InitialsDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);



            _mockQueueService
                .Setup(x => x.PublishToQueueAsync(It.IsAny<string>(), It.IsAny<InitialsDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var mockRequest = CreateMockHttpRequest(initialsDto);

            // Act
            var result = await _function.Run(mockRequest, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Failed", okResult.Value);
          
        }


        private static HttpRequest CreateMockHttpRequest(InitialsDto initialsDto)
        {
            var json = JsonSerializer.Serialize(initialsDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json))
            {
                Position = 0
            };

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(stream);
            mockRequest.Setup(x => x.ContentType).Returns("application/json");
            mockRequest.Setup(x => x.ContentLength).Returns(stream.Length);

            return mockRequest.Object;
        }
    }
}