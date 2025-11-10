using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{
    public class ExamSummaryResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
