

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

// Trạng thái tiếp theo khi nhấn "Advance"
const NEXT_STATUS = {
  Pending:    'Processing',
  Processing: 'Shipping',
  Shipping:   'Delivered',
};

// Trạng thái cho phép huỷ (khớp AllowedTransitions backend)
const CANCELLABLE = new Set(['Pending', 'Processing', 'Shipping']);

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ ĐƠN HÀNG
// ════════════════════════════════════════════════════════════
export const OrdersSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [orders, setOrders]         = useState([]);
  const [loading, setLoading]       = useState(true);
  const [page, setPage]             = useState(1);
  const [pageSize]                  = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);

  const [keyword, setKeyword]       = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [minAmount, setMinAmount]   = useState('');
  const [maxAmount, setMaxAmount]   = useState('');
  const [dateFrom, setDateFrom]     = useState('');
  const [dateTo, setDateTo]         = useState('');

  // Detail modal
  const [detailOrder, setDetailOrder] = useState(null);
  const [detailItems, setDetailItems] = useState([]);
  const [showDetail, setShowDetail]   = useState(false);

  // Status modal
  const [statusOrder, setStatusOrder]     = useState(null);
  const [newStatus, setNewStatus]         = useState('');
  const [statusNote, setStatusNote]       = useState('');
  const [trackingInput, setTrackingInput] = useState('');
  const [statusLoading, setStatusLoading] = useState(false);

  const [msg, setMsg] = useState({ text: '', ok: true });

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => { load(); }, [page, keyword, statusFilter, minAmount, maxAmount, dateFrom, dateTo]);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = async () => {
    setLoading(true);
    try {
      const data = await adminService.getAllOrders({
        keyword:    keyword    || undefined,
        status:     statusFilter || undefined,
        minAmount:  minAmount  || undefined,
        maxAmount:  maxAmount  || undefined,
        dateFrom:   dateFrom   || undefined,
        dateTo:     dateTo     || undefined,
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
    setTrackingInput('');
  };

  const submitStatus = async () => {
    if (!newStatus) return;
    setStatusLoading(true);
    try {
      await adminService.updateOrderStatus(
        statusOrder.id,
        newStatus,
        statusNote || undefined,
        trackingInput || undefined,
      );
      flash(`Đơn ${statusOrder.orderCode || '#' + statusOrder.id} → ${STATUS_LABELS[newStatus]}`);
      setStatusOrder(null);
      load();
    } catch (e) {
      flash(e.response?.data?.message || 'Cập nhật thất bại', false);
    } finally {
      setStatusLoading(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Đơn hàng</h2>
      </div>

      {/* Flash message */}
      {msg.text && (
        <div className={`rounded-2xl px-4 py-3 text-sm ring-1 ${
          msg.ok ? 'bg-emerald-50 text-emerald-700 ring-emerald-200'
                 : 'bg-rose-50 text-rose-700 ring-rose-200'
        }`}>
          {msg.text}
        </div>
      )}

      {/* Bộ lọc */}
      <div className="grid grid-cols-2 gap-2 rounded-2xl border border-slate-200 bg-white p-3 lg:grid-cols-6">
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Mã đơn / Địa chỉ</label>
          <input
            className={inputClass}
            placeholder="VD: MOON-A1B2 hoặc tên đường..."
            value={keyword}
            onChange={(e) => { setPage(1); setKeyword(e.target.value); }}
          />
        </div>
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Trạng thái</label>
          <SearchableSelect
            value={statusFilter}
            onChange={(val) => { setPage(1); setStatusFilter(val); }}
            options={STATUS_OPTIONS.map((s) => ({ value: s, label: STATUS_LABELS[s] }))}
            placeholder="Tất cả"
          />
        </div>
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Tổng tối thiểu</label>
          <input type="number" min="0" className={inputClass} value={minAmount}
            onChange={(e) => { setPage(1); setMinAmount(e.target.value); }} />
        </div>
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Tổng tối đa</label>
          <input type="number" min="0" className={inputClass} value={maxAmount}
            onChange={(e) => { setPage(1); setMaxAmount(e.target.value); }} />
        </div>
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Từ ngày</label>
          <input type="date" className={inputClass} value={dateFrom}
            onChange={(e) => { setPage(1); setDateFrom(e.target.value); }} />
        </div>
        <div>
          <label className="mb-1 block text-xs font-medium text-slate-500">Đến ngày</label>
          <input type="date" className={inputClass} value={dateTo}
            onChange={(e) => { setPage(1); setDateTo(e.target.value); }} />
        </div>
      </div>

      {/* Danh sách */}
      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">
          Đang tải...
        </p>
      ) : (
        <>
          <p className="text-sm text-slate-500">Tổng: {totalCount} đơn hàng</p>
          <AdminTable
            columns={[
              {
                key: 'orderCode',
                label: 'Mã đơn',
                render: (o) => (
                  <span className="font-mono text-sm font-semibold text-slate-800">
                    {o.orderCode || `#${o.id}`}
                  </span>
                ),
              },
              { key: 'orderDate', label: 'Ngày đặt', render: (o) => fmtDate(o.createdAt) },
              {
                key: 'customer',
                label: 'Khách hàng',
                render: (o) => (
                  <span className="text-sm text-slate-600">
                    {o.customerName || o.customerEmail || `User #${o.userId || '—'}`}
                  </span>
                ),
              },
              {
                key: 'totalAmount',
                label: 'Tổng tiền',
                render: (o) => <span className="font-semibold">{fmt(o.totalAmount)}đ</span>,
              },
              {
                key: 'status',
                label: 'Trạng thái',
                render: (o) => (
                  <span className={`rounded-full px-3 py-1 text-xs font-semibold ${STATUS_CLASS[o.status] || ''}`}>
                    {STATUS_LABELS[o.status] || o.status}
                  </span>
                ),
              },
              {
                key: 'tracking',
                label: 'Mã vận đơn',
                render: (o) => o.trackingNumber
                  ? <span className="font-mono text-xs text-sky-700">{o.trackingNumber}</span>
                  : <span className="text-slate-300">—</span>,
              },
              {
                key: 'actions',
                label: 'Hành động',
                render: (o) => (
                  <div className="flex gap-2">
                    <button
                      onClick={() => openDetail(o)}
                      className="rounded-lg bg-slate-100 px-3 py-1.5 text-xs font-semibold text-slate-700 hover:bg-slate-200"
                    >
                      Chi tiết
                    </button>
                    {NEXT_STATUS[o.status] && (
                      <button
                        onClick={() => openStatus(o)}
                        className="rounded-lg bg-emerald-50 px-3 py-1.5 text-xs font-semibold text-emerald-700 ring-1 ring-emerald-200 hover:bg-emerald-100"
                      >
                        Tiến hành
                      </button>
                    )}
                    {CANCELLABLE.has(o.status) && (
                      <button
                        onClick={() => openStatus(o, true)}
                        className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100"
                      >
                        Huỷ
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

      {/* Modal chi tiết đơn hàng */}
      <AdminModal
        open={showDetail}
        title={detailOrder ? `Đơn hàng ${detailOrder.orderCode || '#' + detailOrder.id}` : ''}
        onClose={() => setShowDetail(false)}
        size="lg"
        footer={
          <button
            onClick={() => setShowDetail(false)}
            className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
          >
            Đóng
          </button>
        }
      >
        {detailOrder && (
          <>
            <div className="mb-4 grid grid-cols-2 gap-3 rounded-2xl bg-slate-50 p-4 text-sm">
              <p>
                <span className="text-slate-500">Trạng thái: </span>
                <span className={`rounded-full px-2 py-0.5 text-xs font-semibold ${STATUS_CLASS[detailOrder.status] || ''}`}>
                  {STATUS_LABELS[detailOrder.status]}
                </span>
              </p>
              <p><span className="text-slate-500">Ngày đặt: </span>{fmtDate(detailOrder.createdAt)}</p>
              <p><span className="text-slate-500">Dự kiến giao: </span>{fmtDate(detailOrder.estimatedDelivery)}</p>
              <p><span className="text-slate-500">Thanh toán: </span>{detailOrder.paymentMethod}</p>
              {detailOrder.trackingNumber && (
                <p className="col-span-2">
                  <span className="text-slate-500">Mã vận đơn: </span>
                  <span className="font-mono font-semibold text-sky-700">{detailOrder.trackingNumber}</span>
                </p>
              )}
              {detailOrder.customerName && (
                <p>
                  <span className="text-slate-500">Người nhận: </span>
                  {detailOrder.customerName} · {detailOrder.customerPhone}
                </p>
              )}
              <p className="col-span-2">
                <span className="text-slate-500">Địa chỉ: </span>{detailOrder.shippingAddress || '—'}
              </p>
              {detailOrder.note && (
                <p className="col-span-2">
                  <span className="text-slate-500">Ghi chú: </span>{detailOrder.note}
                </p>
              )}
            </div>

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
                    <td colSpan="4" className="py-4 text-center text-slate-400">Đang tải...</td>
                  </tr>
                ) : (
                  detailItems.map((d) => (
                    <tr key={d.id} className="border-b border-slate-100">
                      <td className="py-2">
                        {d.variant?.product?.name || `#${d.variantId}`}
                        {(d.variant?.color?.name || d.variant?.size?.name) && (
                          <span className="ml-1 text-xs text-slate-500">
                            ({[d.variant?.color?.name, d.variant?.size?.name].filter(Boolean).join(' · ')})
                          </span>
                        )}
                      </td>
                      <td className="py-2">{fmt(d.unitPrice)}đ</td>
                      <td className="py-2 text-center">{d.quantity}</td>
                      <td className="py-2 text-right">{fmt(d.quantity * d.unitPrice)}đ</td>
                    </tr>
                  ))
                )}
              </tbody>
              <tfoot>
                {detailOrder.discountAmount > 0 && (
                  <tr>
                    <td colSpan="3" className="py-1 text-right text-sm text-emerald-600">Giảm giá</td>
                    <td className="py-1 text-right text-sm text-emerald-600">-{fmt(detailOrder.discountAmount)}đ</td>
                  </tr>
                )}
                <tr>
                  <td colSpan="3" className="py-1 text-right text-sm text-slate-500">Phí vận chuyển</td>
                  <td className="py-1 text-right text-sm text-slate-500">
                    {detailOrder.shippingFee === 0 ? 'Miễn phí' : `${fmt(detailOrder.shippingFee)}đ`}
                  </td>
                </tr>
                <tr>
                  <td colSpan="3" className="py-3 text-right font-semibold">Tổng cộng</td>
                  <td className="py-3 text-right font-semibold">{fmt(detailOrder.totalAmount)}đ</td>
                </tr>
              </tfoot>
            </table>
          </>
        )}
      </AdminModal>

      {/* Modal cập nhật trạng thái */}
      <AdminModal
        open={!!statusOrder}
        title={statusOrder ? `Cập nhật trạng thái — ${statusOrder.orderCode || '#' + statusOrder.id}` : ''}
        onClose={() => setStatusOrder(null)}
        footer={
          <>
            <button
              type="button"
              onClick={() => setStatusOrder(null)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
            >
              Huỷ bỏ
            </button>
            <button
              type="button"
              onClick={submitStatus}
              disabled={!newStatus || statusLoading}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400"
            >
              {statusLoading ? 'Đang lưu...' : 'Xác nhận'}
            </button>
          </>
        }
      >
        {statusOrder && (
          <>
            <FormField label="Trạng thái mới" required>
              <select
                className={inputClass}
                value={newStatus}
                onChange={(e) => setNewStatus(e.target.value)}
              >
                <option value="">-- Chọn trạng thái --</option>
                {STATUS_OPTIONS.filter(
                  (s) => s !== statusOrder.status && s !== 'Pending'
                ).map((s) => (
                  <option key={s} value={s}>{STATUS_LABELS[s]}</option>
                ))}
              </select>
            </FormField>

            {/* Ô nhập mã vận đơn — chỉ hiện khi chuyển sang Đang giao */}
            {newStatus === 'Shipping' && (
              <FormField
                label="Mã vận đơn"
                hint="GHN, GHTK, Viettel Post... Sẽ hiển thị cho khách và gửi qua email."
              >
                <input
                  type="text"
                  className={inputClass}
                  value={trackingInput}
                  onChange={(e) => setTrackingInput(e.target.value)}
                  placeholder="VD: GHN-123456789"
                />
              </FormField>
            )}

            <FormField label="Ghi chú (tùy chọn)">
              <textarea
                rows="2"
                className={inputClass}
                value={statusNote}
                onChange={(e) => setStatusNote(e.target.value)}
                placeholder="Ghi chú nội bộ hoặc cho khách hàng..."
              />
            </FormField>
          </>
        )}
      </AdminModal>
    </div>
  );
};

export default OrdersSection;
