using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Question
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int QuestionBankId { get; set; }
    }
}
