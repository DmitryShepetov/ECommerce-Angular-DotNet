using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Models.DTOs
{
    public class CategoryDto
    {
        public int id { get; set; }
        public string categoryName { get; set; }
        public string desc { get; set; }
    }
}
