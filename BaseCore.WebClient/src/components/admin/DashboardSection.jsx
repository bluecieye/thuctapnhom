

import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Package, ListTree, ShoppingCart, Users as UsersIcon,
  Tag, AlertTriangle, BarChart3, Warehouse, TrendingUp, DollarSign,
} from 'lucide-react';
import { adminService } from '../../services/adminService';
import { fmt } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// HẰNG SỐ / SCHEMA
// ════════════════════════════════════════════════════════════

// Các card đếm số lượng — điều hướng khi click
const COUNT_CARDS = [
  { key: 'products',   label: 'Sản phẩm',       icon: Package,      accent: 'bg-sky-50 text-sky-700',          path: '/admin/products' },
  { key: 'variants',   label: 'Biến thể (SKU)',  icon: Warehouse,    accent: 'bg-cyan-50 text-cyan-700',        path: '/admin/inventory' },
  { key: 'categories', label: 'Danh mục',        icon: ListTree,     accent: 'bg-emerald-50 text-emerald-700',  path: '/admin/categories' },
  { key: 'orders',     label: 'Đơn hàng',        icon: ShoppingCart, accent: 'bg-amber-50 text-amber-700',      path: '/admin/orders' },
  { key: 'users',      label: 'Người dùng',      icon: UsersIcon,    accent: 'bg-rose-50 text-rose-700',        path: '/admin/users' },
  { key: 'promotions', label: 'Mã giảm giá',     icon: Tag,          accent: 'bg-fuchsia-50 text-fuchsia-700',  path: '/admin/coupons' },
];

const STATUS_VI = {
  Pending:    'Chờ xác nhận',
  Processing: 'Đang xử lý',
  Shipping:   'Đang giao',
  Delivered:  'Đã giao',
  Cancelled:  'Đã huỷ',
};

// ════════════════════════════════════════════════════════════
// SECTION DASHBOARD TỔNG QUAN
// ════════════════════════════════════════════════════════════
export const DashboardSection = () => {
  const navigate = useNavigate();

  const [stats, setStats]     = useState({});
  const [topSales, setTopSales] = useState([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr]         = useState('');

  useEffect(() => {
    let active = true;
    Promise.all([
      adminService.getStats(),
      adminService.getVariantTopSales(10).catch(() => []),
    ])
      .then(([s, t]) => {
        if (!active) return;
        setStats(s || {});
        setTopSales(t || []);
      })
      .catch(() => active && setErr('Không thể tải số liệu thống kê.'))
      .finally(() => active && setLoading(false));
    return () => { active = false; };
  }, []);

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">Bảng điều khiển</h2>
        <p className="mt-1 text-sm text-slate-500">Tổng quan catalog và hoạt động cửa hàng.</p>
      </div>

      {err && (
        <div className="rounded-2xl border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">{err}</div>
      )}

      {/* ── Cards doanh thu ── */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div className="rounded-2xl border border-slate-200 bg-white p-5">
          <div className="mb-3 inline-flex rounded-xl bg-emerald-50 p-2 text-emerald-700">
            <DollarSign size={20} />
          </div>
          <p className="text-sm font-medium text-slate-500">Doanh thu (đơn không huỷ)</p>
          <p className="mt-1 text-3xl font-bold text-slate-900">
            {loading ? '—' : `${fmt(stats.totalRevenue)}đ`}
          </p>
        </div>
        <div className="rounded-2xl border border-slate-200 bg-white p-5">
          <div className="mb-3 inline-flex rounded-xl bg-violet-50 p-2 text-violet-700">
            <TrendingUp size={20} />
          </div>
          <p className="text-sm font-medium text-slate-500">Giá trị đơn trung bình</p>
          <p className="mt-1 text-3xl font-bold text-slate-900">
            {loading ? '—' : `${fmt(Math.round(stats.avgOrderValue || 0))}đ`}
          </p>
        </div>
      </div>

      {/* ── Cards đếm số lượng ── */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {COUNT_CARDS.map(({ key, label, icon: Icon, accent, path }) => (
          <button
            key={key}
            onClick={() => navigate(path)}
            className="rounded-2xl border border-slate-200 bg-white p-5 text-left transition hover:border-slate-300 hover:shadow"
          >
            <div className={`mb-3 inline-flex rounded-xl p-2 ${accent}`}>
              <Icon size={20} />
            </div>
            <p className="text-sm font-medium text-slate-500">{label}</p>
            <p className="mt-1 text-3xl font-bold text-slate-900">
              {loading ? '—' : fmt(stats[key])}
            </p>
          </button>
        ))}
      </div>

      {/* ── Cảnh báo tồn kho ── */}
      {stats.lowStockVariants > 0 && (
        <div className="rounded-2xl border border-rose-200 bg-rose-50 p-4">
          <div className="flex items-center gap-2 text-rose-700">
            <AlertTriangle size={18} />
            <h3 className="font-semibold">Cảnh báo tồn kho</h3>
          </div>
          <p className="mt-2 text-sm text-rose-600">
            <strong>{stats.lowStockVariants}</strong> biến thể có số lượng khả dụng ≤ 5.
            Vào <strong>Tồn kho (SKU)</strong> để nhập thêm.
          </p>
        </div>
      )}

      {/* ── Đơn hàng theo trạng thái ── */}
      {stats.ordersByStatus && Object.keys(stats.ordersByStatus).length > 0 && (
        <div className="rounded-2xl border border-slate-200 bg-white p-6">
          <div className="flex items-center gap-2 text-slate-700">
            <BarChart3 size={18} />
            <h3 className="font-semibold">Đơn hàng theo trạng thái</h3>
          </div>
          <div className="mt-4 grid grid-cols-2 gap-3 sm:grid-cols-5">
            {Object.entries(stats.ordersByStatus).map(([status, count]) => (
              <div key={status} className="rounded-xl bg-slate-50 p-3 text-center">
                <p className="text-xs uppercase tracking-wider text-slate-400">
                  {STATUS_VI[status] || status}
                </p>
                <p className="mt-1 text-2xl font-bold text-slate-900">{count}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* ── Biến thể bán chạy nhất ── */}
      {topSales.length > 0 && (
        <div className="rounded-2xl border border-slate-200 bg-white p-6">
          <div className="flex items-center gap-2 text-slate-700">
            <TrendingUp size={18} />
            <h3 className="font-semibold">Biến thể bán chạy nhất (Top 10)</h3>
          </div>
          <div className="mt-4 overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-200 text-left text-xs uppercase tracking-wider text-slate-400">
                  <th className="py-2 pr-4">SKU</th>
                  <th className="py-2 pr-4">Sản phẩm</th>
                  <th className="py-2 pr-4">Màu</th>
                  <th className="py-2 pr-4">Size</th>
                  <th className="py-2 pr-4 text-right">Đã bán</th>
                  <th className="py-2 text-right">Doanh thu</th>
                </tr>
              </thead>
              <tbody>
                {topSales.map((v, i) => (
                  <tr key={v.variantId} className="border-b border-slate-100">
                    <td className="py-2 pr-4 font-mono text-xs text-slate-500">{v.sku}</td>
                    <td className="py-2 pr-4 font-medium text-slate-800">{v.productName}</td>
                    <td className="py-2 pr-4 text-slate-600">{v.colorName}</td>
                    <td className="py-2 pr-4 text-slate-600">{v.sizeName}</td>
                    <td className="py-2 pr-4 text-right font-semibold">{v.totalSold}</td>
                    <td className="py-2 text-right text-emerald-700">{fmt(v.revenue)}đ</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};

export default DashboardSection;
