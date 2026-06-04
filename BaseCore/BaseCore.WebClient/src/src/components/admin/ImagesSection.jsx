

import { useEffect, useState, useCallback } from 'react';

import { useSearchParams } from 'react-router-dom';

import { Star } from 'lucide-react';

import { adminService } from '../../services/adminService';

import { productService } from '../../services/productService';

import { AdminTable, AdminPagination } from './AdminTable';

import { AdminModal, FormField, inputClass } from './AdminModal';

import { SearchableSelect } from './SearchableSelect';

import { IMAGE_PLACEHOLDER } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// HẰNG SỐ / SCHEMA
// ════════════════════════════════════════════════════════════
const emptyForm = { fileName: '', colorId: '', isPrimary: false, displayOrder: 0, productId: '' };

const imagePath = (slug, fileName) =>
  slug && fileName ? `/images/products/${slug}/${fileName}` : '';

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ HÌNH ẢNH
// ════════════════════════════════════════════════════════════
export const ImagesSection = () => {

  const [searchParams] = useSearchParams();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [items, setItems] = useState([]);
  
  const [products, setProducts] = useState([]);
  
  const [colors, setColors] = useState([]);
  
  const [loading, setLoading] = useState(true);

  

  const [filterProductId, setFilterProductId] = useState(searchParams.get('productId') || '');
  
  const [page, setPage] = useState(1);
  
  const [pageSize] = useState(20);
  
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);

  
  const [showModal, setShowModal] = useState(false);
  
  const [editing, setEditing] = useState(null);
  
  const [form, setForm] = useState(emptyForm);

  
  const [file, setFile] = useState(null);

  const [previewUrl, setPreviewUrl] = useState('');
  
  const [error, setError] = useState('');
  
  const [submitting, setSubmitting] = useState(false);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    adminService.getProducts({ pageSize: 200 }).then((d) => setProducts(d.items || []));
    
    productService.getColors().then(setColors).catch(() => setColors([]));
  }, []);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = useCallback(async () => {
    setLoading(true);
    try {
      const d = await adminService.getAllImages({

        productId: filterProductId || undefined,
        page,
        pageSize,
      });
      
      setItems(d.items || []);
      setTotalPages(d.totalPages || 0);
      setTotalCount(d.total || 0);
    } catch { setItems([]); } 
    finally { setLoading(false); }
  }, [filterProductId, page, pageSize]);

  useEffect(() => { load(); }, [load]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const openCreate = () => {
    setEditing(null); 

    setForm({ ...emptyForm, productId: filterProductId || products[0]?.id || '', colorId: colors[0]?.id || '' });
    setFile(null);
    setPreviewUrl('');
    setError('');
    setShowModal(true);
  };

  

  const openEdit = (img) => {
    setEditing(img);
    setForm({
      fileName: img.fileName,
      colorId: img.colorId,
      isPrimary: img.isPrimary,
      displayOrder: img.displayOrder,
      productId: img.productId,
    });
    setFile(null);                
    setPreviewUrl('');
    setError('');
    setShowModal(true);
  };

  
  const handleFileChange = (e) => {
    const f = e.target.files?.[0];
    if (!f) { setFile(null); setPreviewUrl(''); return; }
    setFile(f);

    setPreviewUrl(URL.createObjectURL(f));
  };

  useEffect(() => {
    return () => { if (previewUrl) URL.revokeObjectURL(previewUrl); };
  }, [previewUrl]);

  
  
  const submit = async (e) => {
    e.preventDefault(); 
    setError(''); setSubmitting(true);
    try {
      const productId = Number(form.productId);
      const colorId = Number(form.colorId);
      const displayOrder = Number(form.displayOrder) || 0;

      if (editing) {

        if (file) {
          const fd = new FormData();
          fd.append('file', file);
          fd.append('colorId', String(colorId));
          fd.append('isPrimary', String(form.isPrimary));
          fd.append('displayOrder', String(displayOrder));
          await adminService.replaceImageFile(editing.id, fd);
        } else {
          await adminService.updateImage(editing.id, {
            fileName: form.fileName,
            colorId, isPrimary: form.isPrimary, displayOrder,
            productId,
          });
        }
      } else {
        
        if (!file) { setError('Vui lòng chọn ảnh từ máy.'); setSubmitting(false); return; }
        const fd = new FormData();
        fd.append('file', file);
        fd.append('productId', String(productId));
        fd.append('colorId', String(colorId));
        fd.append('isPrimary', String(form.isPrimary));
        fd.append('displayOrder', String(displayOrder));
        await adminService.uploadImage(fd);
      }
      setShowModal(false);
      load(); 
    } catch (err) { setError(err.response?.data?.message || 'Thao tác thất bại.'); }
    finally { setSubmitting(false); }
  };

  
  
  const handleDelete = async (img) => {
    if (!window.confirm('Xoá ảnh này?')) return; 
    try { await adminService.deleteImage(img.id); load(); }
    catch { alert('Xoá thất bại.'); }
  };

  

  
  const handleSetPrimary = async (img) => {
    try { await adminService.setImagePrimary(img.id); load(); }
    catch { alert('Thao tác thất bại.'); }
  };

  
  const colorById = (id) => colors.find((c) => c.id === id);

  const slugByProductId = (pid) => products.find((p) => p.id === Number(pid))?.slug;

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Ảnh sản phẩm</h2>
        <button onClick={openCreate}
          className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800">
          + Thêm ảnh
        </button>
      </div>

      {}
      <div className="rounded-2xl border border-slate-200 bg-white p-3">
        <SearchableSelect
          value={filterProductId}
          onChange={(val) => { setFilterProductId(val); setPage(1); }}
          options={products.map((p) => ({ value: p.id, label: `${p.id}. ${p.name}` }))}
          placeholder="Tất cả sản phẩm"
        />
      </div>

      {}
      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">Đang tải...</p>
      ) : (
        <>
          <p className="text-sm text-slate-500">Tổng: {totalCount} ảnh</p>
          <AdminTable
            
            columns={[
              { key: 'id', label: 'ID', style: { width: 60 } },
              {
                key: 'preview', label: 'Ảnh',
                
                render: (img) => (
                  <img src={imagePath(img.product?.slug || slugByProductId(img.productId), img.fileName)} alt=""
                    className="h-14 w-14 rounded-lg object-cover bg-slate-100"
                    onError={(e) => { e.target.src = IMAGE_PLACEHOLDER; }} />
                ),
              },
              {
                key: 'product', label: 'Sản phẩm',
                
                render: (img) => <span className="font-medium">{img.product?.name || `#${img.productId}`}</span>,
              },
              {
                key: 'color', label: 'Màu',
                
                render: (img) => {
                  const c = colorById(img.colorId);
                  if (!c) return <span className="text-xs text-slate-400">—</span>;
                  return (
                    <span className="inline-flex items-center gap-2 text-xs">
                      <span className="inline-block h-3 w-3 rounded-full border" style={{ backgroundColor: c.hexCode }} />
                      {c.name}
                    </span>
                  );
                },
              },
              {
                key: 'primary', label: 'Ảnh chính',
                
                render: (img) => img.isPrimary
                  ? <span className="flex items-center gap-1 text-amber-600 font-semibold text-xs"><Star size={12} /> Chính</span>
                  : <span className="text-xs text-slate-400">—</span>,
              },
              { key: 'order', label: 'Thứ tự', render: (img) => img.displayOrder },
              {
                key: 'fileName', label: 'Tên file',
                
                render: (img) => (
                  <span className="max-w-[200px] truncate block text-xs text-slate-600">{img.fileName}</span>
                ),
              },
              {
                key: 'actions', label: 'Thao tác',
                
                render: (img) => (
                  <div className="flex flex-wrap gap-1.5">
                    {!img.isPrimary && (
                      <button onClick={() => handleSetPrimary(img)}
                        className="rounded-lg bg-amber-50 px-2 py-1 text-xs font-semibold text-amber-700 ring-1 ring-amber-200 hover:bg-amber-100">
                        Đặt chính
                      </button>
                    )}
                    <button onClick={() => openEdit(img)}
                      className="rounded-lg bg-slate-900 px-2 py-1 text-xs font-semibold text-white hover:bg-slate-800">
                      Sửa
                    </button>
                    <button onClick={() => handleDelete(img)}
                      className="rounded-lg bg-rose-50 px-2 py-1 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100">
                      Xoá
                    </button>
                  </div>
                ),
              },
            ]}
            rows={items}
            emptyText="Không có ảnh nào."
          />
          {}
          <AdminPagination page={page} totalPages={totalPages} onChange={setPage} />
        </>
      )}

      {}
      <AdminModal
        open={showModal}
        title={editing ? 'Sửa ảnh' : 'Thêm ảnh mới'}
        onClose={() => setShowModal(false)}
        footer={
          
          <>
            <button type="button" onClick={() => setShowModal(false)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
              Huỷ
            </button>
            <button type="submit" form="image-form" disabled={submitting}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
              {submitting ? 'Đang lưu...' : editing ? 'Cập nhật' : 'Thêm'}
            </button>
          </>
        }
      >
        {}
        <form id="image-form" onSubmit={submit}>
          {error && <p className="mb-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>}

          {}
          <FormField label="Sản phẩm" required>
            <select className={inputClass} required value={form.productId}
              onChange={(e) => setForm({ ...form, productId: e.target.value })}>
              <option value="">-- Chọn sản phẩm --</option>
              {products.map((p) => <option key={p.id} value={p.id}>{p.id}. {p.name}</option>)}
            </select>
          </FormField>

          {}
          <FormField label="Màu" required>
            <select className={inputClass} required value={form.colorId}
              onChange={(e) => setForm({ ...form, colorId: e.target.value })}>
              <option value="">-- Chọn màu --</option>
              {colors.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </FormField>

          {}
          <FormField
            label={editing ? 'Thay ảnh (tuỳ chọn)' : 'Chọn ảnh từ máy'}
            required={!editing}
            hint={editing
              ? `File hiện tại: ${form.fileName || '—'}. Chỉ chọn file khi muốn thay.`
              : 'Hỗ trợ: .jpg, .png, .webp, .avif, .gif. Tối đa 20MB.'}>
            <input
              type="file"
              accept="image/jpeg,image/png,image/webp,image/avif,image/gif"
              required={!editing}
              onChange={handleFileChange}
              className="block w-full text-sm text-slate-600 file:mr-3 file:rounded-full file:border-0 file:bg-slate-900 file:px-4 file:py-2 file:text-sm file:font-semibold file:text-white file:cursor-pointer hover:file:bg-slate-800"
            />
          </FormField>

          {}
          {previewUrl ? (
            <div className="mb-3 overflow-hidden rounded-xl bg-slate-100">
              <img src={previewUrl} alt="preview" className="mx-auto max-h-48 object-contain" />
              <p className="px-3 py-1 text-xs text-slate-500">
                {file?.name} · {file ? (file.size / 1024).toFixed(1) : 0} KB
              </p>
            </div>
          ) : editing && form.fileName && form.productId && (
            <div className="mb-3 overflow-hidden rounded-xl bg-slate-100">
              <img src={imagePath(slugByProductId(form.productId), form.fileName)} alt="current"
                className="mx-auto max-h-48 object-contain"
                onError={(e) => { e.target.style.display = 'none'; }} />
            </div>
          )}

          {}
          <FormField label="Thứ tự hiển thị">
            <input type="number" className={inputClass} min={0}
              value={form.displayOrder} onChange={(e) => setForm({ ...form, displayOrder: e.target.value })} />
          </FormField>

          {}
          <label className="flex cursor-pointer items-center gap-2">
            <input type="checkbox" className="h-4 w-4 accent-slate-900"
              checked={form.isPrimary} onChange={(e) => setForm({ ...form, isPrimary: e.target.checked })} />
            <span className="text-sm text-slate-700">Đặt làm ảnh chính</span>
          </label>
        </form>
      </AdminModal>
    </div>
  );
};

export default ImagesSection;
