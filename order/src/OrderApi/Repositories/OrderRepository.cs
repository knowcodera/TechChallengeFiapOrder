using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;

namespace OrderApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _context;

        public OrderRepository(OrderContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync() =>
            await _context.Orders.Include(o => o.Items).ToListAsync();

        public async Task<Order> GetOrderByIdAsync(int id) =>
            await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusAsync(int id, string status)
        {
            var order = await GetOrderByIdAsync(id);
            if (order != null)
            {
                order.Status = status;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
