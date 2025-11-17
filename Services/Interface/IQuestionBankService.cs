using DTOs.Request.Question;
using DTOs.Request.QuestionBank;
using DTOs.Response;
using System;
using System.Collections.Generic;
using System.IO;
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
        Task<QuestionResponse> CreateQuestionAsync(CreateQuestionRequest createDto); // create includes QuestionOptions
        Task UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest updateDto);   // update may include options
        Task DeleteQuestionAsync(Guid questionId);

        // --- Import from Excel ---
        // fileStream: uploaded Excel file stream. Template expects columns:
        // QuestionText, QuestionType, OptionA, OptionB, OptionC, OptionD, CorrectOptionIndex (0-based), AudioUrl
        Task ImportQuestionsFromExcelAsync(Guid bankId, Stream fileStream);
    }
}
