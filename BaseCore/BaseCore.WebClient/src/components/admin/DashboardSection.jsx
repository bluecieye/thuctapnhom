

import { useEffect, useState } from 'react';

import { useNavigate } from 'react-router-dom';

import {
  Package, ListTree, ShoppingCart, Users as UsersIcon,
  Tag, AlertTriangle, Star, BarChart3, Warehouse, Image, Ruler,
} from 'lucide-react';

import { adminService } from '../../services/adminService';

import { fmt } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// HẰNG SỐ / SCHEMA
// ════════════════════════════════════════════════════════════
const cards = [
  { key: 'products',   label: 'Sản phẩm',     icon: Package,      accent: 'bg-sky-50 text-sky-700',      path: '/admin/products' },
  { key: 'variants',   label: 'Biến thể (SKU)',icon: Warehouse,    accent: 'bg-cyan-50 text-cyan-700',    path: '/admin/inventory' },
  { key: 'categories', label: 'Danh mục',      icon: ListTree,     accent: 'bg-emerald-50 text-emerald-700', path: '/admin/categories' },
  { key: 'orders',     label: 'Đơn hàng',      icon: ShoppingCart, accent: 'bg-amber-50 text-amber-700',  path: '/admin/orders' },
  { key: 'users',      label: 'Người dùng',    icon: UsersIcon,    accent: 'bg-rose-50 text-rose-700',    path: '/admin/users' },
  { key: 'promotions', label: 'Mã giảm giá',   icon: Tag,          accent: 'bg-fuchsia-50 text-fuchsia-700', path: '/admin/coupons' },
  { key: 'reviews',    label: 'Đánh giá',      icon: Star,         accent: 'bg-yellow-50 text-yellow-700',   path: '/admin/reviews' },
  { key: 'images',     label: 'Ảnh sản phẩm',  icon: Image,        accent: 'bg-sky-50 text-sky-700',         path: '/admin/images' },
  { key: 'sizeguides', label: 'Bảng size',      icon: Ruler,        accent: 'bg-violet-50 text-violet-700',   path: '/admin/sizeguides' },
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

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [stats, setStats] = useState({});
  
  const [loading, setLoading] = useState(true);
  
  const [err, setErr] = useState('');

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    let active = true;

    adminService.getStats()
      .then((d) => active && setStats(d || {}))      
      .catch(() => active && setErr('Không thể tải số liệu thống kê.'))
      .finally(() => active && setLoading(false));

    
    return () => { active = false; };
  }, []);

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-6">
      {}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">Bảng điều khiển</h2>
        <p className="mt-1 text-sm text-slate-500">Tổng quan catalog và hoạt động cửa hàng.</p>
      </div>

      {}
      {err && (
        <div className="rounded-2xl border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">{err}</div>
      )}

      {}
      {}
      {}
      {}
      {}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {}
        {}
        {cards.map(({ key, label, icon: Icon, accent, path }) => (

          <button
            key={key}
            onClick={() => navigate(path)}
            className="rounded-2xl border border-slate-200 bg-white p-5 text-left transition hover:border-slate-300 hover:shadow"
          >
            {}
            <div className={`mb-3 inline-flex rounded-xl p-2 ${accent}`}>
              <Icon size={20} />
            </div>
            {}
            <p className="text-sm font-medium text-slate-500">{label}</p>
            {}
            {}
            {}
            <p className="mt-1 text-3xl font-bold text-slate-900">
              {loading ? '—' : fmt(stats[key])}
            </p>
          </button>
        ))}
      </div>

      {}
      {}
      {stats.lowStockVariants > 0 && (
        <div className="rounded-2xl border border-rose-200 bg-rose-50 p-4">
          <div className="flex items-center gap-2 text-rose-700">
            <AlertTriangle size={18} />
            <h3 className="font-semibold">Cảnh báo tồn kho</h3>
          </div>
          <p className="mt-2 text-sm text-rose-600">
            {stats.lowStockVariants} biến thể có số lượng khả dụng ≤ 5. Vào mục <strong>Tồn kho (SKU)</strong> để nhập thêm.
          </p>
        </div>
      )}

      {}
      {}
      {stats.ordersByStatus && Object.keys(stats.ordersByStatus).length > 0 && (
        <div className="rounded-2xl border border-slate-200 bg-white p-6">
          <div className="flex items-center gap-2 text-slate-700">
            <BarChart3 size={18} />
            <h3 className="font-semibold">Đơn hàng theo trạng thái</h3>
          </div>

          {}
          <div className="mt-4 grid grid-cols-2 gap-3 sm:grid-cols-5">
            {}
            {}
            {}
            {Object.entries(stats.ordersByStatus).map(([status, count]) => (
              <div key={status} className="rounded-xl bg-slate-50 p-3 text-center">
                {}
                <p className="text-xs uppercase tracking-wider text-slate-400">
                  {STATUS_VI[status] || status}
                </p>
                <p className="mt-1 text-2xl font-bold text-slate-900">{count}</p>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default DashboardSection;
