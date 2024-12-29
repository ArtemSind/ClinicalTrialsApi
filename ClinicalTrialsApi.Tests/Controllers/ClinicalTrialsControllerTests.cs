using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using ClinicalTrialsApi.Api.Controllers;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using ClinicalTrialsApi.Core.Enums;

namespace ClinicalTrialsApi.Tests.Controllers
{
    public class ClinicalTrialsControllerTests
    {
        private readonly Mock<IClinicalTrialService> _mockService;
        private readonly ClinicalTrialsController _controller;

        public ClinicalTrialsControllerTests()
        {
            _mockService = new Mock<IClinicalTrialService>();
            _controller = new ClinicalTrialsController(_mockService.Object);
        }

        [Fact]
        public async Task UploadTrial_ValidFile_ReturnsCreatedResult()
        {
            // Arrange
            var trial = new ClinicalTrial { Id = 1, Title = "Test Trial", Status = ClinicalTrialStatusEnum.Ongoing, TrialId = "TEST001" };
            var fileContent = @"{
                ""trialId"": ""TEST001"",
                ""title"": ""Test Trial"",
                ""startDate"": ""2024-01-01"",
                ""status"": ""Ongoing""
            }";

            var fileMock = new Mock<IFormFile>();
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            
            fileMock.Setup(f => f.Length).Returns(memoryStream.Length);
            fileMock.Setup(f => f.FileName).Returns("test.json");
            fileMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);

            _mockService.Setup(s => s.ProcessAndSaveTrialDataAsync(It.IsAny<string>()))
                .ReturnsAsync(trial);

            // Act
            var result = await _controller.UploadTrial(fileMock.Object);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.Value.Should().BeEquivalentTo(trial);
            createdResult.ActionName.Should().Be(nameof(ClinicalTrialsController.GetTrialById));
        }

        [Fact]
        public async Task UploadTrial_InvalidFileType_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");

            // Act
            var result = await _controller.UploadTrial(fileMock.Object);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UploadTrial_FileTooLarge_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(2 * 1024 * 1024); // 2MB
            fileMock.Setup(f => f.FileName).Returns("test.json");

            // Act
            var result = await _controller.UploadTrial(fileMock.Object);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetTrialById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var trial = new ClinicalTrial { Id = 1, TrialId = "TEST001", Status = ClinicalTrialStatusEnum.Ongoing, Title = "TITLE001" };
            _mockService.Setup(s => s.GetTrialByIdAsync(1))
                .ReturnsAsync(trial);

            // Act
            var result = await _controller.GetTrialById(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(trial);
        }

        [Fact]
        public async Task GetTrialById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetTrialByIdAsync(1))
                .ReturnsAsync(null as ClinicalTrial);

            // Act
            var result = await _controller.GetTrialById(1);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetTrialByTrialId_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var trial = new ClinicalTrial { Id = 1, TrialId = "TEST001", Status = ClinicalTrialStatusEnum.Ongoing, Title = "TITLE001" };
            _mockService.Setup(s => s.GetTrialByTrialIdAsync("TEST001"))
                .ReturnsAsync(trial);

            // Act
            var result = await _controller.GetTrialByTrialId("TEST001");

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(trial);
        }

        [Fact]
        public async Task GetTrials_WithStatus_ReturnsFilteredTrials()
        {
            // Arrange
            var trials = new List<ClinicalTrial>
            {
                new ClinicalTrial { Id = 1, Status = ClinicalTrialStatusEnum.Ongoing, Title = "TITLE001", TrialId = "TEST001" }
            };

            _mockService.Setup(s => s.GetTrialsAsync(ClinicalTrialStatusEnum.Ongoing))
                .ReturnsAsync(trials);

            // Act
            var result = await _controller.GetTrials(ClinicalTrialStatusEnum.Ongoing);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTrials = okResult.Value as IEnumerable<ClinicalTrial>;
            returnedTrials.Should().HaveCount(1);
            returnedTrials!.First().Status.Should().Be(ClinicalTrialStatusEnum.Ongoing);
        }
    }
} 