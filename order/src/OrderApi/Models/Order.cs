namespace OrderApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Received";
        public ICollection<OrderItem> Items { get; set; }
    }
}
