

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY DANH MỤC
    // ════════════════════════════════════════════════════════════
    public class Category
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        
        public string Gender { get; set; } = string.Empty; 

        public string Season { get; set; } = "All";

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
