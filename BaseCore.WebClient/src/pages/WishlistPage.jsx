

import { Link } from 'react-router-dom';

import { ProductCard } from '../components/product/ProductCard';

import { useWishlist } from '../contexts/WishlistContext';

// ════════════════════════════════════════════════════════════
// TRANG DANH SÁCH YÊU THÍCH
// ════════════════════════════════════════════════════════════
const WishlistPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const { wishlistItems } = useWishlist();

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  if (wishlistItems.length === 0) {
    return (
      <div className="mx-auto max-w-7xl px-4 py-20 text-center">
        <h2 className="font-serif text-2xl">Danh sách yêu thích trống</h2>
        <p className="mt-2 text-gray-500">Lưu các sản phẩm yêu thích của bạn tại đây</p>
        <Link to="/shop" className="mt-8 inline-block bg-black px-8 py-3 text-white">
          Khám phá sản phẩm
        </Link>
      </div>
    );
  }

  
  
  return (
    <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
      <h1 className="font-serif text-3xl font-light">Yêu thích</h1>
      <p className="mt-2 text-gray-500">{wishlistItems.length} sản phẩm đã lưu</p>

      {}
      {/* ════════════════════════════════════════════════════════════
          RENDER: LƯỚI SẢN PHẨM
          ════════════════════════════════════════════════════════════ */}
      <div className="mt-10 grid grid-cols-2 gap-x-4 gap-y-8 md:grid-cols-3 lg:grid-cols-4 lg:gap-x-6">
        {wishlistItems.map((product) => (
          
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </div>
  );
};

export default WishlistPage;
