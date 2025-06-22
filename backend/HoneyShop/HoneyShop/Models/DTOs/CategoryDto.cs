using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Models.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Desc { get; set; }
    }
}
