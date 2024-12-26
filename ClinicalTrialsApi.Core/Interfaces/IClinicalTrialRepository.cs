using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicalTrialsApi.Core.Models;

namespace ClinicalTrialsApi.Core.Interfaces
{
    public interface IClinicalTrialRepository
    {
        Task<ClinicalTrial> GetByIdAsync(int id);
        Task<ClinicalTrial> GetByTrialIdAsync(string trialId);
        Task<IEnumerable<ClinicalTrial>> GetAllAsync(string status = null);
        Task<ClinicalTrial> AddAsync(ClinicalTrial trial);
        Task<ClinicalTrial> UpdateAsync(ClinicalTrial trial);
        Task<bool> DeleteAsync(int id);
    }
} 