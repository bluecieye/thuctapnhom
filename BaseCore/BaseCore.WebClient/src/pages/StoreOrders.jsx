import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { orderApi } from '../services/api';

const STATUS = {
  Pending:   { label: 'Chờ xử lý',  cls: 'bg-amber-50 text-amber-700 ring-1 ring-amber-200' },
  Completed: { label: 'Hoàn thành', cls: 'bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200' },
  Cancelled: { label: 'Đã huỷ',     cls: 'bg-rose-50 text-rose-700 ring-1 ring-rose-200' },
};

const StoreOrders = () => {
  const [orders, setOrders]       = useState([]);
  const [loading, setLoading]     = useState(true);
  const [expandedId, setExpandedId] = useState(null);
  const [detailsMap, setDetailsMap] = useState({});
  const [cancelling, setCancelling] = useState(null);
  const [msg, setMsg]             = useState({ text: '', ok: true });
  const { isAuthenticated, loading: authLoading } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (authLoading) return;
    if (!isAuthenticated) { navigate('/login'); return; }
    loadOrders();
  }, [isAuthenticated, authLoading]);

  const loadOrders = async () => {
    setLoading(true);
    try {
      const res = await orderApi.getMyOrders();
      setOrders(res.data || []);
    } catch {
      setOrders([]);
    } finally {
      setLoading(false);
    }
  };

  const toggleExpand = async (orderId) => {
    if (expandedId === orderId) { setExpandedId(null); return; }
    setExpandedId(orderId);
    if (!detailsMap[orderId]) {
      try {
        const res = await orderApi.getById(orderId);
        setDetailsMap(prev => ({ ...prev, [orderId]: res.data.details || [] }));
      } catch {
        setDetailsMap(prev => ({ ...prev, [orderId]: [] }));
      }
    }
  };

  const handleCancel = async (orderId) => {
    if (!window.confirm('Bạn có chắc muốn huỷ đơn hàng này không?')) return;
    setCancelling(orderId);
    try {
      await orderApi.cancel(orderId);
      setMsg({ text: `Đơn hàng #${orderId} đã được huỷ thành công.`, ok: true });
      await loadOrders();
    } catch (e) {
      setMsg({ text: e.response?.data?.message || 'Không thể huỷ đơn hàng.', ok: false });
    } finally {
      setCancelling(null);
      setTimeout(() => setMsg({ text: '', ok: true }), 3000);
    }
  };

  const fmt = (n) => Number(n).toLocaleString('vi-VN');
  const fmtDate = (d) =>
    new Date(d).toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
        <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Tài khoản</p>
        <h1 className="mt-2 text-3xl font-semibold text-slate-900">Đơn hàng của tôi</h1>
        <p className="mt-1 text-sm text-slate-600">Theo dõi trạng thái và lịch sử đơn hàng.</p>
      </div>

      {/* Notification */}
      {msg.text && (
        <div className={`rounded-3xl px-5 py-4 text-sm ring-1 ${msg.ok ? 'bg-emerald-50 text-emerald-700 ring-emerald-200' : 'bg-rose-50 text-rose-700 ring-rose-200'}`}>
          {msg.text}
        </div>
      )}

      {/* Loading */}
      {loading ? (
        <div className="rounded-[2rem] bg-white p-10 text-center shadow-soft ring-1 ring-slate-200">
          <p className="text-slate-500">Đang tải đơn hàng...</p>
        </div>
      ) : orders.length === 0 ? (
        /* Empty state */
        <div className="rounded-[2rem] bg-white p-12 text-center shadow-soft ring-1 ring-slate-200">
          <p className="text-4xl">🛍️</p>
          <p className="mt-4 text-lg font-semibold text-slate-900">Chưa có đơn hàng nào</p>
          <p className="mt-2 text-sm text-slate-500">Bạn chưa đặt đơn hàng nào. Hãy khám phá cửa hàng!</p>
          <button
            onClick={() => navigate('/shop')}
            className="mt-6 rounded-full bg-slate-900 px-6 py-3 text-sm font-semibold text-white transition hover:bg-slate-800"
          >
            Bắt đầu mua sắm
          </button>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((order) => {
            const cfg = STATUS[order.status] || STATUS.Pending;
            const isExpanded = expandedId === order.id;
            const orderDetails = detailsMap[order.id];

            return (
              <div key={order.id} className="overflow-hidden rounded-[2rem] bg-white shadow-soft ring-1 ring-slate-200">
                {/* Order row */}
                <div className="flex flex-col gap-4 p-6 sm:flex-row sm:items-center sm:justify-between">
                  <div className="space-y-1.5">
                    <div className="flex flex-wrap items-center gap-3">
                      <span className="text-lg font-semibold text-slate-900">Đơn #{order.id}</span>
                      <span className={`rounded-full px-3 py-1 text-xs font-semibold ${cfg.cls}`}>
                        {cfg.label}
                      </span>
                    </div>
                    <p className="text-sm text-slate-500">{fmtDate(order.orderDate)}</p>
                    <p className="text-sm text-slate-600 max-w-xs truncate">
                      📍 {order.shippingAddress}
                    </p>
                  </div>

                  <div className="flex flex-col items-start gap-3 sm:items-end">
                    <p className="text-2xl font-semibold text-slate-900">{fmt(order.totalAmount)} đ</p>
                    <div className="flex gap-2">
                      {order.status === 'Pending' && (
                        <button
                          onClick={() => handleCancel(order.id)}
                          disabled={cancelling === order.id}
                          className="rounded-full bg-rose-50 px-4 py-2 text-sm font-semibold text-rose-700 ring-1 ring-rose-200 transition hover:bg-rose-100 disabled:opacity-50"
                        >
                          {cancelling === order.id ? 'Đang huỷ...' : 'Huỷ đơn'}
                        </button>
                      )}
                      <button
                        onClick={() => toggleExpand(order.id)}
                        className="rounded-full border border-slate-200 bg-slate-50 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-100"
                      >
                        {isExpanded ? 'Thu gọn ▲' : 'Chi tiết ▼'}
                      </button>
                    </div>
                  </div>
                </div>

                {/* Details panel */}
                {isExpanded && (
                  <div className="border-t border-slate-100 px-6 pb-6 pt-4">
                    {!orderDetails ? (
                      <p className="text-sm text-slate-500">Đang tải chi tiết...</p>
                    ) : orderDetails.length === 0 ? (
                      <p className="text-sm text-slate-500">Không có chi tiết sản phẩm.</p>
                    ) : (
                      <div className="space-y-3">
                        <p className="mb-3 text-xs font-semibold uppercase tracking-[0.2em] text-slate-400">
                          Sản phẩm đã mua
                        </p>
                        {orderDetails.map((d) => (
                          <div key={d.id} className="flex items-center gap-4 rounded-[1.5rem] bg-slate-50 p-3 ring-1 ring-slate-100">
                            {d.product?.imageUrl ? (
                              <img
                                src={d.product.imageUrl}
                                alt={d.product?.name}
                                className="h-16 w-16 flex-shrink-0 rounded-2xl object-cover"
                              />
                            ) : (
                              <div className="h-16 w-16 flex-shrink-0 rounded-2xl bg-slate-200" />
                            )}
                            <div className="flex-1 min-w-0">
                              <p className="font-semibold text-slate-900 truncate">
                                {d.product?.name || `Sản phẩm #${d.productId}`}
                              </p>
                              <p className="text-sm text-slate-500">
                                {d.quantity} × {fmt(d.unitPrice)} đ
                              </p>
                            </div>
                            <p className="flex-shrink-0 font-semibold text-slate-900">
                              {fmt(d.quantity * d.unitPrice)} đ
                            </p>
                          </div>
                        ))}
                        {/* Total row */}
                        <div className="flex justify-between rounded-[1.5rem] bg-slate-900 px-5 py-3">
                          <span className="text-sm font-semibold text-white">Tổng cộng</span>
                          <span className="text-sm font-semibold text-white">{fmt(order.totalAmount)} đ</span>
                        </div>
                      </div>
                    )}
                  </div>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};

export default StoreOrders;
