

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository;
using BaseCore.Repository.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // SERVICE MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // COUPON SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class CouponService : ICouponService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN THÀNH VIÊN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS
        // ════════════════════════════════════════════════════════════
        private readonly ICouponRepositoryEF _repo;
        private readonly MySqlDbContext _context;

        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════
        public CouponService(ICouponRepositoryEF repo, MySqlDbContext context) { _repo = repo; _context = context; }

        // ════════════════════════════════════════════════════════════
        // QUẢN LÝ COUPON
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // QUERY & CRUD METHODS
        // ════════════════════════════════════════════════════════════
        public Task<List<Coupon>> GetActiveAsync() => _repo.GetActiveAsync(DateTime.UtcNow);

        public async Task<List<Coupon>> GetAllAsync() => (await _repo.GetAllAsync()).ToList();

        public Task<Coupon?> GetByCodeAsync(string code) => _repo.GetByCodeAsync(code);

        public Task<Coupon> CreateAsync(Coupon coupon) => _repo.AddAsync(coupon);
        public Task UpdateAsync(Coupon coupon) => _repo.UpdateAsync(coupon);
        public Task DeleteAsync(int id) => _repo.DeleteByIdAsync(id);

        

        

        

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // KIỂM TRA & ÁP DỤNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // APPLY COUPON
        // ════════════════════════════════════════════════════════════
        public async Task<CouponResult> ApplyAsync(string code, decimal subtotal, string? userEmail = null)
        {

            if (string.IsNullOrWhiteSpace(code))
                return new CouponResult { Message = "Mã giảm giá trống." };

            var coupon = await _repo.GetByCodeAsync(code.Trim());
            if (coupon == null)
                return new CouponResult { Message = "Mã giảm giá không tồn tại." };

            var now = DateTime.UtcNow;
            if (!coupon.IsActive || coupon.StartDate > now || coupon.EndDate < now)
                return new CouponResult { Coupon = coupon, Message = "Mã giảm giá đã hết hạn." };

            if (coupon.IsNewsletterCoupon)
            {
                if (string.IsNullOrEmpty(userEmail))
                    return new CouponResult { Coupon = coupon, Message = "Mã này chỉ dành cho thành viên đã đăng ký nhận tin. Vui lòng đăng nhập bằng email đã đăng ký." };
                var subscribed = await _context.NewsletterSubscribers
                    .AnyAsync(s => s.Email == userEmail.Trim().ToLower());
                if (!subscribed)
                    return new CouponResult { Coupon = coupon, Message = "Email tài khoản của bạn chưa đăng ký nhận tin tức." };
            }

            if (coupon.UsageLimit > 0 && coupon.UsedCount >= coupon.UsageLimit)
                return new CouponResult { Coupon = coupon, Message = "Mã giảm giá đã hết lượt sử dụng." };

            
            if (subtotal < coupon.MinOrderAmount)
                return new CouponResult { Coupon = coupon, Message = $"Đơn hàng tối thiểu {coupon.MinOrderAmount:N0}đ." };

            
            
            decimal discount = coupon.DiscountType.Equals("Percent", StringComparison.OrdinalIgnoreCase)
                || coupon.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase)
                ? Math.Round(subtotal * coupon.DiscountValue / 100m, 0) 
                : Math.Min(coupon.DiscountValue, subtotal);             

            if (coupon.MaxDiscount.HasValue)
                discount = Math.Min(discount, coupon.MaxDiscount.Value);

            return new CouponResult { Coupon = coupon, DiscountAmount = discount };
        }
    }
}
