using DTOs.Request.QuestionOption;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Question
{
    public class UpdateQuestionRequest
    {
        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string QuestionType { get; set; }
        [Required]
        public string CorrectAnswer { get; set; }

        public string AudioUrl { get; set; }

        public List<QuestionOptionCreateRequest> Options { get; set; } = new();
    
    }
}
