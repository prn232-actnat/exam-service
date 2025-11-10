using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Exam
{
    public class ExamSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
