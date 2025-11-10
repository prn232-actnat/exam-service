using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Submission
{
    public class StartSubmissionRequest
    {
        [Required]
        public Guid StudentId { get; set; } // ID từ StudentService

        [Required]
        public Guid ExamId { get; set; }
    }
}
