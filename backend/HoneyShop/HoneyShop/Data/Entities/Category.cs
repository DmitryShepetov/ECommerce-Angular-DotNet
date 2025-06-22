using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "Название категории")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Название категории не должно превышать 50 символов")]
        [Required(ErrorMessage = "Напишите название категории")]
        public string CategoryName { get; set; }
        [Display(Name = "Описание категории")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Описание категории не должно превышать 500 символов")]
        [Required(ErrorMessage = "Напишите описание категории")]
        public string Desc { get; set; }
    }
}
