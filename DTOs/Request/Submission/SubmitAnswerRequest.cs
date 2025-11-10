using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Submission
{
    public class SubmitAnswerRequest
    {
        [Required]
        public Guid QuestionId { get; set; }

        [Required]
        public string SelectedAnswer { get; set; }
    }
}
