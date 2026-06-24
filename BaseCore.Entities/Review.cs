

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════
    public class Review
    {
        
        public int Id { get; set; }

        public int Rating { get; set; }       

        public string Comment { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        
        public int SizeAccuracy { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        
        public User? User { get; set; }

        public int ProductId { get; set; }
        
        public Product? Product { get; set; }
    }
}
