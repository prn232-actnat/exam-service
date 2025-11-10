using DTOs.Request.Question;
using DTOs.Request.QuestionBank;
using DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IQuestionBankService
    {
        // --- Question Bank ---
        Task<QuestionBankResponse> GetQuestionBankByIdAsync(Guid id);
        Task<IEnumerable<QuestionBankSummaryResponse>> GetAllQuestionBanksAsync();
        Task<QuestionBankResponse> CreateQuestionBankAsync(CreateQuestionBankRequest createDto);
        Task UpdateQuestionBankAsync(Guid id, UpdateQuestionBankRequest updateDto);
        Task DeleteQuestionBankAsync(Guid id);

        // --- Question ---
        Task<QuestionResponse> GetQuestionByIdAsync(Guid questionId);
        Task<IEnumerable<QuestionResponse>> GetQuestionsByBankIdAsync(Guid bankId);
        Task<QuestionResponse> CreateQuestionAsync(CreateQuestionRequest createDto);
        Task UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest updateDto);
        Task DeleteQuestionAsync(Guid questionId);

        // --- Chức năng Import ---
        Task ImportQuestionsFromExcelAsync(Guid bankId, Stream fileStream);
    }
}
