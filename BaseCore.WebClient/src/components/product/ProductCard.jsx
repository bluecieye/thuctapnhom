

import { Link, useNavigate } from 'react-router-dom';

import { Heart, Eye } from 'lucide-react';

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

  const disc = product?.discountPercent || 0;
  const factor = disc > 0 ? (1 - disc / 100) : 1;
  const minPrice = productService.getMinVariantPrice(product);
  const maxPrice = productService.getMaxVariantPrice(product);
  const singlePrice = minPrice === maxPrice;
  const priceLabel = singlePrice ? `${fmt(minPrice)} đ` : `${fmt(minPrice)} – ${fmt(maxPrice)} đ`;
  const origLabel  = singlePrice
    ? `${fmt(minPrice / factor)} đ`
    : `${fmt(minPrice / factor)} – ${fmt(maxPrice / factor)} đ`;

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

        {disc > 0 && (
          <span className="absolute left-3 top-3 rounded-full bg-rose-500 px-2 py-1 text-[10px] font-semibold text-white">
            -{disc}%
          </span>
        )}
        {outOfStock && (
          <span className={`absolute left-3 rounded-full bg-slate-700 px-2 py-1 text-[10px] font-semibold text-white ${disc > 0 ? 'top-8' : 'top-3'}`}>
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

        {disc > 0 ? (
          <div className="mt-1">
            <span className="text-sm font-semibold text-gray-900">{priceLabel}</span>
            <br />
            <span className="text-xs text-gray-400 line-through">{origLabel}</span>
          </div>
        ) : (
          <p className="mt-1 text-sm text-gray-900">{priceLabel}</p>
        )}

        {}
        <button
          onClick={goPickVariant}
          disabled={outOfStock}
          className="mt-2 flex w-full items-center justify-center gap-1 border border-gray-200 py-1.5 text-xs font-medium hover:bg-gray-50 disabled:bg-gray-100 disabled:text-gray-400"
        >
          <Eye size={14} /> {outOfStock ? 'Hết hàng' : 'Xem chi tiết'}
        </button>
      </div>
    </Link>
  );
};

export default ProductCard;
