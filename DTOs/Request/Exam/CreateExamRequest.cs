using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.Exam
{
    public class CreateExamRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Thời gian thi phải lớn hơn 0")]
        public int DurationInMinutes { get; set; }

        // Danh sách ID của các câu hỏi được thêm vào bài thi
        [Required]
        [MinLength(1, ErrorMessage = "Bài thi phải có ít nhất 1 câu hỏi.")]
        public List<Guid> QuestionIds { get; set; }
    }
}
