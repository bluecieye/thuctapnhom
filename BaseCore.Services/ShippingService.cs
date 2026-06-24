

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaseCore.Common;
using BaseCore.Entities;
using BaseCore.Repository;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // INTERFACE SERVICE VẬN CHUYỂN
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // SHIPPING SERVICE — INTERFACE
    // ════════════════════════════════════════════════════════════
    public interface IShippingService
    {

        

        

        

        Task<ShippingQuote> QuoteAsync(int carrierId, int provinceId, decimal orderTotal);





        Task<List<ShippingQuote>> QuoteAllCarriersAsync(int provinceId, decimal orderTotal);
    }

    // ════════════════════════════════════════════════════════════
    // KẾT QUẢ TÍNH CƯỚC (SHIPPING QUOTE)
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // SHIPPING QUOTE — DTO
    // ════════════════════════════════════════════════════════════
    public class ShippingQuote
    {
        public int CarrierId { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public int? ProvinceId { get; set; }

        public decimal Fee { get; set; }

        public int EstimatedDays { get; set; }

        public bool IsFreeShip { get; set; }

        public string? FreeShipReason { get; set; }

        public bool Available { get; set; } = true;

        public string? UnavailableReason { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // SERVICE VẬN CHUYỂN
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // SHIPPING SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class ShippingService : IShippingService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS & CONFIG
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;
        public ShippingService(MySqlDbContext context) { _context = context; }

        // Ngưỡng miễn phí vận chuyển — nguồn duy nhất: Constants.FREE_SHIPPING_FROM

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // TÍNH CƯỚC 1 ĐƠN VỊ
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // SINGLE-CARRIER QUOTE
        // ════════════════════════════════════════════════════════════
        public async Task<ShippingQuote> QuoteAsync(int carrierId, int provinceId, decimal orderTotal)
        {
            
            var carrier = await _context.ShippingCarriers.FirstOrDefaultAsync(c => c.Id == carrierId && c.IsActive)
                ?? throw new InvalidOperationException("Đơn vị vận chuyển không tồn tại hoặc đã tắt.");

            var province = await _context.Provinces.FirstOrDefaultAsync(p => p.Id == provinceId)
                ?? throw new InvalidOperationException("Tỉnh/thành không hợp lệ.");

            var rate = await _context.ShippingRates
                .FirstOrDefaultAsync(r => r.CarrierId == carrierId && r.ProvinceId == provinceId);

            
            var quote = new ShippingQuote
            {
                CarrierId = carrier.Id,
                CarrierName = carrier.Name,
                ProvinceId = province.Id,
                Fee = rate?.Fee ?? 0m,
                EstimatedDays = rate?.EstimatedDays ?? 0
            };

            if (rate == null)
            {
                quote.Available = false;
                quote.UnavailableReason = $"{carrier.Name} chưa hỗ trợ giao tới {province.Name}.";
                return quote;
            }

            
            if (orderTotal >= Constants.FREE_SHIPPING_FROM)
            {
                quote.IsFreeShip = true;
                quote.Fee = 0;
                quote.FreeShipReason = $"Miễn phí ship cho đơn từ {Constants.FREE_SHIPPING_FROM:N0} đ.";
                return quote;
            }

            return quote;
        }

        

        

        
        // ════════════════════════════════════════════════════════════
        // TÍNH CƯỚC TẤT CẢ ĐƠN VỊ
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // MULTI-CARRIER QUOTE
        // ════════════════════════════════════════════════════════════
        public async Task<List<ShippingQuote>> QuoteAllCarriersAsync(int provinceId, decimal orderTotal)
        {
            var carriers = await _context.ShippingCarriers.Where(c => c.IsActive).OrderBy(c => c.Id).ToListAsync();
            var quotes = new List<ShippingQuote>();
            foreach (var c in carriers)
            {

                try { quotes.Add(await QuoteAsync(c.Id, provinceId, orderTotal)); }
                catch (Exception ex)
                {
                    quotes.Add(new ShippingQuote
                    {
                        CarrierId = c.Id, CarrierName = c.Name, Available = false,
                        UnavailableReason = ex.Message
                    });
                }
            }
            return quotes;
        }
    }
}
