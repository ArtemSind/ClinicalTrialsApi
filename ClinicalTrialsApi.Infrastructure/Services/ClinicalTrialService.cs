using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using NJsonSchema;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;

namespace ClinicalTrialsApi.Infrastructure.Services
{
    public class ClinicalTrialService : IClinicalTrialService
    {
        private readonly IClinicalTrialRepository _repository;
        private readonly JsonSchema _schema;

        public ClinicalTrialService(IClinicalTrialRepository repository)
        {
            _repository = repository;
            _schema = JsonSchema.FromJsonAsync(GetJsonSchema()).Result;
        }

        public async Task<ClinicalTrial> ProcessAndSaveTrialDataAsync(string jsonData)
        {
            if (!await ValidateJsonSchemaAsync(jsonData))
                throw new ArgumentException("Invalid JSON data format");

            var trialData = JsonSerializer.Deserialize<ClinicalTrial>(jsonData);
            
            // Apply business rules
            if (trialData.EndDate == null && trialData.Status == "Ongoing")
            {
                trialData.EndDate = trialData.StartDate.AddMonths(1);
            }

            // Calculate duration
            if (trialData.EndDate.HasValue)
            {
                trialData.DurationInDays = (int)(trialData.EndDate.Value - trialData.StartDate).TotalDays;
            }

            var existingTrial = await _repository.GetByTrialIdAsync(trialData.TrialId);
            if (existingTrial != null)
            {
                trialData.Id = existingTrial.Id;
                return await _repository.UpdateAsync(trialData);
            }

            return await _repository.AddAsync(trialData);
        }

        public async Task<ClinicalTrial> GetTrialByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ClinicalTrial> GetTrialByTrialIdAsync(string trialId)
        {
            return await _repository.GetByTrialIdAsync(trialId);
        }

        public async Task<IEnumerable<ClinicalTrial>> GetTrialsAsync(string status = null)
        {
            return await _repository.GetAllAsync(status);
        }

        public async Task<bool> ValidateJsonSchemaAsync(string jsonData)
        {
            try
            {
                var errors = _schema.Validate(jsonData);
                return errors.Count == 0;
            }
            catch
            {
                return false;
            }
        }

        private string GetJsonSchema()
        {
            return @"{
                ""$schema"": ""http://json-schema.org/draft-07/schema#"",
                ""title"": ""ClinicalTrialMetadata"",
                ""type"": ""object"",
                ""properties"": {
                    ""trialId"": {
                        ""type"": ""string""
                    },
                    ""title"": {
                        ""type"": ""string""
                    },
                    ""startDate"": {
                        ""type"": ""string"",
                        ""format"": ""date""
                    },
                    ""endDate"": {
                        ""type"": ""string"",
                        ""format"": ""date""
                    },
                    ""participants"": {
                        ""type"": ""integer"",
                        ""minimum"": 1
                    },
                    ""status"": {
                        ""type"": ""string"",
                        ""enum"": [
                            ""Not Started"",
                            ""Ongoing"",
                            ""Completed""
                        ]
                    }
                },
                ""required"": [
                    ""trialId"",
                    ""title"",
                    ""startDate"",
                    ""status""
                ],
                ""additionalProperties"": false
            }";
        }
    }
} 