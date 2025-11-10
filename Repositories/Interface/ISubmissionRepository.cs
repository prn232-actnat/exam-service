using BO.Entities;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ISubmissionRepository : IRepository<Submission>
    {
        // Ví dụ về một phương thức đặc thù:
        // Lấy tất cả các bài nộp của một sinh viên cụ thể
        Task<IEnumerable<Submission>> GetSubmissionsByStudentIdAsync(Guid studentId);
    }
}
