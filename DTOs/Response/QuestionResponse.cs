using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{
    public class QuestionResponse
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public Guid QuestionBankId { get; set; }
        public string AudioUrl { get; set; }
    }
}
