using ClinicalTrialsApi.Core.Enums;
using System;

namespace ClinicalTrialsApi.Core.Models
{
    public class ClinicalTrial
    {
        public int Id { get; set; }
        public required string TrialId { get; set; }
        public required string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Participants { get; set; }
        public required ClinicalTrialStatusEnum Status { get; set; }
        public int? DurationInDays { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 