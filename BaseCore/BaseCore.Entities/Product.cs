

namespace BaseCore.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Slug { get; set; }

        public decimal Price { get; set; }

        public decimal? OriginalPrice { get; set; }

        public bool IsNew { get; set; }

        public string? Brand { get; set; }

        public string? Gender { get; set; }

        public int Stock { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
