using BO.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IEnumerable<Question>> GetQuestionsByBankIdAsync(Guid bankId);

        Task<Question?> GetByIdWithOptionsAsync(Guid questionId);
    }
}
