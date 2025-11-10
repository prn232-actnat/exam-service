using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Lấy một đối tượng bằng ID (khóa chính)
        /// </summary>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Lấy tất cả các đối tượng
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Tìm các đối tượng dựa trên một biểu thức (predicate)
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Thêm một đối tượng mới
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Thêm một danh sách các đối tượng mới
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Đánh dấu đối tượng là đã bị thay đổi (cho Update)
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Đánh dấu đối tượng là đã bị xóa
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Đánh dấu một danh sách đối tượng là đã bị xóa
        /// </summary>
        void DeleteRange(IEnumerable<T> entities);
    }
}
