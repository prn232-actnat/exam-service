using DTOs.Request.ExamSet;
using DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IExamSetService
    {
        Task<ExamSetResponse> GetExamSetByIdAsync(Guid id);
        Task<IEnumerable<ExamSetResponse>> GetAllExamSetsAsync();
        Task<ExamSetResponse> CreateExamSetAsync(CreateExamSetRequest createDto);
    }
}
