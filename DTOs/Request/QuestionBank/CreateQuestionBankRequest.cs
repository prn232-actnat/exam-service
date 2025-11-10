using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.QuestionBank
{
    public class CreateQuestionBankRequest
    {
        [Required(ErrorMessage = "Tên ngân hàng câu hỏi là bắt buộc.")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
