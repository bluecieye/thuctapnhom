

import { Link, useNavigate } from 'react-router-dom';

import { useEffect, useState } from 'react';

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

import { fmt, fmtDate } from '../utils/format';

// ════════════════════════════════════════════════════════════
// TRANG TÀI KHOẢN
// ════════════════════════════════════════════════════════════
const AccountPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const { user, logout, isStaff } = useAuth();
  const navigate = useNavigate();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [orders, setOrders] = useState([]);

  const [loading, setLoading] = useState(true);

  // ════════════════════════════════════════════════════════════
  // EFFECT: TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    
    if (!user) return;

    let active = true;

    orderService
      .getMyOrders()
      .then((data) => active && setOrders(Array.isArray(data) ? data : []))
      .catch(() => active && setOrders([]))
      .finally(() => active && setLoading(false));

    return () => {
      active = false;
    };
  }, [user]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const handleLogout = () => {
    logout();
    navigate('/');
  };

  // ════════════════════════════════════════════════════════════
  // TÍNH TOÁN
  // ════════════════════════════════════════════════════════════
  const recent = orders.slice(0, 5);

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="mx-auto max-w-5xl px-4 py-12">
      <h1 className="font-serif text-3xl font-light">Tài Khoản Của Tôi</h1>

      {}
      <div className="mt-8 grid gap-6 lg:grid-cols-3">
        {}
        <div className="rounded-2xl border border-slate-200 bg-white p-6 lg:col-span-1">
          <p className="text-xs uppercase tracking-[0.3em] text-slate-400">Hồ Sơ</p>
          <p className="mt-2 text-lg font-semibold text-slate-900">Tên người dùng: {user.username}</p>

          {}
          {user.email && <p className="mt-1 text-sm text-slate-500">Email: {user.email}</p>}

          {}
          <p className="mt-1 text-xs text-slate-400">Vai trò: {user.role}</p>

          <div className="mt-4 flex flex-col gap-2 text-sm">
            <Link
              to="/my-orders"
              className="rounded-xl border border-slate-200 px-4 py-2 text-center font-semibold text-slate-700 hover:bg-slate-50"
            >
              Đơn Hàng Của Tôi
            </Link>
            <Link
              to="/wishlist"
              className="rounded-xl border border-slate-200 px-4 py-2 text-center font-semibold text-slate-700 hover:bg-slate-50"
            >
              Yêu Thích
            </Link>

            {}
            {isStaff() && (
              <Link
                to="/admin"
                className="rounded-xl bg-slate-900 px-4 py-2 text-center font-semibold text-white hover:bg-slate-800"
              >
                Quản Trị
              </Link>
            )}

            <button
              onClick={handleLogout}
              className="rounded-xl bg-rose-50 px-4 py-2 text-sm font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100"
            >
              Đăng Xuất
            </button>
          </div>
        </div>

        {}
        <div className="rounded-2xl border border-slate-200 bg-white p-6 lg:col-span-2">
          <div className="flex items-center justify-between">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-400">Đơn Hàng Gần Đây</p>
            {}
            <Link
              to="/my-orders"
              className="text-sm font-semibold text-slate-700 hover:underline"
            >
              Xem tất cả →
            </Link>
          </div>

          {}
          {loading ? (
            <p className="mt-6 text-sm text-slate-500">Đang tải...</p>
          ) : recent.length === 0 ? (
            <p className="mt-6 text-sm text-slate-500">Bạn chưa có đơn hàng nào.</p>
          ) : (
            <ul className="mt-6 divide-y divide-slate-100">
              {recent.map((o) => (
                <li key={o.id} className="flex items-center justify-between py-3">
                  <div>
                    {}
                    <Link
                      to={`/order-confirmation/${o.id}`}
                      className="font-semibold text-slate-900 hover:underline"
                    >
                      Đơn #{o.id}
                    </Link>
                    <p className="text-xs text-slate-500">{fmtDate(o.createdAt)}</p>
                  </div>
                  <div className="flex items-center gap-3">
                    {}
                    <span
                      className={`rounded-full px-3 py-1 text-xs font-semibold ${
                        STATUS_CLASS[o.status] || ''
                      }`}
                    >
                      {STATUS_LABEL[o.status] || o.status}
                    </span>
                    <span className="text-sm font-semibold text-slate-900">
                      {fmt(o.totalAmount)} đ
                    </span>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
};

export default AccountPage;
