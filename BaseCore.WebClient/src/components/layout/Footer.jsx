

import { Link } from 'react-router-dom';

// ════════════════════════════════════════════════════════════
// COMPONENT: INSTAGRAM ICON
// ════════════════════════════════════════════════════════════
const InstagramIcon = ({ size = 20 }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="currentColor">
    {}
    <path d="M12 2c2.717 0 3.056.01 4.122.06 1.065.05 1.79.217 2.428.465.66.254 1.216.598 1.772 1.153.509.5.902 1.105 1.153 1.772.247.637.415 1.363.465 2.428.047 1.066.06 1.405.06 4.122 0 2.717-.01 3.056-.06 4.122-.05 1.065-.218 1.79-.465 2.428a4.883 4.883 0 01-1.153 1.772 4.915 4.915 0 01-1.772 1.153c-.637.247-1.363.415-2.428.465-1.066.047-1.405.06-4.122.06-2.717 0-3.056-.01-4.122-.06-1.065-.05-1.79-.218-2.428-.465a4.89 4.89 0 01-1.772-1.153 4.904 4.904 0 01-1.153-1.772c-.248-.637-.415-1.363-.465-2.428C2.013 15.056 2 14.717 2 12c0-2.717.01-3.056.06-4.122.05-1.066.217-1.79.465-2.428a4.88 4.88 0 011.153-1.772A4.897 4.897 0 015.45 2.525c.638-.248 1.362-.415 2.428-.465C8.944 2.013 9.283 2 12 2zm0 5a5 5 0 100 10 5 5 0 000-10zm6.5-.25a1.25 1.25 0 10-2.5 0 1.25 1.25 0 002.5 0zM12 9a3 3 0 110 6 3 3 0 010-6z" />
  </svg>
);

// ════════════════════════════════════════════════════════════
// COMPONENT: TWITTER ICON
// ════════════════════════════════════════════════════════════
const TwitterIcon = ({ size = 20 }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="currentColor">
    {}
    <path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z" />
  </svg>
);

// ════════════════════════════════════════════════════════════
// COMPONENT: FACEBOOK ICON
// ════════════════════════════════════════════════════════════
const FacebookIcon = ({ size = 20 }) => (
  <svg width={size} height={size} viewBox="0 0 24 24" fill="currentColor">
    <path d="M22 12c0-5.52-4.48-10-10-10S2 6.48 2 12c0 4.84 3.44 8.87 8 9.8V15H8v-3h2V9.5C10 7.57 11.57 6 13.5 6H16v3h-2c-.55 0-1 .45-1 1v2h3v3h-3v6.95c5.05-.5 9-4.76 9-9.95z" />
  </svg>
);

// ════════════════════════════════════════════════════════════
// COMPONENT FOOTER
// ════════════════════════════════════════════════════════════
const Footer = () => {
  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    
    <footer className="border-t border-gray-100 bg-white">
      {}
      <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        {}
        <div className="grid grid-cols-1 gap-8 md:grid-cols-4">
          {}
          <div>
            {}
            <h3 className="font-serif text-xl font-light tracking-wider">MOON</h3>
            {}
            <p className="mt-4 text-sm text-gray-500">
              Nền tảng mua sắm thời trang cao cấp — phong cách tinh tế, chất lượng vượt trội.
            </p>
            {}
            <div className="mt-6 flex space-x-4">
              {}
              <a href="https://web.facebook.com/hoang.nhat.71736" className="text-gray-400 hover:text-black"><InstagramIcon size={20} /></a>
              <a href="https://www.facebook.com/hoang.nhat.71736" className="text-gray-400 hover:text-black"><FacebookIcon size={20} /></a>
              <a href="https://web.facebook.com/hoang.nhat.71736" className="text-gray-400 hover:text-black"><TwitterIcon size={20} /></a>
            </div>
          </div>

          {}
          <div>
            <h4 className="mb-4 text-sm font-medium uppercase tracking-wider">Mua Sắm</h4>
            <ul className="space-y-2 text-sm text-gray-500">
              {}
              <li><Link to="/shop?newOnly=true" className="hover:text-black">Hàng Mới</Link></li>
              <li><Link to="/shop" className="hover:text-black">Tất Cả Sản Phẩm</Link></li>
              {}
              <li><Link to="/shop?sale=true" className="hover:text-black">Khuyến Mãi</Link></li>
              <li><Link to="/wishlist" className="hover:text-black">Yêu Thích</Link></li>
            </ul>
          </div>

          {}
          <div>
            <h4 className="mb-4 text-sm font-medium uppercase tracking-wider">Hỗ Trợ</h4>
            <ul className="space-y-2 text-sm text-gray-500">
              <li><Link to="/my-orders" className="hover:text-black">Theo Dõi Đơn</Link></li>
              <li><Link to="/account" className="hover:text-black">Tài Khoản</Link></li>
              <li><Link to="/cart" className="hover:text-black">Giỏ Hàng</Link></li>
            </ul>
          </div>

          {}
          <div>
            <h4 className="mb-4 text-sm font-medium uppercase tracking-wider">Đăng Ký Nhận Tin</h4>
            <p className="text-sm text-gray-500">Đăng ký để nhận ưu đãi 10% cho đơn hàng đầu tiên.</p>
            {}
            <div className="mt-4 flex">
              {}
              <input
                type="email"
                placeholder="Địa chỉ email"
                className="flex-1 border border-gray-200 px-4 py-2 text-sm outline-none focus:border-black"
              />
              {}
              <button className="bg-black px-4 py-2 text-sm text-white hover:bg-gray-800">
                Đăng Ký
              </button>
            </div>
          </div>
        </div>

        {}
        <div className="mt-12 border-t border-gray-100 pt-8 text-center text-xs text-gray-400">
          <p>&copy; {new Date().getFullYear()} Moon Fashion.</p>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
