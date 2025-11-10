using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Entities
{
    public class ExamSet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // ID từ FlashcardService
        public Guid FlashcardId { get; set; }
    }
}
