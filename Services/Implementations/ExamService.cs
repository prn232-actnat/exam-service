using AutoMapper;
using BO.Entities;
using DTOs.Request.Exam;
using DTOs.Response;
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
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ExamDetailResponse> CreateExamAsync(CreateExamRequest createDto)
        {
            if (createDto.QuestionIds == null || !createDto.QuestionIds.Any())
            {
                throw new BusinessRuleException("Bài thi phải có ít nhất 1 câu hỏi.");
            }

            var questions = new List<Question>();
            foreach (var qId in createDto.QuestionIds.Distinct())
            {
                var question = await _unitOfWork.Questions.GetByIdAsync(qId);
                if (question == null)
                {
                    throw new BusinessRuleException($"Không tìm thấy câu hỏi với ID={qId}");
                }
                questions.Add(question);
            }

            var exam = _mapper.Map<Exam>(createDto);
            exam.Id = Guid.NewGuid(); // Tạo Guid mới cho Exam
            await _unitOfWork.Exams.AddAsync(exam);

            int order = 1;
            foreach (var question in questions)
            {
                exam.ExamQuestions.Add(new ExamQuestion
                {
                    Question = question,
                    Exam = exam,
                    Order = order++
                });
            }

            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ExamDetailResponse>(exam);
        }

        public async Task DeleteExamAsync(Guid examId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
            if (exam == null)
            {
                throw new NotFoundException($"Không tìm thấy bài thi với ID={examId}");
            }
            _unitOfWork.Exams.Delete(exam);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ExamSummaryResponse>> GetAllExamsAsync()
        {
            var exams = await _unitOfWork.Exams.GetAllAsync();
            return _mapper.Map<IEnumerable<ExamSummaryResponse>>(exams);
        }

        public async Task<ExamDetailResponse> GetExamDetailsAsync(Guid examId)
        {
            var exam = await _unitOfWork.Exams.GetExamWithQuestionsAsync(examId);
            if (exam == null)
            {
                throw new NotFoundException($"Không tìm thấy bài thi với ID={examId}");
            }
            return _mapper.Map<ExamDetailResponse>(exam);
        }

        public async Task UpdateExamAsync(Guid examId, UpdateExamRequest updateDto)
        {
            var exam = await _unitOfWork.Exams.GetExamWithQuestionsAsync(examId);
            if (exam == null)
            {
                throw new NotFoundException($"Không tìm thấy bài thi với ID={examId}");
            }

            _mapper.Map(updateDto, exam);

            exam.ExamQuestions.Clear();
            var questions = new List<Question>();
            foreach (var qId in updateDto.QuestionIds.Distinct())
            {
                var question = await _unitOfWork.Questions.GetByIdAsync(qId);
                if (question == null)
                {
                    throw new BusinessRuleException($"Không tìm thấy câu hỏi với ID={qId}");
                }
                questions.Add(question);
            }

            int order = 1;
            foreach (var question in questions)
            {
                exam.ExamQuestions.Add(new ExamQuestion
                {
                    QuestionId = question.Id,
                    ExamId = exam.Id,
                    Order = order++
                });
            }

            _unitOfWork.Exams.Update(exam);
            await _unitOfWork.CompleteAsync();
        }
    }
}
