using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class OrderStatusHistory
    {
        public int id { get; set; }
        [Display(Name = "Название статуса")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Название статуса обязательно")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название статуса не должно превышать 50 символов")]
        public string status { get; set; }
        [Display(Name = "Дата изменений")]
        public DateTime changedAt { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
