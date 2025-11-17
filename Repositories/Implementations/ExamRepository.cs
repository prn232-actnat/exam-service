using BO.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class ExamRepository : Repository<Exam>, IExamRepository
    {
        // Kế thừa constructor của lớp cha (Repository<Exam>)
        public ExamRepository(ExamServiceDBContext context) : base(context)
        {
        }

        // Triển khai phương thức đặc thù
        public async Task<Exam> GetExamWithQuestionsAsync(Guid examId)
        {
            return await _context.Exams
                .AsNoTracking()
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                        .ThenInclude(q => q.QuestionOptions)   // <- bắt buộc
                .Where(e => e.Id == examId)
                .FirstOrDefaultAsync();
        }
    }
}



