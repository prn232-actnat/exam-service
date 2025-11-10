using BO.Entities;
using Repositories.Data;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class QuestionBankRepository : Repository<QuestionBank>, IQuestionBankRepository
    {
        public QuestionBankRepository(ExamServiceDbContext context) : base(context)
        {
        }
    }
}



