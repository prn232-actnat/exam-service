using DTOs.Request.Question;
using DTOs.Request.QuestionBank;
using DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Services.Exceptions;
using Services.Interface;
using WebAPI.CustomResponse;
using System.IO;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class QuestionBanksController : ControllerBase
    {
        private readonly IQuestionBankService _qBankService;

        public QuestionBanksController(IQuestionBankService qBankService)
        {
            _qBankService = qBankService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<QuestionBankSummaryResponse>>), 200)]
        public async Task<IActionResult> GetAllBanks()
        {
            var banks = await _qBankService.GetAllQuestionBanksAsync();
            return Ok(ApiResponse<IEnumerable<QuestionBankSummaryResponse>>.Success(banks));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<QuestionBankResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetBankById(Guid id)
        {
            var bank = await _qBankService.GetQuestionBankByIdAsync(id);
            return Ok(ApiResponse<QuestionBankResponse>.Success(bank));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<QuestionBankResponse>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateBank([FromBody] CreateQuestionBankRequest createDto)
        {
            var newBank = await _qBankService.CreateQuestionBankAsync(createDto);
            var response = ApiResponse<QuestionBankResponse>.Success(newBank);

            return CreatedAtAction(
                nameof(GetBankById),
                new { id = newBank.Id },
                response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> UpdateBank(Guid id, [FromBody] UpdateQuestionBankRequest updateDto)
        {
            await _qBankService.UpdateQuestionBankAsync(id, updateDto);
            return Ok(ApiResponse<object>.Success(null, ResponseMessage.RequestSuccessful));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteBank(Guid id)
        {
            await _qBankService.DeleteQuestionBankAsync(id);
            return Ok(ApiResponse<object>.Success(null, ResponseMessage.RequestSuccessful));
        }

        [HttpGet("{bankId}/questions")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<QuestionResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetQuestionsInBank(Guid bankId)
        {
            var questions = await _qBankService.GetQuestionsByBankIdAsync(bankId);
            return Ok(ApiResponse<IEnumerable<QuestionResponse>>.Success(questions));
        }

        [HttpPost("questions")]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponse>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequest createDto)
        {
            var newQuestion = await _qBankService.CreateQuestionAsync(createDto);
            var response = ApiResponse<QuestionResponse>.Success(newQuestion);

            // Trả về 201 Created (vì đã tạo resource mới)
            return CreatedAtAction(
                nameof(GetQuestionById), // Giả sử bạn sẽ tạo endpoint này
                new { id = newQuestion.Id },
                response);
        }

        [HttpGet("questions/{id}")]
        [ProducesResponseType(typeof(ApiResponse<QuestionResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetQuestionById(Guid id)
        {
            var question = await _qBankService.GetQuestionByIdAsync(id);
            return Ok(ApiResponse<QuestionResponse>.Success(question));
        }

        [HttpPost("{bankId}/import-questions")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> ImportQuestions(Guid bankId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BusinessRuleException("Vui lòng tải lên một file Excel hợp lệ.");
            }

            // Kiểm tra extension file
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new BusinessRuleException("Chỉ chấp nhận file Excel (.xlsx hoặc .xls).");
            }

            // Kiểm tra kích thước file (tối đa 10MB)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
            {
                throw new BusinessRuleException("Kích thước file không được vượt quá 10MB.");
            }

            using (var stream = file.OpenReadStream())
            {
                await _qBankService.ImportQuestionsFromExcelAsync(bankId, stream);
            }

            return Ok(ApiResponse<object>.Success(null, ResponseMessage.RequestSuccessful));
        }
    }
}
