

import { useEffect, useState, useCallback } from 'react';

import { useNavigate } from 'react-router-dom';
import { adminService } from '../../services/adminService';
import { AdminTable, AdminPagination } from './AdminTable';
import { inputClass } from './AdminModal';

import { AlertTriangle, Plus, Minus } from 'lucide-react';

import { fmt } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// COMPONENT NHÃN TRẠNG THÁI TỒN KHO
// ════════════════════════════════════════════════════════════
const StockBadge = ({ available }) => {
  if (available <= 0)  return <span className="rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200">Hết hàng</span>;
  if (available <= 5)  return <span className="rounded-full bg-amber-50 px-2 py-0.5 text-xs font-semibold text-amber-700 ring-1 ring-amber-200">Sắp hết ({available})</span>;
  return <span className="rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-700 ring-1 ring-emerald-200">Còn hàng ({available})</span>;
};

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ TỒN KHO
// ════════════════════════════════════════════════════════════
export const InventorySection = () => {
  const navigate = useNavigate();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [items, setItems]         = useState([]);
  const [loading, setLoading]     = useState(true);
  const [keyword, setKeyword]     = useState('');    
  const [page, setPage]           = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize]                = useState(20);    

  

  const [adjustId, setAdjustId]   = useState(null);

  const [delta, setDelta]         = useState('');
  const [adjustErr, setAdjustErr] = useState('');
  const [adjusting, setAdjusting] = useState(false); 
  
  const [flash, setFlash]         = useState('');

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = useCallback(async () => {
    setLoading(true);
    try {
      const d = await adminService.getInventory({ keyword, page, pageSize });
      
      setItems(d.items || []);
      setTotalPages(d.totalPages || 1);
      setTotalCount(d.total || 0);
    } catch {
      setItems([]);
    } finally {
      setLoading(false);
    }
  }, [keyword, page, pageSize]);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => { load(); }, [load]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const openAdjust = (v) => { setAdjustId(v); setDelta(''); setAdjustErr(''); };

  
  
  const submitAdjust = async () => {
    
    const num = parseInt(delta, 10);
    
    if (isNaN(num) || num === 0) { setAdjustErr('Nhập số khác 0 (dương: nhập thêm, âm: xuất kho)'); return; }

    setAdjusting(true);
    try {

      await adminService.adjustStock(adjustId.id, num);
      setAdjustId(null); 
      setFlash('Cập nhật tồn kho thành công!');
      
      setTimeout(() => setFlash(''), 2500);
      load(); 
    } catch (e) {
      setAdjustErr(e.response?.data?.message || 'Cập nhật thất bại');
    } finally {
      setAdjusting(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-semibold text-slate-900">Tồn kho (SKU)</h2>
          <p className="mt-1 text-sm text-slate-500">Quản lý số lượng từng biến thể sản phẩm</p>
        </div>
      </div>

      {}
      {flash && (
        <div className="rounded-xl bg-emerald-50 px-4 py-2 text-sm font-medium text-emerald-700 ring-1 ring-emerald-200">
          {flash}
        </div>
      )}

      {}
      <div className="rounded-2xl border border-slate-200 bg-white p-3">
        <input
          className={`${inputClass} w-full`}
          placeholder="Tìm theo SKU hoặc tên sản phẩm..."
          value={keyword}
          
          onChange={(e) => { setKeyword(e.target.value); setPage(1); }}
        />
      </div>

      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-8 text-center text-slate-500">Đang tải...</p>
      ) : (
        <>
          <p className="text-sm text-slate-500">Hiển thị {items.length} / {fmt(totalCount)} biến thể</p>
          <AdminTable
            columns={[
              
              { key: 'sku',     label: 'SKU', render: (v) => <span className="font-mono text-xs">{v.sku}</span> },
              { key: 'product', label: 'Sản phẩm', render: (v) => <span className="font-medium">{v.product?.name}</span> },
              
              { key: 'color',   label: 'Màu', render: (v) => v.color
                  ? <span className="flex items-center gap-1.5">
                      <span className="inline-block h-3 w-3 rounded-full border" style={{ backgroundColor: v.color.hexCode }} />
                      {v.color.name}
                    </span>
                  : '—'
              },
              { key: 'size',    label: 'Size',  render: (v) => v.size?.name || '—' },

              { key: 'stock',   label: 'Tổng kho', render: (v) => fmt(v.stock) },
              { key: 'reserved',label: 'Đã giữ',   render: (v) => fmt(v.reservedStock) },
              { key: 'avail',   label: 'Khả dụng', render: (v) => <StockBadge available={v.stock - v.reservedStock} /> },
              { key: 'price',   label: 'Giá', render: (v) => `${fmt(v.price)} đ` },
              {
                key: 'actions', label: 'Thao tác',
                render: (v) => (
                  <div className="flex flex-wrap gap-1.5">
                    {}
                    <button
                      onClick={() => openAdjust(v)}
                      className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800"
                    >
                      Nhập/Xuất
                    </button>
                    {}
                    <button
                      onClick={() => navigate(`/admin/images?productId=${v.product?.id}`)}
                      className="rounded-lg bg-sky-50 px-3 py-1.5 text-xs font-semibold text-sky-700 ring-1 ring-sky-200 hover:bg-sky-100"
                    >
                      Ảnh
                    </button>
                    {}
                    <button
                      onClick={() => navigate(`/admin/sizeguides?productId=${v.product?.id}`)}
                      className="rounded-lg bg-violet-50 px-3 py-1.5 text-xs font-semibold text-violet-700 ring-1 ring-violet-200 hover:bg-violet-100"
                    >
                      Size
                    </button>
                  </div>
                ),
              },
            ]}
            rows={items}
            emptyText="Không có biến thể nào"
          />
          <AdminPagination page={page} totalPages={totalPages} onChange={setPage} />
        </>
      )}

      {}
      {adjustId && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-sm rounded-2xl bg-white p-6 shadow-2xl">
            <h3 className="text-lg font-semibold text-slate-900">Điều chỉnh tồn kho</h3>

            {}
            <div className="mt-3 space-y-1 rounded-xl bg-slate-50 p-3 text-sm text-slate-600">
              <p><span className="font-medium">SKU:</span> {adjustId.sku}</p>
              <p><span className="font-medium">Sản phẩm:</span> {adjustId.product?.name} — {adjustId.color?.name} / {adjustId.size?.name}</p>
              <p><span className="font-medium">Tồn kho hiện tại:</span> {adjustId.stock} (khả dụng: {adjustId.stock - adjustId.reservedStock})</p>
            </div>

            {}
            <div className="mt-4 flex items-center gap-2">
              <button onClick={() => setDelta((d) => String((parseInt(d, 10) || 0) - 1))} className="rounded-lg border border-slate-200 p-2 hover:bg-slate-50"><Minus size={16} /></button>
              <input
                type="number"
                value={delta}
                onChange={(e) => setDelta(e.target.value)}
                placeholder="VD: 10 hoặc -5"
                className={`${inputClass} flex-1 text-center`}
              />
              <button onClick={() => setDelta((d) => String((parseInt(d, 10) || 0) + 1))} className="rounded-lg border border-slate-200 p-2 hover:bg-slate-50"><Plus size={16} /></button>
            </div>
            <p className="mt-1 text-xs text-slate-400">Số dương = nhập kho, số âm = xuất kho</p>

            {}
            {adjustErr && <p className="mt-2 text-sm text-rose-600">{adjustErr}</p>}

            {}
            <div className="mt-5 flex justify-end gap-3">
              <button onClick={() => setAdjustId(null)} className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
                Huỷ
              </button>
              <button onClick={submitAdjust} disabled={adjusting} className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
                {adjusting ? 'Đang lưu...' : 'Xác nhận'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default InventorySection;
