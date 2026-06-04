

import { useEffect, useState } from 'react';
import { adminService } from '../../services/adminService';
import { AdminTable } from './AdminTable';
import { AdminModal, FormField, inputClass } from './AdminModal';

import { includesAccentInsensitive } from '../../utils/slug';

import { fmt, sortByIdDesc } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// HẰNG SỐ / SCHEMA
// ════════════════════════════════════════════════════════════
const empty = {
  code: '',                  
  discountType: 'Percent',   
  discountValue: 0,          
  minOrderAmount: 0,         
  maxDiscount: '',           
  startDate: '',             
  endDate: '',
  usageLimit: 0,             
  isActive: true,            
};

const toLocalDate = (d) => d ? new Date(d).toISOString().slice(0, 16) : '';

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ MÃ GIẢM GIÁ
// ════════════════════════════════════════════════════════════
export const CouponsSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [items, setItems] = useState([]);
  const [keyword, setKeyword] = useState('');      
  const [loading, setLoading] = useState(true);    

  const [editing, setEditing] = useState(null);    
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState(empty); 
  const [error, setError] = useState('');          
  const [submitting, setSubmitting] = useState(false);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => { load(); }, []);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = async () => {
    setLoading(true);
    try {
      const data = await adminService.getCoupons();

      setItems(sortByIdDesc(data));
    } catch {
      setItems([]); 
    } finally {
      setLoading(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const filtered = items.filter(
    (c) => !keyword || includesAccentInsensitive(c.code, keyword)
  );

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const openCreate = () => { setEditing(null); setFormData(empty); setError(''); setShowModal(true); };

  

  

  const openEdit = (c) => {
    setEditing(c);
    setFormData({
      code: c.code,
      discountType: c.discountType,
      discountValue: c.discountValue,
      minOrderAmount: c.minOrderAmount || 0,
      maxDiscount: c.maxDiscount || '',
      startDate: toLocalDate(c.startDate),
      endDate: toLocalDate(c.endDate),
      usageLimit: c.usageLimit || 0,
      isActive: c.isActive,
    });
    setError('');
    setShowModal(true);
  };

  

  

  
  const submit = async (e) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);
    try {
      const body = {
        ...formData,
        
        discountValue: parseFloat(formData.discountValue) || 0,
        minOrderAmount: parseFloat(formData.minOrderAmount) || 0,

        maxDiscount: formData.maxDiscount === '' ? null : parseFloat(formData.maxDiscount),
        usageLimit: parseInt(formData.usageLimit) || 0,
        
        startDate: new Date(formData.startDate).toISOString(),
        endDate: new Date(formData.endDate).toISOString(),
      };

      if (editing) await adminService.updateCoupon(editing.id, { id: editing.id, ...body });
      else await adminService.createCoupon(body);

      setShowModal(false);
      load();
    } catch (err) {
      setError(err.response?.data?.message || 'Lưu thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  
  
  const handleDelete = async (c) => {
    if (!window.confirm(`Xoá coupon "${c.code}"?`)) return;
    try {
      await adminService.deleteCoupon(c.id);
      load();
    } catch { alert('Xoá thất bại.'); }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Mã giảm giá</h2>
        <button onClick={openCreate} className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800">
          + Thêm coupon
        </button>
      </div>

      {}
      <div className="rounded-2xl border border-slate-200 bg-white p-3">
        <input className={`${inputClass} w-full`} placeholder="Tìm theo mã..."
          value={keyword} onChange={(e) => setKeyword(e.target.value)} />
      </div>

      {}
      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">Đang tải...</p>
      ) : (
        
        <AdminTable
          columns={[
            
            { key: 'code', label: 'Mã', render: (c) => <span className="font-mono font-semibold">{c.code}</span> },

            
            { key: 'discount', label: 'Giảm', render: (c) =>
              c.discountType === 'Percent' || c.discountType === 'Percentage'
                ? `${c.discountValue}%`
                : `${fmt(c.discountValue)} đ`
            },

            { key: 'minOrder', label: 'Đơn tối thiểu', render: (c) => `${fmt(c.minOrderAmount)} đ` },

            { key: 'usage', label: 'Lượt dùng', render: (c) =>
              c.usageLimit > 0 ? `${c.usedCount}/${c.usageLimit}` : `${c.usedCount}/∞`
            },

            { key: 'period', label: 'Hiệu lực', render: (c) =>
              `${new Date(c.startDate).toLocaleDateString('vi-VN')} → ${new Date(c.endDate).toLocaleDateString('vi-VN')}`
            },

            { key: 'active', label: 'Trạng thái', render: (c) => (
              <span className={`rounded-full px-2 py-1 text-xs font-semibold ${
                c.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-500'
              }`}>
                {c.isActive ? 'Active' : 'Inactive'}
              </span>
            )},

            { key: 'actions', label: 'Thao tác', render: (c) => (
              <div className="flex gap-2">
                <button onClick={() => openEdit(c)} className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800">
                  Sửa
                </button>
                <button onClick={() => handleDelete(c)} className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100">
                  Xoá
                </button>
              </div>
            )},
          ]}
          rows={filtered}
          emptyText="Không có coupon nào"
        />
      )}

      {}
      <AdminModal
        open={showModal}
        title={editing ? 'Sửa coupon' : 'Thêm coupon'}
        onClose={() => setShowModal(false)}
        size="lg" 
        footer={
          <>
            <button type="button" onClick={() => setShowModal(false)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
              Huỷ
            </button>
            {}
            <button type="submit" form="coupon-form" disabled={submitting}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
              {submitting ? 'Đang lưu...' : editing ? 'Cập nhật' : 'Thêm'}
            </button>
          </>
        }
      >
        {}
        <form id="coupon-form" onSubmit={submit}>
          {error && <p className="mb-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>}

          {}
          <div className="grid grid-cols-2 gap-4">
            <FormField label="Mã" required>
              {}
              <input className={inputClass} required value={formData.code}
                onChange={(e) => setFormData({ ...formData, code: e.target.value.toUpperCase() })} />
            </FormField>
            <FormField label="Loại giảm giá" required>
              {}
              <select className={inputClass} value={formData.discountType}
                onChange={(e) => setFormData({ ...formData, discountType: e.target.value })}>
                <option value="Percent">Phần trăm (%)</option>
                <option value="Fixed">Số tiền cố định (đ)</option>
              </select>
            </FormField>
          </div>

          {}
          <div className="grid grid-cols-2 gap-4">
            <FormField label="Giá trị giảm" required>
              {}
              <input type="number" min="0" required className={inputClass} value={formData.discountValue}
                onChange={(e) => setFormData({ ...formData, discountValue: e.target.value })} />
            </FormField>
            {}
            <FormField label="Giảm tối đa (đ)" hint="Để trống = không giới hạn">
              <input type="number" min="0" className={inputClass} value={formData.maxDiscount}
                onChange={(e) => setFormData({ ...formData, maxDiscount: e.target.value })} />
            </FormField>
          </div>

          {}
          <FormField label="Đơn hàng tối thiểu (đ)">
            <input type="number" min="0" className={inputClass} value={formData.minOrderAmount}
              onChange={(e) => setFormData({ ...formData, minOrderAmount: e.target.value })} />
          </FormField>

          {}
          <div className="grid grid-cols-2 gap-4">
            <FormField label="Bắt đầu" required>
              {}
              <input type="datetime-local" required className={inputClass} value={formData.startDate}
                onChange={(e) => setFormData({ ...formData, startDate: e.target.value })} />
            </FormField>
            <FormField label="Kết thúc" required>
              <input type="datetime-local" required className={inputClass} value={formData.endDate}
                onChange={(e) => setFormData({ ...formData, endDate: e.target.value })} />
            </FormField>
          </div>

          {}
          <div className="grid grid-cols-2 gap-4">
            <FormField label="Giới hạn lượt dùng" hint="0 = không giới hạn">
              <input type="number" min="0" className={inputClass} value={formData.usageLimit}
                onChange={(e) => setFormData({ ...formData, usageLimit: e.target.value })} />
            </FormField>
            <FormField label="Trạng thái">
              {}
              {}
              <label className="flex items-center gap-2 pt-3">
                {}
                <input type="checkbox" checked={formData.isActive}
                  onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })} />
                <span className="text-sm text-slate-700">Đang hoạt động</span>
              </label>
            </FormField>
          </div>
        </form>
      </AdminModal>
    </div>
  );
};

export default CouponsSection;
