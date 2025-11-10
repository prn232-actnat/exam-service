using BO.Entities;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IExamRepository : IRepository<Exam>
    {
        // Ví dụ về một phương thức đặc thù:
        // Lấy một bài thi kèm theo danh sách câu hỏi của nó
        Task<Exam> GetExamWithQuestionsAsync(Guid examId);
    }
}
