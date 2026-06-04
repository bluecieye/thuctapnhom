

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // DTO YÊU CẦU ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // CREATE REVIEW REQUEST — DTO
    // ════════════════════════════════════════════════════════════
    public class CreateReviewRequest
    {
        public int UserId { get; set; }                  
        public int ProductId { get; set; }               
        public int Rating { get; set; }                  
        public string Comment { get; set; } = "";        
        public int SizeAccuracy { get; set; } = 3;       
        public string? ImageUrl { get; set; }            
    }

    // ════════════════════════════════════════════════════════════
    // INTERFACE SERVICE ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // REVIEW SERVICE — INTERFACE
    // ════════════════════════════════════════════════════════════
    public interface IReviewService
    {
        
        Task<Review> CreateAsync(CreateReviewRequest request);

        Task<List<Review>> GetByProductAsync(int productId);

        Task<List<Review>> GetAllAdminAsync(int page, int pageSize);

        Task<int> CountAllAsync();

        
        Task<(double Average, int Count)> GetSummaryAsync(int productId);

        Task DeleteAsync(int reviewId);
    }
}
