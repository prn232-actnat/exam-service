using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{
    public class ExamDetailResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int DurationInMinutes { get; set; }
        public List<QuestionResponse> Questions { get; set; } = new List<QuestionResponse>();
    }
}
