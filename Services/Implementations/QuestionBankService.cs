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
using System.IO;
using System.Linq;
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
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            var bank = _mapper.Map<QuestionBank>(createDto);
            bank.Id = Guid.NewGuid();
            await _unitOfWork.QuestionBanks.AddAsync(bank);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<QuestionBankResponse>(bank);
        }

        public async Task DeleteQuestionBankAsync(Guid id)
        {
            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(id);
            if (bank == null) throw new NotFoundException($"Không tìm thấy Ngân hàng câu hỏi ID={id}");
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
            if (bank == null) throw new NotFoundException($"Không tìm thấy Ngân hàng câu hỏi ID={id}");
            return _mapper.Map<QuestionBankResponse>(bank);
        }

        public async Task UpdateQuestionBankAsync(Guid id, UpdateQuestionBankRequest updateDto)
        {
            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(id);
            if (bank == null) throw new NotFoundException($"Không tìm thấy Ngân hàng câu hỏi ID={id}");
            _mapper.Map(updateDto, bank);
            _unitOfWork.QuestionBanks.Update(bank);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<QuestionResponse> CreateQuestionAsync(CreateQuestionRequest createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(createDto.QuestionBankId);
            if (bank == null)
                throw new BusinessRuleException($"Không tìm thấy Ngân hàng câu hỏi ID={createDto.QuestionBankId} để thêm câu hỏi.");

            if (string.IsNullOrWhiteSpace(createDto.QuestionText))
                throw new BusinessRuleException("QuestionText không được để trống.");

            var qType = (createDto.QuestionType ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(qType))
                throw new BusinessRuleException("QuestionType không được để trống.");

            if (qType.Equals("MultipleChoice", StringComparison.OrdinalIgnoreCase))
            {
                if (createDto.Options == null || createDto.Options.Count < 2)
                    throw new BusinessRuleException("MultipleChoice phải có ít nhất 2 options.");

                var hasIsCorrectFlags = createDto.Options.Any(o => o.IsCorrect.HasValue);
                if (hasIsCorrectFlags)
                {
                    var correctCount = createDto.Options.Count(o => o.IsCorrect == true);
                    if (correctCount != 1)
                        throw new BusinessRuleException("Phải có đúng 1 option được đánh dấu IsCorrect = true.");
                }
            }

            var question = new Question
            {
                Id = Guid.NewGuid(),
                QuestionBankId = createDto.QuestionBankId,
                QuestionText = createDto.QuestionText,
                QuestionType = createDto.QuestionType,
                QuestionOptions = new List<QuestionOption>()
            };

            if (createDto.Options != null && createDto.Options.Any())
            {
                foreach (var opt in createDto.Options)
                {
                    var option = new QuestionOption
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = question.Id,
                        OptionIndex = opt.OptionIndex,
                        OptionText = opt.OptionText ?? string.Empty,
                        IsCorrect = opt.IsCorrect == true
                    };
                    question.QuestionOptions.Add(option);
                }

                var correctOpt = question.QuestionOptions.FirstOrDefault(o => o.IsCorrect);
                question.CorrectAnswer = correctOpt?.OptionText;
            }

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.CompleteAsync();


            return _mapper.Map<QuestionResponse>(question);
        }

        public async Task DeleteQuestionAsync(Guid questionId)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
            if (question == null) throw new NotFoundException($"Không tìm thấy Câu hỏi ID={questionId}");
            _unitOfWork.Questions.Delete(question);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<QuestionResponse> GetQuestionByIdAsync(Guid questionId)
        {
            var question = await _unitOfWork.Questions.GetByIdWithOptionsAsync(questionId);
            if (question == null) throw new NotFoundException($"Không tìm thấy Câu hỏi ID={questionId}");
            return _mapper.Map<QuestionResponse>(question);
        }

        public async Task<IEnumerable<QuestionResponse>> GetQuestionsByBankIdAsync(Guid bankId)
        {
            var questions = await _unitOfWork.Questions.GetQuestionsByBankIdAsync(bankId); // should include options
            return _mapper.Map<IEnumerable<QuestionResponse>>(questions);
        }

        public async Task UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest updateDto)
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));

            var question = await _unitOfWork.Questions.GetByIdWithOptionsAsync(questionId);
            if (question == null) throw new NotFoundException($"Không tìm thấy Câu hỏi ID={questionId}");

            _mapper.Map(updateDto, question);

            if (updateDto.Options != null)
            {
                if (question.QuestionType.Equals("MultipleChoice", StringComparison.OrdinalIgnoreCase))
                {
                    if (updateDto.Options.Count < 2)
                        throw new BusinessRuleException("MultipleChoice phải có ít nhất 2 options.");

                    var correctFlags = updateDto.Options.Count(o => o.IsCorrect == true);
                    if (correctFlags > 1)
                        throw new BusinessRuleException("Chỉ được phép có 1 option đúng.");
                }

                question.QuestionOptions = question.QuestionOptions ?? new List<QuestionOption>();
                question.QuestionOptions.Clear();

                foreach (var opt in updateDto.Options)
                {
                    question.QuestionOptions.Add(new QuestionOption
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = question.Id,
                        OptionIndex = opt.OptionIndex,
                        OptionText = opt.OptionText,
                        IsCorrect = opt.IsCorrect == true
                    });
                }

                var correctOption = question.QuestionOptions.FirstOrDefault(o => o.IsCorrect);
                question.CorrectAnswer = correctOption?.OptionText;
            }

            _unitOfWork.Questions.Update(question);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ImportQuestionsFromExcelAsync(Guid bankId, Stream fileStream)
        {
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));

            var bank = await _unitOfWork.QuestionBanks.GetByIdAsync(bankId);
            if (bank == null) throw new NotFoundException($"Không tìm thấy Ngân hàng câu hỏi ID={bankId}");

            var questionsToAdd = new List<Question>();
            var errorMessages = new List<string>();
            var validQuestionTypes = new[] { "MultipleChoice", "ShortAnswer" };

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(fileStream))
                {
                    if (package.Workbook.Worksheets.Count == 0)
                        throw new BusinessRuleException("File Excel không có worksheet nào.");

                    var worksheet = package.Workbook.Worksheets[0];

                    if (worksheet.Dimension == null)
                        throw new BusinessRuleException("File Excel không có dữ liệu.");

                    var rowCount = worksheet.Dimension.Rows;
                    if (rowCount < 2)
                        throw new BusinessRuleException("File Excel phải có ít nhất 1 dòng dữ liệu (không tính header).");

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var questionText = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        var questionType = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                        var optionA = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                        var optionB = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                        var optionC = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                        var optionD = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                        var correctIndexRaw = worksheet.Cells[row, 7].Value?.ToString()?.Trim();

                        if (string.IsNullOrWhiteSpace(questionText) &&
                            string.IsNullOrWhiteSpace(questionType) &&
                            string.IsNullOrWhiteSpace(optionA) &&
                            string.IsNullOrWhiteSpace(optionB))
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(questionText))
                        {
                            errorMessages.Add($"Dòng {row}: Thiếu QuestionText.");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(questionType))
                        {
                            errorMessages.Add($"Dòng {row}: Thiếu QuestionType.");
                            continue;
                        }

                        if (!validQuestionTypes.Contains(questionType, StringComparer.OrdinalIgnoreCase))
                        {
                            errorMessages.Add($"Dòng {row}: QuestionType không hợp lệ. Chấp nhận: {string.Join(", ", validQuestionTypes)}.");
                            continue;
                        }

                        var q = new Question
                        {
                            Id = Guid.NewGuid(),
                            QuestionBankId = bankId,
                            QuestionText = questionText,
                            QuestionType = questionType,
                            QuestionOptions = new List<QuestionOption>()
                        };

                        if (questionType.Equals("MultipleChoice", StringComparison.OrdinalIgnoreCase))
                        {
                            var optionsList = new List<(int idx, string text)>();
                            if (!string.IsNullOrWhiteSpace(optionA)) optionsList.Add((0, optionA));
                            if (!string.IsNullOrWhiteSpace(optionB)) optionsList.Add((1, optionB));
                            if (!string.IsNullOrWhiteSpace(optionC)) optionsList.Add((2, optionC));
                            if (!string.IsNullOrWhiteSpace(optionD)) optionsList.Add((3, optionD));

                            if (optionsList.Count < 2)
                            {
                                errorMessages.Add($"Dòng {row}: MultipleChoice phải có ít nhất 2 options (A và B tối thiểu).");
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(correctIndexRaw) || !int.TryParse(correctIndexRaw, out int correctIndex))
                            {
                                errorMessages.Add($"Dòng {row}: Thiếu hoặc không hợp lệ CorrectOptionIndex (0-based).");
                                continue;
                            }

                            if (!optionsList.Any(o => o.idx == correctIndex))
                            {
                                errorMessages.Add($"Dòng {row}: CorrectOptionIndex ({correctIndex}) không tương ứng với option có dữ liệu.");
                                continue;
                            }

                            foreach (var (idx, text) in optionsList)
                            {
                                q.QuestionOptions.Add(new QuestionOption
                                {
                                    Id = Guid.NewGuid(),
                                    QuestionId = q.Id,
                                    OptionIndex = idx,
                                    OptionText = text,
                                    IsCorrect = (idx == correctIndex)
                                });
                            }

                            var correctOpt = q.QuestionOptions.FirstOrDefault(o => o.IsCorrect);
                            q.CorrectAnswer = correctOpt?.OptionText;
                        }
                        else 
                        {
                            var shortAnswer = optionA;
                            if (string.IsNullOrWhiteSpace(shortAnswer))
                            {
                                errorMessages.Add($"Dòng {row}: ShortAnswer cần có đáp án (đặt vào cột OptionA).");
                                continue;
                            }
                            q.CorrectAnswer = shortAnswer;
                        }

                        questionsToAdd.Add(q);
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

            if (errorMessages.Any())
            {
                var errorSummary = string.Join(Environment.NewLine, errorMessages);
                throw new BusinessRuleException($"Có lỗi trong file Excel:{Environment.NewLine}{errorSummary}");
            }

            if (!questionsToAdd.Any())
            {
                throw new BusinessRuleException("File Excel không có dữ liệu hợp lệ.");
            }

            await _unitOfWork.Questions.AddRangeAsync(questionsToAdd);
            await _unitOfWork.CompleteAsync();
        }
    }
}
