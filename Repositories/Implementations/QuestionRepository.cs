using BO.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(ExamServiceDbContext context) : base(context)
        {
        }

        // Triển khai phương thức đặc thù
        public async Task<IEnumerable<Question>> GetQuestionsByBankIdAsync(Guid questionBankId)
        {
            return await _context.Questions
                .Where(q => q.QuestionBankId == questionBankId)
                .ToListAsync();
        }
    }
}



