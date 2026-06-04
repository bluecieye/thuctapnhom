

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // SERVICE ĐỊA CHỈ
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // ADDRESS SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class AddressService : IAddressService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN THÀNH VIÊN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS
        // ════════════════════════════════════════════════════════════
        private readonly IAddressRepositoryEF _repo;

        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════
        public AddressService(IAddressRepositoryEF repo) { _repo = repo; }

        

        
        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // QUERY METHODS
        // ════════════════════════════════════════════════════════════
        public Task<List<Address>> GetByUserAsync(int userId) => _repo.GetByUserAsync(userId);

        
        
        public Task<Address?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        

        

        

        

        
        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC THAY ĐỔI
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CREATE / UPDATE / DELETE
        // ════════════════════════════════════════════════════════════
        public async Task<Address> CreateAsync(Address address)
        {
            
            var existing = await _repo.GetByUserAsync(address.UserId);

            if (existing.Count == 0) address.IsDefault = true;

            var saved = await _repo.AddAsync(address);

            
            
            if (address.IsDefault) await _repo.SetDefaultAsync(address.UserId, saved.Id);

            return saved;
        }

        

        public Task UpdateAsync(Address address) => _repo.UpdateAsync(address);

        

        

        public async Task DeleteAsync(int id)
        {
            var addr = await _repo.GetByIdAsync(id);
            if (addr != null) await _repo.DeleteAsync(addr);
        }

        

        // ════════════════════════════════════════════════════════════
        // ĐẶT ĐỊA CHỈ MẶC ĐỊNH
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // SET DEFAULT
        // ════════════════════════════════════════════════════════════
        public Task SetDefaultAsync(int userId, int addressId) => _repo.SetDefaultAsync(userId, addressId);
    }
}
