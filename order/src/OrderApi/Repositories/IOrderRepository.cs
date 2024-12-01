using OrderApi.Models;

namespace OrderApi.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task CreateOrderAsync(Order order);
        Task UpdateOrderStatusAsync(int id, string status);
    }
}
