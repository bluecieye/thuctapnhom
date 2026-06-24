

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY BANNER
    // ════════════════════════════════════════════════════════════
    public class Banner
    {
        
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string? LinkUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;
    }
}
