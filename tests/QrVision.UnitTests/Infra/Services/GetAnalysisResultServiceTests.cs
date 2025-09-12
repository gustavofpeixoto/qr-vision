using FluentAssertions;
using Moq;
using QrVision.Domain.Dtos;
using QrVision.Domain.Entities;
using QrVision.Domain.Interfaces.Repositories;
using QrVision.Infra.Services;

namespace QrVision.UnitTests.Infra.Services
{
    public class GetAnalysisResultServiceTests
    {
        private readonly Mock<IVideoAnalysisRepository> _videoAnalysisRepositoryMock;
        private readonly GetAnalysisResultService _service;

        public GetAnalysisResultServiceTests()
        {
            // Arrange
            _videoAnalysisRepositoryMock = new Mock<IVideoAnalysisRepository>();
            _service = new GetAnalysisResultService(_videoAnalysisRepositoryMock.Object);
        }

        [Fact]
        public async Task Execute_Async_When_Analysis_Exists_Should_Return_Mapped_Dto_With_Qr_Codes()
        {
            // Arrange
            var videoAnalysis = new VideoAnalysis();
            var analysisId = videoAnalysis.Id;
            videoAnalysis.AddResults(
            [
                new() { Content = "qrcode-1", TimestampInSeconds = 10.5 },
                new() { Content = "qrcode-2", TimestampInSeconds = 25.0 }
            ]);

            _videoAnalysisRepositoryMock
                .Setup(repo => repo.GetByIdAsync(analysisId))
                .ReturnsAsync(videoAnalysis);

            // Act
            var result = await _service.ExecuteAsync(analysisId);

            // Assert
            result.Should().NotBeNull();
            result.AnalysisId.Should().Be(analysisId);
            result.QrCodes.Should().HaveCount(2);
            result.QrCodes.Should().BeEquivalentTo(videoAnalysis.QrCodeResults, options =>
                options.ComparingByMembers<QrCodeResultDto>());

            _videoAnalysisRepositoryMock.Verify(repo => repo.GetByIdAsync(analysisId), Times.Once);
        }

        [Fact]
        public async Task Execute_Async_When_Analysis_Does_Not_Exist_Should_Return_Empty_Dto()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            _videoAnalysisRepositoryMock
                .Setup(repo => repo.GetByIdAsync(nonExistentId))
                .ReturnsAsync((VideoAnalysis)null);

            // Act
            var result = await _service.ExecuteAsync(nonExistentId);

            // Assert
            result.Should().NotBeNull();
            result.AnalysisId.Should().Be(Guid.Empty);
            result.QrCodes.Should().BeEmpty();

            _videoAnalysisRepositoryMock.Verify(repo => repo.GetByIdAsync(nonExistentId), Times.Once);
        }
    }
}
