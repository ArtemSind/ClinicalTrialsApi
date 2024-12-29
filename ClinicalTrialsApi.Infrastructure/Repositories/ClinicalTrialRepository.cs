using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using ClinicalTrialsApi.Infrastructure.Data;
using ClinicalTrialsApi.Core.Enums;

namespace ClinicalTrialsApi.Infrastructure.Repositories
{
    public class ClinicalTrialRepository : IClinicalTrialRepository
    {
        private readonly ApplicationDbContext _context;

        public ClinicalTrialRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClinicalTrial?> GetByIdAsync(int id)
        {
            return await _context.ClinicalTrials.FindAsync(id);
        }

        public async Task<ClinicalTrial?> GetByTrialIdAsync(string trialId)
        {
            return await _context.ClinicalTrials
            .FirstOrDefaultAsync(x => x.TrialId == trialId);
        }

        public async Task<IEnumerable<ClinicalTrial>> GetAllAsync(ClinicalTrialStatusEnum? status = null)
        {
            var query = _context.ClinicalTrials.AsQueryable();

            if (status != null)
            {
                query = query.Where(x => x.Status == status);
            }

            return await query.ToListAsync();
        }

        public async Task<ClinicalTrial> AddAsync(ClinicalTrial trial)
        {
            trial.CreatedAt = DateTime.UtcNow;
            _context.ClinicalTrials.Add(trial);
            await _context.SaveChangesAsync();
            return trial;
        }

        public async Task<ClinicalTrial> UpdateAsync(ClinicalTrial trial)
        {
            trial.UpdatedAt = DateTime.UtcNow;
            _context.Entry(trial).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return trial;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var trial = await _context.ClinicalTrials.FindAsync(id);
            if (trial == null)
                return false;

            _context.ClinicalTrials.Remove(trial);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}