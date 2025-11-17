using BO.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(ExamServiceDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Question>> GetQuestionsByBankIdAsync(Guid bankId)
        {
            return await _context.Questions
                .Where(q => q.QuestionBankId == bankId)
                .Include(q => q.QuestionOptions) 
                .OrderBy(q => q.QuestionText)
                .ToListAsync();
        }

        public async Task<Question?> GetByIdWithOptionsAsync(Guid questionId)
        {
            return await _context.Questions
                .Include(q => q.QuestionOptions)
                .FirstOrDefaultAsync(q => q.Id == questionId);
        }
    }
}
