using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{
    public class SubmissionResultResponse
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public double? Score { get; set; }
    }
}
