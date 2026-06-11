

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY CHI TIẾT ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    public class OrderDetail
    {
        
        public int Id { get; set; }

        public int Quantity { get; set; }

        
        public decimal UnitPrice { get; set; }

        public int OrderId { get; set; }
        
        public Order? Order { get; set; }

        public int VariantId { get; set; }
        
        public ProductVariant? Variant { get; set; }
    }
}
