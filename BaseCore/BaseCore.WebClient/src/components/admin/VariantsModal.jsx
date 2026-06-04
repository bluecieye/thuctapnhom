

import { useEffect, useState, useMemo } from 'react';

import { Plus, Trash2, Pencil, Check, X } from 'lucide-react';

import { adminService } from '../../services/adminService';
import { productService } from '../../services/productService';

import { AdminModal, FormField, inputClass } from './AdminModal';

import { buildSku } from '../../utils/slug';

import { fmt, sortByIdDesc } from '../../utils/format';

/**
 * Modal quản lý variants (SKU) của 1 sản phẩm.
 * SKU được auto-sinh theo công thức: <product.slug>-<colorSlug>-<sizeSlug>.
 * Props:
 *   - open: boolean
 *   - product: { id, name, slug, basePrice }
 *   - onClose: () => void
 */

// ════════════════════════════════════════════════════════════
// MODAL QUẢN LÝ BIẾN THỂ SẢN PHẨM
// ════════════════════════════════════════════════════════════
export const VariantsModal = ({ open, product, onClose }) => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [variants, setVariants] = useState([]);
  
  const [colors, setColors] = useState([]);
  const [sizes, setSizes] = useState([]);
  
  const [loading, setLoading] = useState(false);
  
  const [flash, setFlash] = useState('');
  
  const [error, setError] = useState('');

  
  const emptyForm = { colorId: '', sizeId: '', price: '', stock: '' };
  const [form, setForm] = useState(emptyForm);
  const [submitting, setSubmitting] = useState(false);

  
  
  const [editingId, setEditingId] = useState(null);
  const [editPrice, setEditPrice] = useState('');
  const [editStock, setEditStock] = useState('');

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = async () => {
    
    if (!product?.id) return;
    setLoading(true);
    try {
      const data = await adminService.getVariantsByProduct(product.id);
      
      setVariants(sortByIdDesc(data));
    } catch { setVariants([]); }
    finally { setLoading(false); }
  };

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    if (!open) return;

    
    setForm({ ...emptyForm, price: product?.basePrice ?? '' });
    setError('');
    
    productService.getColors().then(setColors).catch(() => setColors([]));
    productService.getSizes().then(setSizes).catch(() => setSizes([]));
    load();
  }, [open, product?.id]);

  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const usedPairs = useMemo(
    () => new Set(variants.map((v) => `${v.colorId}|${v.sizeId}`)),
    [variants]
  );

  

  
  const previewSku = useMemo(() => {
    if (!form.colorId || !form.sizeId || !product?.slug) return '';

    const color = colors.find((c) => c.id === Number(form.colorId));
    const size = sizes.find((s) => s.id === Number(form.sizeId));
    if (!color || !size) return '';
    
    return buildSku(product.slug, color.name, size.name);
  }, [form.colorId, form.sizeId, colors, sizes, product?.slug]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const submitCreate = async (e) => {
    e.preventDefault();
    setError('');
    
    if (!previewSku) { setError('Chọn Màu và Size trước.'); return; }
    setSubmitting(true);
    try {
      await adminService.createVariant({
        sku: previewSku,                              
        price: Number(form.price) || 0,
        stock: Number(form.stock) || 0,
        reservedStock: 0,                             
        productId: product.id,
        colorId: Number(form.colorId),
        sizeId: Number(form.sizeId),
      });
      
      setFlash(`Đã thêm SKU: ${previewSku}`);
      
      setTimeout(() => setFlash(''), 2500);
      
      setForm({ ...emptyForm, price: product?.basePrice ?? '' });
      load();
    } catch (err) {
      setError(err.response?.data?.message || 'Thêm thất bại');
    } finally { setSubmitting(false); }
  };

  

  const startEdit = (v) => {
    setEditingId(v.id);
    setEditPrice(v.price);
    setEditStock(v.stock);
  };

  
  const saveEdit = async (v) => {
    try {
      await adminService.updateVariant(v.id, {
        id: v.id,
        sku: v.sku,                                  
        price: Number(editPrice) || 0,
        stock: Number(editStock) || 0,
        reservedStock: v.reservedStock,              
        productId: v.productId,
        colorId: v.colorId,
        sizeId: v.sizeId,
      });
      setEditingId(null);                            
      load();
    } catch (err) {
      alert(err.response?.data?.message || 'Cập nhật thất bại');
    }
  };

  const handleDelete = async (v) => {
    if (!window.confirm(`Xoá variant ${v.sku}?`)) return;
    try { await adminService.deleteVariant(v.id); load(); }
    catch { alert('Xoá thất bại (có thể đã có đơn hàng dùng SKU này).'); }
  };

  if (!product) return null;

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <AdminModal
      open={open}
      title={`Biến thể: ${product.name}`}
      size="xl"                                       
      onClose={onClose}
      footer={

        <button type="button" onClick={onClose}
          className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
          Đóng
        </button>
      }
    >
      {}
      {flash && (
        <div className="mb-3 rounded-xl bg-emerald-50 px-3 py-2 text-sm text-emerald-700 ring-1 ring-emerald-200">
          {flash}
        </div>
      )}

      {}
      <div className="mb-5">
        <p className="mb-2 text-xs font-semibold uppercase tracking-wider text-slate-500">
          Biến thể hiện có ({variants.length})
        </p>
        {loading ? (
          <p className="py-4 text-center text-sm text-slate-400">Đang tải...</p>
        ) : variants.length === 0 ? (
          
          <p className="rounded-xl bg-slate-50 py-4 text-center text-sm text-slate-400">
            Sản phẩm chưa có variant nào. Hãy thêm bên dưới.
          </p>
        ) : (
          <div className="overflow-x-auto rounded-xl border border-slate-200">
            <table className="w-full text-sm">
              <thead className="bg-slate-50">
                <tr className="text-left">
                  <th className="px-3 py-2 font-semibold text-slate-600">Mã SKU</th>
                  <th className="px-3 py-2 font-semibold text-slate-600">Màu</th>
                  <th className="px-3 py-2 font-semibold text-slate-600">Cỡ</th>
                  <th className="px-3 py-2 font-semibold text-slate-600">Giá</th>
                  <th className="px-3 py-2 font-semibold text-slate-600">Tồn kho</th>
                  <th className="px-3 py-2 font-semibold text-slate-600">Đã giữ</th>
                  <th className="px-3 py-2 font-semibold text-slate-600 text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {variants.map((v) => {

                  const editing = editingId === v.id;
                  return (
                    <tr key={v.id} className="hover:bg-slate-50">
                      {}
                      <td className="px-3 py-2 font-mono text-xs">{v.sku}</td>
                      <td className="px-3 py-2">
                        <span className="inline-flex items-center gap-1.5">
                          {}
                          <span className="inline-block h-3 w-3 rounded-full border" style={{ backgroundColor: v.color?.hexCode }} />
                          {v.color?.name}
                        </span>
                      </td>
                      <td className="px-3 py-2">{v.size?.name}</td>
                      <td className="px-3 py-2">
                        {}
                        {editing
                          ? <input type="number" min="0" className={`${inputClass} w-28`}
                              value={editPrice} onChange={(e) => setEditPrice(e.target.value)} />
                          : <span className="font-semibold">{fmt(v.price)}</span>}
                      </td>
                      <td className="px-3 py-2">
                        {}
                        {editing
                          ? <input type="number" min="0" className={`${inputClass} w-20`}
                              value={editStock} onChange={(e) => setEditStock(e.target.value)} />
                          : v.stock}
                      </td>
                      {}
                      <td className="px-3 py-2 text-slate-500">{v.reservedStock}</td>
                      <td className="px-3 py-2">
                        <div className="flex justify-end gap-1.5">
                          {editing ? (
                            
                            <>
                              <button onClick={() => saveEdit(v)}
                                className="rounded-lg bg-emerald-50 px-2 py-1 text-xs font-semibold text-emerald-700 ring-1 ring-emerald-200 hover:bg-emerald-100">
                                <Check size={12} />
                              </button>
                              <button onClick={() => setEditingId(null)}
                                className="rounded-lg bg-slate-100 px-2 py-1 text-xs font-semibold text-slate-600 hover:bg-slate-200">
                                <X size={12} />
                              </button>
                            </>
                          ) : (
                            
                            <>
                              <button onClick={() => startEdit(v)}
                                className="rounded-lg bg-slate-900 px-2 py-1 text-xs font-semibold text-white hover:bg-slate-800">
                                <Pencil size={12} />
                              </button>
                              <button onClick={() => handleDelete(v)}
                                className="rounded-lg bg-rose-50 px-2 py-1 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100">
                                <Trash2 size={12} />
                              </button>
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {}
      <div className="rounded-xl border border-slate-200 bg-slate-50 p-4">
        <p className="mb-3 text-xs font-semibold uppercase tracking-wider text-slate-500">
          Thêm biến thể mới
        </p>
        {error && <p className="mb-3 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>}
        <form onSubmit={submitCreate} className="space-y-3">
          <div className="grid grid-cols-2 gap-3">
            {}
            <FormField label="Màu" required>
              <select required className={inputClass} value={form.colorId}
                onChange={(e) => setForm({ ...form, colorId: e.target.value })}>
                <option value="">-- Chọn màu --</option>
                {colors.map((c) => {

                  const dup = form.sizeId && usedPairs.has(`${c.id}|${form.sizeId}`);
                  return (
                    <option key={c.id} value={c.id} disabled={dup}>
                      {c.name} {dup && '(đã có)'}
                    </option>
                  );
                })}
              </select>
            </FormField>

            {}
            <FormField label="Cỡ" required>
              <select required className={inputClass} value={form.sizeId}
                onChange={(e) => setForm({ ...form, sizeId: e.target.value })}>
                <option value="">-- Chọn cỡ --</option>
                {sizes.map((s) => {
                  
                  const dup = form.colorId && usedPairs.has(`${form.colorId}|${s.id}`);
                  return (
                    <option key={s.id} value={s.id} disabled={dup}>
                      {s.name} {dup && '(đã có)'}
                    </option>
                  );
                })}
              </select>
            </FormField>
          </div>

          {}
          <FormField label="SKU (tự sinh)" hint="Tự ghép từ slug sản phẩm + màu + size">
            <input className={`${inputClass} bg-slate-100 font-mono`} readOnly
              value={previewSku || '— chọn Màu và Size để xem SKU —'} />
          </FormField>

          <div className="grid grid-cols-2 gap-3">
            {}
            <FormField label="Giá (đ)" required>
              <input type="number" min="0" step="1000" required className={inputClass}
                value={form.price} onChange={(e) => setForm({ ...form, price: e.target.value })} />
            </FormField>
            {}
            <FormField label="Stock ban đầu" required>
              <input type="number" min="0" required className={inputClass}
                value={form.stock} onChange={(e) => setForm({ ...form, stock: e.target.value })} />
            </FormField>
          </div>

          {}
          <button type="submit" disabled={submitting || !previewSku}
            className="flex w-full items-center justify-center gap-2 rounded-xl bg-slate-900 px-4 py-2.5 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
            <Plus size={14} /> {submitting ? 'Đang thêm...' : 'Thêm biến thể'}
          </button>
        </form>
      </div>
    </AdminModal>
  );
};

export default VariantsModal;
