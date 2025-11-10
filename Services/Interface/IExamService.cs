using DTOs.Request.Exam;
using DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IExamService
    {
        Task<ExamDetailResponse> GetExamDetailsAsync(Guid examId);
        Task<IEnumerable<ExamSummaryResponse>> GetAllExamsAsync();
        Task<ExamDetailResponse> CreateExamAsync(CreateExamRequest createDto);
        Task UpdateExamAsync(Guid examId, UpdateExamRequest updateDto);
        Task DeleteExamAsync(Guid examId);
    }
}
