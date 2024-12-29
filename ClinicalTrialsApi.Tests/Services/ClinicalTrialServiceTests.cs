using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using ClinicalTrialsApi.Infrastructure.Services;
using ClinicalTrialsApi.Core.Enums;

namespace ClinicalTrialsApi.Tests.Services
{
    public class ClinicalTrialServiceTests
    {
        private readonly Mock<IClinicalTrialRepository> _mockRepository;
        private readonly IClinicalTrialService _service;
        private readonly string _validJsonData;
        private readonly string _invalidJsonData;

        public ClinicalTrialServiceTests()
        {
            _mockRepository = new Mock<IClinicalTrialRepository>();
            _service = new ClinicalTrialService(_mockRepository.Object);

            _validJsonData = @"{
                ""trialId"": ""TEST001"",
                ""title"": ""Test Trial"",
                ""startDate"": ""2024-01-01"",
                ""status"": ""Ongoing""
            }";

            _invalidJsonData = @"{
                ""trialId"": ""TEST001"",
                ""startDate"": ""invalid-date"",
                ""status"": ""Invalid""
            }";
        }

        [Fact]
        public async Task ProcessAndSaveTrialDataAsync_ValidData_AppliesBusinessRules()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByTrialIdAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ClinicalTrial);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<ClinicalTrial>()))
                .ReturnsAsync((ClinicalTrial trial) => trial);

            // Act
            var result = await _service.ProcessAndSaveTrialDataAsync(_validJsonData);

            // Assert
            result.Should().NotBeNull();
            result.EndDate.Should().Be(new DateTime(2024, 02, 01)); // One month from start date
            result.DurationInDays.Should().Be(31); // Duration should be calculated
            
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ClinicalTrial>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAndSaveTrialDataAsync_ExistingTrial_UpdatesExistingRecord()
        {
            // Arrange
            var existingTrial = new ClinicalTrial { Id = 1, TrialId = "TEST001", Status = ClinicalTrialStatusEnum.Ongoing, Title = "TITLE001" };
            
            _mockRepository.Setup(r => r.GetByTrialIdAsync("TEST001"))
                .ReturnsAsync(existingTrial);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ClinicalTrial>()))
                .ReturnsAsync((ClinicalTrial trial) => trial);

            // Act
            var result = await _service.ProcessAndSaveTrialDataAsync(_validJsonData);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ClinicalTrial>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ClinicalTrial>()), Times.Never);
        }

        [Fact]
        public void ValidateJsonSchema_InvalidData_ReturnsFalse()
        {
            // Act
            var result = _service.IsValidJsonSchema(_invalidJsonData);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ProcessAndSaveTrialDataAsync_InvalidData_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.ProcessAndSaveTrialDataAsync(_invalidJsonData));
        }

        [Fact]
        public async Task GetTrialByIdAsync_ExistingId_ReturnsTrial()
        {
            // Arrange
            var expectedTrial = new ClinicalTrial { Id = 1, TrialId = "TEST001", Status = ClinicalTrialStatusEnum.Ongoing, Title = "TITLE001" };
            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(expectedTrial);

            // Act
            var result = await _service.GetTrialByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTrial);
        }

        [Fact]
        public async Task GetTrialByTrialIdAsync_ExistingTrialId_ReturnsTrial()
        {
            // Arrange
            var expectedTrial = new ClinicalTrial { Id = 1, TrialId = "TEST001", Status = ClinicalTrialStatusEnum.Ongoing, Title = "TITLE001" };
            _mockRepository.Setup(r => r.GetByTrialIdAsync("TEST001"))
                .ReturnsAsync(expectedTrial);

            // Act
            var result = await _service.GetTrialByTrialIdAsync("TEST001");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedTrial);
        }

        [Fact]
        public async Task GetTrialsAsync_WithStatus_FiltersCorrectly()
        {
            // Arrange
            var trials = new List<ClinicalTrial>
            {
                new ClinicalTrial { Id = 1, TrialId = "TEST001", Status = ClinicalTrialStatusEnum.Ongoing, Title = "TITLE001" }
            };

            _mockRepository.Setup(r => r.GetAllAsync(ClinicalTrialStatusEnum.Ongoing))
                .ReturnsAsync(trials);

            // Act
            var result = await _service.GetTrialsAsync(ClinicalTrialStatusEnum.Ongoing);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Status.Should().Be(ClinicalTrialStatusEnum.Ongoing);
        }
    }
} 