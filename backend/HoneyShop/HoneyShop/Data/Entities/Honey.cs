using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class Honey
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Название мёда")]
        [Required(ErrorMessage = "Название мёда обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; }
        [Display(Name = "Короткое описание")]
        [Required(ErrorMessage = "Напишите короткое описание")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Короткое описание не должно превышать 300 символов")]
        public string ShortDesc { set; get; }
        [Display(Name = "Полное описание")]
        [MinLength(10, ErrorMessage = "Длинное описание должно содержать минимум 10 символа")]
        [Required(ErrorMessage = "Напишите полное описание")]
        public string LongDesc { get; set; }
        [Display(Name = "Срок Хранения")]
        [Required(ErrorMessage = "Напишите срок хранения")]
        [StringLength(50, ErrorMessage = "Срок не должно превышать 50 символов")]
        public string ShelfLife { get; set; }
        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Напишите цену")]
        [Range(1, 100000, ErrorMessage = "Цена должна быть больше 0")]
        public int Price { get; set; }
        [Display(Name = "БЖУ")]
        [Required(ErrorMessage = "Напишите полезные свойства")]
        public string Bju { get; set; }
        [Display(Name = "Тип для цены")]
        [Required(ErrorMessage = "Напишите тип для цены")]
        public string PriceType { get; set; }
        [Display(Name = "Лучший в ассортименте мёд")]
        [Required]
        public bool IsFavorite { get; set; }
        [Display(Name = "Новая партия мёда")]
        [Required]
        public bool NewHoney { get; set; }
        [Display(Name = "Ссылка на картинку мёда")]
        [Required(ErrorMessage = "Добавьте ссылку на изображение")]
        public string ImageUrl { get; set; }
        [Display(Name = "В наличии")]
        [Required]
        public bool Available { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
