using AutoMapper;
using BO.Entities;
using DTOs.Request.ExamSet;
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
    public class ExamSetService : IExamSetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamSetService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ExamSetResponse> CreateExamSetAsync(CreateExamSetRequest createDto)
        {
            var examSet = _mapper.Map<ExamSet>(createDto);
            examSet.Id = Guid.NewGuid(); // Tạo Guid mới cho ExamSet
            await _unitOfWork.ExamSets.AddAsync(examSet);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ExamSetResponse>(examSet);
        }

        public async Task<IEnumerable<ExamSetResponse>> GetAllExamSetsAsync()
        {
            var examSets = await _unitOfWork.ExamSets.GetAllAsync();
            return _mapper.Map<IEnumerable<ExamSetResponse>>(examSets);
        }

        public async Task<ExamSetResponse> GetExamSetByIdAsync(Guid id)
        {
            var examSet = await _unitOfWork.ExamSets.GetByIdAsync(id);
            if (examSet == null)
            {
                throw new NotFoundException($"Không tìm thấy Bộ đề thi ID={id}");
            }
            return _mapper.Map<ExamSetResponse>(examSet);
        }
    }
}
