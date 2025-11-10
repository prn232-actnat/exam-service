using DTOs.Request.ExamSet;
using DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using WebAPI.CustomResponse;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExamSetsController : ControllerBase
    {
        private readonly IExamSetService _examSetService;

        public ExamSetsController(IExamSetService examSetService)
        {
            _examSetService = examSetService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExamSetResponse>>), 200)]
        public async Task<IActionResult> GetAllExamSets()
        {
            var examSets = await _examSetService.GetAllExamSetsAsync();
            return Ok(ApiResponse<IEnumerable<ExamSetResponse>>.Success(examSets));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ExamSetResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetExamSetById(Guid id)
        {
            var examSet = await _examSetService.GetExamSetByIdAsync(id);
            return Ok(ApiResponse<ExamSetResponse>.Success(examSet));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExamSetResponse>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateExamSet([FromBody] CreateExamSetRequest createDto)
        {
            var newExamSet = await _examSetService.CreateExamSetAsync(createDto);
            var response = ApiResponse<ExamSetResponse>.Success(newExamSet);

            return CreatedAtAction(
                nameof(GetExamSetById),
                new { id = newExamSet.Id },
                response);
        }
    }
}
