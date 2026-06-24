

import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';

import { Star, Plus, Trash2 } from 'lucide-react';
import { adminService } from '../../services/adminService';
import { AdminTable, AdminPagination } from './AdminTable';
import { AdminModal, FormField, inputClass } from './AdminModal';
import { SearchableSelect } from './SearchableSelect';
import { productService } from '../../services/productService';

import { fmt, IMAGE_PLACEHOLDER } from '../../utils/format';

import { VariantsModal } from './VariantsModal';

// ════════════════════════════════════════════════════════════
// HẰNG SỐ / SCHEMA
// ════════════════════════════════════════════════════════════
const imagePath = (slug, fileName) =>
  slug && fileName ? `/images/products/${slug}/${fileName}` : '';

// ════════════════════════════════════════════════════════════
// COMPONENT TAB ẢNH INLINE TRONG MODAL SẢN PHẨM
// ════════════════════════════════════════════════════════════
const InlineImagesTab = ({ productId, productSlug }) => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [images, setImages] = useState([]);
  const [colors, setColors] = useState([]);
  const [loading, setLoading] = useState(true);

  const [fileName, setFileName] = useState('');
  const [colorId, setColorId] = useState('');
  const [isPrimary, setIsPrimary] = useState(false);
  const [order, setOrder] = useState(0);
  const [adding, setAdding] = useState(false);
  const [err, setErr] = useState('');

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = useCallback(async () => {
    setLoading(true);
    try { setImages(await adminService.getImagesByProduct(productId) || []); }
    catch { setImages([]); }
    finally { setLoading(false); }
  }, [productId]);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => { load(); }, [load]);

  useEffect(() => {
    productService.getColors().then((cs) => {
      setColors(cs);
      if (cs.length && !colorId) setColorId(cs[0].id);
    }).catch(() => setColors([]));

  }, []);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const handleAdd = async () => {
    
    if (!fileName.trim() || !colorId) return;
    setAdding(true); setErr('');
    try {
      await adminService.createImage({
        fileName: fileName.trim(),
        colorId: Number(colorId),
        isPrimary,
        displayOrder: Number(order),
        productId: Number(productId),
      });
      
      setFileName(''); setIsPrimary(false); setOrder(0);
      load();
    } catch (e) { setErr(e.response?.data?.message || 'Thêm thất bại'); }
    finally { setAdding(false); }
  };

  const handleDelete = async (img) => {
    if (!window.confirm('Xoá ảnh này?')) return;
    await adminService.deleteImage(img.id);
    load();
  };

  const handleSetPrimary = async (img) => {
    await adminService.setImagePrimary(img.id);
    load();
  };

  if (loading) return <p className="py-6 text-center text-sm text-slate-400">Đang tải...</p>;

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      {images.length === 0 && (
        <p className="rounded-xl bg-slate-50 py-6 text-center text-sm text-slate-400">Chưa có ảnh nào.</p>
      )}

      {}
      <div className="grid grid-cols-3 gap-3 sm:grid-cols-4">
        {images.map((img) => {
          
          const c = colors.find((x) => x.id === img.colorId);
          return (
            <div key={img.id} className="relative rounded-xl border border-slate-200 bg-white p-1.5">
              <div className="relative aspect-square overflow-hidden rounded-lg bg-slate-100">
                {}
                <img src={imagePath(productSlug, img.fileName)} alt="" className="h-full w-full object-cover"
                  onError={(e) => { e.target.src = IMAGE_PLACEHOLDER; }} />
                {}
                {img.isPrimary && (
                  <span className="absolute left-1 top-1 flex items-center gap-0.5 rounded-full bg-amber-400 px-1.5 py-0.5 text-[9px] font-bold text-white">
                    <Star size={8} /> Chính
                  </span>
                )}
                {}
                {c && (
                  <span className="absolute bottom-1 left-1 inline-flex items-center gap-1 rounded-full bg-white/90 px-1.5 py-0.5 text-[9px] font-semibold">
                    <span className="inline-block h-2 w-2 rounded-full border" style={{ backgroundColor: c.hexCode }} />
                    {c.name}
                  </span>
                )}
              </div>
              {}
              <div className="mt-1.5 flex gap-1">
                {!img.isPrimary && (
                  <button onClick={() => handleSetPrimary(img)}
                    className="flex-1 rounded bg-amber-50 py-0.5 text-[10px] font-semibold text-amber-700 hover:bg-amber-100">
                    Đặt chính
                  </button>
                )}
                <button onClick={() => handleDelete(img)}
                  className="rounded bg-rose-50 p-0.5 text-rose-600 hover:bg-rose-100">
                  <Trash2 size={12} />
                </button>
              </div>
            </div>
          );
        })}
      </div>

      {}
      <div className="rounded-xl border border-slate-200 bg-slate-50 p-3">
        <p className="mb-2 text-xs font-semibold uppercase tracking-wider text-slate-500">Thêm ảnh mới</p>
        <div className="grid grid-cols-2 gap-2">
          <input className={inputClass} placeholder="Tên file (vd: red-front-01.jpg)"
            value={fileName} onChange={(e) => setFileName(e.target.value)} />
          <select className={inputClass} value={colorId}
            onChange={(e) => setColorId(e.target.value)}>
            <option value="">-- Chọn màu --</option>
            {colors.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>
        </div>
        {}
        {fileName && (
          <img src={imagePath(productSlug, fileName)} alt="preview" className="mt-2 max-h-24 rounded-lg object-contain"
            onError={(e) => { e.target.style.display = 'none'; }} />
        )}
        <div className="mt-2 flex items-center gap-3">
          <input type="number" className={`${inputClass} w-24`} min={0} placeholder="Thứ tự" value={order}
            onChange={(e) => setOrder(e.target.value)} />
          <label className="flex items-center gap-1.5 text-sm text-slate-700">
            <input type="checkbox" className="accent-slate-900" checked={isPrimary}
              onChange={(e) => setIsPrimary(e.target.checked)} />
            Ảnh chính
          </label>
          {}
          <button onClick={handleAdd} disabled={!fileName.trim() || !colorId || adding}
            className="ml-auto flex items-center gap-1 rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800 disabled:bg-slate-300">
            <Plus size={12} /> {adding ? 'Đang thêm...' : 'Thêm'}
          </button>
        </div>
        {err && <p className="mt-1.5 text-xs text-rose-600">{err}</p>}
      </div>
    </div>
  );
};

// ════════════════════════════════════════════════════════════
// COMPONENT NÚT TAB
// ════════════════════════════════════════════════════════════
const TabBtn = ({ active, onClick, children }) => (
  <button
    type="button"
    onClick={onClick}
    className={`border-b-2 px-4 py-2 text-sm font-medium transition ${
      active ? 'border-slate-900 text-slate-900' : 'border-transparent text-slate-400 hover:text-slate-700'
    }`}
  >
    {children}
  </button>
);

const emptyForm = { name: '', description: '', basePrice: 0, discountPercent: 0, categoryId: '', slug: '' };

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ SẢN PHẨM
// ════════════════════════════════════════════════════════════
export const ProductsSection = () => {
  const navigate = useNavigate();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);

  const [keyword, setKeyword] = useState('');
  const [categoryId, setCategoryId] = useState(''); 
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);

  const [showModal, setShowModal] = useState(false);   
  const [editing, setEditing] = useState(null);        
  const [formData, setFormData] = useState(emptyForm);
  
  const [activeTab, setActiveTab] = useState('info');
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);
  
  const [variantsFor, setVariantsFor] = useState(null);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    adminService.getCategories().then(setCategories).catch(() => {});
  }, []);

  useEffect(() => { load(); }, [page, keyword, categoryId]);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = async () => {
    setLoading(true);
    try {

      
      const data = await adminService.getProducts({ keyword, categoryId: categoryId || undefined, page, pageSize, sortBy: 'idDesc' });
      setProducts(data.items || []);
      setTotalPages(data.totalPages || 0);
      setTotalCount(data.totalCount || 0);
    } catch { setProducts([]); }
    finally { setLoading(false); }
  };

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const openCreate = () => {
    setEditing(null);
    
    setFormData({ ...emptyForm, categoryId: categories[0]?.id || '' });
    setActiveTab('info');
    setError('');
    setShowModal(true);
  };

  const openEdit = (p) => {
    setEditing(p);
    setFormData({ name: p.name, description: p.description || '', basePrice: p.basePrice || 0, discountPercent: p.discountPercent || 0, categoryId: p.categoryId, slug: p.slug || '' });
    setActiveTab('info'); 
    setError('');
    setShowModal(true);
  };

  const submit = async (e) => {
    e.preventDefault();
    setError(''); setSubmitting(true);
    try {
      
      const body = { ...formData, basePrice: parseFloat(formData.basePrice) || 0, discountPercent: Math.min(100, Math.max(0, parseInt(formData.discountPercent) || 0)), categoryId: parseInt(formData.categoryId) };
      if (editing) await adminService.updateProduct(editing.id, { id: editing.id, ...body });
      else await adminService.createProduct(body);
      setShowModal(false);
      load();
    } catch (err) { setError(err.response?.data?.message || 'Lưu thất bại'); }
    finally { setSubmitting(false); }
  };

  const handleDelete = async (p) => {
    if (!window.confirm(`Xoá sản phẩm "${p.name}"?`)) return;
    try { await adminService.deleteProduct(p.id); load(); }
    catch { alert('Xoá thất bại.'); }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Sản phẩm</h2>
        <button onClick={openCreate} className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800">
          + Thêm sản phẩm
        </button>
      </div>

      {}
      <div className="flex flex-wrap gap-2 rounded-2xl border border-slate-200 bg-white p-3">
        <input className={`${inputClass} flex-1 min-w-[180px]`} placeholder="Tìm theo tên..."
          value={keyword} onChange={(e) => { setPage(1); setKeyword(e.target.value); }} />
        <SearchableSelect
          className="w-44"
          value={categoryId}
          onChange={(val) => { setPage(1); setCategoryId(val); }}
          options={categories.map((c) => ({ value: c.id, label: c.name }))}
          placeholder="Tất cả danh mục"
        />
      </div>

      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">Đang tải...</p>
      ) : (
        <>
          <p className="text-sm text-slate-500">Tổng: {totalCount} sản phẩm</p>
          <AdminTable
            columns={[
              { key: 'id', label: 'ID', style: { width: 60 } },
              {
                key: 'name', label: 'Tên',
                
                render: (p) => {
                  const img = productService.getPrimaryImage(p);
                  return (
                    <div className="flex items-center gap-3">
                      {img ? <img src={img} alt="" className="h-10 w-10 rounded-lg object-cover" />
                           : <div className="h-10 w-10 rounded-lg bg-slate-100" />}
                      <span className="font-medium text-slate-900">{p.name}</span>
                    </div>
                  );
                },
              },
              { key: 'category', label: 'Danh mục', render: (p) => p.category?.name || '—' },
              {
                key: 'price', label: 'Giá',
                render: (p) => {
                  const disc = p.discountPercent || 0;
                  const effective = p.basePrice * (1 - disc / 100);
                  return disc > 0 ? (
                    <div className="flex flex-col leading-tight">
                      <span className="font-semibold text-rose-600">{fmt(effective)} đ</span>
                      <span className="text-xs text-slate-400 line-through">{fmt(p.basePrice)} đ</span>
                      <span className="mt-0.5 w-fit rounded-full bg-rose-100 px-1.5 py-0.5 text-[10px] font-bold text-rose-600">-{disc}%</span>
                    </div>
                  ) : (
                    <span className="font-semibold">{fmt(p.basePrice)} đ</span>
                  );
                },
              },
              
              { key: 'stock',    label: 'Tồn kho', render: (p) => productService.getTotalStock(p) },
              { key: 'variants', label: 'Variants', render: (p) => (p.variants || []).length },
              {
                key: 'actions', label: 'Thao tác',
                
                render: (p) => (
                  <div className="flex flex-wrap gap-1.5">
                    <button onClick={() => openEdit(p)}
                      className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800">
                      Sửa
                    </button>
                    {}
                    <button onClick={() => navigate(`/admin/images?productId=${p.id}`)}
                      className="rounded-lg bg-sky-50 px-3 py-1.5 text-xs font-semibold text-sky-700 ring-1 ring-sky-200 hover:bg-sky-100">
                      Ảnh
                    </button>
                    {}
                    <button onClick={() => setVariantsFor(p)}
                      className="rounded-lg bg-violet-50 px-3 py-1.5 text-xs font-semibold text-violet-700 ring-1 ring-violet-200 hover:bg-violet-100">
                      Biến thể
                    </button>
                    <button onClick={() => handleDelete(p)}
                      className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100">
                      Xoá
                    </button>
                  </div>
                ),
              },
            ]}
            rows={products}
            emptyText="Không có sản phẩm nào"
          />
          <AdminPagination page={page} totalPages={totalPages} onChange={setPage} />
        </>
      )}

      {}
      <AdminModal
        open={showModal}
        title={editing ? `Sửa: ${editing.name}` : 'Thêm sản phẩm'}
        
        size={editing ? 'xl' : 'lg'}
        onClose={() => setShowModal(false)}
        footer={

          
          activeTab === 'info' ? (
            <>
              <button type="button" onClick={() => setShowModal(false)}
                className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
                {editing ? 'Đóng' : 'Huỷ'}
              </button>
              <button type="submit" form="product-form" disabled={submitting}
                className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
                {submitting ? 'Đang lưu...' : editing ? 'Cập nhật' : 'Thêm'}
              </button>
            </>
          ) : (
            <button type="button" onClick={() => setShowModal(false)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
              Đóng
            </button>
          )
        }
      >
        {}
        {editing && (
          <div className="mb-4 flex border-b border-slate-200">
            <TabBtn active={activeTab === 'info'}   onClick={() => setActiveTab('info')}>Thông tin</TabBtn>
            <TabBtn active={activeTab === 'images'} onClick={() => setActiveTab('images')}>Ảnh</TabBtn>
          </div>
        )}

        {}
        {activeTab === 'info' && (
          <form id="product-form" onSubmit={submit}>
            {error && <p className="mb-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>}
            <FormField label="Tên" required>
              <input className={inputClass} required value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })} />
            </FormField>
            <FormField label="Danh mục" required>
              <select className={inputClass} required value={formData.categoryId}
                onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}>
                <option value="">Chọn danh mục</option>
                {categories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </FormField>
            <FormField label="Giá cơ bản" required hint="Có thể override ở từng variant">
              <input type="number" min="0" required className={inputClass}
                value={formData.basePrice}
                onChange={(e) => setFormData({ ...formData, basePrice: e.target.value })} />
            </FormField>
            <FormField label="% Giảm giá" hint={`Giá sau giảm: ${fmt((parseFloat(formData.basePrice) || 0) * (1 - (parseInt(formData.discountPercent) || 0) / 100))} đ`}>
              <input type="number" min="0" max="100" className={inputClass}
                value={formData.discountPercent}
                onChange={(e) => setFormData({ ...formData, discountPercent: e.target.value })} />
            </FormField>
            <FormField label="Mô tả">
              <textarea rows="4" className={inputClass} value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })} />
            </FormField>
          </form>
        )}

        {}
        {activeTab === 'images' && editing && (
          <InlineImagesTab productId={editing.id} productSlug={editing.slug} />
        )}
      </AdminModal>

      {}
      <VariantsModal
        open={!!variantsFor}
        product={variantsFor}
        onClose={() => { setVariantsFor(null); load(); }}
      />
    </div>
  );
};

export default ProductsSection;
