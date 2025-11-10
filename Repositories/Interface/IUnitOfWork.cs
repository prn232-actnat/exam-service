using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        // Khai báo TẤT CẢ các repository cụ thể của bạn ở đây
        // Lớp Service sẽ truy cập vào các repository thông qua các thuộc tính này
        IExamRepository Exams { get; }
        IQuestionRepository Questions { get; }
        IQuestionBankRepository QuestionBanks { get; }
        ISubmissionRepository Submissions { get; }
        IExamSetRepository ExamSets { get; }
        ISubmissionAnswerRepository SubmissionAnswers { get; }

        /// <summary>
        /// Lưu tất cả các thay đổi vào CSDL trong một transaction
        /// </summary>
        /// <returns>Số lượng bản ghi đã bị ảnh hưởng</returns>
        Task<int> CompleteAsync();
    }
}
