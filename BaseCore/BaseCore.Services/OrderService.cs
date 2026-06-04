

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Repository;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // SERVICE ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // ORDER SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class OrderService : IOrderService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // BIẾN THÀNH VIÊN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;

        private readonly ICouponService _couponService;
        private readonly IShippingService _shippingService;

        

        

        
        // ════════════════════════════════════════════════════════════
        // HẰNG SỐ & TRẠNG THÁI TĨNH
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // BẢNG CHUYỂN TRẠNG THÁI HỢP LỆ
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CONSTANTS / STATIC STATE
        // ════════════════════════════════════════════════════════════
        private static readonly Dictionary<string, string[]> AllowedTransitions = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Pending"]    = new[] { "Processing", "Cancelled" },
            ["Processing"] = new[] { "Shipping", "Cancelled" },
            ["Shipping"]   = new[] { "Delivered", "Cancelled" },
            ["Delivered"]  = Array.Empty<string>(),  
            ["Cancelled"]  = Array.Empty<string>()   
        };

        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════
        public OrderService(MySqlDbContext context, ICouponService couponService, IShippingService shippingService)
        {
            _context = context;
            _couponService = couponService;
            _shippingService = shippingService;
        }

        

        

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // ĐẶT HÀNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // PLACE ORDER
        // ════════════════════════════════════════════════════════════
        public async Task<Order> PlaceOrderAsync(CreateOrderRequest request)
        {

            
            
            var address = await _context.Addresses
                .Include(a => a.Province)
                .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.UserId)
                ?? throw new InvalidOperationException("Địa chỉ giao hàng không hợp lệ.");

            

            
            var items = request.Items;
            if (items == null || items.Count == 0)
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId);
                items = cart?.CartItems
                    .Select(i => new CreateOrderItem { VariantId = i.VariantId, Quantity = i.Quantity })
                    .ToList() ?? new List<CreateOrderItem>();
            }

            if (items.Count == 0)
                throw new InvalidOperationException("Đơn hàng không có sản phẩm.");

            

            
            var variantIds = items.Select(i => i.VariantId).Distinct().ToList();
            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => variantIds.Contains(v.Id))
                .ToListAsync();
            if (variants.Count != variantIds.Count)
                throw new InvalidOperationException("Một số biến thể không tồn tại.");

            

            using var tx = await _context.Database.BeginTransactionAsync();

            

            

            
            decimal subtotal = 0;
            var details = new List<OrderDetail>();
            foreach (var item in items)
            {
                
                var v = variants.First(x => x.Id == item.VariantId);

                if (v.Stock - v.ReservedStock < item.Quantity)
                    throw new InvalidOperationException($"Không đủ tồn kho cho SKU {v.SKU}.");

                v.ReservedStock += item.Quantity;

                var unitPrice = v.Price;
                subtotal += unitPrice * item.Quantity;

                details.Add(new OrderDetail
                {
                    VariantId = v.Id,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
            }

            

            

            
            
            decimal discount = 0;
            int? couponId = null;
            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var result = await _couponService.ApplyAsync(request.CouponCode, subtotal);
                if (result.IsValid)
                {
                    discount = result.DiscountAmount;
                    couponId = result.Coupon!.Id;
                    
                    result.Coupon.UsedCount += 1;
                }
            }

            

            
            
            var quote = await _shippingService.QuoteAsync(
                request.ShippingCarrierId, address.ProvinceId, subtotal - discount);
            if (!quote.Available)
                throw new InvalidOperationException(quote.UnavailableReason ?? "Đơn vị vận chuyển không hỗ trợ địa chỉ này.");

            var shippingFee = quote.Fee;
            var total = subtotal - discount + shippingFee;

            

            

            var order = new Order
            {
                UserId = request.UserId,
                Status = "Pending",                     
                PaymentMethod = request.PaymentMethod,
                ShippingAddress = $"{address.FullName}, {address.Phone}, {address.Street}, {address.Ward}, {address.Province?.Name}",
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
                EstimatedDelivery = DateTime.UtcNow.AddDays(quote.EstimatedDays > 0 ? quote.EstimatedDays : 3),
                CouponId = couponId,
                ShippingCarrierId = request.ShippingCarrierId,
                ShippingFee = shippingFee,
                DiscountAmount = discount,
                TotalAmount = total,
                OrderDetails = details                  
            };
            _context.Orders.Add(order);

            

            if (request.Items == null || request.Items.Count == 0)
            {
                var cartItems = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == request.UserId)
                    .ToListAsync();
                _context.CartItems.RemoveRange(cartItems);
            }

            

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return order;
        }

        

        
        // ════════════════════════════════════════════════════════════
        // TRUY VẤN ĐƠN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // LẤY ĐƠN HÀNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // QUERY METHODS
        // ════════════════════════════════════════════════════════════
        public Task<List<Order>> GetByUserAsync(int userId)
            => _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Product)
                .Include(o => o.ShippingCarrier)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        

        
        public Task<Order?> GetByIdAsync(int id)
            => _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Product).ThenInclude(p => p.Images)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Color)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Size)
                .Include(o => o.Coupon)
                .Include(o => o.ShippingCarrier)
                .FirstOrDefaultAsync(o => o.Id == id);

        

        

        

        

        
        // ════════════════════════════════════════════════════════════
        // CHUYỂN TRẠNG THÁI & HỦY ĐƠN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // STATUS TRANSITION
        // ════════════════════════════════════════════════════════════
        public async Task<Order?> ChangeStatusAsync(int orderId, string newStatus, string? note = null)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return null;

            
            if (!AllowedTransitions.TryGetValue(order.Status, out var allowed) || !allowed.Contains(newStatus, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Không thể chuyển từ {order.Status} sang {newStatus}.");

            using var tx = await _context.Database.BeginTransactionAsync();

            if (newStatus.Equals("Processing", StringComparison.OrdinalIgnoreCase) && order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                await CommitReservedStock(order);
            else if (newStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                await ReleaseStock(order);

            order.Status = newStatus;
            if (!string.IsNullOrEmpty(note)) order.Note = note;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return order;
        }

        

        public async Task<bool> CancelAsync(int orderId)
            => (await ChangeStatusAsync(orderId, "Cancelled")) != null;

        

        

        
        // ════════════════════════════════════════════════════════════
        // XỬ LÝ TỒN KHO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // STOCK HELPERS
        // ════════════════════════════════════════════════════════════
        private async Task CommitReservedStock(Order order)
        {
            foreach (var d in order.OrderDetails)
            {
                var v = await _context.ProductVariants.FindAsync(d.VariantId);
                if (v != null)
                {
                    v.Stock = Math.Max(0, v.Stock - d.Quantity);
                    v.ReservedStock = Math.Max(0, v.ReservedStock - d.Quantity);
                }
            }
        }

        

        

        

        
        
        private async Task ReleaseStock(Order order)
        {
            foreach (var d in order.OrderDetails)
            {
                var v = await _context.ProductVariants.FindAsync(d.VariantId);
                if (v == null) continue;
                if (order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    v.ReservedStock = Math.Max(0, v.ReservedStock - d.Quantity);
                else
                    v.Stock += d.Quantity;
            }
        }
    }
}
