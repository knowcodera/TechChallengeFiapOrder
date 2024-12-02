using Microsoft.AspNetCore.Mvc;
using OrderApi.Models;
using OrderApi.Repositories;
using OrderApi.Services;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("v1/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly IRabbitMQPublisher _publisher;

        public OrderController(IOrderRepository repository, IRabbitMQPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders() =>
            Ok(await _repository.GetOrdersAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _repository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            await _repository.CreateOrderAsync(order);

            var message = $"Order {order.Id} created with {order.Items.Count} items";
            _publisher.Publish("orders_queue", message);

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            await _repository.UpdateOrderStatusAsync(id, status);

            var message = $"Order {id} updated to status {status}";
            _publisher.Publish("orders_queue", message);

            return NoContent();
        }
    }
}
