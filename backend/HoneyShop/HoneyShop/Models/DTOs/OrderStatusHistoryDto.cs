namespace HoneyShop.Models.DTOs
{
    public class OrderStatusHistoryDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public int OrderId { get; set; }
    }
}
