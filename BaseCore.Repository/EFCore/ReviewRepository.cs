

using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════
    public interface IReviewRepositoryEF : IRepository<Review>
    {

        Task<List<Review>> GetByProductAsync(int productId);

        Task<List<Review>> GetAllAdminAsync(int page, int pageSize);

        Task<int> CountAllAsync();

        Task<(double Average, int Count)> GetRatingSummaryAsync(int productId);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════
    public class ReviewRepositoryEF : Repository<Review>, IReviewRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public ReviewRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        public Task<List<Review>> GetByProductAsync(int productId)
            => _dbSet
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        

        
        public Task<List<Review>> GetAllAdminAsync(int page, int pageSize)
            => _dbSet
                .Include(r => r.User)
                .Include(r => r.Product)

                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

        

        public Task<int> CountAllAsync()
            => _dbSet.CountAsync();

        // ════════════════════════════════════════════════════════════
        // TỔNG HỢP
        // ════════════════════════════════════════════════════════════

        public async Task<(double Average, int Count)> GetRatingSummaryAsync(int productId)
        {
            var reviews = _dbSet.Where(r => r.ProductId == productId);
            var count = await reviews.CountAsync();
            if (count == 0) return (0, 0);
            var avg = await reviews.AverageAsync(r => (double)r.Rating);
            return (Math.Round(avg, 2), count);
        }
    }
}
