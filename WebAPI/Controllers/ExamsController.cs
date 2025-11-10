using DTOs.Request.Exam;
using DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using WebAPI.CustomResponse;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExamSummaryResponse>>), 200)]
        public async Task<IActionResult> GetAllExams()
        {
            var exams = await _examService.GetAllExamsAsync();
            return Ok(ApiResponse<IEnumerable<ExamSummaryResponse>>.Success(exams));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ExamDetailResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetExamDetails(Guid id)
        {
            var examDetail = await _examService.GetExamDetailsAsync(id);
            return Ok(ApiResponse<ExamDetailResponse>.Success(examDetail));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExamDetailResponse>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamRequest createDto)
        {
            var newExam = await _examService.CreateExamAsync(createDto);
            var response = ApiResponse<ExamDetailResponse>.Success(newExam);

            return CreatedAtAction(
                nameof(GetExamDetails),
                new { id = newExam.Id },
                response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> UpdateExam(Guid id, [FromBody] UpdateExamRequest updateDto)
        {
            await _examService.UpdateExamAsync(id, updateDto);
            return Ok(ApiResponse<object>.Success(null, ResponseMessage.RequestSuccessful));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteExam(Guid id)
        {
            await _examService.DeleteExamAsync(id);
            return Ok(ApiResponse<object>.Success(null, ResponseMessage.RequestSuccessful));
        }
    }
}
