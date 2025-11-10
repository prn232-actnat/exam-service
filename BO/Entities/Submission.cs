using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Entities
{
    public class Submission
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }
        public Guid StudentId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public double? Score { get; set; }

        // Một lượt nộp bài có nhiều câu trả lời
        public ICollection<SubmissionAnswer> Answers { get; set; } = new List<SubmissionAnswer>();
    }
}
