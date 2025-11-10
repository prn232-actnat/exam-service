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
    public class ExamSetRepository : Repository<ExamSet>, IExamSetRepository
    {
        public ExamSetRepository(ExamServiceDbContext context) : base(context)
        {
        }
    }
}



