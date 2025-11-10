using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Entities
{
    public class Exam
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int DurationInMinutes { get; set; }

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}
