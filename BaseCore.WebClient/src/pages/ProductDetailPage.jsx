

import { useState, useEffect, useMemo } from 'react';

import { useParams, Link } from 'react-router-dom';

import { Minus, Plus, ShoppingBag, Heart, Ruler, ChevronLeft, ChevronRight } from 'lucide-react';

import { useCart } from '../contexts/CartContext';

import { useWishlist } from '../contexts/WishlistContext';

import { productService } from '../services/productService';

import { slugify } from '../utils/slug';

import { Loading } from '../components/common/Loading';

import { ReviewSection } from '../components/product/ReviewSection';

import { fmt, IMAGE_PLACEHOLDER } from '../utils/format';

// ════════════════════════════════════════════════════════════
// TRANG CHI TIẾT SẢN PHẨM
// ════════════════════════════════════════════════════════════
const ProductDetailPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const { id } = useParams();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [product, setProduct] = useState(null);

  const [loading, setLoading] = useState(true);


  const [colorId, setColorId] = useState(null);
  const [sizeId, setSizeId] = useState(null);

  const [quantity, setQuantity] = useState(1);

  const [activeImageIdx, setActiveImageIdx] = useState(0);

  const [added, setAdded] = useState(false);

  const [error, setError] = useState('');

  const [showSizeGuide, setShowSizeGuide] = useState(false);

  const [sizeGuide, setSizeGuide] = useState([]);

  const { addToCart } = useCart();
  const { addToWishlist, removeFromWishlist, isInWishlist } = useWishlist();

  // ════════════════════════════════════════════════════════════
  // EFFECT: TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    setLoading(true);
    productService
      .getProductById(id)
      .then((data) => setProduct(data))
      .catch(() => setProduct(null))
      .finally(() => setLoading(false));
  }, [id]);

  

  useEffect(() => {
    if (showSizeGuide && sizeGuide.length === 0) {
      productService.getSizeGuide().then(setSizeGuide).catch(() => {});
    }
  }, [showSizeGuide, sizeGuide.length]);

  // ════════════════════════════════════════════════════════════
  // TÍNH TOÁN
  // ════════════════════════════════════════════════════════════
  const variants = product?.variants || [];

  const colors = useMemo(() => {
    const map = new Map();
    variants.forEach((v) => {
      if (v.color && !map.has(v.color.id)) map.set(v.color.id, v.color);
    });
    return Array.from(map.values());
  }, [variants]);

  const sizes = useMemo(() => {
    const map = new Map();
    variants.forEach((v) => {
      if (v.size && !map.has(v.size.id)) map.set(v.size.id, v.size);
    });
    return Array.from(map.values()).sort((a, b) => (a.id || 0) - (b.id || 0));
  }, [variants]);

  

  const selectedVariant = useMemo(
    () => variants.find((v) => v.colorId === colorId && v.sizeId === sizeId),
    [variants, colorId, sizeId]
  );

  

  
  const colorStock = useMemo(() => {
    if (!colorId) return null;
    return variants
      .filter((v) => v.colorId === colorId)
      .reduce((sum, v) => sum + Math.max(0, v.stock - v.reservedStock), 0);
  }, [variants, colorId]);

  
  const isVariantAvailable = (cid, sid) => {
    const v = variants.find((x) => x.colorId === cid && x.sizeId === sid);
    return v && (v.stock - v.reservedStock) > 0;
  };

  

  
  
  const gallery = product?.images || [];

  

  
  useEffect(() => {
    if (gallery.length === 0) return;
    const primaryIdx = gallery.findIndex((img) => img.isPrimary);
    setActiveImageIdx(primaryIdx >= 0 ? primaryIdx : 0);
  }, [gallery]);

  

  

  

  

  
  useEffect(() => {
    if (!colorId || gallery.length === 0) return;
    const byColor = gallery
      .map((img, idx) => ({ img, idx }))                       
      .filter(({ img }) => img.colorId === colorId);           
    if (byColor.length === 0) return;                          
    const nonPrimary = byColor.find(({ img }) => !img.isPrimary);
    setActiveImageIdx((nonPrimary || byColor[0]).idx);          
  }, [colorId, gallery]);

  

  

  const resolveImageUrl = (img) => {
    if (!img?.fileName || !product) return '';
    const slug = product.slug || slugify(product.name);
    return productService.imagePath(slug, img.fileName);
  };

  const activeImageUrl = gallery.length > 0
    ? resolveImageUrl(gallery[activeImageIdx])
    : IMAGE_PLACEHOLDER;

  const showPrevImage = () =>
    setActiveImageIdx((i) => (gallery.length ? (i - 1 + gallery.length) % gallery.length : 0));

  const showNextImage = () =>
    setActiveImageIdx((i) => (gallery.length ? (i + 1) % gallery.length : 0));


  if (loading) return <div className="flex h-96 items-center justify-center"><Loading /></div>;

  if (!product) return (
    <div className="mx-auto max-w-2xl px-4 py-20 text-center">
      <h2 className="font-serif text-3xl font-light">Không tìm thấy sản phẩm</h2>
      <Link to="/shop" className="mt-6 inline-block bg-black px-8 py-3 text-white">Về cửa hàng</Link>
    </div>
  );


  const minPrice = productService.getMinVariantPrice(product);
  const totalStock = productService.getTotalStock(product);
  const inWishlist = isInWishlist(product.id);

  const disc = product?.discountPercent || 0;
  const factor = 1 - disc / 100;
  const displayPrice = selectedVariant ? selectedVariant.price * factor : minPrice;
  const origPrice = disc > 0 ? (selectedVariant ? selectedVariant.price : minPrice / factor) : null;

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const handleAddToCart = async () => {
    setError('');
    if (!selectedVariant) {
      setError('Vui lòng chọn màu và size.');
      return;
    }
    const available = selectedVariant.stock - selectedVariant.reservedStock;
    if (quantity > available) {
      setError(`Chỉ còn ${available} sản phẩm cho biến thể này.`);
      return;
    }
    try {
      await addToCart(selectedVariant, product, quantity);
      setAdded(true);
      setTimeout(() => setAdded(false), 1800);
    } catch (e) {
      setError(e.response?.data?.message || 'Không thêm được vào giỏ hàng.');
    }
  };

  const handleWishlist = () => inWishlist ? removeFromWishlist(product.id) : addToWishlist(product);

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="bg-white">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        {}
        {/* ════════════════════════════════════════════════════════════
            RENDER: BREADCRUMB
            ════════════════════════════════════════════════════════════ */}
        <nav className="py-6 text-sm text-gray-500">
          <ol className="flex items-center space-x-2">
            <li><Link to="/" className="hover:text-black">Trang chủ</Link></li>
            <li>/</li>
            <li><Link to="/shop" className="hover:text-black">Cửa hàng</Link></li>
            <li>/</li>
            <li className="text-black">{product.name}</li>
          </ol>
        </nav>

        {}
        <div className="py-8 lg:grid lg:grid-cols-2 lg:gap-12">
          {}
          {/* ════════════════════════════════════════════════════════════
              RENDER: GALLERY
              ════════════════════════════════════════════════════════════ */}
          <div>
            {}
            <div className="relative aspect-[3/4] overflow-hidden bg-gray-100">
              <img
                src={activeImageUrl}
                alt={product.name}
                className="h-full w-full object-cover"
              />
              {}
              {gallery.length > 1 && (
                <>
                  <button
                    type="button"
                    onClick={showPrevImage}
                    aria-label="Ảnh trước"
                    className="absolute left-3 top-1/2 -translate-y-1/2 rounded-full bg-white/80 p-2 text-black shadow hover:bg-white"
                  >
                    <ChevronLeft size={20} />
                  </button>
                  <button
                    type="button"
                    onClick={showNextImage}
                    aria-label="Ảnh sau"
                    className="absolute right-3 top-1/2 -translate-y-1/2 rounded-full bg-white/80 p-2 text-black shadow hover:bg-white"
                  >
                    <ChevronRight size={20} />
                  </button>
                </>
              )}
            </div>

            {}
            {gallery.length > 1 && (
              <div className="mt-3 flex gap-2 overflow-x-auto pb-1">
                {gallery.map((img, idx) => {
                  const active = idx === activeImageIdx;          
                  return (
                    <button
                      key={img.id ?? idx}                         
                      type="button"
                      onClick={() => setActiveImageIdx(idx)}      
                      className={`relative h-20 w-16 flex-shrink-0 overflow-hidden border ${
                        active ? 'border-black' : 'border-gray-200 hover:border-gray-400'
                      }`}
                      aria-label={`Chọn ảnh ${idx + 1}`}
                    >
                      <img
                        src={resolveImageUrl(img)}
                        alt={`${product.name} ${idx + 1}`}
                        className="h-full w-full object-cover"
                      />
                    </button>
                  );
                })}
              </div>
            )}
          </div>

          {}
          {/* ════════════════════════════════════════════════════════════
              RENDER: THÔNG TIN & CHỌN BIẾN THỂ
              ════════════════════════════════════════════════════════════ */}
          <div className="mt-8 lg:mt-0">
            {}
            <p className="text-xs uppercase tracking-[0.3em] text-gray-400">
              {product.category?.name || 'Product'}
            </p>
            {}
            <h1 className="mt-2 font-serif text-3xl font-light">{product.name}</h1>

            {}
            <div className="mt-3 flex items-baseline gap-3">
              <p className="text-2xl font-light">{fmt(displayPrice)} đ</p>
              {disc > 0 && (
                <span className="text-sm text-gray-400 line-through">{fmt(origPrice)} đ</span>
              )}
            </div>

            {}
            <p className="mt-4 text-sm text-gray-600 whitespace-pre-line">{product.description}</p>

            {}
            {colors.length > 0 && (
              <div className="mt-6">
                <p className="text-xs uppercase tracking-[0.2em] text-slate-400">Màu sắc</p>
                <div className="mt-2 flex flex-wrap gap-2">
                  {colors.map((c) => {
                    const active = c.id === colorId;
                    return (
                      <button
                        key={c.id}

                        onClick={() => { setColorId(c.id); setSizeId(null); }}
                        className={`flex items-center gap-1 border px-3 py-1.5 text-sm ${
                          active ? 'border-black bg-black text-white' : 'border-gray-200 hover:border-gray-400'
                        }`}
                      >
                        {}
                        <span className="inline-block h-3 w-3 rounded-full border" style={{ backgroundColor: c.hexCode }} />
                        {c.name}
                      </button>
                    );
                  })}
                </div>
              </div>
            )}

            {}
            {sizes.length > 0 && (
              <div className="mt-4">
                <div className="flex items-center justify-between">
                  <p className="text-xs uppercase tracking-[0.2em] text-slate-400">Kích cỡ</p>
                  {}
                  <button
                    onClick={() => setShowSizeGuide((s) => !s)}
                    className="flex items-center gap-1 text-xs text-gray-500 hover:text-black"
                  >
                    <Ruler size={12} /> Bảng size
                  </button>
                </div>
                <div className="mt-2 flex flex-wrap gap-2">
                  {sizes.map((s) => {
                    const active = s.id === sizeId;

                    // Chưa chọn màu → khoá size & hướng dẫn người dùng chọn màu trước.
                    const disabled = colorId ? !isVariantAvailable(colorId, s.id) : true;
                    return (
                      <button
                        key={s.id}
                        disabled={disabled}
                        title={!colorId ? 'Chọn màu trước' : undefined}
                        onClick={() => setSizeId(s.id)}
                        className={`min-w-[3rem] border px-3 py-1.5 text-sm ${
                          active ? 'border-black bg-black text-white'
                          : !colorId ? 'border-gray-200 text-gray-300 cursor-not-allowed'
                          : disabled ? 'border-gray-200 text-gray-300 line-through cursor-not-allowed'
                          : 'border-gray-200 hover:border-gray-400'
                        }`}
                      >
                        {s.name}
                      </button>
                    );
                  })}
                </div>
                {}
                {!colorId && (
                  <p className="mt-2 text-xs text-amber-600">Vui lòng chọn màu trước để xem size còn hàng.</p>
                )}
              </div>
            )}

            {}
            {showSizeGuide && sizeGuide.length > 0 && (
              <div className="mt-4 overflow-x-auto rounded-lg border bg-slate-50 p-3 text-xs">
                <table className="w-full">
                  <thead>
                    <tr className="text-left text-slate-500">
                      <th className="pb-2 pr-4">Cỡ</th>
                      <th className="pb-2 pr-4">Ngực</th>
                      <th className="pb-2 pr-4">Eo</th>
                      <th className="pb-2 pr-4">Vai</th>
                      <th className="pb-2">Dài</th>
                    </tr>
                  </thead>
                  <tbody>
                    {sizeGuide.map((g) => (
                      <tr key={g.id}>
                        <td className="py-1 pr-4 font-semibold">{g.size?.name}</td>
                        <td className="py-1 pr-4">{g.chest} cm</td>
                        <td className="py-1 pr-4">{g.waist} cm</td>
                        <td className="py-1 pr-4">{g.shoulder} cm</td>
                        <td className="py-1">{g.length} cm</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            {}
            <p className={`mt-4 text-sm ${
              selectedVariant
                ? (selectedVariant.stock - selectedVariant.reservedStock) > 0 ? 'text-green-600' : 'text-red-500'
                : colorId
                  ? colorStock > 0 ? 'text-green-600' : 'text-red-500'
                  : totalStock > 0 ? 'text-green-600' : 'text-red-500'
            }`}>
              {selectedVariant
                ? `Còn lại: ${selectedVariant.stock - selectedVariant.reservedStock}`
                : colorId
                  ? colorStock > 0 ? `Tồn kho màu này: ${colorStock}` : 'Hết hàng màu này'
                  : totalStock > 0 ? `Tổng tồn kho: ${totalStock}` : 'Hết hàng'}
            </p>

            {}
            <div className="mt-6 flex items-center gap-4">
              {}
              <div className="flex items-center border border-gray-200">
                {}
                <button onClick={() => setQuantity(Math.max(1, quantity - 1))} className="p-3">
                  <Minus size={16} />
                </button>
                {}
                <input
                  type="number"
                  min={1}
                  value={quantity}
                  onChange={(e) => {
                    const v = e.target.value;
                    if (v === '') { setQuantity(''); return; }          
                    const n = parseInt(v, 10);
                    if (!Number.isNaN(n)) setQuantity(Math.max(1, n));   
                  }}
                  onBlur={() => { if (!quantity || quantity < 1) setQuantity(1); }}
                  className="w-12 border-0 bg-transparent text-center outline-none focus:ring-0 [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                  aria-label="Số lượng"
                />
                {}
                <button onClick={() => setQuantity((quantity || 0) + 1)} className="p-3">
                  <Plus size={16} />
                </button>
              </div>
              {}
              <button
                onClick={handleAddToCart}
                disabled={totalStock === 0}
                className="flex flex-1 items-center justify-center gap-2 bg-black py-3 text-white hover:bg-gray-800 disabled:bg-gray-300"
              >
                <ShoppingBag size={18} />
                {added ? 'Đã thêm!' : totalStock > 0 ? 'Thêm vào giỏ' : 'Hết hàng'}
              </button>
              {}
              <button onClick={handleWishlist} className="border border-gray-200 p-3 hover:border-black">
                <Heart size={18} className={inWishlist ? 'fill-black text-black' : ''} />
              </button>
            </div>

            {}
            {error && <p className="mt-3 text-sm text-red-500">{error}</p>}

            {}
            <div className="mt-8 rounded-2xl bg-slate-50 p-4 text-sm text-slate-600">
              <p className="text-xs uppercase tracking-[0.2em] text-slate-400">Danh mục</p>
              <p className="mt-1">{product.category?.name || '—'}</p>
            </div>
          </div>
        </div>

        {}
        {/* ════════════════════════════════════════════════════════════
            RENDER: TAB MÔ TẢ / ĐÁNH GIÁ
            ════════════════════════════════════════════════════════════ */}
        <ReviewSection productId={Number(id)} />
      </div>
    </div>
  );
};

export default ProductDetailPage;
