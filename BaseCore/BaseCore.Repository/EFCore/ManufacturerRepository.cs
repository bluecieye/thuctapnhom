using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{
    public interface IManufacturerRepositoryEF : IRepository<Manufacturer>
    {
        Task<Manufacturer?> GetByNameAsync(string name);
    }

    public class ManufacturerRepositoryEF : Repository<Manufacturer>, IManufacturerRepositoryEF
    {
        public ManufacturerRepositoryEF(MySqlDbContext context) : base(context)
        {
        }

        public async Task<Manufacturer?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower());
        }
    }
}
