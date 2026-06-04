

import { useEffect, useState } from 'react';

import { shippingCarriersApi } from '../../services/api';

import { AdminTable } from './AdminTable';
import { AdminModal, FormField, inputClass } from './AdminModal';

import { sortByIdDesc } from '../../utils/format';

const emptyForm = { name: '', code: '', logoFileName: '', isActive: true };

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ ĐƠN VỊ VẬN CHUYỂN
// ════════════════════════════════════════════════════════════
export const ShippingCarriersSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [items, setItems] = useState([]);

  
  const [loading, setLoading] = useState(true);

  
  
  const [editing, setEditing] = useState(null);

  const [showModal, setShowModal] = useState(false);

  
  
  const [formData, setFormData] = useState(emptyForm);

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
      
      const { data } = await shippingCarriersApi.getAll();
      
      setItems(sortByIdDesc(data));
    } finally {

      setLoading(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const openCreate = () => {
    setEditing(null);          
    setFormData(emptyForm);    
    setError('');              
    setShowModal(true);        
  };

  

  const openEdit = (c) => {
    setEditing(c);             
    setFormData({
      name: c.name,
      code: c.code,
      
      logoFileName: c.logoFileName || '',
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
      if (editing) {

        await shippingCarriersApi.update(editing.id, { id: editing.id, ...formData });
      } else {
        
        await shippingCarriersApi.create(formData);
      }
      setShowModal(false);     
      load();                  
    } catch (err) {
      
      setError(err.response?.data?.message || 'Thao tác thất bại');
    } finally {
      setSubmitting(false);    
    }
  };

  

  const handleDelete = async (c) => {
    
    if (!window.confirm(`Xoá đơn vị vận chuyển "${c.name}"? Tất cả bảng cước liên quan cũng sẽ bị xoá.`)) return;
    try { await shippingCarriersApi.delete(c.id); load(); }
    
    catch { alert('Xoá thất bại.'); }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Đơn vị vận chuyển</h2>
        <button onClick={openCreate}
          className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800">
          + Thêm đơn vị
        </button>
      </div>

      {}
      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">Đang tải...</p>
      ) : (
        <AdminTable
          
          columns={[
            { key: 'id', label: 'ID', style: { width: 60 } },
            
            { key: 'code', label: 'Mã', render: (c) => <span className="font-mono text-xs">{c.code}</span> },
            { key: 'name', label: 'Tên', render: (c) => <span className="font-medium">{c.name}</span> },
            {
              key: 'isActive',
              label: 'Trạng thái',
              
              render: (c) => c.isActive
                ? <span className="rounded-full bg-emerald-50 px-2 py-0.5 text-xs text-emerald-700 ring-1 ring-emerald-200">Đang hoạt động</span>
                : <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-500">Tắt</span>,
            },
            {
              key: 'actions',
              label: 'Hành động',
              
              render: (c) => (
                <div className="flex gap-2">
                  <button onClick={() => openEdit(c)}
                    className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800">
                    Sửa
                  </button>
                  <button onClick={() => handleDelete(c)}
                    className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100">
                    Xoá
                  </button>
                </div>
              ),
            },
          ]}
          rows={items}                                  
          emptyText="Chưa có đơn vị vận chuyển"        
        />
      )}

      {}
      <AdminModal
        open={showModal}
        
        title={editing ? 'Sửa đơn vị vận chuyển' : 'Thêm đơn vị vận chuyển'}
        onClose={() => setShowModal(false)}
        
        footer={
          <>
            <button type="button" onClick={() => setShowModal(false)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
              Huỷ
            </button>
            {}
            <button type="submit" form="carrier-form" disabled={submitting}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
              {submitting ? 'Đang lưu...' : editing ? 'Cập nhật' : 'Tạo mới'}
            </button>
          </>
        }
      >
        {}
        <form id="carrier-form" onSubmit={submit}>
          {}
          {error && <p className="mb-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>}

          {}
          <FormField label="Mã" required>
            <input className={inputClass} required value={formData.code}
              
              onChange={(e) => setFormData({ ...formData, code: e.target.value })} />
          </FormField>

          <FormField label="Tên" required>
            <input className={inputClass} required value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })} />
          </FormField>

          {}
          <FormField label="Tên file logo (trong /images/carriers/)">
            <input className={inputClass} value={formData.logoFileName}
              placeholder="ghn.png"
              onChange={(e) => setFormData({ ...formData, logoFileName: e.target.value })} />
          </FormField>

          {}
          <FormField label="Trạng thái">
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" className="accent-slate-900" checked={formData.isActive}
                
                onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })} />
              Đang hoạt động
            </label>
          </FormField>
        </form>
      </AdminModal>
    </div>
  );
};

export default ShippingCarriersSection;
