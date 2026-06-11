

import { useEffect, useState } from 'react';

import { useParams, useLocation, useNavigate, Link } from 'react-router-dom';

import { AnimatePresence, motion } from 'framer-motion';

import { CheckCircle2, Package, MapPin, Calendar, X } from 'lucide-react';

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

import { fmt, fmtDateTime as fmtDate } from '../utils/format';

// ════════════════════════════════════════════════════════════
// TRANG XÁC NHẬN ĐƠN HÀNG
// ════════════════════════════════════════════════════════════
const OrderConfirmationPage = () => {
  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const { orderId } = useParams();


  const location = useLocation();

  const navigate = useNavigate();

  const [order, setOrder] = useState(null);

  const [loading, setLoading] = useState(true);

  const [error, setError] = useState('');



  const [showSuccessToast, setShowSuccessToast] = useState(
    Boolean(location.state?.justPlaced)
  );

  // ════════════════════════════════════════════════════════════
  // EFFECT: TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    
    let active = true;
    setLoading(true);

    orderService.getOrderById(orderId)
      .then((data) => { if (active) setOrder(data); })
      .catch(() => { if (active) setError('Không tìm thấy đơn hàng hoặc bạn không có quyền xem.'); })
      .finally(() => active && setLoading(false));

    
    return () => { active = false; };
  }, [orderId]); 

  

  
  useEffect(() => {
    if (!showSuccessToast) return undefined;

    
    navigate(location.pathname, { replace: true, state: {} });

    const id = setTimeout(() => setShowSuccessToast(false), 4000);

    return () => clearTimeout(id);

  }, []);

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="bg-slate-50 min-h-screen">
      {}
      <AnimatePresence>
        {showSuccessToast && (
          <motion.div
            initial={{ opacity: 0, y: -20 }}   
            animate={{ opacity: 1, y: 0 }}     
            exit={{ opacity: 0, y: -20 }}      
            transition={{ duration: 0.3 }}
            className="fixed top-24 left-1/2 z-50 -translate-x-1/2 flex items-center gap-3 rounded-full bg-emerald-600 px-5 py-3 text-sm text-white shadow-lg ring-1 ring-emerald-700/30"
          >
            <CheckCircle2 size={18} />
            <span className="font-medium">
              Đặt hàng <span className="font-semibold">#{orderId}</span> thành công!
            </span>
            {}
            <button
              type="button"
              onClick={() => setShowSuccessToast(false)}
              aria-label="Đóng thông báo"
              className="ml-2 -mr-1 rounded-full p-1 hover:bg-emerald-700/40"
            >
              <X size={14} />
            </button>
          </motion.div>
        )}
      </AnimatePresence>

      <div className="mx-auto max-w-3xl px-4 py-16 sm:px-6 lg:px-8">
        {}
        <div className="rounded-3xl bg-gradient-to-br from-slate-50 via-white to-slate-50 p-10 text-center shadow-sm ring-1 ring-slate-100">
          <div className="mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-slate-100 ring-8 ring-slate-50">
            <Package size={42} className="text-slate-600" strokeWidth={1.5} />
          </div>
          <h1 className="mt-6 font-serif text-4xl font-light text-slate-900">Chi tiết đơn hàng</h1>
          <p className="mt-3 text-slate-600">
            Đơn hàng <span className="font-semibold text-slate-900">#{orderId}</span>
          </p>
        </div>

        {}
        {loading ? (
          <div className="mt-10 flex justify-center">
            {}
            <div className="h-8 w-8 animate-spin rounded-full border-2 border-slate-900 border-t-transparent" />
          </div>
        ) : error ? (
          <p className="mt-10 text-center text-rose-500">{error}</p>
        ) : order ? (
          <div className="mt-10 space-y-6">
            {}
            {/* ════════════════════════════════════════════════════════════
                RENDER: THÔNG TIN TRẠNG THÁI
                ════════════════════════════════════════════════════════════ */}
            <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                  <p className="text-xs uppercase tracking-[0.3em] text-slate-400">Trạng thái</p>
                  {}
                  <span className={`mt-2 inline-block rounded-full px-3 py-1 text-xs font-semibold ${STATUS_CLASS[order.status] || ''}`}>
                    {STATUS_LABEL[order.status] || order.status}
                  </span>
                </div>
                <div className="text-right">
                  <p className="text-xs uppercase tracking-[0.3em] text-slate-400">Đặt lúc</p>
                  <p className="mt-1 text-sm text-slate-700">{fmtDate(order.createdAt)}</p>
                </div>
              </div>
              {}
              <div className="mt-5 grid gap-4 border-t border-slate-100 pt-5 text-sm sm:grid-cols-2">
                <div className="flex items-start gap-3">
                  <MapPin size={18} className="mt-0.5 shrink-0 text-slate-400" />
                  <div>
                    <p className="text-xs uppercase tracking-wider text-slate-400">Địa chỉ giao</p>
                    <p className="mt-1 text-slate-700">{order.shippingAddress || '—'}</p>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <Calendar size={18} className="mt-0.5 shrink-0 text-slate-400" />
                  <div>
                    <p className="text-xs uppercase tracking-wider text-slate-400">Dự kiến giao</p>
                    <p className="mt-1 text-slate-700">{fmtDate(order.estimatedDelivery)}</p>
                  </div>
                </div>
              </div>
            </div>

            {}
            <div className="rounded-2xl border border-slate-200 bg-white shadow-sm">
              <div className="flex items-center gap-2 border-b border-slate-200 px-6 py-4">
                <Package size={18} className="text-slate-400" />
                <span className="text-sm font-semibold text-slate-700">
                  Sản phẩm ({(order.orderDetails || []).length})
                </span>
              </div>
              {}
              <div className="divide-y divide-slate-100">
                {(order.orderDetails || []).map((d) => {
                  
                  const p = d.variant?.product;
                  
                  const fileName = p?.images?.[0]?.fileName;
                  
                  const img = fileName && p?.slug ? `/images/products/${p.slug}/${fileName}` : '';
                  
                  const name = p?.name || `Variant #${d.variantId}`;
                  return (
                    <div key={d.id} className="flex items-center gap-4 px-6 py-4">
                      {}
                      {img ? (
                        <img src={img} alt={name} className="h-16 w-16 rounded-xl object-cover" />
                      ) : (
                        <div className="h-16 w-16 rounded-xl bg-slate-100" />
                      )}
                      <div className="flex-1">
                        <p className="font-medium text-slate-900">{name}</p>
                        {}
                        <p className="text-xs text-slate-500">
                          {d.variant?.color?.name} · Size {d.variant?.size?.name}
                        </p>
                        {}
                        <p className="text-sm text-slate-500">{d.quantity} × {fmt(d.unitPrice)} đ</p>
                      </div>
                      {}
                      <p className="text-sm font-semibold text-slate-900">{fmt(d.quantity * d.unitPrice)} đ</p>
                    </div>
                  );
                })}
              </div>
              {}
              <div className="space-y-1 border-t border-slate-200 px-6 py-4 text-sm">
                <div className="flex justify-between text-slate-600">
                  <span>Phí ship</span>
                  {}
                  <span>{order.shippingFee === 0 ? 'Free' : `${fmt(order.shippingFee)} đ`}</span>
                </div>
                {}
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
        ) : null}

        {}
        <div className="mt-10 flex flex-col justify-center gap-3 sm:flex-row">
          <Link to="/my-orders" className="rounded-xl bg-slate-900 px-8 py-3 text-center text-sm font-semibold uppercase tracking-wider text-white transition hover:bg-slate-700">
            Xem đơn hàng của tôi
          </Link>
          <Link to="/shop" className="rounded-xl border border-slate-300 px-8 py-3 text-center text-sm font-semibold uppercase tracking-wider text-slate-900 transition hover:bg-white">
            Tiếp tục mua sắm
          </Link>
        </div>
      </div>
    </div>
  );
};

export default OrderConfirmationPage;
