import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { getCartCount } from '../utils/cart';

const StoreLayout = ({ children }) => {
  const { user, isAuthenticated, logout } = useAuth();
  const [cartCount, setCartCount] = useState(getCartCount());
  const navigate = useNavigate();

  useEffect(() => {
    const handleStorage = () => setCartCount(getCartCount());
    window.addEventListener('storage', handleStorage);
    return () => window.removeEventListener('storage', handleStorage);
  }, []);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <header className="sticky top-0 z-50 border-b border-slate-200 bg-white/95 backdrop-blur">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-4 sm:px-6">
          <Link to="/" className="text-lg font-semibold uppercase tracking-[0.3em] text-slate-900">
            SLAY STORE
          </Link>

          <nav className="hidden items-center gap-6 md:flex">
            <Link to="/" className="text-sm font-medium text-slate-600 hover:text-slate-900">
              Home
            </Link>
            <Link to="/shop" className="text-sm font-medium text-slate-600 hover:text-slate-900">
              Shop
            </Link>
            <Link to="/cart" className="text-sm font-medium text-slate-600 hover:text-slate-900">
              Cart
            </Link>
            <Link to="/my-orders" className="text-sm font-medium text-slate-600 hover:text-slate-900">
              My Orders
            </Link>
          </nav>

          <div className="flex items-center gap-3">
            <Link to="/cart" className="relative inline-flex items-center rounded-full bg-slate-900 px-4 py-2 text-sm font-medium text-white shadow-sm shadow-slate-200/30 hover:bg-slate-800">
              Cart
              {cartCount > 0 && (
                <span className="ml-2 inline-flex h-6 min-w-[24px] items-center justify-center rounded-full bg-slate-200 px-2 text-xs font-semibold text-slate-900">
                  {cartCount}
                </span>
              )}
            </Link>
            {isAuthenticated ? (
              <div className="flex items-center gap-3">
                <span className="hidden sm:inline text-sm text-slate-700">Hi, {user?.name || user?.username}</span>
                <button className="rounded-full border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-700 shadow-sm hover:bg-slate-50" onClick={handleLogout}>
                  Logout
                </button>
              </div>
            ) : (
              <Link className="rounded-full bg-slate-900 px-4 py-2 text-sm font-medium text-white shadow-sm shadow-slate-200/30 hover:bg-slate-800" to="/login">
                Sign in
              </Link>
            )}
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-7xl px-4 py-8 sm:px-6">{children}</main>

      <footer className="border-t border-slate-200 bg-white/95 py-10">
        <div className="mx-auto max-w-7xl px-4 sm:px-6">
          <div className="grid gap-6 md:grid-cols-3">
            <div>
              <p className="text-sm font-semibold uppercase tracking-[0.3em] text-slate-900">SLAY STORE</p>
              <p className="mt-3 max-w-md text-sm text-slate-600">
                A modern unisex boutique powered by BaseCore API. Discover trend-ready apparel, shoes and accessories in one place.
              </p>
            </div>
            <div>
              <p className="text-sm font-semibold uppercase tracking-[0.3em] text-slate-900">Quick Links</p>
              <ul className="mt-3 space-y-2 text-sm text-slate-600">
                <li><Link to="/" className="hover:text-slate-900">Home</Link></li>
                <li><Link to="/shop" className="hover:text-slate-900">Shop</Link></li>
                <li><Link to="/cart" className="hover:text-slate-900">Cart</Link></li>
              </ul>
            </div>
            <div>
              <p className="text-sm font-semibold uppercase tracking-[0.3em] text-slate-900">Contact</p>
              <p className="mt-3 text-sm text-slate-600">support@slaystore.com</p>
              <p className="mt-2 text-sm text-slate-600">+84 123 456 789</p>
            </div>
          </div>

          <div className="mt-10 text-center text-xs text-slate-500">
            © 2026 SLAY STORE. Built with BaseCore backend APIs.
          </div>
        </div>
      </footer>
    </div>
  );
};

export default StoreLayout;
