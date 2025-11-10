using AutoMapper;
using BO.Entities;
using DTOs.Request.Exam;
using DTOs.Request.ExamSet;
using DTOs.Request.Question;
using DTOs.Request.QuestionBank;
using DTOs.Request.Submission;
using DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // --- Map từ Request DTO -> BO (Entity) ---
            CreateMap<CreateQuestionBankRequest, QuestionBank>();
            CreateMap<UpdateQuestionBankRequest, QuestionBank>();

            CreateMap<CreateQuestionRequest, Question>();
            CreateMap<UpdateQuestionRequest, Question>();

            CreateMap<CreateExamRequest, Exam>()
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore());
            CreateMap<UpdateExamRequest, Exam>()
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore());

            CreateMap<StartSubmissionRequest, Submission>();
            CreateMap<SubmitAnswerRequest, SubmissionAnswer>();

            CreateMap<CreateExamSetRequest, ExamSet>();

            // --- Map từ BO (Entity) -> Response DTO ---
            CreateMap<QuestionBank, QuestionBankResponse>();
            CreateMap<QuestionBank, QuestionBankSummaryResponse>();

            CreateMap<Question, QuestionResponse>(); // Sẽ tự động bỏ qua CorrectAnswer

            CreateMap<Exam, ExamSummaryResponse>();
            CreateMap<Exam, ExamDetailResponse>()
                .ForMember(dest => dest.Questions,
                           opt => opt.MapFrom(src =>
                               src.ExamQuestions.Select(eq => eq.Question)));

            CreateMap<Submission, SubmissionResultResponse>();
            CreateMap<ExamSet, ExamSetResponse>();
        }
    }
}
