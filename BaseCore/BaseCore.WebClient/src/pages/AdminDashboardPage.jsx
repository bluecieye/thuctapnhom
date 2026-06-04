

import { useState, useEffect } from 'react';

import { Link, useLocation, useNavigate } from 'react-router-dom';

import {
  Home, Package, ListTree, ShoppingCart,
  Users as UsersIcon, Tag, LogOut, Menu, X, Store,
  Warehouse, Star, Image, Ruler, Truck, MapPin,
} from 'lucide-react';

import { useAuth } from '../contexts/AuthContext';

import { DashboardSection } from '../components/admin/DashboardSection';
import { ProductsSection } from '../components/admin/ProductsSection';
import { CategoriesSection } from '../components/admin/CategoriesSection';
import { CouponsSection } from '../components/admin/CouponsSection';
import { OrdersSection } from '../components/admin/OrdersSection';
import { UsersSection } from '../components/admin/UsersSection';
import { InventorySection } from '../components/admin/InventorySection';
import { ReviewsSection } from '../components/admin/ReviewsSection';
import { ImagesSection } from '../components/admin/ImagesSection';
import { SizeGuidesSection } from '../components/admin/SizeGuidesSection';
import { ShippingCarriersSection } from '../components/admin/ShippingCarriersSection';
import { ShippingRatesSection } from '../components/admin/ShippingRatesSection';

// ════════════════════════════════════════════════════════════
// CONSTANTS / SCHEMA
// ════════════════════════════════════════════════════════════
const MENU = [
  { id: 'dashboard',  label: 'Tổng quan',      icon: Home,         path: '/admin',            roles: ['Admin', 'WarehouseStaff', 'Marketing'] },
  { id: 'products',   label: 'Sản phẩm',       icon: Package,      path: '/admin/products',   roles: ['Admin', 'Marketing'] },
  { id: 'categories', label: 'Danh mục',        icon: ListTree,     path: '/admin/categories', roles: ['Admin', 'Marketing'] },
  { id: 'coupons',    label: 'Mã giảm giá',     icon: Tag,          path: '/admin/coupons',    roles: ['Admin', 'Marketing'] },
  { id: 'orders',     label: 'Đơn hàng',        icon: ShoppingCart, path: '/admin/orders',     roles: ['Admin', 'WarehouseStaff'] },
  { id: 'inventory',  label: 'Tồn kho (SKU)',   icon: Warehouse,    path: '/admin/inventory',  roles: ['Admin', 'WarehouseStaff'] },
  { id: 'reviews',    label: 'Đánh giá',        icon: Star,         path: '/admin/reviews',     roles: ['Admin', 'Marketing'] },
  { id: 'images',     label: 'Ảnh sản phẩm',    icon: Image,        path: '/admin/images',      roles: ['Admin', 'Marketing'] },
  { id: 'sizeguides', label: 'Bảng size',        icon: Ruler,        path: '/admin/sizeguides',  roles: ['Admin', 'Marketing'] },
  { id: 'carriers',   label: 'Đơn vị vận chuyển', icon: Truck,        path: '/admin/carriers',    roles: ['Admin'] },
  { id: 'rates',      label: 'Cước vận chuyển',  icon: MapPin,       path: '/admin/rates',       roles: ['Admin'] },
  { id: 'users',      label: 'Người dùng',      icon: UsersIcon,    path: '/admin/users',       roles: ['Admin'] },
];

const ROLE_LABELS = {
  Admin:          'Quản trị viên',
  WarehouseStaff: 'Nhân viên kho',
  Marketing:      'Marketing',
  Customer:       'Khách hàng',
};

// ════════════════════════════════════════════════════════════
// RENDER: HELPERS
// ════════════════════════════════════════════════════════════
const sectionFromPath = (pathname) => {
  const match = MENU.find((m) => m.path === pathname);
  return match ? match.id : 'dashboard';
};

const renderSection = (id) => {
  switch (id) {
    case 'products':   return <ProductsSection />;
    case 'categories': return <CategoriesSection />;
    case 'coupons':    return <CouponsSection />;
    case 'orders':     return <OrdersSection />;
    case 'inventory':  return <InventorySection />;
    case 'reviews':    return <ReviewsSection />;
    case 'images':     return <ImagesSection />;
    case 'sizeguides': return <SizeGuidesSection />;
    case 'carriers':   return <ShippingCarriersSection />;
    case 'rates':      return <ShippingRatesSection />;
    case 'users':      return <UsersSection />;
    case 'dashboard':
    default:
      return <DashboardSection />;
  }
};

// ════════════════════════════════════════════════════════════
// TRANG ADMIN DASHBOARD
// ════════════════════════════════════════════════════════════
const AdminDashboardPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const { user, logout } = useAuth();

  const location = useLocation();
  const navigate = useNavigate();

  // ════════════════════════════════════════════════════════════
  // STATE & TAB ACTIVE
  // ════════════════════════════════════════════════════════════
  const [collapsed, setCollapsed] = useState(false);

  // ════════════════════════════════════════════════════════════
  // TÍNH TOÁN
  // ════════════════════════════════════════════════════════════
  const active = sectionFromPath(location.pathname);

  const userRole = user?.role || '';

  const visibleMenu = MENU.filter((m) => m.roles.includes(userRole));

  // ════════════════════════════════════════════════════════════
  // EFFECT: KIỂM TRA QUYỀN TAB
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    const current = MENU.find((m) => m.id === active);
    if (current && !current.roles.includes(userRole)) {
      navigate('/admin', { replace: true });
    }
  }, [active, userRole, navigate]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  if (!user) return null;

  const handleLogout = () => { logout(); navigate('/'); };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="flex min-h-screen bg-slate-50">
      {}
      {/* ════════════════════════════════════════════════════════════
          RENDER: SIDEBAR NAV ADMIN
          ════════════════════════════════════════════════════════════ */}
      <aside
        
        className={`flex flex-col border-r border-slate-200 bg-white transition-all duration-200 ${
          collapsed ? 'w-20' : 'w-64'
        }`}
      >
        {}
        <div className="flex h-16 items-center justify-between border-b border-slate-200 px-4">
          {!collapsed && (
            <span className="font-serif text-lg font-semibold tracking-wider text-slate-900">BASECORE</span>
          )}
          <button onClick={() => setCollapsed(!collapsed)} className="rounded-lg p-2 text-slate-500 hover:bg-slate-100">
            {}
            {collapsed ? <Menu size={18} /> : <X size={18} />}
          </button>
        </div>

        {}
        <nav className="flex-1 space-y-1 p-3">
          {visibleMenu.map(({ id, label, icon: Icon, path }) => (
            <Link
              key={id}
              to={path}
              
              className={`flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition ${
                active === id
                  ? 'bg-slate-900 text-white'
                  : 'text-slate-600 hover:bg-slate-100 hover:text-slate-900'
              }`}
              
              title={collapsed ? label : ''}
            >
              <Icon size={18} />
              {!collapsed && <span>{label}</span>}
            </Link>
          ))}
        </nav>

        {}
        <div className="space-y-1 border-t border-slate-200 p-3">
          <Link
            to="/"
            className="flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-slate-600 hover:bg-slate-100"
            title={collapsed ? 'Về cửa hàng' : ''}
          >
            <Store size={18} />
            {!collapsed && <span>Về cửa hàng</span>}
          </Link>
          <button
            onClick={handleLogout}
            className="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-rose-600 hover:bg-rose-50"
            title={collapsed ? 'Đăng xuất' : ''}
          >
            <LogOut size={18} />
            {!collapsed && <span>Đăng xuất</span>}
          </button>
        </div>
      </aside>

      {}
      {/* ════════════════════════════════════════════════════════════
          RENDER: NỘI DUNG TAB ACTIVE
          ════════════════════════════════════════════════════════════ */}
      <main className="flex-1 overflow-y-auto">
        {}
        <header className="flex h-16 items-center justify-between border-b border-slate-200 bg-white px-6">
          <div>
            <p className="text-xs uppercase tracking-[0.3em] text-slate-400">Quản trị</p>
            <h1 className="text-lg font-semibold text-slate-900">
              {}
              {MENU.find((m) => m.id === active)?.label || 'Tổng quan'}
            </h1>
          </div>
          <div className="flex items-center gap-3 text-sm">
            <span className="text-slate-500">@{user.username}</span>
            {}
            <span className="rounded-full bg-slate-900 px-3 py-1 text-xs font-semibold text-white">
              {ROLE_LABELS[userRole] || userRole}
            </span>
          </div>
        </header>

        {}
        <div className="p-6">{renderSection(active)}</div>
      </main>
    </div>
  );
};

export default AdminDashboardPage;
