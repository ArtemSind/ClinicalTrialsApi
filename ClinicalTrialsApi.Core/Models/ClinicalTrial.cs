using System;

namespace ClinicalTrialsApi.Core.Models
{
    public class ClinicalTrial
    {
        public int Id { get; set; }
        public string TrialId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Participants { get; set; }
        public string Status { get; set; }
        public int? DurationInDays { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 