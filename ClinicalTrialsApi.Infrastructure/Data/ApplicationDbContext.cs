using Microsoft.EntityFrameworkCore;
using ClinicalTrialsApi.Core.Models;

namespace ClinicalTrialsApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClinicalTrial> ClinicalTrials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClinicalTrial>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TrialId).IsUnique();
                entity.Property(e => e.TrialId).IsRequired();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
} 