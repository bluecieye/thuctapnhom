

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // SERVICE DANH MỤC
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // CATEGORY SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class CategoryService : ICategoryService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN THÀNH VIÊN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS
        // ════════════════════════════════════════════════════════════
        private readonly ICategoryRepositoryEF _repo;

        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════
        public CategoryService(ICategoryRepositoryEF repo) { _repo = repo; }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC CRUD
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CRUD METHODS
        // ════════════════════════════════════════════════════════════
        public async Task<List<Category>> GetAllAsync() => (await _repo.GetAllAsync()).ToList();

        public Task<Category?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public Task<Category> CreateAsync(Category category) => _repo.AddAsync(category);

        public Task UpdateAsync(Category category) => _repo.UpdateAsync(category);

        public Task DeleteAsync(int id) => _repo.DeleteByIdAsync(id);
    }
}
