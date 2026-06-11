

using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Common.Helpers;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    public interface IOrderRepositoryEF : IRepository<Order>
    {

        Task<List<Order>> GetByUserAsync(int userId);

        Task<Order?> GetWithDetailsAsync(int orderId);

        Task<(List<Order> Orders, int TotalCount)> SearchAdminAsync(
            string? keyword, string? status,
            decimal? minAmount, decimal? maxAmount,
            DateTime? dateFrom, DateTime? dateTo,
            int page, int pageSize);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    public class OrderRepositoryEF : Repository<Order>, IOrderRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public OrderRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        public Task<List<Order>> GetByUserAsync(int userId)
            => _dbSet
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();



        public Task<Order?> GetWithDetailsAsync(int orderId)
            => _dbSet
                .Include(o => o.User)
                .Include(o => o.Coupon)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Product)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Color)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Size)
                .FirstOrDefaultAsync(o => o.Id == orderId);

        // ════════════════════════════════════════════════════════════
        // TÌM KIẾM ADMIN
        // ════════════════════════════════════════════════════════════

        public async Task<(List<Order> Orders, int TotalCount)> SearchAdminAsync(
            string? keyword, string? status,
            decimal? minAmount, decimal? maxAmount,
            DateTime? dateFrom, DateTime? dateTo,
            int page, int pageSize)
        {

            
            var query = _dbSet.Include(o => o.User).AsQueryable();

            
            
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(o =>
                    EF.Functions.Collate(o.User.Username, SearchHelper.Collation).Contains(kw) ||
                    EF.Functions.Collate(o.ShippingAddress, SearchHelper.Collation).Contains(kw));
            }

            
            
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(o => o.Status == status);
            if (minAmount.HasValue) query = query.Where(o => o.TotalAmount >= minAmount);
            if (maxAmount.HasValue) query = query.Where(o => o.TotalAmount <= maxAmount);
            if (dateFrom.HasValue)  query = query.Where(o => o.CreatedAt >= dateFrom);

            if (dateTo.HasValue)    query = query.Where(o => o.CreatedAt <= dateTo.Value.AddDays(1));

            var total = await query.CountAsync();

            
            
            var orders = await query
                .OrderByDescending(o => o.Id)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();
            return (orders, total);
        }
    }
}
