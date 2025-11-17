using BO.Entities;

using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class SubmissionAnswerRepository : Repository<SubmissionAnswer>, ISubmissionAnswerRepository
    {
        public SubmissionAnswerRepository(ExamServiceDBContext context) : base(context)
        {
        }
    }
}



