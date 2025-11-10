using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{
    public class ExamSetResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid FlashcardId { get; set; }
    }
}
