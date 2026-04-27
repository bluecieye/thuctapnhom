using BaseCore.Entities;
using BaseCore.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly MySqlDbContext _context;

        public OrderService(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            var maxOrder = await _context.Orders
                .OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();
            order.Id = (maxOrder?.Id ?? 0) + 1;

            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
