using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{
    namespace DTOs.Response
    {
        public class QuestionOptionResponse
        {
            public Guid Id { get; set; }
            public int OptionIndex { get; set; }
            public string OptionText { get; set; }
            public bool IsCorrect { get; set; }
        }
    }

}
