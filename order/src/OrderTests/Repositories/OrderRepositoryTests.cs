using OrderApi.Models;
using OrderApi.Repositories;

namespace OrderTests.Repositories
{
    public class OrderRepositoryTests
    {
        [Fact]
        public async Task CreateOrderAsync_ShouldAddOrderToDatabase()
        {
            // Arrange
            var context = TestDbContextFactory.CreateDbContext("TestDb_CreateOrder");
            var repository = new OrderRepository(context);

            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Burger", Quantity = 2 },
                    new OrderItem { ProductName = "Fries", Quantity = 1 }
                }
            };

            // Act
            await repository.CreateOrderAsync(order);
            var orders = await repository.GetOrdersAsync();

            // Assert
            Assert.Single(orders);
            Assert.Equal("Pending", orders.First().Status);
            Assert.Equal(2, orders.First().Items.Count);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnCorrectOrder()
        {
            // Arrange
            var context = TestDbContextFactory.CreateDbContext("TestDb_GetOrderById");
            var repository = new OrderRepository(context);

            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Burger", Quantity = 2 }
                }
            };

            await repository.CreateOrderAsync(order);

            // Act
            var retrievedOrder = await repository.GetOrderByIdAsync(order.Id);

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal("Pending", retrievedOrder.Status);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldUpdateOrderStatus()
        {
            // Arrange
            var context = TestDbContextFactory.CreateDbContext("TestDb_UpdateOrderStatus");
            var repository = new OrderRepository(context);

            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            await repository.CreateOrderAsync(order);

            // Act
            await repository.UpdateOrderStatusAsync(order.Id, "Completed");
            var updatedOrder = await repository.GetOrderByIdAsync(order.Id);

            // Assert
            Assert.NotNull(updatedOrder);
            Assert.Equal("Completed", updatedOrder.Status);
        }
    }
}
