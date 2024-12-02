using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderApi.Controllers;
using OrderApi.Models;
using OrderApi.Repositories;
using OrderApi.Services;

namespace OrderTests.Controllers
{
    public class OrderControllerTests
    {
        [Fact]
        public async Task GetOrders_ShouldReturnAllOrders()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            mockRepo.Setup(repo => repo.GetOrdersAsync())
                    .ReturnsAsync(new List<Order>
                    {
                        new Order { Id = 1, Status = "Pending" },
                        new Order { Id = 2, Status = "Completed" }
                    });

            var mockPublisher = new Mock<IRabbitMQPublisher>();
            var controller = new OrderController(mockRepo.Object, mockPublisher.Object);

            // Act
            var result = await controller.GetOrders();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);

            // Assert
            Assert.Equal(2, orders.Count());
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnCreatedOrder()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var mockPublisher = new Mock<IRabbitMQPublisher>();

            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductName = "Burger", Quantity = 2 }
                }
            };

            var controller = new OrderController(mockRepo.Object, mockPublisher.Object);

            // Act
            var result = await controller.CreateOrder(order);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);

            // Assert
            Assert.Equal("GetOrderById", createdAtActionResult.ActionName);
            Assert.Equal(order, createdAtActionResult.Value);
            mockPublisher.Verify(p => p.Publish(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnNoContent()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            mockRepo.Setup(repo => repo.GetOrderByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(new Order { Id = 1, Status = "Pending" });

            var mockPublisher = new Mock<IRabbitMQPublisher>();
            var controller = new OrderController(mockRepo.Object, mockPublisher.Object);

            // Act
            var result = await controller.UpdateOrderStatus(1, "Completed");

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockPublisher.Verify(p => p.Publish(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
