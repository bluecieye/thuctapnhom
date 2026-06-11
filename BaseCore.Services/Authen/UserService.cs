

using BaseCore.Common;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCore.Services.Authen
{
    // ════════════════════════════════════════════════════════════
    // INTERFACE SERVICE NGƯỜI DÙNG
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // USER SERVICE — INTERFACE
    // ════════════════════════════════════════════════════════════
    public interface IUserService
    {

        
        
        Task<User?> Authenticate(string username, string password);

        Task<List<User>> GetAll();

        Task<User?> GetById(int id);

        Task<User?> GetByUsername(string username);

        
        Task<User> Create(User user, string password);

        Task Update(User user, string? password = null);

        Task Delete(int id);

        
        
        Task<(List<User> Users, int TotalCount)> Search(
            string? keyword, int page, int pageSize, string? role = null, bool? isActive = null);
    }

    // ════════════════════════════════════════════════════════════
    // SERVICE NGƯỜI DÙNG
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // USER SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class UserService : IUserService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS
        // ════════════════════════════════════════════════════════════
        private readonly IUserRepositoryEF _repo;

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════
        public UserService(IUserRepositoryEF repo) { _repo = repo; }

        

        

        

        

        
        // ════════════════════════════════════════════════════════════
        // ĐĂNG NHẬP / XÁC THỰC
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // LOGIN / JWT
        // ════════════════════════════════════════════════════════════
        public async Task<User?> Authenticate(string username, string password)
        {
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return null;

            var user = await _repo.GetByUsernameAsync(username);
            if (user == null || !user.IsActive) return null;

            
            var ok = !string.IsNullOrEmpty(user.Salt)
                ? TokenHelper.IsValidPassword(password, user.Salt, user.PasswordHash)
                : user.PasswordHash == password; 

            return ok ? user : null;
        }

        

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // QUERY METHODS
        // ════════════════════════════════════════════════════════════
        public async Task<List<User>> GetAll() => (await _repo.GetAllAsync()).ToList();

        public Task<User?> GetById(int id) => _repo.GetByIdAsync(id);

        public Task<User?> GetByUsername(string username) => _repo.GetByUsernameAsync(username);

        

        

        
        // ════════════════════════════════════════════════════════════
        // ĐĂNG KÝ TÀI KHOẢN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // REGISTRATION
        // ════════════════════════════════════════════════════════════
        public async Task<User> Create(User user, string password)
        {
            user.PasswordHash = TokenHelper.HashPassword(password, out string salt);
            user.Salt = salt;
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            return await _repo.AddAsync(user);
        }

        

        
        
        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT & XỬ LÝ MẬT KHẨU
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // PASSWORD HELPERS
        // ════════════════════════════════════════════════════════════
        public async Task Update(User user, string? password = null)
        {
            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = TokenHelper.HashPassword(password, out string salt);
                user.Salt = salt;
            }
            await _repo.UpdateAsync(user);
        }

        

        
        
        // ════════════════════════════════════════════════════════════
        // XOÁ NGƯỜI DÙNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // DELETE USER
        // ════════════════════════════════════════════════════════════
        public async Task Delete(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _repo.UpdateAsync(user);
            }
        }

        

        // ════════════════════════════════════════════════════════════
        // TÌM KIẾM NGƯỜI DÙNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // SEARCH
        // ════════════════════════════════════════════════════════════
        public Task<(List<User> Users, int TotalCount)> Search(
            string? keyword, int page, int pageSize, string? role = null, bool? isActive = null)
            => _repo.SearchAsync(keyword, role, isActive, page, pageSize);
    }
}
