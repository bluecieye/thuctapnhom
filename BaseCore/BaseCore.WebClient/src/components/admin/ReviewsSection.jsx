

import { useEffect, useState, useCallback } from 'react';

import { Star } from 'lucide-react';
import { adminService } from '../../services/adminService';
import { AdminTable, AdminPagination } from './AdminTable';

import { fmt, fmtDate } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// COMPONENT HIỂN THỊ SAO ĐÁNH GIÁ
// ════════════════════════════════════════════════════════════
const Stars = ({ rating }) => (
  <span className="flex gap-0.5">
    {[1, 2, 3, 4, 5].map((i) => (
      <Star key={i} size={12} className={i <= rating ? 'fill-amber-400 text-amber-400' : 'text-gray-300'} />
    ))}
  </span>
);

// ════════════════════════════════════════════════════════════
// HẰNG SỐ / SCHEMA
// ════════════════════════════════════════════════════════════
const SIZE_ACCURACY_LABEL = { 1: 'Nhỏ hơn size', 3: 'Đúng size', 5: 'Lớn hơn size' };

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ ĐÁNH GIÁ
// ════════════════════════════════════════════════════════════
export const ReviewsSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [items, setItems]     = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage]       = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize]            = useState(20);      
  
  const [flash, setFlash]     = useState('');

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = useCallback(async () => {
    setLoading(true);
    try {
      const d = await adminService.getAllReviews(page, pageSize);
      
      setItems(d.items || []);
      setTotalPages(d.totalPages || 1);
      setTotalCount(d.total || 0);
    } catch {
      setItems([]);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => { load(); }, [load]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const handleDelete = async (review) => {
    if (!window.confirm(`Xoá đánh giá của "${review.user?.username}"?`)) return;
    try {
      await adminService.deleteReview(review.id);
      setFlash('Đã xoá đánh giá.');
      
      setTimeout(() => setFlash(''), 2500);
      load();
    } catch {
      alert('Xoá thất bại.');
    }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">Đánh giá sản phẩm</h2>
        <p className="mt-1 text-sm text-slate-500">Xem và kiểm duyệt đánh giá từ khách hàng</p>
      </div>

      {}
      {flash && (
        <div className="rounded-xl bg-emerald-50 px-4 py-2 text-sm font-medium text-emerald-700 ring-1 ring-emerald-200">
          {flash}
        </div>
      )}

      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-8 text-center text-slate-500">Đang tải...</p>
      ) : (
        <>
          <p className="text-sm text-slate-500">Tổng cộng {fmt(totalCount)} đánh giá</p>
          <AdminTable
            columns={[
              
              { key: 'product',  label: 'Sản phẩm',  render: (r) => <span className="font-medium">{r.product?.name || '—'}</span> },
              { key: 'user',     label: 'Khách hàng', render: (r) => r.user?.username || '—' },
              
              { key: 'rating',   label: 'Sao',        render: (r) => <Stars rating={r.rating} /> },
              {
                key: 'sizeAccuracy', label: 'Cỡ size',

                
                
                render: (r) => {
                  const label = r.sizeAccuracy <= 2 ? 'Nhỏ hơn' : r.sizeAccuracy >= 4 ? 'Lớn hơn' : 'Đúng size';
                  const cls   = r.sizeAccuracy <= 2 ? 'text-blue-600' : r.sizeAccuracy >= 4 ? 'text-rose-600' : 'text-emerald-600';
                  return <span className={`text-xs font-medium ${cls}`}>{label}</span>;
                },
              },
              {
                key: 'comment', label: 'Nhận xét',
                
                render: (r) => (
                  <span className="line-clamp-2 max-w-xs text-slate-600">{r.comment || '—'}</span>
                ),
              },
              {
                key: 'image', label: 'Ảnh',
                
                render: (r) => r.imageUrl
                  ? <img src={r.imageUrl} alt="review" className="h-12 w-12 rounded-lg object-cover" />
                  : '—',
              },
              { key: 'date', label: 'Ngày', render: (r) => fmtDate(r.createdAt) },
              {
                key: 'actions', label: 'Thao tác',
                
                render: (r) => (
                  <button
                    onClick={() => handleDelete(r)}
                    className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100"
                  >
                    Xoá
                  </button>
                ),
              },
            ]}
            rows={items}
            emptyText="Chưa có đánh giá nào"
          />
          <AdminPagination page={page} totalPages={totalPages} onChange={setPage} />
        </>
      )}
    </div>
  );
};

export default ReviewsSection;
