

using System.Linq.Expressions;

namespace BaseCore.Repository.EFCore
{
    /// <summary>
    /// Generic Repository Interface for Entity Framework Core
    /// Teaching Repository Pattern (Bài 10)
    /// </summary>

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY TỔNG QUÁT
    // ════════════════════════════════════════════════════════════
    public interface IRepository<T> where T : class
    {

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        Task<T?> GetByIdAsync(object id);

        Task<IEnumerable<T>> GetAllAsync();


        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);


        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC THAY ĐỔI
        // ════════════════════════════════════════════════════════════

        Task<T> AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task DeleteByIdAsync(object id);

        // ════════════════════════════════════════════════════════════
        // PHÂN TRANG
        // ════════════════════════════════════════════════════════════

        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false);
    }
}
