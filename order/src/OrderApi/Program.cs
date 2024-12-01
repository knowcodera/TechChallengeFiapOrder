using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Repositories;
using PaymentApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddSingleton<RabbitMQConnection>();
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddSingleton<RabbitMQConsumer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.Run();
}

app.Run("http://*:8480");
