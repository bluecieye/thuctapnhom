

import { useEffect, useMemo, useState } from 'react';

import { shippingCarriersApi, shippingRatesApi, provincesApi } from '../../services/api';

import { AdminTable, AdminPagination } from './AdminTable';
import { AdminModal, FormField, inputClass } from './AdminModal';
import { SearchableSelect } from './SearchableSelect';

import { fmt, sortByIdDesc } from '../../utils/format';

const PAGE_SIZE = 10;

const emptyForm = { carrierId: '', provinceId: '', fee: 0, estimatedDays: 3 };

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ CƯỚC VẬN CHUYỂN
// ════════════════════════════════════════════════════════════
export const ShippingRatesSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [carriers, setCarriers] = useState([]);
  const [provinces, setProvinces] = useState([]);

  const [rates, setRates] = useState([]);

  const [filterCarrier, setFilterCarrier] = useState('');
  const [filterProvince, setFilterProvince] = useState('');

  
  const [page, setPage] = useState(1);

  const [loading, setLoading] = useState(true);

  
  const [editing, setEditing] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState(emptyForm);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    (async () => {
      
      const [c, p] = await Promise.all([
        shippingCarriersApi.getAll(),
        provincesApi.getAll(),
      ]);
      setCarriers(c.data || []);
      setProvinces(p.data || []);
      
      await loadRates();
    })();
  }, []);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const loadRates = async (params) => {
    setLoading(true);
    try {
      const { data } = await shippingRatesApi.getAll(params);
      
      setRates(sortByIdDesc(data));
      
      setPage(1);
    } finally {
      setLoading(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const totalPages = Math.max(1, Math.ceil(rates.length / PAGE_SIZE));
  const pagedRates = useMemo(
    () => rates.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE),
    [rates, page]
  );

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const applyFilter = () => {
    const params = {};
    if (filterCarrier) params.carrierId = filterCarrier;
    if (filterProvince) params.provinceId = filterProvince;
    loadRates(params);
  };

  
  
  const openCreate = () => {
    setEditing(null);
    setFormData(emptyForm);
    setError('');
    setShowModal(true);
  };

  const openEdit = (r) => {
    setEditing(r);
    
    setFormData({
      carrierId: r.carrierId,
      provinceId: r.provinceId,
      fee: r.fee,
      estimatedDays: r.estimatedDays,
    });
    setError('');
    setShowModal(true);
  };

  
  
  const submit = async (e) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);
    try {

      const payload = {
        carrierId: Number(formData.carrierId),
        provinceId: Number(formData.provinceId),
        fee: Number(formData.fee),
        estimatedDays: Number(formData.estimatedDays),
      };
      if (editing) {
        await shippingRatesApi.update(editing.id, { id: editing.id, ...payload });
      } else {
        await shippingRatesApi.create(payload);
      }
      setShowModal(false);
      loadRates();
    } catch (err) {
      setError(err.response?.data?.message || 'Thao tác thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  
  
  const handleDelete = async (r) => {
    if (!window.confirm(`Xoá cước ${r.carrier?.name} → ${r.province?.name}?`)) return;
    try { await shippingRatesApi.delete(r.id); loadRates(); }
    catch { alert('Xoá thất bại.'); }
  };

  const usedKeys = useMemo(
    () => new Set(rates.map((r) => `${r.carrierId}|${r.provinceId}`)),
    [rates]
  );

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Bảng cước vận chuyển</h2>
        <button onClick={openCreate}
          className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800">
          + Thêm cước
        </button>
      </div>

      {}
      <div className="rounded-2xl border border-slate-200 bg-white p-4">
        {}
        <p className="mb-2 text-sm text-slate-600">
          <strong>Quy tắc miễn phí ship:</strong> đơn ≥ 1,000,000 đ. Các trường hợp khác áp dụng bảng cước bên dưới.
        </p>
        <div className="flex flex-wrap items-center gap-2">
          {}
          <SearchableSelect
            className="max-w-xs"
            value={filterCarrier}
            onChange={(val) => setFilterCarrier(val)}
            options={carriers.map((c) => ({ value: c.id, label: c.name }))}
            placeholder="Tất cả đơn vị"
          />
          {}
          <SearchableSelect
            className="max-w-xs"
            value={filterProvince}
            onChange={(val) => setFilterProvince(val)}
            options={provinces.map((p) => ({ value: p.id, label: p.name }))}
            placeholder="Tất cả tỉnh thành"
          />
          <button onClick={applyFilter}
            className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-800">
            Lọc
          </button>
        </div>
      </div>

      {}
      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">Đang tải...</p>
      ) : (
        <>
          {}
          <p className="text-sm text-slate-500">Tổng: {rates.length} cước</p>
          <AdminTable
            columns={[
              { key: 'id', label: 'ID', style: { width: 60 } },
              
              { key: 'carrier', label: 'Đơn vị', render: (r) => r.carrier?.name || `#${r.carrierId}` },
              { key: 'province', label: 'Tỉnh/Thành', render: (r) => r.province?.name || `#${r.provinceId}` },
              
              { key: 'fee', label: 'Cước (đ)', render: (r) => <span className="font-semibold">{fmt(r.fee)}</span> },
              { key: 'days', label: 'Số ngày dự kiến', render: (r) => `${r.estimatedDays} ngày` },
              {
                key: 'actions',
                label: 'Hành động',
                render: (r) => (
                  <div className="flex gap-2">
                    <button onClick={() => openEdit(r)}
                      className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800">
                      Sửa
                    </button>
                    <button onClick={() => handleDelete(r)}
                      className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100">
                      Xoá
                    </button>
                  </div>
                ),
              },
            ]}
            rows={pagedRates}
            emptyText="Chưa có cước nào được cấu hình"
          />
          {}
          <AdminPagination page={page} totalPages={totalPages} onChange={setPage} />
        </>
      )}

      {}
      <AdminModal
        open={showModal}
        title={editing ? 'Sửa cước vận chuyển' : 'Thêm cước vận chuyển'}
        onClose={() => setShowModal(false)}
        footer={
          <>
            <button type="button" onClick={() => setShowModal(false)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
              Huỷ
            </button>
            <button type="submit" form="rate-form" disabled={submitting}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
              {submitting ? 'Đang lưu...' : editing ? 'Cập nhật' : 'Tạo mới'}
            </button>
          </>
        }
      >
        <form id="rate-form" onSubmit={submit}>
          {error && <p className="mb-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>}

          {}
          <FormField label="Đơn vị vận chuyển" required>
            <select required className={inputClass} value={formData.carrierId}
              onChange={(e) => setFormData({ ...formData, carrierId: e.target.value })}>
              <option value="">-- Chọn --</option>
              {carriers.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </FormField>

          {}
          <FormField label="Tỉnh/Thành" required>
            <select required className={inputClass} value={formData.provinceId}
              onChange={(e) => setFormData({ ...formData, provinceId: e.target.value })}>
              <option value="">-- Chọn --</option>
              {provinces.map((p) => {

                
                const dup = !editing && formData.carrierId &&
                  usedKeys.has(`${formData.carrierId}|${p.id}`);
                return (
                  
                  <option key={p.id} value={p.id} disabled={dup}>
                    {p.name} {dup && '(đã có cước)'}
                  </option>
                );
              })}
            </select>
          </FormField>

          {}
          <FormField label="Cước (đ)" required>
            <input type="number" min="0" step="1000" required className={inputClass}
              value={formData.fee}
              onChange={(e) => setFormData({ ...formData, fee: e.target.value })} />
          </FormField>

          {}
          <FormField label="Số ngày dự kiến" required>
            <input type="number" min="1" max="14" required className={inputClass}
              value={formData.estimatedDays}
              onChange={(e) => setFormData({ ...formData, estimatedDays: e.target.value })} />
          </FormField>
        </form>
      </AdminModal>
    </div>
  );
};

export default ShippingRatesSection;
