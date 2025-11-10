using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Entities
{
    public class QuestionBank
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
