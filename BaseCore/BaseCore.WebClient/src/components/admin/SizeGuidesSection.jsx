

import { useEffect, useState, useCallback } from 'react';

import { adminService } from '../../services/adminService';

import { AdminTable } from './AdminTable';
import { AdminModal, FormField, inputClass } from './AdminModal';

import { sortByIdDesc } from '../../utils/format';

import { lookupsApi } from '../../services/api';

const emptyForm = { sizeId: '', chest: '', waist: '', shoulder: '', length: '' };

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ BẢNG SIZE
// ════════════════════════════════════════════════════════════
export const SizeGuidesSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [items, setItems] = useState([]);
  
  const [sizes, setSizes] = useState([]);
  
  const [loading, setLoading] = useState(true);

  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form, setForm] = useState(emptyForm);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    lookupsApi.sizes().then((r) => setSizes(r.data || []));
  }, []);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = useCallback(async () => {
    setLoading(true);
    try {
      const d = await adminService.getAllSizeGuides();
      
      const list = Array.isArray(d) ? d : (d.items || []);
      setItems(sortByIdDesc(list));
    } catch { setItems([]); }  
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(); }, [load]);

  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const usedSizeIds = items.map((g) => g.sizeId);

  const availableSizes = editing ? sizes : sizes.filter((s) => !usedSizeIds.includes(s.id));

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const openCreate = () => {
    setEditing(null);
    setForm({ ...emptyForm, sizeId: availableSizes[0]?.id || '' });
    setError('');
    setShowModal(true);
  };

  const openEdit = (g) => {
    setEditing(g);
    setForm({ sizeId: g.sizeId, chest: g.chest, waist: g.waist, shoulder: g.shoulder, length: g.length });
    setError('');
    setShowModal(true);
  };

  
  
  const submit = async (e) => {
    e.preventDefault();
    setError(''); setSubmitting(true);
    try {
      
      const body = {
        sizeId: Number(form.sizeId),
        chest: Number(form.chest),
        waist: Number(form.waist),
        shoulder: Number(form.shoulder),
        length: Number(form.length),
      };
      if (editing) await adminService.updateSizeGuide(editing.id, body);
      else await adminService.createSizeGuide(body);
      setShowModal(false);
      load();
    } catch (err) { setError(err.response?.data?.message || 'Thao tác thất bại.'); }
    finally { setSubmitting(false); }
  };

  const handleDelete = async (g) => {
    if (!window.confirm(`Xoá bảng đo của size "${g.size?.name || g.sizeId}"?`)) return;
    try { await adminService.deleteSizeGuide(g.id); load(); }
    catch { alert('Xoá thất bại.'); }
  };

  const f = (v) => setForm((p) => ({ ...p, ...v }));

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Bảng size (đồng bộ)</h2>
        {}
        <button onClick={openCreate} disabled={availableSizes.length === 0}
          className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-300">
          + Thêm size
        </button>
      </div>
      {}
      <p className="text-sm text-slate-500">
        Bảng đo này áp dụng cho toàn bộ sản phẩm. Mỗi size chỉ có 1 bản đo.
      </p>

      {}
      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">Đang tải...</p>
      ) : (
        <AdminTable
          columns={[
            {
              key: 'size', label: 'Size',
              
              render: (g) => (
                <span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-bold text-slate-700">
                  {g.size?.name || g.sizeId}
                </span>
              ),
            },
            
            { key: 'chest',    label: 'Ngực (cm)',  render: (g) => g.chest },
            { key: 'waist',    label: 'Eo (cm)',    render: (g) => g.waist },
            { key: 'shoulder', label: 'Vai (cm)',   render: (g) => g.shoulder },
            { key: 'length',   label: 'Dài (cm)',   render: (g) => g.length },
            {
              key: 'actions', label: 'Thao tác',
              render: (g) => (
                <div className="flex gap-1.5">
                  <button onClick={() => openEdit(g)}
                    className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800">
                    Sửa
                  </button>
                  <button onClick={() => handleDelete(g)}
                    className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100">
                    Xoá
                  </button>
                </div>
              ),
            },
          ]}
          rows={items}
          emptyText="Chưa có bảng size nào."
        />
      )}

      {}
      <AdminModal
        open={showModal}
        title={editing ? 'Sửa bảng size' : 'Thêm size mới'}
        onClose={() => setShowModal(false)}
        footer={
          <>
            <button type="button" onClick={() => setShowModal(false)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
              Huỷ
            </button>
            <button type="submit" form="sg-form" disabled={submitting}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
              {submitting ? 'Đang lưu...' : editing ? 'Cập nhật' : 'Thêm'}
            </button>
          </>
        }
      >
        <form id="sg-form" onSubmit={submit}>
          {error && <p className="mb-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>}

          {}
          <FormField label="Cỡ" required>
            <select className={inputClass} required value={form.sizeId}
              onChange={(e) => f({ sizeId: e.target.value })} disabled={!!editing}>
              <option value="">-- Chọn cỡ --</option>
              {}
              {(editing ? sizes : availableSizes).map((s) => (
                <option key={s.id} value={s.id}>{s.name}</option>
              ))}
            </select>
          </FormField>

          {}
          <div className="grid grid-cols-2 gap-3">
            {[['chest','Ngực (cm)'],['waist','Eo (cm)'],['shoulder','Vai (cm)'],['length','Dài (cm)']].map(([k, label]) => (
              <FormField key={k} label={label} required>
                <input type="number" className={inputClass} required min={0} step="0.5"
                  value={form[k]} onChange={(e) => f({ [k]: e.target.value })} />
              </FormField>
            ))}
          </div>
        </form>
      </AdminModal>
    </div>
  );
};

export default SizeGuidesSection;
