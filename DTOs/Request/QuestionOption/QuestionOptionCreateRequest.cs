using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.QuestionOption
{
    public class QuestionOptionCreateRequest
    {
        public int OptionIndex { get; set; }
        public string OptionText { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
