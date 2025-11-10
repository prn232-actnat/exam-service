using AutoMapper;
using BO.Entities;
using DTOs.Request.Submission;
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
    public class SubmissionService : ISubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubmissionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubmissionResultResponse> GetSubmissionResultAsync(Guid submissionId)
        {
            var submission = await _unitOfWork.Submissions.GetByIdAsync(submissionId);
            if (submission == null)
            {
                throw new NotFoundException($"Không tìm thấy lượt làm bài ID={submissionId}");
            }
            return _mapper.Map<SubmissionResultResponse>(submission);
        }

        public async Task<SubmissionResultResponse> StartExamAsync(StartSubmissionRequest requestDto)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(requestDto.ExamId);
            if (exam == null)
            {
                throw new BusinessRuleException($"Không tìm thấy bài thi ID={requestDto.ExamId}");
            }

            var submission = _mapper.Map<Submission>(requestDto);
            submission.Id = Guid.NewGuid(); // Tạo Guid mới cho Submission
            submission.StartedAt = DateTime.UtcNow;

            await _unitOfWork.Submissions.AddAsync(submission);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SubmissionResultResponse>(submission);
        }

        public async Task<SubmissionResultResponse> SubmitExamAsync(Guid submissionId, SubmitExamRequest answersDto)
        {
            var submission = await _unitOfWork.Submissions.GetByIdAsync(submissionId);
            if (submission == null)
            {
                throw new NotFoundException($"Không tìm thấy lượt làm bài ID={submissionId}");
            }

            if (submission.SubmittedAt != null)
            {
                throw new BusinessRuleException("Bài thi này đã được nộp trước đó.");
            }

            var exam = await _unitOfWork.Exams.GetExamWithQuestionsAsync(submission.ExamId);
            if (exam == null)
            {
                throw new NotFoundException($"Không tìm thấy bài thi ID={submission.ExamId}");
            }

            var timeLimit = submission.StartedAt.AddMinutes(exam.DurationInMinutes);
            if (DateTime.UtcNow > timeLimit.AddMinutes(1)) // Thêm 1 phút du di
            {
                throw new BusinessRuleException("Đã hết thời gian làm bài.");
            }

            int correctCount = 0;
            var questionsInExam = exam.ExamQuestions.Select(eq => eq.Question).ToList();

            foreach (var answerDto in answersDto.Answers)
            {
                var question = questionsInExam.FirstOrDefault(q => q.Id == answerDto.QuestionId);
                if (question == null) continue;

                var answer = _mapper.Map<SubmissionAnswer>(answerDto);
                answer.Id = Guid.NewGuid(); // Tạo Guid mới cho SubmissionAnswer
                answer.SubmissionId = submissionId;

                // So sánh đáp án (nên dùng StringComparison.OrdinalIgnoreCase
                // để không phân biệt hoa thường)
                answer.IsCorrect = string.Equals(
                    question.CorrectAnswer,
                    answer.SelectedAnswer,
                    StringComparison.OrdinalIgnoreCase);

                if (answer.IsCorrect)
                {
                    correctCount++;
                }
                await _unitOfWork.SubmissionAnswers.AddAsync(answer);
            }

            submission.SubmittedAt = DateTime.UtcNow;
            submission.Score = (double)correctCount / questionsInExam.Count * 10.0; // Tính điểm thang 10

            _unitOfWork.Submissions.Update(submission);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SubmissionResultResponse>(submission);
        }
    }
}
