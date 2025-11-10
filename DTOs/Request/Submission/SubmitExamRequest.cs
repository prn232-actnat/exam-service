using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Submission
{
    public class SubmitExamRequest
    {
        [Required]
        [MinLength(1)]
        public List<SubmitAnswerRequest> Answers { get; set; }
    }
}
