namespace HoneyShop.Models.DTOs
{
    public class HoneyCategoryDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string shortDesc { set; get; }
        public string longDesc { get; set; }
        public string shelfLife { get; set; }
        public int price { get; set; }
        public string bju { get; set; }
        public string priceType { get; set; }
        public bool isFavorite { get; set; }
        public bool newHoney { get; set; }
        public bool avaliable { get; set; }
        public string imageUrl { get; set; }
        public CategoryDto Categories { get; set; }
    }
}
