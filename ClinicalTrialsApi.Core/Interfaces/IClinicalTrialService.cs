using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicalTrialsApi.Core.Models;

namespace ClinicalTrialsApi.Core.Interfaces
{
    public interface IClinicalTrialService
    {
        Task<ClinicalTrial> ProcessAndSaveTrialDataAsync(string jsonData);
        Task<ClinicalTrial> GetTrialByIdAsync(int id);
        Task<ClinicalTrial> GetTrialByTrialIdAsync(string trialId);
        Task<IEnumerable<ClinicalTrial>> GetTrialsAsync(string status = null);
        Task<bool> ValidateJsonSchemaAsync(string jsonData);
    }
} 