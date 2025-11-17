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

            // Validate questions exist
            var questionIds = createDto.QuestionIds.Distinct().ToList();
            var questions = new List<Question>();
            foreach (var qId in questionIds)
            {
                var question = await _unitOfWork.Questions.GetByIdAsync(qId);
                if (question == null)
                {
                    throw new BusinessRuleException($"Không tìm thấy câu hỏi với ID={qId}");
                }
                questions.Add(question);
            }

            // Map createDto -> Exam entity (ignore ExamQuestions in mapping config)
            var exam = _mapper.Map<Exam>(createDto);
            exam.Id = exam.Id == Guid.Empty ? Guid.NewGuid() : exam.Id;

            // Ensure collection exists
            exam.ExamQuestions = new List<ExamQuestion>();

            // Add exam to repo (EF will track it)
            await _unitOfWork.Exams.AddAsync(exam);

            int order = 1;
            foreach (var question in questions)
            {
                // Only set IDs to avoid cross-context attach issues
                exam.ExamQuestions.Add(new ExamQuestion
                {
                    ExamId = exam.Id,
                    QuestionId = question.Id,
                    Order = order++
                });
            }

            await _unitOfWork.CompleteAsync();

            // Reload exam with questions if your repo returns detached entity (optional)
            var created = await _unitOfWork.Exams.GetExamWithQuestionsAsync(exam.Id);
            return _mapper.Map<ExamDetailResponse>(created ?? exam);
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

            // Ensure collection exists
            exam.ExamQuestions = exam.ExamQuestions ?? new List<ExamQuestion>();
            exam.ExamQuestions.Clear();

            var questionIds = (updateDto.QuestionIds ?? new List<Guid>()).Distinct().ToList();
            var questions = new List<Question>();
            foreach (var qId in questionIds)
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
