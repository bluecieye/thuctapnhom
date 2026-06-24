

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
        private readonly IEmailService _emailService;

        

        

        
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
        public OrderService(MySqlDbContext context, ICouponService couponService, IShippingService shippingService, IEmailService emailService)
        {
            _context = context;
            _couponService = couponService;
            _shippingService = shippingService;
            _emailService = emailService;
        }

        

        

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // ĐẶT HÀNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // PLACE ORDER
        // ════════════════════════════════════════════════════════════
        public async Task<Order> PlaceOrderAsync(CreateOrderRequest request)
        {

            // Khách vãng lai (guest) khi không có UserId hợp lệ.
            var isGuest = !request.UserId.HasValue || request.UserId.Value == 0;

            // ── Phân giải địa chỉ giao hàng + thông tin người nhận ──
            // Thành viên: lấy từ sổ địa chỉ (AddressId gắn UserId).
            // Khách vãng lai: nhận địa chỉ nhập trực tiếp trong request.
            int provinceId;
            string shippingAddress;
            string? customerName;
            string? customerPhone;
            string? customerEmail = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

            if (isGuest)
            {
                if (string.IsNullOrWhiteSpace(request.GuestName)
                    || string.IsNullOrWhiteSpace(request.GuestPhone)
                    || string.IsNullOrWhiteSpace(request.GuestStreet)
                    || !request.GuestProvinceId.HasValue)
                    throw new InvalidOperationException("Vui lòng nhập đầy đủ thông tin giao hàng.");

                if (string.IsNullOrWhiteSpace(customerEmail))
                    throw new InvalidOperationException("Vui lòng nhập email để nhận xác nhận đơn hàng.");

                var province = await _context.Provinces.FirstOrDefaultAsync(p => p.Id == request.GuestProvinceId.Value)
                    ?? throw new InvalidOperationException("Tỉnh/thành phố không hợp lệ.");

                provinceId = province.Id;
                customerName = request.GuestName!.Trim();
                customerPhone = request.GuestPhone!.Trim();
                shippingAddress = $"{customerName}, {customerPhone}, {request.GuestStreet}, {request.GuestWard}, {province.Name}";
            }
            else
            {
                var address = await _context.Addresses
                    .Include(a => a.Province)
                    .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.UserId)
                    ?? throw new InvalidOperationException("Địa chỉ giao hàng không hợp lệ.");

                provinceId = address.ProvinceId;
                customerName = address.FullName;
                customerPhone = address.Phone;
                shippingAddress = $"{address.FullName}, {address.Phone}, {address.Street}, {address.Ward}, {address.Province?.Name}";

                // Nếu thành viên không nhập email riêng, dùng email tài khoản.
                if (string.IsNullOrWhiteSpace(customerEmail))
                    customerEmail = (await _context.Users.FindAsync(request.UserId!.Value))?.Email;
            }


            // Khách vãng lai bắt buộc gửi kèm Items (không có giỏ hàng phía server).
            var items = request.Items;
            if ((items == null || items.Count == 0) && !isGuest)
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId);
                items = cart?.CartItems
                    .Select(i => new CreateOrderItem { VariantId = i.VariantId, Quantity = i.Quantity })
                    .ToList() ?? new List<CreateOrderItem>();
            }

            if (items == null || items.Count == 0)
                throw new InvalidOperationException("Đơn hàng không có sản phẩm.");

            

            
            var variantIds = items.Select(i => i.VariantId).Distinct().ToList();
            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Size)
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
                    Variant = v,                 // gắn navigation để email đọc tên SP/màu/size
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
            }

            

            

            
            
            decimal discount = 0;
            int? couponId = null;
            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var result = await _couponService.ApplyAsync(request.CouponCode, subtotal, request.UserEmail);
                if (result.IsValid)
                {
                    discount = result.DiscountAmount;
                    couponId = result.Coupon!.Id;
                    
                    result.Coupon.UsedCount += 1;
                }
            }

            

            
            
            var quote = await _shippingService.QuoteAsync(
                request.ShippingCarrierId, provinceId, subtotal - discount);
            if (!quote.Available)
                throw new InvalidOperationException(quote.UnavailableReason ?? "Đơn vị vận chuyển không hỗ trợ địa chỉ này.");

            var shippingFee = quote.Fee;
            var total = subtotal - discount + shippingFee;

            

            

            var order = new Order
            {
                OrderCode = await GenerateOrderCodeAsync(),
                UserId = isGuest ? null : request.UserId,
                Status = "Pending",
                PaymentMethod = request.PaymentMethod,
                ShippingAddress = shippingAddress,
                CustomerEmail = customerEmail,
                CustomerName = customerName,
                CustomerPhone = customerPhone,
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

            // Chỉ xoá giỏ phía server cho THÀNH VIÊN đặt từ giỏ (không gửi Items).
            // Khách vãng lai giữ giỏ ở localStorage, FE sẽ tự xoá sau khi đặt.
            if (!isGuest && (request.Items == null || request.Items.Count == 0))
            {
                var cartItems = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == request.UserId)
                    .ToListAsync();
                _context.CartItems.RemoveRange(cartItems);
            }



            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            // Gửi email xác nhận đơn — KHÔNG để lỗi gửi mail làm hỏng đơn đã đặt.
            try
            {
                if (!string.IsNullOrWhiteSpace(order.CustomerEmail))
                    await _emailService.SendOrderConfirmationAsync(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Gửi mail thất bại cho {order.CustomerEmail}: {ex.Message}");
            }

            return await GetByIdAsync(order.Id) ?? order;
        }

        // ════════════════════════════════════════════════════════════
        // SINH MÃ ĐƠN HÀNG DUY NHẤT
        // ════════════════════════════════════════════════════════════
        private async Task<string> GenerateOrderCodeAsync()
        {
            // Bỏ ký tự dễ nhầm (0/O, 1/I). Thử lại nếu trùng (xác suất cực thấp).
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            for (var attempt = 0; attempt < 10; attempt++)
            {
                var rnd = Random.Shared;
                var code = "MOON-" + new string(Enumerable.Range(0, 6)
                    .Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
                if (!await _context.Orders.AnyAsync(o => o.OrderCode == code))
                    return code;
            }
            // Hi hữu: fallback theo timestamp đảm bảo không trùng.
            return "MOON-" + DateTime.UtcNow.Ticks.ToString("X");
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
        // TRA CỨU ĐƠN (KHÁCH VÃNG LAI)
        // ════════════════════════════════════════════════════════════
        public async Task<Order?> TrackAsync(string orderCode, string contact)
        {
            if (string.IsNullOrWhiteSpace(orderCode) || string.IsNullOrWhiteSpace(contact))
                return null;

            var code = orderCode.Trim();
            var c = contact.Trim();

            var order = await _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Product).ThenInclude(p => p.Images)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Color)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Variant).ThenInclude(v => v.Size)
                .Include(o => o.Coupon)
                .Include(o => o.ShippingCarrier)
                .FirstOrDefaultAsync(o => o.OrderCode == code);

            if (order == null) return null;

            // Xác minh quyền xem: phải khớp email HOẶC số điện thoại đã đặt.
            // Tránh việc dò đơn người khác chỉ bằng mã.
            var match =
                (!string.IsNullOrWhiteSpace(order.CustomerEmail) && order.CustomerEmail.Equals(c, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(order.CustomerPhone) && order.CustomerPhone == c);

            return match ? order : null;
        }

        

        

        

        

        
        // ════════════════════════════════════════════════════════════
        // CHUYỂN TRẠNG THÁI & HỦY ĐƠN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // STATUS TRANSITION
        // ════════════════════════════════════════════════════════════
        public async Task<Order?> ChangeStatusAsync(int orderId, string newStatus, string? note = null, string? trackingNumber = null)
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
            if (!string.IsNullOrEmpty(trackingNumber) && newStatus.Equals("Shipping", StringComparison.OrdinalIgnoreCase))
                order.TrackingNumber = trackingNumber;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            // Thông báo email khi trạng thái thay đổi — lỗi không ảnh hưởng đến đơn hàng.
            try
            {
                if (!string.IsNullOrWhiteSpace(order.CustomerEmail))
                    await _emailService.SendOrderStatusUpdateAsync(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Status update email thất bại cho {order.CustomerEmail}: {ex.Message}");
            }

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
