

import { useEffect, useState } from 'react';
import { adminService } from '../../services/adminService';
import { AdminTable, AdminPagination } from './AdminTable';
import { AdminModal, FormField, inputClass } from './AdminModal';
import { SearchableSelect } from './SearchableSelect';

import { fmt, fmtDate } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// HẰNG SỐ / SCHEMA
// ════════════════════════════════════════════════════════════
const STATUS_OPTIONS = ['Pending', 'Processing', 'Shipping', 'Delivered', 'Cancelled'];

const STATUS_LABELS = {
  Pending:    'Chờ xác nhận',
  Processing: 'Đang xử lý',
  Shipping:   'Đang giao',
  Delivered:  'Đã giao',
  Cancelled:  'Đã huỷ',
};

const STATUS_CLASS = {
  Pending:    'bg-amber-50 text-amber-700 ring-1 ring-amber-200',
  Processing: 'bg-blue-50 text-blue-700 ring-1 ring-blue-200',
  Shipping:   'bg-sky-50 text-sky-700 ring-1 ring-sky-200',
  Delivered:  'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200',
  Cancelled:  'bg-rose-50 text-rose-700 ring-1 ring-rose-200',
};

const NEXT_STATUS = { Pending: 'Processing', Processing: 'Shipping', Shipping: 'Delivered' };

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ ĐƠN HÀNG
// ════════════════════════════════════════════════════════════
export const OrdersSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);

  
  const [keyword, setKeyword] = useState('');
  
  const [statusFilter, setStatusFilter] = useState('');
  
  const [minAmount, setMinAmount] = useState('');
  const [maxAmount, setMaxAmount] = useState('');
  
  const [dateFrom, setDateFrom] = useState('');
  const [dateTo, setDateTo] = useState('');

  
  const [detailOrder, setDetailOrder] = useState(null);
  
  const [detailItems, setDetailItems] = useState([]);
  const [showDetail, setShowDetail] = useState(false);

  
  const [statusOrder, setStatusOrder] = useState(null);
  
  const [newStatus, setNewStatus] = useState('');
  
  const [statusNote, setStatusNote] = useState('');
  
  const [estDelivery, setEstDelivery] = useState('');
  const [statusLoading, setStatusLoading] = useState(false);

  
  const [msg, setMsg] = useState({ text: '', ok: true });

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    load();

  }, [page, keyword, statusFilter, minAmount, maxAmount, dateFrom, dateTo]);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = async () => {
    setLoading(true);
    try {
      
      const data = await adminService.getAllOrders({
        keyword: keyword || undefined,
        status: statusFilter || undefined,
        minAmount: minAmount || undefined,
        maxAmount: maxAmount || undefined,
        dateFrom: dateFrom || undefined,
        dateTo: dateTo || undefined,
        page,
        pageSize,
      });
      
      setOrders(data.items || []);
      setTotalCount(data.totalCount || 0);
      setTotalPages(data.totalPages || 0);
    } catch (e) {
      console.error(e);
      setOrders([]);
    } finally {
      setLoading(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const flash = (text, ok = true) => {
    setMsg({ text, ok });
    setTimeout(() => setMsg({ text: '', ok: true }), 3000);
  };

  
  
  const openDetail = async (order) => {
    setDetailOrder(order);
    setDetailItems([]); 
    setShowDetail(true);
    try {
      
      const data = await adminService.getOrderById(order.id);
      setDetailItems(data.orderDetails || []);
    } catch {
      setDetailItems([]);
    }
  };

  

  
  const openStatus = (order, forceCancel = false) => {
    setStatusOrder(order);
    setNewStatus(forceCancel ? 'Cancelled' : NEXT_STATUS[order.status] || '');
    setStatusNote('');
    setEstDelivery('');
  };

  
  
  const submitStatus = async () => {
    if (!newStatus) return; 
    setStatusLoading(true);
    try {
      await adminService.updateOrderStatus(statusOrder.id, newStatus, statusNote || undefined);
      flash(`Order #${statusOrder.id} → ${newStatus}`);
      setStatusOrder(null); 
      load();               
    } catch (e) {
      flash(e.response?.data?.message || 'Update failed', false);
    } finally {
      setStatusLoading(false);
    }
  };

  const shortGuid = (id) => (id ? String(id).substring(0, 8).toUpperCase() : '—');

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Đơn hàng</h2>
      </div>

      {}
      {msg.text && (
        <div
          className={`rounded-2xl px-4 py-3 text-sm ring-1 ${
            msg.ok
              ? 'bg-emerald-50 text-emerald-700 ring-emerald-200'
              : 'bg-rose-50 text-rose-700 ring-rose-200'
          }`}
        >
          {msg.text}
        </div>
      )}

      {}
      {}
      <div className="grid grid-cols-2 gap-2 rounded-2xl border border-slate-200 bg-white p-3 lg:grid-cols-6">
        {}
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Mã / Địa chỉ</label>
          <input
            className={inputClass}
            placeholder="VD: 12 hoặc tên đường..."
            value={keyword}
            onChange={(e) => {
              setPage(1); 
              setKeyword(e.target.value);
            }}
          />
        </div>

        {}
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Trạng thái</label>
          <SearchableSelect
            value={statusFilter}
            onChange={(val) => { setPage(1); setStatusFilter(val); }}
            options={STATUS_OPTIONS.map((s) => ({ value: s, label: STATUS_LABELS[s] }))}
            placeholder="Tất cả"
          />
        </div>

        {}
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Tổng tối thiểu</label>
          <input
            type="number"
            min="0"
            className={inputClass}
            value={minAmount}
            onChange={(e) => {
              setPage(1);
              setMinAmount(e.target.value);
            }}
          />
        </div>

        {}
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Tổng tối đa</label>
          <input
            type="number"
            min="0"
            className={inputClass}
            value={maxAmount}
            onChange={(e) => {
              setPage(1);
              setMaxAmount(e.target.value);
            }}
          />
        </div>

        {}
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Từ ngày</label>
          <input
            type="date"
            className={inputClass}
            value={dateFrom}
            onChange={(e) => {
              setPage(1);
              setDateFrom(e.target.value);
            }}
          />
        </div>

        {}
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Đến ngày</label>
          <input
            type="date"
            className={inputClass}
            value={dateTo}
            onChange={(e) => {
              setPage(1);
              setDateTo(e.target.value);
            }}
          />
        </div>
      </div>

      {}
      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">
          Loading...
        </p>
      ) : (
        <>
          <p className="text-sm text-slate-500">Total: {totalCount} orders</p>
          <AdminTable
            columns={[
              { key: 'id', label: 'ID', render: (o) => <span className="font-semibold">#{o.id}</span> },
              { key: 'orderDate', label: 'Date', render: (o) => fmtDate(o.createdAt) },
              {
                key: 'userId',
                label: 'Customer',
                
                render: (o) => <code className="text-xs text-slate-500">{shortGuid(o.userId)}</code>,
              },
              {
                key: 'shippingAddress',
                label: 'Địa chỉ',
                
                render: (o) => (
                  <span className="block max-w-[180px] truncate text-sm text-slate-600">
                    {o.shippingAddress || '—'}
                  </span>
                ),
              },
              {
                key: 'totalAmount',
                label: 'Total',
                render: (o) => <span className="font-semibold">{fmt(o.totalAmount)} đ</span>,
              },
              {
                key: 'status',
                label: 'Status',
                
                render: (o) => (
                  <span
                    className={`rounded-full px-3 py-1 text-xs font-semibold ${STATUS_CLASS[o.status] || ''}`}
                  >
                    {STATUS_LABELS[o.status] || o.status}
                  </span>
                ),
              },
              { key: 'estimatedDelivery', label: 'Estimated', render: (o) => fmtDate(o.estimatedDelivery) },
              {
                key: 'actions',
                label: 'Actions',
                render: (o) => (
                  <div className="flex gap-2">
                    {}
                    <button
                      onClick={() => openDetail(o)}
                      className="rounded-lg bg-slate-100 px-3 py-1.5 text-xs font-semibold text-slate-700 hover:bg-slate-200"
                    >
                      View
                    </button>
                    {}
                    {NEXT_STATUS[o.status] && (
                      <button
                        onClick={() => openStatus(o)}
                        className="rounded-lg bg-emerald-50 px-3 py-1.5 text-xs font-semibold text-emerald-700 ring-1 ring-emerald-200 hover:bg-emerald-100"
                      >
                        Advance
                      </button>
                    )}
                    {}
                    {(o.status === 'Pending' || o.status === 'Shipping') && (
                      <button
                        onClick={() => openStatus(o, true)}
                        className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100"
                      >
                        Cancel
                      </button>
                    )}
                  </div>
                ),
              },
            ]}
            rows={orders}
            emptyText="Không có đơn hàng nào"
          />
          <AdminPagination page={page} totalPages={totalPages} onChange={setPage} />
        </>
      )}

      {}
      <AdminModal
        open={showDetail}
        title={detailOrder ? `Order #${detailOrder.id}` : ''}
        onClose={() => setShowDetail(false)}
        size="lg"
        footer={
          <button
            onClick={() => setShowDetail(false)}
            className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
          >
            Close
          </button>
        }
      >
        {detailOrder && (
          <>
            {}
            <div className="mb-4 grid grid-cols-2 gap-3 rounded-2xl bg-slate-50 p-4 text-sm">
              <p>
                <span className="text-slate-500">Status:</span>{' '}
                <span
                  className={`rounded-full px-2 py-0.5 text-xs font-semibold ${STATUS_CLASS[detailOrder.status] || ''}`}
                >
                  {STATUS_LABELS[detailOrder.status]}
                </span>
              </p>
              <p>
                <span className="text-slate-500">Date:</span> {fmtDate(detailOrder.createdAt)}
              </p>
              <p>
                <span className="text-slate-500">Estimated:</span>{' '}
                {fmtDate(detailOrder.estimatedDelivery)}
              </p>
              {}
              <p className="col-span-2">
                <span className="text-slate-500">Địa chỉ:</span>{' '}
                {detailOrder.shippingAddress || '—'}
              </p>
              {}
              {detailOrder.note && (
                <p className="col-span-2">
                  <span className="text-slate-500">Note:</span> {detailOrder.note}
                </p>
              )}
            </div>

            {}
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-200 text-left text-xs uppercase tracking-wider text-slate-500">
                  <th className="py-2">Sản phẩm</th>
                  <th className="py-2">Đơn giá</th>
                  <th className="py-2 text-center">SL</th>
                  <th className="py-2 text-right">Tạm tính</th>
                </tr>
              </thead>
              <tbody>
                {detailItems.length === 0 ? (
                  
                  <tr>
                    <td colSpan="4" className="py-4 text-center text-slate-400">
                      Loading...
                    </td>
                  </tr>
                ) : (
                  detailItems.map((d) => (
                    <tr key={d.id} className="border-b border-slate-100">
                      <td className="py-2">
                        {}
                        {d.variant?.product?.name || `#${d.variantId}`}
                        {}
                        {(d.variant?.color?.name || d.variant?.size?.name) && (
                          <span className="ml-1 text-xs text-slate-500">
                            ({d.variant?.color?.name} · {d.variant?.size?.name})
                          </span>
                        )}
                      </td>
                      <td className="py-2">{fmt(d.unitPrice)} đ</td>
                      <td className="py-2 text-center">{d.quantity}</td>
                      {}
                      <td className="py-2 text-right">{fmt(d.quantity * d.unitPrice)} đ</td>
                    </tr>
                  ))
                )}
              </tbody>
              <tfoot>
                <tr>
                  <td colSpan="3" className="py-3 text-right font-semibold">Tổng cộng</td>
                  <td className="py-3 text-right font-semibold">{fmt(detailOrder.totalAmount)} đ</td>
                </tr>
              </tfoot>
            </table>
          </>
        )}
      </AdminModal>

      {}
      <AdminModal
        
        open={!!statusOrder}
        title={statusOrder ? `Update status — Order #${statusOrder.id}` : ''}
        onClose={() => setStatusOrder(null)}
        footer={
          <>
            <button
              type="button"
              onClick={() => setStatusOrder(null)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
            >
              Cancel
            </button>
            <button
              type="button"
              onClick={submitStatus}
              disabled={!newStatus || statusLoading}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400"
            >
              {statusLoading ? 'Saving...' : 'Update'}
            </button>
          </>
        }
      >
        {statusOrder && (
          <>
            {}
            <FormField label="Trạng thái mới" required>
              <select
                className={inputClass}
                value={newStatus}
                onChange={(e) => setNewStatus(e.target.value)}
              >
                <option value="">-- Select status --</option>
                {STATUS_OPTIONS.filter((s) => s !== statusOrder.status && s !== 'Pending').map(
                  (s) => (
                    <option key={s} value={s}>
                      {STATUS_LABELS[s]}
                    </option>
                  )
                )}
              </select>
            </FormField>

            {}
            {newStatus === 'Shipping' && (
              <FormField label="Dự kiến giao" hint="Để trống sẽ mặc định +3 ngày kể từ hôm nay.">
                <input
                  type="datetime-local"
                  className={inputClass}
                  value={estDelivery}
                  onChange={(e) => setEstDelivery(e.target.value)}
                />
              </FormField>
            )}

            {}
            <FormField label="Ghi chú (tùy chọn)">
              <textarea
                rows="2"
                className={inputClass}
                value={statusNote}
                onChange={(e) => setStatusNote(e.target.value)}
                placeholder="Ghi chú cho khách hàng..."
              />
            </FormField>
          </>
        )}
      </AdminModal>
    </div>
  );
};

export default OrdersSection;
