using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Submission
{
    public class SubmissionResultDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public double? Score { get; set; }
    }
}
