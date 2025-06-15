using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class Category
    {
        public int id { get; set; }
        [Display(Name = "Название категории")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название категории не должно превышать 50 символов")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Напишите название категории")]
        public string categoryName { get; set; }
        [Display(Name = "Описание категории")]
        [StringLength(500, ErrorMessage = "Описание категории не должно превышать 500 символов")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Напишите описание категории")]
        public string desc { get; set; }
    }
}
