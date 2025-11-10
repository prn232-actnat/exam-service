using DTOs.Request.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Exam
{
    public class ExamDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DurationInMinutes { get; set; }

        // Trả về danh sách câu hỏi đã được map sang QuestionDto
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
