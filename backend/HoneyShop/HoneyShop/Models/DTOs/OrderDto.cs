using HoneyShop.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Models.DTOs
{
    public class OrderDto
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string adress { get; set; }
        public string methodDeliveryData { get; set; }
        public string deliveryMethod { get; set; }
        public string paymentMethod { get; set; }    
        public List<OrderItemDto> Items { get; set; }
        public int totalPrice { get; set; }
        public DateTime dateTime { get; set; }
    }
}
