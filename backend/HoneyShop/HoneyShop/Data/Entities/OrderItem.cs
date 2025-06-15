using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class OrderItem
    {
        [Key]
        public int id { get; set; }
        public string imageUrl { get; set; }
        [Display(Name = "Название товара")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Название товара обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название товара не должно превышать 100 символов")]
        public string name { get; set; }
        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Напишите цену")]
        [Range(1, 100000, ErrorMessage = "Цена должна быть больше 0")]
        public decimal price { get; set; }
        [Display(Name = "Количество")]
        [Required(ErrorMessage = "Напишите количество")]
        [Range(1, 100000, ErrorMessage = "Количество должно быть больше 0")]
        public int quantity { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
