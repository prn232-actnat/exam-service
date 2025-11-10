using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Question
{
    public class CreateQuestionRequest
    {
        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string QuestionType { get; set; } // "MultipleChoice", "ShortAnswer"

        [Required]
        public Guid QuestionBankId { get; set; }
        [Required]
        public string CorrectAnswer { get; set; }

        public string AudioUrl { get; set; }
    }
}
