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
    public class ExamRepository : Repository<Exam>, IExamRepository
    {
        // Kế thừa constructor của lớp cha (Repository<Exam>)
        public ExamRepository(ExamServiceDbContext context) : base(context)
        {
        }

        // Triển khai phương thức đặc thù
        public async Task<Exam> GetExamWithQuestionsAsync(Guid examId)
        {
            // _context có thể được truy cập vì nó là 'protected'
            return await _context.Exams
                .Include(e => e.ExamQuestions)       // Nạp bảng trung gian
                .ThenInclude(eq => eq.Question) // Nạp bảng Question từ bảng trung gian
                .FirstOrDefaultAsync(e => e.Id == examId);
        }
    }
}



