using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Entities
{
    public class Question
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public Guid QuestionBankId { get; set; }
        public QuestionBank QuestionBank { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

        // Giả sử ta lưu đáp án đúng (ví dụ: "A", hoặc "Paris")
        public string CorrectAnswer { get; set; }
        public string AudioUrl { get; set; }
    }
}
