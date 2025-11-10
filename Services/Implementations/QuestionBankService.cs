using AutoMapper;
using BO.Entities;
using DTOs.Request.Question;
using DTOs.Request.QuestionBank;
using DTOs.Response;
using OfficeOpenXml;
using Repositories.Interface;
using Services.Exceptions;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class QuestionBankService : IQuestionBankService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionBankService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<QuestionBankResponse> CreateQuestionBankAsync(CreateQuestionBankRequest createDto)
        {
            var bank = _mapper.Map<QuestionBank>(createDto);
            bank.Id = Guid.NewGuid(); // Tạo Guid mới cho QuestionBank
            await _unitOfWork.QuestionBanks.AddAsync(bank);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<QuestionBankResponse>(bank);
        }

        public async Task DeleteQuestionBankAsync(Guid id)
        {
            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(id);
            if (bank == null)
            {
                throw new NotFoundException($"Không tìm thấy Ngân hàng câu hỏi ID={id}");
            }
            _unitOfWork.QuestionBanks.Delete(bank);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<QuestionBankSummaryResponse>> GetAllQuestionBanksAsync()
        {
            var banks = await _unitOfWork.QuestionBanks.GetAllAsync();
            return _mapper.Map<IEnumerable<QuestionBankSummaryResponse>>(banks);
        }

        public async Task<QuestionBankResponse> GetQuestionBankByIdAsync(Guid id)
        {
            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(id);
            if (bank == null)
            {
                throw new NotFoundException($"Không tìm thấy Ngân hàng câu hỏi ID={id}");
            }
            return _mapper.Map<QuestionBankResponse>(bank);
        }

        public async Task UpdateQuestionBankAsync(Guid id, UpdateQuestionBankRequest updateDto)
        {
            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(id);
            if (bank == null)
            {
                throw new NotFoundException($"Không tìm thấy Ngân hàng câu hỏi ID={id}");
            }
            _mapper.Map(updateDto, bank);
            _unitOfWork.QuestionBanks.Update(bank);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<QuestionResponse> CreateQuestionAsync(CreateQuestionRequest createDto)
        {
            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(createDto.QuestionBankId);
            if (bank == null)
            {
                throw new BusinessRuleException($"Không tìm thấy Ngân hàng câu hỏi ID={createDto.QuestionBankId} để thêm câu hỏi.");
            }

            var question = _mapper.Map<Question>(createDto);
            question.Id = Guid.NewGuid(); // Tạo Guid mới cho Question
            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<QuestionResponse>(question);
        }

        public async Task DeleteQuestionAsync(Guid questionId)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
            if (question == null)
            {
                throw new NotFoundException($"Không tìm thấy Câu hỏi ID={questionId}");
            }
            _unitOfWork.Questions.Delete(question);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<QuestionResponse> GetQuestionByIdAsync(Guid questionId)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
            if (question == null)
            {
                throw new NotFoundException($"Không tìm thấy Câu hỏi ID={questionId}");
            }
            return _mapper.Map<QuestionResponse>(question);
        }

        public async Task<IEnumerable<QuestionResponse>> GetQuestionsByBankIdAsync(Guid bankId)
        {
            var questions = await _unitOfWork.Questions.GetQuestionsByBankIdAsync(bankId);
            return _mapper.Map<IEnumerable<QuestionResponse>>(questions);
        }

        public async Task UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest updateDto)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
            if (question == null)
            {
                throw new NotFoundException($"Không tìm thấy Câu hỏi ID={questionId}");
            }
            _mapper.Map(updateDto, question);
            _unitOfWork.Questions.Update(question);
            await _unitOfWork.CompleteAsync();
        }
        public async Task ImportQuestionsFromExcelAsync(Guid bankId, Stream fileStream)
        {
            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(bankId);
            if (bank == null)
            {
                throw new NotFoundException($"Không tìm thấy Ngân hàng câu hỏi ID={bankId}");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var questionsToAdd = new List<Question>();
            var errorMessages = new List<string>();
            var validQuestionTypes = new[] { "MultipleChoice", "ShortAnswer" };

            try
            {
                using (var package = new ExcelPackage(fileStream))
                {
                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        throw new BusinessRuleException("File Excel không có worksheet nào.");
                    }

                    var worksheet = package.Workbook.Worksheets[0];
                    
                    if (worksheet.Dimension == null)
                    {
                        throw new BusinessRuleException("File Excel không có dữ liệu.");
                    }

                    var rowCount = worksheet.Dimension.Rows;

                    // Kiểm tra header row (row 1)
                    if (rowCount < 2)
                    {
                        throw new BusinessRuleException("File Excel phải có ít nhất 1 dòng dữ liệu (không tính header).");
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var questionText = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        var questionType = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                        var correctAnswer = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                        var audioUrl = worksheet.Cells[row, 4].Value?.ToString()?.Trim();

                        // Bỏ qua dòng trống
                        if (string.IsNullOrEmpty(questionText) &&
                            string.IsNullOrEmpty(questionType) &&
                            string.IsNullOrEmpty(correctAnswer))
                        {
                            continue;
                        }

                        // Validate dữ liệu bắt buộc
                        if (string.IsNullOrEmpty(questionText))
                        {
                            errorMessages.Add($"Dòng {row}: Thiếu nội dung câu hỏi.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(questionType))
                        {
                            errorMessages.Add($"Dòng {row}: Thiếu loại câu hỏi.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(correctAnswer))
                        {
                            errorMessages.Add($"Dòng {row}: Thiếu đáp án đúng.");
                            continue;
                        }

                        // Validate QuestionType
                        if (!validQuestionTypes.Contains(questionType, StringComparer.OrdinalIgnoreCase))
                        {
                            errorMessages.Add($"Dòng {row}: Loại câu hỏi không hợp lệ. Chỉ chấp nhận: {string.Join(", ", validQuestionTypes)}.");
                            continue;
                        }

                        questionsToAdd.Add(new Question
                        {
                            Id = Guid.NewGuid(), // Tạo Guid mới cho mỗi Question
                            QuestionBankId = bankId,
                            QuestionText = questionText,
                            QuestionType = questionType,
                            CorrectAnswer = correctAnswer,
                            AudioUrl = audioUrl
                        });
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessRuleException($"Lỗi khi đọc file Excel: {ex.Message}");
            }
            catch (Exception ex) when (!(ex is BusinessRuleException))
            {
                throw new BusinessRuleException($"Lỗi khi xử lý file Excel: {ex.Message}");
            }

            // Nếu có lỗi validation, báo lỗi
            if (errorMessages.Any())
            {
                var errorSummary = string.Join(" ", errorMessages);
                throw new BusinessRuleException($"Có lỗi trong file Excel:\n{errorSummary}");
            }

            if (!questionsToAdd.Any())
            {
                throw new BusinessRuleException("File Excel không có dữ liệu hợp lệ hoặc bị rỗng.");
            }

            await _unitOfWork.Questions.AddRangeAsync(questionsToAdd);
            await _unitOfWork.CompleteAsync();
        }
    }
}
