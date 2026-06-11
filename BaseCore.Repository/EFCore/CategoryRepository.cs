

using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY DANH MỤC
    // ════════════════════════════════════════════════════════════
    public interface ICategoryRepositoryEF : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY DANH MỤC
    // ════════════════════════════════════════════════════════════
    public class CategoryRepositoryEF : Repository<Category>, ICategoryRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public CategoryRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        public override async Task<IEnumerable<Category>> GetAllAsync()
            => await _dbSet.OrderBy(c => c.Name).ToListAsync();



        public Task<Category?> GetByNameAsync(string name)
            => _dbSet.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
    }
}
