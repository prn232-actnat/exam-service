using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.ExamSet
{
    public class CreateExamSetRequest
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        public Guid FlashcardId { get; set; } // ID của Flashcard liên quan
    }
}
