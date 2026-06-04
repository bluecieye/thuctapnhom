

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // DTO KẾT QUẢ ÁP DỤNG MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // COUPON RESULT — DTO
    // ════════════════════════════════════════════════════════════
    public class CouponResult
    {
        
        public Coupon? Coupon { get; set; }

        public decimal DiscountAmount { get; set; }

        public string? Message { get; set; }

        public bool IsValid => Coupon != null && Message == null;
    }

    // ════════════════════════════════════════════════════════════
    // INTERFACE SERVICE MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // COUPON SERVICE — INTERFACE
    // ════════════════════════════════════════════════════════════
    public interface ICouponService
    {

        

        
        
        Task<CouponResult> ApplyAsync(string code, decimal subtotal);

        Task<List<Coupon>> GetActiveAsync();

        Task<List<Coupon>> GetAllAsync();

        Task<Coupon?> GetByCodeAsync(string code);

        Task<Coupon> CreateAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task DeleteAsync(int id);
    }
}
