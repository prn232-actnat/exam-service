using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Request.ExamSet
{
    public class ExamSetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid FlashcardId { get; set; } // ID từ FlashcardService
    }
}
