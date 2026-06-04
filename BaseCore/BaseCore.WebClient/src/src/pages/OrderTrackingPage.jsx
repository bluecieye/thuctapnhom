

import { useState, useEffect } from 'react';

import { Link } from 'react-router-dom';

import { useAuth } from '../contexts/AuthContext';

import { orderService } from '../services/orderService';

// ════════════════════════════════════════════════════════════
// CONSTANTS / SCHEMA
// ════════════════════════════════════════════════════════════
const STATUS_CLASS = {
  Pending:    'bg-amber-50 text-amber-700 ring-1 ring-amber-200',
  Processing: 'bg-blue-50 text-blue-700 ring-1 ring-blue-200',
  Shipping:   'bg-sky-50 text-sky-700 ring-1 ring-sky-200',
  Delivered:  'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200',
  Cancelled:  'bg-rose-50 text-rose-700 ring-1 ring-rose-200',
};

const STATUS_LABEL = {
  Pending: 'Chờ xác nhận', Processing: 'Đang xử lý',
  Shipping: 'Đang giao', Delivered: 'Đã giao', Cancelled: 'Đã huỷ',
};

import { fmt } from '../utils/format';
const fmtDate = (d) =>
  d
    ? new Date(d).toLocaleString('vi-VN', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      })
    : '—';

// ════════════════════════════════════════════════════════════
// TRANG THEO DÕI ĐƠN HÀNG
// ════════════════════════════════════════════════════════════
const OrderTrackingPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const { user, loading: authLoading } = useAuth();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [orders, setOrders] = useState([]);

  const [loading, setLoading] = useState(true);


  const [cancelling, setCancelling] = useState(null);


  const [msg, setMsg] = useState({ text: '', ok: true });

  const flash = (text, ok = true) => {
    setMsg({ text, ok });
    setTimeout(() => setMsg({ text: '', ok: true }), 3000);
  };

  // ════════════════════════════════════════════════════════════
  // EFFECT: TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  useEffect(() => {

    if (authLoading) return;

    if (!user) {
      setLoading(false);
      return;
    }

    load();
  }, [user, authLoading]);

  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const load = async () => {
    setLoading(true);
    try {
      const data = await orderService.getMyOrders();
      
      setOrders(Array.isArray(data) ? data : []);
    } catch {
      setOrders([]);
    } finally {
      setLoading(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const handleCancel = async (orderId) => {
    
    if (!window.confirm('Bạn có chắc muốn huỷ đơn hàng này?')) return;

    setCancelling(orderId);
    try {
      await orderService.cancelOrder(orderId);
      flash(`Đơn hàng #${orderId} đã huỷ thành công.`);
      
      await load();
    } catch (e) {
      
      flash(e.response?.data?.message || 'Không thể huỷ đơn hàng.', false);
    } finally {
      setCancelling(null);
    }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  if (!user && !authLoading) {
    return (
      <div className="mx-auto max-w-2xl px-4 py-20 text-center">
        <h1 className="font-serif text-3xl font-light">Đơn Hàng Của Tôi</h1>
        <p className="mt-4 text-gray-500">Vui lòng đăng nhập để xem đơn hàng.</p>
        <Link to="/login" className="mt-6 inline-block bg-black px-8 py-3 text-white">
          Đăng Nhập
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl px-4 py-12">
      <h1 className="font-serif text-3xl font-light text-center">Đơn Hàng Của Tôi</h1>

      {}
      {msg.text && (
        <div
          className={`mt-6 rounded-2xl px-4 py-3 text-sm ring-1 ${
            msg.ok
              ? 'bg-emerald-50 text-emerald-700 ring-emerald-200'
              : 'bg-rose-50 text-rose-700 ring-rose-200'
          }`}
        >
          {msg.text}
        </div>
      )}

      {}
      {loading ? (
        <p className="mt-10 text-center text-gray-500">Đang tải...</p>
      ) : orders.length === 0 ? (
        
        <div className="mt-10 rounded-2xl border border-slate-200 bg-white p-10 text-center">
          <p className="text-4xl">🛍️</p>
          <p className="mt-3 text-lg font-semibold text-slate-900">Chưa có đơn hàng</p>
          <p className="mt-1 text-sm text-slate-500">Hãy khám phá cửa hàng và đặt hàng ngay.</p>
          <Link to="/shop" className="mt-6 inline-block bg-black px-8 py-3 text-white">
            Mua Sắm Ngay
          </Link>
        </div>
      ) : (
        
        /* ════════════════════════════════════════════════════════════
           RENDER: TIMELINE TRẠNG THÁI
           ════════════════════════════════════════════════════════════ */
        <div className="mt-8 space-y-4">
          {orders.map((order) => (
            <div key={order.id} className="rounded-2xl border border-slate-200 bg-white p-5">
              <div className="flex flex-wrap items-start justify-between gap-3">
                {}
                <div>
                  <div className="flex items-center gap-3">
                    <span className="text-base font-semibold text-slate-900">
                      Đơn hàng #{order.id}
                    </span>
                    <span
                      className={`rounded-full px-3 py-1 text-xs font-semibold ${
                        STATUS_CLASS[order.status] || ''
                      }`}
                    >
                      {STATUS_LABEL[order.status] || order.status}
                    </span>
                  </div>
                  <p className="mt-1 text-sm text-slate-500">{fmtDate(order.createdAt)}</p>
                  {}
                  <p className="mt-1 max-w-md truncate text-sm text-slate-600">
                    📍 {order.shippingAddress}
                  </p>
                </div>
                {}
                <div className="text-right">
                  <p className="text-xl font-semibold text-slate-900">
                    {fmt(order.totalAmount)} đ
                  </p>
                  <div className="mt-2 flex justify-end gap-2">
                    {}
                    {(order.status === 'Pending' || order.status === 'Processing') && (
                      <button
                        onClick={() => handleCancel(order.id)}
                        disabled={cancelling === order.id}
                        className="rounded-full bg-rose-50 px-4 py-2 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100 disabled:opacity-50"
                      >
                        {cancelling === order.id ? 'Đang huỷ...' : 'Huỷ đơn'}
                      </button>
                    )}
                    {}
                    <Link
                      to={`/order-confirmation/${order.id}`}
                      className="rounded-full bg-slate-900 px-4 py-2 text-xs font-semibold text-white hover:bg-slate-800"
                    >
                      Xem Chi Tiết
                    </Link>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default OrderTrackingPage;
