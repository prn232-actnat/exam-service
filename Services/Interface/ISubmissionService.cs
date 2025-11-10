using DTOs.Request.Submission;
using DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ISubmissionService
    {
        Task<SubmissionResultResponse> StartExamAsync(StartSubmissionRequest requestDto);
        Task<SubmissionResultResponse> GetSubmissionResultAsync(Guid submissionId);
        Task<SubmissionResultResponse> SubmitExamAsync(Guid submissionId, SubmitExamRequest answersDto);
    }
}
