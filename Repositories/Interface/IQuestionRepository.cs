using BO.Entities;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IQuestionRepository : IRepository<Question>
    {
        // Ví dụ về một phương thức đặc thù:
        // Lấy tất cả câu hỏi thuộc về một ngân hàng câu hỏi
        Task<IEnumerable<Question>> GetQuestionsByBankIdAsync(Guid questionBankId);
    }
}
