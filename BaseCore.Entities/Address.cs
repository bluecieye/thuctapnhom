

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY ĐỊA CHỈ
    // ════════════════════════════════════════════════════════════
    public class Address
    {
        
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public string Ward { get; set; } = string.Empty;

        public int ProvinceId { get; set; }

        public Province? Province { get; set; }

        
        public bool IsDefault { get; set; } = false;

        public int UserId { get; set; }

        public User? User { get; set; }
    }
}
