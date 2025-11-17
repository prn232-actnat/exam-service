using DTOs.Request.Submission;
using DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using WebAPI.CustomResponse;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/Exams/[controller]")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionsController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        [HttpPost("start")]
        [ProducesResponseType(typeof(ApiResponse<SubmissionResultResponse>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> StartExam([FromBody] StartSubmissionRequest requestDto)
        {
            var submissionResult = await _submissionService.StartExamAsync(requestDto);
            var response = ApiResponse<SubmissionResultResponse>.Success(submissionResult);

            return CreatedAtAction(
                nameof(GetSubmissionResult),
                new { id = submissionResult.Id },
                response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SubmissionResultResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetSubmissionResult(Guid id)
        {
            var result = await _submissionService.GetSubmissionResultAsync(id);
            return Ok(ApiResponse<SubmissionResultResponse>.Success(result));
        }

        [HttpPost("{id}/submit")]
        [ProducesResponseType(typeof(ApiResponse<SubmissionResultResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> SubmitExam(Guid id, [FromBody] SubmitExamRequest answersDto)
        {
            var result = await _submissionService.SubmitExamAsync(id, answersDto);
            return Ok(ApiResponse<SubmissionResultResponse>.Success(result));
        }
    }
}
