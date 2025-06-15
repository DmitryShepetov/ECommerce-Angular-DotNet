using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class Honey
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Название мёда")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Название мёда обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название не должно превышать 100 символов")]
        public string name { get; set; }
        [Display(Name = "Короткое описание")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Напишите короткое описание")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Короткое описание не должно превышать 300 символов")]
        public string shortDesc { set; get; }
        [Display(Name = "Полное описание")]
        [MinLength(10, ErrorMessage = "Длинное описание должно содержать минимум 10 символа")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Напишите полное описание")]
        public string longDesc { get; set; }
        [Display(Name = "Срок Хранения")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Напишите срок хранения")]
        [StringLength(50, ErrorMessage = "Срок не должно превышать 50 символов")]
        public string shelfLife { get; set; }
        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Напишите цену")]
        [Range(1, 100000, ErrorMessage = "Цена должна быть больше 0")]
        public int price { get; set; }
        [Display(Name = "БЖУ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Напишите полезные свойства")]
        public string bju { get; set; }
        [Display(Name = "Тип для цены")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Напишите тип для цены")]
        public string priceType { get; set; }
        [Display(Name = "Лучший в ассортименте мёд")]
        [Required]
        public bool isFavorite { get; set; }
        [Display(Name = "Новая партия мёда")]
        [Required]
        public bool newHoney { get; set; }
        [Display(Name = "Ссылка на картинку мёда")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Добавьте ссылку на изображение")]
        public string imageUrl { get; set; }
        [Display(Name = "В наличии")]
        [Required]
        public bool avaliable { get; set; }
        [ForeignKey(nameof(Category))]
        public int Categoryid { get; set; }
        public Category Category { get; set; }
    }
}
