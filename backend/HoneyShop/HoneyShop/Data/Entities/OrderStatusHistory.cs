using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        [Display(Name = "Название статуса")]
        [Required(ErrorMessage = "Название статуса обязательно")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название статуса не должно превышать 50 символов")]
        public string Status { get; set; }
        [Display(Name = "Дата изменений")]
        public DateTime ChangedAt { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
