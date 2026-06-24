

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // INTERFACE SERVICE ĐỊA CHỈ
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // ADDRESS SERVICE — INTERFACE
    // ════════════════════════════════════════════════════════════
    public interface IAddressService
    {

        

        
        Task<List<Address>> GetByUserAsync(int userId);

        

        
        
        Task<Address?> GetByIdAsync(int id);

        

        

        
        Task<Address> CreateAsync(Address address);

        

        
        
        Task UpdateAsync(Address address);

        

        

        Task DeleteAsync(int id);

        

        

        
        Task SetDefaultAsync(int userId, int addressId);
    }
}
