

import { Link, useNavigate } from 'react-router-dom';

import { Heart, ShoppingCart } from 'lucide-react';

import { useWishlist } from '../../contexts/WishlistContext';

import { productService } from '../../services/productService';

import { fmt, IMAGE_PLACEHOLDER } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// COMPONENT THẺ SẢN PHẨM
// ════════════════════════════════════════════════════════════
export const ProductCard = ({ product }) => {
  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const navigate = useNavigate();



  const { addToWishlist, removeFromWishlist, isInWishlist } = useWishlist();

  const inWishlist = isInWishlist(product.id);

  // ════════════════════════════════════════════════════════════
  // DERIVED VALUES / MEMOS
  // ════════════════════════════════════════════════════════════
  const imageSrc = productService.getPrimaryImage(product) || IMAGE_PLACEHOLDER;


  const minPrice = productService.getMinVariantPrice(product);

  const totalStock = productService.getTotalStock(product);

  const outOfStock = totalStock === 0;



  // ════════════════════════════════════════════════════════════
  // HANDLERS
  // ════════════════════════════════════════════════════════════
  const handleWishlist = (e) => {
    e.preventDefault();   
    e.stopPropagation();  

    if (inWishlist) removeFromWishlist(product.id);
    else addToWishlist(product);
  };

  

  const goPickVariant = (e) => {
    e.preventDefault();   
    e.stopPropagation();  
    
    navigate(`/product/${product.id}`);
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    <Link to={`/product/${product.id}`} className="group block">
      {}
      <div className="relative aspect-[3/4] overflow-hidden bg-gray-100">
        <img
          src={imageSrc}
          alt={product.name}

          onError={(e) => { if (e.target.src !== IMAGE_PLACEHOLDER) e.target.src = IMAGE_PLACEHOLDER; }}

          
          className="h-full w-full object-cover transition-transform duration-700 group-hover:scale-105"
        />

        {}
        {outOfStock && (
          <span className="absolute left-3 top-3 rounded-full bg-slate-700 px-2 py-1 text-[10px] font-semibold text-white">
            HẾT HÀNG
          </span>
        )}

        {}
        <button
          onClick={handleWishlist}
          
          className="absolute right-3 top-3 rounded-full bg-white/80 p-2 backdrop-blur-sm transition-colors hover:bg-white"
          aria-label="Thêm vào yêu thích"
        >
          {}
          <Heart size={18} className={inWishlist ? 'fill-black text-black' : 'text-gray-700'} />
        </button>
      </div>

      {}
      <div className="mt-4">
        {}
        {}
        <p className="text-[11px] uppercase tracking-wide text-gray-400">
          {product.category?.name || ''}
        </p>

        {}
        <h3 className="text-sm font-medium line-clamp-1">{product.name}</h3>

        {}
        <p className="mt-1 text-sm text-gray-900">{fmt(minPrice)} đ</p>

        {}
        <button
          onClick={goPickVariant}
          disabled={outOfStock}
          className="mt-2 flex w-full items-center justify-center gap-1 border border-gray-200 py-1.5 text-xs font-medium hover:bg-gray-50 disabled:bg-gray-100 disabled:text-gray-400"
        >
          <ShoppingCart size={14} /> {outOfStock ? 'Hết hàng' : 'Chi tiết sản phẩm'}
        </button>
      </div>
    </Link>
  );
};

export default ProductCard;
