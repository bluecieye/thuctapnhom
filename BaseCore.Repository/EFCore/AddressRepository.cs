

using Microsoft.EntityFrameworkCore;

using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY ĐỊA CHỈ
    // ════════════════════════════════════════════════════════════
    public interface IAddressRepositoryEF : IRepository<Address>
    {

        Task<List<Address>> GetByUserAsync(int userId);

        Task<Address?> GetDefaultAsync(int userId);

        Task SetDefaultAsync(int userId, int addressId);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY ĐỊA CHỈ
    // ════════════════════════════════════════════════════════════
    public class AddressRepositoryEF : Repository<Address>, IAddressRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public AddressRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════



        public Task<List<Address>> GetByUserAsync(int userId)
            => _dbSet.Where(a => a.UserId == userId)
                     .OrderByDescending(a => a.IsDefault)
                     .ThenByDescending(a => a.Id)
                     .ToListAsync();

        

        
        
        public Task<Address?> GetDefaultAsync(int userId)
            => _dbSet.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC THAY ĐỔI
        // ════════════════════════════════════════════════════════════

        public async Task SetDefaultAsync(int userId, int addressId)
        {
            var addresses = await _dbSet.Where(a => a.UserId == userId).ToListAsync();
            foreach (var a in addresses) a.IsDefault = a.Id == addressId;
            await _context.SaveChangesAsync();
        }
    }
}
