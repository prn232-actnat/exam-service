using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        // Dùng 'protected' để các lớp con (ExamRepository) có thể truy cập
        protected readonly ExamServiceDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ExamServiceDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            // FindAsync là cách tối ưu nhất để lấy bằng khóa chính
            return await _dbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
            // Đánh dấu entity là đã bị sửa đổi
            _dbSet.Update(entity);
        }
    }
}

