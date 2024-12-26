using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using ClinicalTrialsApi.Infrastructure.Services;

namespace ClinicalTrialsApi.Tests
{
    public class ClinicalTrialServiceTests
    {
        private readonly Mock<IClinicalTrialRepository> _mockRepository;
        private readonly IClinicalTrialService _service;

        public ClinicalTrialServiceTests()
        {
            _mockRepository = new Mock<IClinicalTrialRepository>();
            _service = new ClinicalTrialService(_mockRepository.Object);
        }

        [Fact]
        public async Task ProcessAndSaveTrialDataAsync_ValidData_AppliesBusinessRules()
        {
            // Arrange
            var jsonData = @"{
                ""trialId"": ""TEST001"",
                ""title"": ""Test Trial"",
                ""startDate"": ""2024-01-01"",
                ""status"": ""Ongoing""
            }";

            _mockRepository.Setup(r => r.GetByTrialIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ClinicalTrial)null);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<ClinicalTrial>()))
                .ReturnsAsync((ClinicalTrial trial) => trial);

            // Act
            var result = await _service.ProcessAndSaveTrialDataAsync(jsonData);

            // Assert
            result.Should().NotBeNull();
            result.EndDate.Should().Be(new DateTime(2024, 02, 01)); // One month from start date
            result.DurationInDays.Should().Be(31); // Duration should be calculated
            
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ClinicalTrial>()), Times.Once);
        }

        [Fact]
        public async Task ValidateJsonSchemaAsync_InvalidData_ReturnsFalse()
        {
            // Arrange
            var invalidJsonData = @"{
                ""trialId"": ""TEST001"",
                ""startDate"": ""invalid-date"",
                ""status"": ""Invalid""
            }";

            // Act
            var result = await _service.ValidateJsonSchemaAsync(invalidJsonData);

            // Assert
            result.Should().BeFalse();
        }
    }
} 