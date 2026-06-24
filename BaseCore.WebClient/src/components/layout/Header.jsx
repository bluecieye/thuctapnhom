

import { useState, useEffect, useRef } from 'react';

import { createPortal } from 'react-dom';

import { Link, NavLink, useNavigate } from 'react-router-dom';

import {
  ShoppingBag,
  Heart,
  Search,
  Menu,
  X,
  User,
  LogOut,
  UserCircle,
  LayoutDashboard,
  Package,
} from 'lucide-react';

import { motion, AnimatePresence } from 'framer-motion';

import { useCart } from '../../contexts/CartContext';
import { useWishlist } from '../../contexts/WishlistContext';
import { useAuth } from '../../contexts/AuthContext';

// ════════════════════════════════════════════════════════════
// HẰNG SỐ NAV
// ════════════════════════════════════════════════════════════

const MEN_CATEGORIES = [
  { name: 'Sản phẩm mới', path: '/shop?gender=Male&newOnly=true', accent: true },
  { name: 'Áo',           path: '/shop?categoryId=2' },
  { name: 'Áo khoác',     path: '/shop?categoryId=1' },
  { name: 'Quần dài',     path: '/shop?categoryId=3' },
  { name: 'Quần short',   path: '/shop?categoryId=4' },
];

const WOMEN_CATEGORIES = [
  { name: 'Sản phẩm mới', path: '/shop?gender=Female&newOnly=true', accent: true },
  { name: 'Áo',           path: '/shop?categoryId=5' },
  { name: 'Áo khoác',     path: '/shop?categoryId=8' },
  { name: 'Quần',         path: '/shop?categoryIds=6,7' },
  { name: 'Váy',          path: '/shop?categoryId=9' },
  { name: 'Chân váy',     path: '/shop?categoryId=10' },
];

const NAV_GROUPS = [
  { key: 'men',   label: 'Nam',  items: MEN_CATEGORIES },
  { key: 'women', label: 'Nữ',   items: WOMEN_CATEGORIES },
  { key: 'shop',  label: 'Tất cả sản phẩm', path: '/shop' },
];

// ════════════════════════════════════════════════════════════
// COMPONENT HEADER
// ════════════════════════════════════════════════════════════

const Header = () => {

  // ════════════════════════════════════════════════════════════
  // STATE & HOOK
  // ════════════════════════════════════════════════════════════

  

  
  const [isScrolled, setIsScrolled] = useState(false);

  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const [searchOpen, setSearchOpen] = useState(false);

  
  const [activeDropdown, setActiveDropdown] = useState(null);

  const [userMenuOpen, setUserMenuOpen] = useState(false);

  
  const [searchValue, setSearchValue] = useState('');

  

  
  const { cartItems, totalItems } = useCart();

  const { wishlistItems } = useWishlist();

  const navigate = useNavigate();

  

  
  const { user, logout, isAdmin, isStaff } = useAuth();

  

  
  const searchRef = useRef(null);

  

  const dropdownCloseTimer = useRef(null);





  // ════════════════════════════════════════════════════════════
  // XỬ LÝ DROPDOWN
  // ════════════════════════════════════════════════════════════



  const openDropdown = (key) => {
    if (dropdownCloseTimer.current) clearTimeout(dropdownCloseTimer.current);
    setActiveDropdown(key);
  };

  
  
  const scheduleCloseDropdown = () => {
    if (dropdownCloseTimer.current) clearTimeout(dropdownCloseTimer.current);
    dropdownCloseTimer.current = setTimeout(() => setActiveDropdown(null), 150);
  };

  
  const closeDropdownNow = () => {
    if (dropdownCloseTimer.current) clearTimeout(dropdownCloseTimer.current);
    setActiveDropdown(null);
  };

  

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════


  useEffect(() => () => {
    if (dropdownCloseTimer.current) clearTimeout(dropdownCloseTimer.current);
  }, []);

  
  
  useEffect(() => {

    const handleScroll = () => setIsScrolled(window.scrollY > 10);

    
    window.addEventListener('scroll', handleScroll);

    
    return () => window.removeEventListener('scroll', handleScroll);
  }, []); 

  

  useEffect(() => {
    const handleClickOutside = (event) => {
      
      if (searchRef.current && !searchRef.current.contains(event.target)) {
        setSearchOpen(false);
      }
    };

    if (searchOpen) document.addEventListener('mousedown', handleClickOutside);

    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [searchOpen]); 

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════



  const handleSearchSubmit = (e) => {

    e.preventDefault();

    if (!searchValue.trim()) return;

    // Điều hướng trong SPA (không reload toàn trang như window.location.href).
    navigate(`/shop?keyword=${encodeURIComponent(searchValue.trim())}`);
    setSearchOpen(false);
    setSearchValue('');
  };

  

  // ════════════════════════════════════════════════════════════
  // RENDER: HEADER & NAV DESKTOP
  // ════════════════════════════════════════════════════════════

  return (

    <header
      className={`fixed top-0 z-50 w-full transition-all duration-300 ${
        isScrolled
          ? 'bg-white/90 backdrop-blur-md border-b border-gray-100' 
          : 'bg-white/80 backdrop-blur-sm'                          
      }`}
    >
      {}
      <div className="w-full px-4 sm:px-6 lg:px-8">
        {}
        <div className="flex h-16 items-center justify-between lg:h-20">
          {}
          <div className="flex items-center gap-6 lg:gap-10">
            {}
            <button
              className="lg:hidden"
              onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            >
              {}
              {mobileMenuOpen ? <X size={24} /> : <Menu size={24} />}
            </button>

            {}
            <Link
              to="/"
              className="absolute left-1/2 -translate-x-1/2 lg:static lg:translate-x-0"
            >
              <h1 className="font-display text-3xl font-normal tracking-[0.25em] lg:text-4xl">
                MOON
              </h1>
            </Link>

            {}
            <nav className="relative z-[80] hidden lg:flex lg:items-center lg:gap-8">
              {NAV_GROUPS.map((group) => {
                if (!group.items) {
                  return (
                    <NavLink
                      key={group.key}
                      to={group.path}
                      onPointerEnter={closeDropdownNow}
                      onClick={closeDropdownNow}
                      className={({ isActive }) =>
                        `relative cursor-pointer px-3 py-3 text-sm font-medium uppercase tracking-[0.18em] transition-colors hover:text-black ${
                          isActive ? 'text-black' : 'text-gray-600'
                        }`
                      }
                    >
                      {group.label}
                    </NavLink>
                  );
                }

                const isActive = activeDropdown === group.key;
                return (
                  <button
                    key={group.key}
                    type="button"

                    onPointerEnter={() => openDropdown(group.key)}
                    onPointerLeave={scheduleCloseDropdown}

                    onFocus={() => openDropdown(group.key)}

                    onClick={() => (isActive ? closeDropdownNow() : openDropdown(group.key))}
                    className={`relative cursor-pointer px-3 py-3 text-sm font-medium uppercase tracking-[0.18em] transition-colors hover:text-black ${
                      isActive ? 'text-black' : 'text-gray-600'
                    }`}
                  >
                    {group.label}
                    {}
                    <span
                      className={`pointer-events-none absolute bottom-2 left-3 right-3 h-px bg-black transition-transform duration-200 ${
                        isActive ? 'scale-x-100' : 'scale-x-0'
                      }`}
                    />
                  </button>
                );
              })}
            </nav>
          </div>

          {/* ════════════════════════════════════════════════════════════ */}
          {/* RENDER: USER MENU & ACTIONS                                   */}
          {/* ════════════════════════════════════════════════════════════ */}

          {}
          <div className="flex items-center gap-4">
            {}
            <div
              className="relative hidden lg:block"
              onMouseEnter={() => setUserMenuOpen(true)}
              onMouseLeave={() => setUserMenuOpen(false)}
            >
              <div className="py-2 cursor-default">
                {}
                <Link
                  to={user ? '/account' : '/login'}
                  className="flex items-center gap-1 text-sm font-medium text-gray-500 hover:text-black transition-colors"
                >
                  <User size={18} />
                  {}
                  <span>{user ? user.name || user.username : 'Đăng Nhập'}</span>
                </Link>
              </div>

              {}
              <AnimatePresence>
                {userMenuOpen && (
                  <motion.div

                    
                    initial={{ opacity: 0, y: -5 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, y: -5 }}
                    transition={{ duration: 0.15 }}
                    className="absolute right-0 top-full pt-0"
                  >
                    <div className="min-w-[200px] rounded-xl bg-white/95 backdrop-blur-sm p-1.5 shadow-2xl shadow-black/5 ring-1 ring-black/5">
                      {}
                      {user ? (
                        
                        <>
                          <Link
                            to="/account"
                            className="flex items-center gap-2 rounded-lg px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-100/80"
                          >
                            <UserCircle size={16} />
                            Tài Khoản
                          </Link>
                          <Link
                            to="/my-orders"
                            className="flex items-center gap-2 rounded-lg px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-100/80"
                          >
                            <ShoppingBag size={16} />
                            Đơn Hàng
                          </Link>
                          {}
                          {isStaff() && (
                            <Link
                              to="/admin"
                              className="flex items-center gap-2 rounded-lg px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-100/80"
                            >
                              <LayoutDashboard size={16} />
                              Quản Trị
                            </Link>
                          )}
                          <hr className="my-1" />
                          {}
                          <button
                            onClick={logout}
                            className="flex w-full items-center gap-2 rounded-lg px-4 py-2.5 text-sm text-red-500 hover:bg-red-50"
                          >
                            <LogOut size={16} />
                            Đăng Xuất
                          </button>
                        </>
                      ) : (
                        
                        <>
                          <Link
                            to="/login"
                            className="block rounded-lg px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-100/80"
                          >
                            Đăng Nhập
                          </Link>
                          <Link
                            to="/register"
                            className="block rounded-lg px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-100/80"
                          >
                            Tạo Tài Khoản
                          </Link>
                        </>
                      )}
                    </div>
                  </motion.div>
                )}
              </AnimatePresence>
            </div>

            {}
            <Link
              to="/track"
              className="hidden lg:flex items-center gap-1 text-sm font-medium text-gray-500 hover:text-black transition-colors"
            >
              <Package size={18} />
              <span>Tra Cứu</span>
            </Link>

            {}
            <button onClick={() => setSearchOpen(!searchOpen)} className="p-1">
              <Search size={20} />
            </button>

            {}
            <Link to="/wishlist" className="relative p-1">
              <Heart size={20} />
              {}
              {wishlistItems.length > 0 && (
                <span className="absolute -right-1 -top-1 flex h-4 w-4 items-center justify-center rounded-full bg-black text-[10px] text-white">
                  {wishlistItems.length}
                </span>
              )}
            </Link>

            {}
            <Link to="/cart" className="relative p-1">
              <ShoppingBag size={20} />
              {totalItems > 0 && (
                <span className="absolute -right-1 -top-1 flex h-4 w-4 items-center justify-center rounded-full bg-black text-[10px] text-white">
                  {totalItems}
                </span>
              )}
            </Link>
          </div>
        </div>

        {/* ════════════════════════════════════════════════════════════ */}
        {/* RENDER: SEARCH BAR                                            */}
        {/* ════════════════════════════════════════════════════════════ */}

        {}
        <AnimatePresence>
          {searchOpen && (
            <motion.div
              ref={searchRef}
              initial={{ opacity: 0, y: -10 }}       
              animate={{ opacity: 1, y: 0 }}         
              exit={{ opacity: 0, y: -10 }}          
              className="absolute left-0 top-full w-full border-t bg-white p-4 shadow-sm"
            >
              <form onSubmit={handleSearchSubmit} className="mx-auto max-w-2xl">
                <input
                  type="text"
                  value={searchValue}                                 
                  onChange={(e) => setSearchValue(e.target.value)}    
                  placeholder="Tìm kiếm sản phẩm..."
                  className="w-full border-b border-gray-200 bg-transparent py-3 text-lg outline-none focus:border-black"
                  autoFocus                                           
                />
              </form>
            </motion.div>
          )}
        </AnimatePresence>
      </div>

      {/* ════════════════════════════════════════════════════════════ */}
      {/* RENDER: MOBILE MENU                                           */}
      {/* ════════════════════════════════════════════════════════════ */}

      {}
      <AnimatePresence>
        {mobileMenuOpen && (
          <motion.div

            initial={{ opacity: 0, height: 0 }}
            animate={{ opacity: 1, height: 'auto' }}
            exit={{ opacity: 0, height: 0 }}
            className="lg:hidden overflow-hidden bg-white border-t border-gray-100"
          >
            <nav className="flex flex-col space-y-1 px-4 py-6">
              {}
              {NAV_GROUPS.map((group) => (
                <div key={group.key} className="py-1">
                  {group.items ? (
                    <>
                      <p className="text-xs uppercase tracking-wider text-gray-400 mb-1">{group.label}</p>
                      {group.items.map((cat) => (
                        <NavLink
                          key={cat.name}
                          to={cat.path}
                          className={`block pl-3 py-1 text-base font-light tracking-wide ${
                            cat.accent ? 'text-red-500' : ''
                          }`}
                          onClick={() => setMobileMenuOpen(false)}
                        >
                          {cat.name}
                        </NavLink>
                      ))}
                    </>
                  ) : (
                    <NavLink
                      to={group.path}
                      className="block py-1 text-base font-medium uppercase tracking-wider"
                      onClick={() => setMobileMenuOpen(false)}
                    >
                      {group.label}
                    </NavLink>
                  )}
                </div>
              ))}

              <hr className="my-2 border-gray-100" />

              {}
              <NavLink
                to="/my-orders"
                className="text-lg font-light tracking-wide py-1"
                onClick={() => setMobileMenuOpen(false)}
              >
                Đơn Hàng
              </NavLink>
              <NavLink
                to="/wishlist"
                className="text-lg font-light tracking-wide py-1"
                onClick={() => setMobileMenuOpen(false)}
              >
                Yêu Thích
              </NavLink>
              <NavLink
                to="/track"
                className="text-lg font-light tracking-wide py-1"
                onClick={() => setMobileMenuOpen(false)}
              >
                Tra Cứu Đơn Hàng
              </NavLink>

              <hr className="my-2 border-gray-100" />

              {}
              {user ? (
                <>
                  <NavLink
                    to="/account"
                    className="text-lg font-light tracking-wide py-1"
                    onClick={() => setMobileMenuOpen(false)}
                  >
                    Tài Khoản
                  </NavLink>
                  {isStaff() && (
                    <NavLink
                      to="/admin"
                      className="text-lg font-light tracking-wide py-1"
                      onClick={() => setMobileMenuOpen(false)}
                    >
                      Quản Trị
                    </NavLink>
                  )}
                  {}
                  <button
                    onClick={() => {
                      logout();
                      setMobileMenuOpen(false);
                    }}
                    className="text-left text-lg font-light tracking-wide text-red-500 py-1"
                  >
                    Đăng Xuất
                  </button>
                </>
              ) : (
                <>
                  <NavLink
                    to="/login"
                    className="text-lg font-light tracking-wide py-1"
                    onClick={() => setMobileMenuOpen(false)}
                  >
                    Đăng Nhập
                  </NavLink>
                  <NavLink
                    to="/register"
                    className="text-lg font-light tracking-wide py-1"
                    onClick={() => setMobileMenuOpen(false)}
                  >
                    Đăng Ký
                  </NavLink>
                </>
              )}
            </nav>
          </motion.div>
        )}
      </AnimatePresence>

      {/* ════════════════════════════════════════════════════════════ */}
      {/* RENDER: DROPDOWN PANEL (PORTAL)                               */}
      {/* ════════════════════════════════════════════════════════════ */}

      {}
      {createPortal(
        <AnimatePresence>
          {activeDropdown && (
            <>
              {}
              <motion.div
                key="nav-backdrop"
                className="fixed left-0 right-0 top-16 z-[60] hidden bg-black/40 lg:block lg:top-20"
                style={{ bottom: 0 }}
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                transition={{ duration: 0.25 }}
                
                onClick={closeDropdownNow}
              />
              {}
              <motion.aside
                key="nav-panel"
                className="fixed left-0 top-16 z-[70] hidden w-full max-w-md overflow-y-auto bg-white shadow-2xl lg:block lg:top-20 lg:w-1/2"
                style={{ bottom: 0 }}

                initial={{ x: '-100%' }}
                animate={{ x: 0 }}
                exit={{ x: '-100%' }}
                transition={{ type: 'tween', duration: 0.3, ease: 'easeOut' }}
                
                onMouseEnter={() => openDropdown(activeDropdown)}
                
                onMouseLeave={scheduleCloseDropdown}
              >
                <div className="px-10 py-10">
                  {}
                  <p className="mb-6 text-xs uppercase tracking-[0.3em] text-gray-400">
                    {NAV_GROUPS.find((g) => g.key === activeDropdown)?.label}
                  </p>
                  <nav className="flex flex-col">
                    {}
                    {NAV_GROUPS.find((g) => g.key === activeDropdown)?.items.map((cat) => (
                      <Link
                        key={cat.name}
                        to={cat.path}
                        
                        onClick={closeDropdownNow}
                        className={`py-3 text-base tracking-wide transition-colors ${
                          cat.accent
                            ? 'font-semibold text-red-500 hover:text-red-600' 
                            : 'text-gray-700 hover:text-black'
                        }`}
                      >
                        {cat.name}
                      </Link>
                    ))}
                  </nav>
                </div>
              </motion.aside>
            </>
          )}
        </AnimatePresence>,
        document.body 
      )}
    </header>
  );
};

export default Header;
