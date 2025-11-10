using Repositories.Data;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExamServiceDbContext _context;

        // --- Khai báo các thuộc tính từ Interface ---
        public IExamRepository Exams { get; private set; }
        public IQuestionRepository Questions { get; private set; }
        public IQuestionBankRepository QuestionBanks { get; private set; }
        public ISubmissionRepository Submissions { get; private set; }
        public IExamSetRepository ExamSets { get; private set; }
        public ISubmissionAnswerRepository SubmissionAnswers { get; private set; }

        public UnitOfWork(ExamServiceDbContext context)
        {
            _context = context;

            // --- Khởi tạo các Repository Implementation ---
            // Lớp Service sẽ không bao giờ biết về các lớp "ExamRepository", "QuestionRepository"...
            // Nó chỉ biết về "IExamRepository", "IQuestionRepository"...
            Exams = new ExamRepository(_context);
            Questions = new QuestionRepository(_context);
            QuestionBanks = new QuestionBankRepository(_context);
            Submissions = new SubmissionRepository(_context);
            ExamSets = new ExamSetRepository(_context);
            SubmissionAnswers = new SubmissionAnswerRepository(_context);
        }

        /// <summary>
        /// Triển khai phương thức "Lưu thay đổi"
        /// Gọi SaveChangesAsync trên DbContext
        /// </summary>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Triển khai phương thức Dispose từ IDisposable
        /// Dùng để giải phóng tài nguyên của DbContext
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}



