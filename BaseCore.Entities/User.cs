

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY NGƯỜI DÙNG
    // ════════════════════════════════════════════════════════════
    public class User
    {
        
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        
        public string Role { get; set; } = "Customer";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

        public Cart? Cart { get; set; }
    }
}
