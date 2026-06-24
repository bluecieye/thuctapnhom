

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // SERVICE ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // REVIEW SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class ReviewService : IReviewService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS
        // ════════════════════════════════════════════════════════════
        private readonly IReviewRepositoryEF _repo;

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════
        public ReviewService(IReviewRepositoryEF repo) { _repo = repo; }

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC THAY ĐỔI
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CREATE REVIEW
        // ════════════════════════════════════════════════════════════
        public async Task<Review> CreateAsync(CreateReviewRequest request)
        {
            
            if (request.Rating < 1 || request.Rating > 5)
                throw new InvalidOperationException("Rating phải từ 1 đến 5.");
            if (request.SizeAccuracy < 1 || request.SizeAccuracy > 5)
                throw new InvalidOperationException("SizeAccuracy phải từ 1 đến 5.");

            var review = new Review
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                Rating = request.Rating,
                Comment = request.Comment,
                ImageUrl = request.ImageUrl,
                SizeAccuracy = request.SizeAccuracy,
                CreatedAt = DateTime.UtcNow
            };
            return await _repo.AddAsync(review);
        }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // QUERY METHODS
        // ════════════════════════════════════════════════════════════
        public Task<List<Review>> GetByProductAsync(int productId) => _repo.GetByProductAsync(productId);

        public Task<List<Review>> GetAllAdminAsync(int page, int pageSize) => _repo.GetAllAdminAsync(page, pageSize);

        public Task<int> CountAllAsync() => _repo.CountAllAsync();

        public Task<(double Average, int Count)> GetSummaryAsync(int productId) => _repo.GetRatingSummaryAsync(productId);

        // ════════════════════════════════════════════════════════════
        // XOÁ ĐÁNH GIÁ
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // DELETE REVIEW
        // ════════════════════════════════════════════════════════════
        public Task DeleteAsync(int reviewId) => _repo.DeleteByIdAsync(reviewId);
    }
}
