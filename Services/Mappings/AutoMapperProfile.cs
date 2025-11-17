using AutoMapper;
using BO.Entities;
using DTOs.Request.Exam;
using DTOs.Request.ExamSet;
using DTOs.Request.Question;
using DTOs.Request.QuestionBank;
using DTOs.Request.Submission;
using DTOs.Response;
using DTOs.Response.DTOs.Response;
using System;
using System.Linq;

namespace Services.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
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

            CreateMap<QuestionBank, QuestionBankResponse>();
            CreateMap<QuestionBank, QuestionBankSummaryResponse>();

            CreateMap<Question, QuestionResponse>()
                .ForMember(dest => dest.Options,
                           opt => opt.MapFrom(src => src.QuestionOptions.OrderBy(o => o.OptionIndex)));

            CreateMap<QuestionOption, QuestionOptionResponse>();

            CreateMap<Exam, ExamSummaryResponse>();

            CreateMap<Exam, ExamDetailResponse>()
                .ForMember(dest => dest.Questions,
                           opt => opt.MapFrom(src =>
                               src.ExamQuestions
                                  .OrderBy(eq => eq.Order)
                                  .Select(eq => eq.Question)));

            CreateMap<Submission, SubmissionResultResponse>();
            CreateMap<ExamSet, ExamSetResponse>();
        }
    }
}
