

import { useState } from 'react';

import { Search, Package, MapPin } from 'lucide-react';

import { orderService } from '../services/orderService';

import { OrderStatusTimeline } from '../components/order/OrderStatusTimeline';

import { fmt, fmtDateTime } from '../utils/format';

// ════════════════════════════════════════════════════════════
// CONSTANTS / SCHEMA
// ════════════════════════════════════════════════════════════
const STATUS_LABEL = {
  Pending: 'Chờ xác nhận', Processing: 'Đang xử lý',
  Shipping: 'Đang giao', Delivered: 'Đã giao', Cancelled: 'Đã huỷ',
};

// ════════════════════════════════════════════════════════════
// TRANG TRA CỨU ĐƠN HÀNG (khách vãng lai)
// ════════════════════════════════════════════════════════════
const OrderLookupPage = () => {
  const [code, setCode] = useState('');
  const [contact, setContact] = useState('');
  const [order, setOrder] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setOrder(null);
    if (!code.trim() || !contact.trim()) {
      setError('Vui lòng nhập mã đơn hàng và email/số điện thoại đã đặt.');
      return;
    }
    setLoading(true);
    try {
      const data = await orderService.trackOrder(code.trim(), contact.trim());
      setOrder(data);
    } catch (err) {
      setError(err.response?.data?.message || 'Không tìm thấy đơn hàng khớp với thông tin đã nhập.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-slate-50 min-h-screen">
      <div className="mx-auto max-w-3xl px-4 py-16 sm:px-6 lg:px-8">
        {/* ── Tiêu đề ── */}
        <div className="text-center">
          <p className="text-xs uppercase tracking-[0.4em] text-slate-400">Theo dõi</p>
          <h1 className="mt-2 font-serif text-4xl font-light text-slate-900">Tra cứu đơn hàng</h1>
          <p className="mt-3 text-sm text-slate-500">
            Nhập mã đơn hàng (vd: MOON-ABC123) cùng email hoặc số điện thoại bạn đã dùng khi đặt.
          </p>
        </div>

        {/* ── Form tra cứu ── */}
        <form onSubmit={handleSubmit} className="mt-8 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="grid gap-4 sm:grid-cols-2">
            <div>
              <label className="mb-1 block text-xs font-medium uppercase tracking-wider text-slate-500">Mã đơn hàng</label>
              <input
                value={code}
                onChange={(e) => setCode(e.target.value.toUpperCase())}
                placeholder="MOON-ABC123"
                className="w-full rounded-lg border border-slate-200 px-4 py-3 text-sm uppercase outline-none focus:border-slate-900 focus:ring-2 focus:ring-slate-900/10"
              />
            </div>
            <div>
              <label className="mb-1 block text-xs font-medium uppercase tracking-wider text-slate-500">Email / Số điện thoại</label>
              <input
                value={contact}
                onChange={(e) => setContact(e.target.value)}
                placeholder="email@example.com hoặc 09xxxxxxxx"
                className="w-full rounded-lg border border-slate-200 px-4 py-3 text-sm outline-none focus:border-slate-900 focus:ring-2 focus:ring-slate-900/10"
              />
            </div>
          </div>

          {error && <p className="mt-4 rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700 ring-1 ring-rose-200">{error}</p>}

          <button
            type="submit"
            disabled={loading}
            className="mt-5 flex w-full items-center justify-center gap-2 rounded-xl bg-slate-900 py-3.5 text-sm font-semibold uppercase tracking-wider text-white transition hover:bg-slate-700 disabled:bg-slate-300"
          >
            <Search size={16} />
            {loading ? 'Đang tra cứu...' : 'Tra cứu'}
          </button>
        </form>

        {/* ── Kết quả ── */}
        {order && (
          <div className="mt-8 space-y-6">
            {/* Trạng thái + timeline */}
            <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                  <p className="text-xs uppercase tracking-[0.3em] text-slate-400">Mã đơn</p>
                  <p className="mt-1 text-lg font-semibold text-slate-900">{order.orderCode || `#${order.id}`}</p>
                </div>
                <div className="text-right">
                  <p className="text-xs uppercase tracking-[0.3em] text-slate-400">Trạng thái</p>
                  <p className="mt-1 text-sm font-medium text-slate-700">{STATUS_LABEL[order.status] || order.status}</p>
                </div>
              </div>
              <div className="mt-6 border-t border-slate-100 pt-6">
                <OrderStatusTimeline status={order.status} />
              </div>
              <div className="mt-6 grid gap-4 border-t border-slate-100 pt-5 text-sm sm:grid-cols-2">
                <div className="flex items-start gap-3">
                  <MapPin size={18} className="mt-0.5 shrink-0 text-slate-400" />
                  <div>
                    <p className="text-xs uppercase tracking-wider text-slate-400">Địa chỉ giao</p>
                    <p className="mt-1 text-slate-700">{order.shippingAddress || '—'}</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <Package size={18} className="mt-0.5 shrink-0 text-slate-400" />
                  <div>
                    <p className="text-xs uppercase tracking-wider text-slate-400">Đặt lúc</p>
                    <p className="mt-1 text-slate-700">{fmtDateTime(order.createdAt)}</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Sản phẩm + tổng tiền */}
            <div className="rounded-2xl border border-slate-200 bg-white shadow-sm">
              <div className="flex items-center gap-2 border-b border-slate-200 px-6 py-4">
                <Package size={18} className="text-slate-400" />
                <span className="text-sm font-semibold text-slate-700">
                  Sản phẩm ({(order.orderDetails || []).length})
                </span>
              </div>
              <div className="divide-y divide-slate-100">
                {(order.orderDetails || []).map((d) => {
                  const p = d.variant?.product;
                  const name = p?.name || `Sản phẩm #${d.variantId}`;
                  return (
                    <div key={d.id} className="flex items-center gap-4 px-6 py-4">
                      <div className="flex-1">
                        <p className="font-medium text-slate-900">{name}</p>
                        <p className="text-xs text-slate-500">
                          {d.variant?.color?.name} {d.variant?.size?.name && `· Size ${d.variant.size.name}`}
                        </p>
                        <p className="text-sm text-slate-500">{d.quantity} × {fmt(d.unitPrice)} đ</p>
                      </div>
                      <p className="text-sm font-semibold text-slate-900">{fmt(d.quantity * d.unitPrice)} đ</p>
                    </div>
                  );
                })}
              </div>
              <div className="space-y-1 border-t border-slate-200 px-6 py-4 text-sm">
                <div className="flex justify-between text-slate-600">
                  <span>Phí ship</span>
                  <span>{order.shippingFee === 0 ? 'Miễn phí' : `${fmt(order.shippingFee)} đ`}</span>
                </div>
                {order.discountAmount > 0 && (
                  <div className="flex justify-between text-green-600">
                    <span>Giảm giá</span>
                    <span>-{fmt(order.discountAmount)} đ</span>
                  </div>
                )}
                <div className="flex justify-between border-t border-slate-100 pt-2 text-base font-semibold">
                  <span>Tổng</span>
                  <span>{fmt(order.totalAmount)} đ</span>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default OrderLookupPage;
