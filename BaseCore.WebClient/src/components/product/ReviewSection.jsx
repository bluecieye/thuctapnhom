

import { useEffect, useState } from 'react';

import { Star } from 'lucide-react';

import { reviewApi } from '../../services/api';

import { useAuth } from '../../contexts/AuthContext';

// ════════════════════════════════════════════════════════════
// CONSTANTS / SCHEMA
// ════════════════════════════════════════════════════════════
const SIZE_LABELS = ['Quá nhỏ', 'Hơi nhỏ', 'Vừa vặn', 'Hơi rộng', 'Quá rộng'];

// ════════════════════════════════════════════════════════════
// COMPONENT KHU VỰC ĐÁNH GIÁ SẢN PHẨM
// ════════════════════════════════════════════════════════════
export const ReviewSection = ({ productId }) => {
  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const { isAuthenticated } = useAuth();

  

  

  const [data, setData] = useState({ average: 0, count: 0, reviews: [] });

  

  
  const [showForm, setShowForm] = useState(false);

  

  

  const [form, setForm] = useState({ rating: 5, sizeAccuracy: 3, comment: '', imageUrl: '' });

  
  
  const [submitting, setSubmitting] = useState(false);



  const [error, setError] = useState('');

  // Trạng thái upload ảnh thực tế.
  const [uploading, setUploading] = useState(false);

  

  // ════════════════════════════════════════════════════════════
  // DATA LOADING
  // ════════════════════════════════════════════════════════════
  const refresh = async () => {
    try {
      
      const res = await reviewApi.getByProduct(productId);

      setData(res.data || { average: 0, count: 0, reviews: [] });
    } catch {
      
    }
  };

  

  // ════════════════════════════════════════════════════════════
  // EFFECTS
  // ════════════════════════════════════════════════════════════
  useEffect(() => { refresh(); }, [productId]);


  // ════════════════════════════════════════════════════════════
  // HANDLERS
  // ════════════════════════════════════════════════════════════
  // Upload ảnh thực tế → nhận URL trả về và lưu vào form.imageUrl.
  const handleImageChange = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setError('');
    setUploading(true);
    try {
      const fd = new FormData();
      fd.append('file', file);
      const { data } = await reviewApi.uploadImage(fd);
      setForm((f) => ({ ...f, imageUrl: data.url }));
    } catch (err) {
      setError(err.response?.data?.message || 'Tải ảnh lên thất bại.');
    } finally {
      setUploading(false);
    }
  };

  const handleSubmit = async (e) => {
    
    e.preventDefault();

    setError('');

    setSubmitting(true);

    try {

      await reviewApi.create({
        productId,
        rating: form.rating,
        sizeAccuracy: form.sizeAccuracy,
        comment: form.comment,
        imageUrl: form.imageUrl || null,
      });

      setShowForm(false);
      setForm({ rating: 5, sizeAccuracy: 3, comment: '', imageUrl: '' });
      await refresh();
    } catch (e2) {

      setError(e2.response?.data?.message || 'Gửi đánh giá thất bại.');
    } finally {
      
      setSubmitting(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    <section className="border-t border-gray-100 py-12">
      {}
      <div className="flex items-end justify-between">
        <h3 className="font-serif text-2xl font-light">Đánh giá khách hàng</h3>

        {}
        {data.count > 0 && (
          <p className="text-sm text-gray-600">
            {}
            {data.average.toFixed(1)}/5 · {data.count} đánh giá
          </p>
        )}
      </div>

      {}
      <div className="mt-6 space-y-6">
        {}
        {data.reviews.length === 0 ? (
          <p className="text-gray-500">Chưa có đánh giá. Hãy là người đầu tiên!</p>
        ) : (
          
          data.reviews.map((review) => (
            <div key={review.id} className="border-b border-gray-100 pb-6">
              {}
              <div className="flex items-center gap-2">
                {}
                <span className="font-medium">{review.user?.username || 'User'}</span>

                {}
                <div className="flex">
                  {}
                  {[...Array(5)].map((_, i) => (
                    <Star key={i} size={14} className={i < review.rating ? 'fill-black text-black' : 'text-gray-300'} />
                  ))}
                </div>

                {}
                {}
                {review.sizeAccuracy && (
                  <span className="rounded bg-slate-100 px-2 py-0.5 text-[10px] text-slate-600">
                    Size: {SIZE_LABELS[review.sizeAccuracy - 1]}
                  </span>
                )}
              </div>

              {}
              {review.comment && <p className="mt-2 text-gray-600">{review.comment}</p>}

              {}
              {review.imageUrl && (
                <img src={review.imageUrl} alt="review" className="mt-2 h-32 w-32 rounded object-cover" />
              )}
            </div>
          ))
        )}
      </div>

      {}
      {}
      {}
      {isAuthenticated ? (
        
        <>
          {}
          <button onClick={() => setShowForm(!showForm)} className="mt-6 text-sm font-medium underline">
            {showForm ? 'Huỷ' : 'Viết đánh giá'}
          </button>

          {}
          {showForm && (
            <form onSubmit={handleSubmit} className="mt-6 max-w-md space-y-4">
              {}
              <div>
                <label className="mb-1 block text-sm">Đánh giá sao</label>
                <div className="flex gap-1">
                  {}
                  {[1, 2, 3, 4, 5].map((v) => (
                    
                    <button type="button" key={v} onClick={() => setForm({ ...form, rating: v })}>
                      {}
                      <Star size={22} className={v <= form.rating ? 'fill-black text-black' : 'text-gray-300'} />
                    </button>
                  ))}
                </div>
              </div>

              {}
              <div>
                <label className="mb-1 block text-sm">Size có chuẩn không?</label>
                <div className="flex flex-wrap gap-2">
                  {}
                  {SIZE_LABELS.map((label, idx) => {
                    const value = idx + 1;                       
                    const active = form.sizeAccuracy === value;  
                    return (
                      <button
                        type="button"
                        key={value}
                        onClick={() => setForm({ ...form, sizeAccuracy: value })}
                        
                        className={`border px-2 py-1 text-xs ${
                          active ? 'border-black bg-black text-white' : 'border-gray-200'
                        }`}
                      >
                        {label}
                      </button>
                    );
                  })}
                </div>
              </div>

              {}
              <div>
                <label className="mb-1 block text-sm">Nội dung</label>
                <textarea
                  value={form.comment}

                  onChange={(e) => setForm({ ...form, comment: e.target.value })}
                  rows="4"
                  className="w-full border border-gray-200 p-3 text-sm"
                  placeholder="Chia sẻ trải nghiệm của bạn về sản phẩm..."
                />
              </div>

              {}
              <div>
                <label className="mb-1 block text-sm">Ảnh thực tế (tuỳ chọn)</label>
                <input
                  type="file"
                  accept="image/*"
                  onChange={handleImageChange}
                  className="block w-full text-sm text-gray-600 file:mr-3 file:border-0 file:bg-black file:px-4 file:py-2 file:text-sm file:text-white hover:file:bg-gray-800"
                />
                {uploading && <p className="mt-2 text-xs text-gray-500">Đang tải ảnh lên...</p>}
                {form.imageUrl && !uploading && (
                  <div className="mt-2 flex items-center gap-3">
                    <img src={form.imageUrl} alt="Xem trước" className="h-20 w-20 rounded object-cover" />
                    <button
                      type="button"
                      onClick={() => setForm({ ...form, imageUrl: '' })}
                      className="text-xs text-gray-500 underline hover:text-red-500"
                    >
                      Xoá ảnh
                    </button>
                  </div>
                )}
              </div>

              {}
              {error && <p className="text-sm text-red-500">{error}</p>}

              {}
              <button type="submit" disabled={submitting}
                className="bg-black px-6 py-2 text-sm text-white hover:bg-gray-800 disabled:bg-gray-400">
                {submitting ? 'Đang gửi...' : 'Gửi đánh giá'}
              </button>
            </form>
          )}
        </>
      ) : (
        
        <p className="mt-6 text-sm text-gray-500">Đăng nhập để viết đánh giá.</p>
      )}
    </section>
  );
};

export default ReviewSection;
