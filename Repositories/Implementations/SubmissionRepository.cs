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
    public class SubmissionRepository : Repository<Submission>, ISubmissionRepository
    {
        public SubmissionRepository(ExamServiceDbContext context) : base(context)
        {
        }

        // Triển khai phương thức đặc thù
        public async Task<IEnumerable<Submission>> GetSubmissionsByStudentIdAsync(Guid studentId)
        {
            return await _context.Submissions
                .Where(s => s.StudentId == studentId)
                .OrderByDescending(s => s.SubmittedAt) // Sắp xếp cho dễ xem
                .ToListAsync();
        }
    }
}



