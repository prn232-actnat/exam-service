using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Entities
{
    public class SubmissionAnswer
    {
        public Guid Id { get; set; }

        // Khóa ngoại tới Submission
        public Guid SubmissionId { get; set; }
        public Submission Submission { get; set; }

        // Khóa ngoại tới Question
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        // Câu trả lời của sinh viên (ví dụ: "A", "B", hoặc "Paris")
        public string SelectedAnswer { get; set; }

        public bool IsCorrect { get; set; }
    }
}
