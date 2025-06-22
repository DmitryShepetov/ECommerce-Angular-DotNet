using HoneyShop.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public string MethodDeliveryData { get; set; }
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }    
        public List<OrderItemDto> Items { get; set; }
        public int TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}
