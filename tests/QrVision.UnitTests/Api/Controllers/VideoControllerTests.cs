using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using QrVision.Api.Controllers;
using QrVision.Domain.Constants;
using QrVision.Domain.Dtos;
using QrVision.Domain.Interfaces.Services;
using System.Text;

namespace QrVision.UnitTests.Api.Controllers
{
    public class VideoControllerTests
    {
        private readonly Mock<IUploadVideoService> _uploadVideoServiceMock;
        private readonly Mock<IGetAnalysisResultService> _getAnalysisResultServiceMock;
        private readonly VideoController _controller;

        public VideoControllerTests()
        {
            _uploadVideoServiceMock = new Mock<IUploadVideoService>();
            _getAnalysisResultServiceMock = new Mock<IGetAnalysisResultService>();
            _controller = new VideoController(
                _uploadVideoServiceMock.Object,
                _getAnalysisResultServiceMock.Object);
        }

        [Fact]
        public async Task Upload_Async_When_File_Is_Valid_Should_Return_Ok_With_Analysis_Id()
        {
            // Arrange
            var newAnalysisId = Guid.NewGuid();
            var content = "fake video content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var videoFileMock = new Mock<IFormFile>();

            videoFileMock.Setup(f => f.Length).Returns(stream.Length);
            videoFileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            _uploadVideoServiceMock
                .Setup(s => s.ExecuteAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(newAnalysisId);

            // Act
            var result = await _controller.UploadAsync(videoFileMock.Object);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);

            var returnedValue = okResult.Value;
            var idProperty = returnedValue.GetType().GetProperty("Id");
            var returnedId = (Guid)idProperty.GetValue(returnedValue);
            
            idProperty.Should().NotBeNull();
            returnedId.Should().Be(newAnalysisId);

            _uploadVideoServiceMock.Verify(s => s.ExecuteAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Upload_Async_When_File_Is_Null_Should_Return_Bad_Request()
        {
            // Arrange
            IFormFile videoFile = null;

            // Act
            var result = await _controller.UploadAsync(videoFile);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(ErrorMessagesConst.FileNotFound);
        }

        [Fact]
        public async Task Upload_Async_When_File_Is_Empty_Should_Return_Bad_Request()
        {
            // Arrange
            var videoFileMock = new Mock<IFormFile>();
            videoFileMock.Setup(f => f.Length).Returns(0);

            // Act
            var result = await _controller.UploadAsync(videoFileMock.Object);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(ErrorMessagesConst.FileNotFound);
        }

        [Fact]
        public async Task Get_Analysis_Result_Async_When_Analysis_Exists_Should_Return_Ok_With_Result()
        {
            // Arrange
            var analysisId = Guid.NewGuid();
            var expectedDto = new VideoAnalysisResultDto
            {
                AnalysisId = analysisId,
                QrCodes = [new QrCodeResultDto
                {
                    Content = "test-content",
                    TimestampInSeconds = TimeSpan.FromSeconds(10).TotalSeconds
                }]
            };

            _getAnalysisResultServiceMock
                .Setup(s => s.ExecuteAsync(analysisId))
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _controller.GetAnalysisResultAsync(analysisId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(expectedDto);

            _getAnalysisResultServiceMock.Verify(s => s.ExecuteAsync(analysisId), Times.Once);
        }

        [Fact]
        public async Task Get_Analysis_Result_Async_When_Analysis_Not_Found_Should_Return_Not_Found()
        {
            // Arrange
            var analysisId = Guid.NewGuid();
            var emptyDto = new VideoAnalysisResultDto
            {
                AnalysisId = analysisId,
                QrCodes = []
            };

            _getAnalysisResultServiceMock
                .Setup(s => s.ExecuteAsync(analysisId))
                .ReturnsAsync(emptyDto);

            // Act
            var result = await _controller.GetAnalysisResultAsync(analysisId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            var notFoundResult = result as NotFoundResult;
            notFoundResult.StatusCode.Should().Be(404);

            _getAnalysisResultServiceMock.Verify(s => s.ExecuteAsync(analysisId), Times.Once);
        }
    }
}
