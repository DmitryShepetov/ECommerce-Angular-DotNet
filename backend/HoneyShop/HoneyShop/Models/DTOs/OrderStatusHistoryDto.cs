namespace HoneyShop.Models.DTOs
{
    public class OrderStatusHistoryDto
    {
        public int id { get; set; }
        public string status { get; set; }
        public DateTime changedAt { get; set; }
        public int OrderId { get; set; }
    }
}
