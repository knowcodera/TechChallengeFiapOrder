using Microsoft.EntityFrameworkCore;
using OrderApi.Data;

namespace OrderTests
{
    public static class TestDbContextFactory
    {
        public static OrderContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<OrderContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new OrderContext(options);
        }
    }
}
