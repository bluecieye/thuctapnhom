

using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Common.Helpers;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY NGƯỜI DÙNG
    // ════════════════════════════════════════════════════════════
    public interface IUserRepositoryEF : IRepository<User>
    {

        Task<User?> GetByUsernameAsync(string username);

        Task<User?> GetByEmailAsync(string email);

        Task<bool> UsernameExistsAsync(string username);

        Task<bool> EmailExistsAsync(string email);

        Task<(List<User> Users, int TotalCount)> SearchAsync(
            string? keyword, string? role, bool? isActive, int page, int pageSize);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY NGƯỜI DÙNG
    // ════════════════════════════════════════════════════════════
    public class UserRepositoryEF : Repository<User>, IUserRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public UserRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRA CỨU
        // ════════════════════════════════════════════════════════════

        public Task<User?> GetByUsernameAsync(string username)
            => _dbSet.FirstOrDefaultAsync(u => u.Username == username);

        public Task<User?> GetByEmailAsync(string email)
            => _dbSet.FirstOrDefaultAsync(u => u.Email == email);

        // ════════════════════════════════════════════════════════════
        // KIỂM TRA TỒN TẠI
        // ════════════════════════════════════════════════════════════

        public Task<bool> UsernameExistsAsync(string username)
            => _dbSet.AnyAsync(u => u.Username == username);

        public Task<bool> EmailExistsAsync(string email)
            => _dbSet.AnyAsync(u => u.Email == email);

        // ════════════════════════════════════════════════════════════
        // TÌM KIẾM ADMIN
        // ════════════════════════════════════════════════════════════

        public async Task<(List<User> Users, int TotalCount)> SearchAsync(
            string? keyword, string? role, bool? isActive, int page, int pageSize)
        {
            
            var query = _dbSet.AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(u =>
                    EF.Functions.Collate(u.Username, SearchHelper.Collation).Contains(kw) ||
                    EF.Functions.Collate(u.Email, SearchHelper.Collation).Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(role)) query = query.Where(u => u.Role == role);
            if (isActive.HasValue) query = query.Where(u => u.IsActive == isActive);

            var total = await query.CountAsync();

            
            var users = await query
                .OrderByDescending(u => u.Id)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();
            return (users, total);
        }
    }
}
