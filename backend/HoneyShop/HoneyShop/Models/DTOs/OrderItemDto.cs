namespace HoneyShop.Models.DTOs
{
    public class OrderItemDto
    {
        public int id { get; set; }
        public string imageUrl { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }
}
