using HoneyShop.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Models.DTOs
{
    public class HoneyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDesc { set; get; }
        public string LongDesc { get; set; }
        public string ShelfLife { get; set; }
        public int Price { get; set; }
        public string Bju { get; set; }
        public string PriceType { get; set; }
        public bool IsFavorite { get; set; }
        public bool NewHoney { get; set; }
        public bool Available { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
