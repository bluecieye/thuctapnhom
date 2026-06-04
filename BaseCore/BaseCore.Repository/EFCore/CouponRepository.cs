

using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════
    public interface ICouponRepositoryEF : IRepository<Coupon>
    {

        Task<Coupon?> GetByCodeAsync(string code);

        Task<List<Coupon>> GetActiveAsync(DateTime now);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════
    public class CouponRepositoryEF : Repository<Coupon>, ICouponRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public CouponRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        public Task<Coupon?> GetByCodeAsync(string code)
            => _dbSet.FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());



        public Task<List<Coupon>> GetActiveAsync(DateTime now)
            => _dbSet
                .Where(c => c.IsActive && c.StartDate <= now && c.EndDate >= now
                            && (c.UsageLimit == 0 || c.UsedCount < c.UsageLimit))
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();
    }
}
