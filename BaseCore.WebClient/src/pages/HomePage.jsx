

import { useEffect, useRef, useState } from 'react';

import { Link } from 'react-router-dom';

import { ArrowRight, ChevronLeft, ChevronRight, Truck, RefreshCw, ShieldCheck, Headphones } from 'lucide-react';

import { AnimatePresence, motion } from 'framer-motion';

import { ProductCard } from '../components/product/ProductCard';

import { productService } from '../services/productService';

// ════════════════════════════════════════════════════════════
// CONSTANTS / SCHEMA
// ════════════════════════════════════════════════════════════
const HERO_SLIDES = [
  {
    image: '/hero/banner1.avif',
    title: 'Mùa Mới',
    subtitle: 'Phong cách tinh tế cho tủ đồ hiện đại',
    cta: 'Khám Phá Ngay',
    link: '/shop?newOnly=true',
  },
  {
    image: '/hero/banner2.jpg',
    title: 'Khoác Lên Phong Cách',
    subtitle: 'Trench & áo khoác cho ngày se lạnh',
    cta: 'Xem Áo Khoác',
    link: '/shop?categoryIds=1,8',
    position: 'center 20%',
  },
  {
    image: '/hero/banner3.avif',
    title: 'Giảm đến 50%',
    subtitle: 'Sản phẩm chọn lọc — còn hàng có hạn',
    cta: 'Xem Khuyến Mãi',
    link: '/shop?sale=true',
  },
];

// Hai tile danh mục lớn (Nữ / Nam) hiển thị ngay dưới Hero.
const CATEGORY_TILES = [ 
  {
    image: '/hero/woman.jpg',
    label: 'Nữ',
    desc: 'Đầm, áo, chân váy & hơn thế',
    link: '/shop?gender=Female',
  },
  {
    image: '/hero/man.jpg',
    label: 'Nam',
    desc: 'Áo khoác, quần & phụ kiện',
    link: '/shop?gender=Male',
    position: 'center 15%',
  },
];

// Banner editorial (ảnh lớn + nội dung) xen giữa hai khối sản phẩm.
const EDITORIAL = {
  image: '/hero/winter.avif',
  eyebrow: 'Bộ sưu tập',
  title: 'Thu Đông 2026',
  desc: 'Những thiết kế tối giản, chất liệu cao cấp — tôn dáng cho mọi khoảnh khắc thường nhật.',
  cta: 'Khám phá bộ sưu tập',
  link: '/shop?newOnly=true',
};

// Lưới danh mục nhanh (4 ô) dẫn tới các nhóm sản phẩm phổ biến.
const QUICK_CATEGORIES = [
  { image: '/hero/tshirt.jpg', label: 'Áo',       link: '/shop?categoryId=5' },
  { image: '/hero/jacket.jpg', label: 'Áo khoác', link: '/shop?categoryId=8' },
  { image: '/hero/jean.jpg',   label: 'Quần',     link: '/shop?categoryIds=6,7' },
  { image: '/hero/dress.avif', label: 'Váy',      link: '/shop?categoryId=9' },
];

// Thanh tiện ích / cam kết dịch vụ.
const FEATURES = [
  { icon: Truck,       title: 'Miễn phí vận chuyển', desc: 'Cho đơn hàng từ 1.000.000đ' },
  { icon: RefreshCw,   title: 'Đổi trả 30 ngày',     desc: 'Hoàn tiền dễ dàng, nhanh chóng' },
  { icon: ShieldCheck, title: 'Thanh toán an toàn',  desc: 'Bảo mật thông tin tuyệt đối' },
  { icon: Headphones,  title: 'Hỗ trợ 24/7',         desc: 'Luôn sẵn sàng giúp đỡ bạn' },
];

// ════════════════════════════════════════════════════════════
// TRANG CHỦ
// ════════════════════════════════════════════════════════════
const HomePage = () => {
  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [bestSellers, setBestSellers] = useState([]);

  const [newArrivals, setNewArrivals] = useState([]);

  const [slide, setSlide] = useState(0);


  const [direction, setDirection] = useState(1);


  const carouselRef = useRef(null);

  // ════════════════════════════════════════════════════════════
  // EFFECT: TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    productService.getBestSellers(30).then(setBestSellers).catch(() => {});
    productService.getNewArrivals(12).then(setNewArrivals).catch(() => {});
  }, []);

  

  
  useEffect(() => {
    const id = setInterval(() => {
      setDirection(1);                                         
      setSlide((s) => (s + 1) % HERO_SLIDES.length);           
    }, 6000);
    
    return () => clearInterval(id);
  }, []);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const goSlide = (n) => {
    setDirection(n > slide || (slide === HERO_SLIDES.length - 1 && n === 0) ? 1 : -1);
    setSlide(n);
  };

  const prevSlide = () => {
    setDirection(-1);
    setSlide((s) => (s - 1 + HERO_SLIDES.length) % HERO_SLIDES.length);
  };

  const nextSlide = () => {
    setDirection(1);
    setSlide((s) => (s + 1) % HERO_SLIDES.length);
  };

  

  

  
  const scrollCarousel = (direction) => {
    const el = carouselRef.current;
    if (!el) return; 
    const cardWidth = el.firstChild?.getBoundingClientRect().width || 280;
    const gap = 16;
    el.scrollBy({ left: direction * (cardWidth + gap) * 2, behavior: 'smooth' });
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="bg-white">
      {}
      {}
      {}
      {/* ════════════════════════════════════════════════════════════
          RENDER: HERO SLIDER
          ════════════════════════════════════════════════════════════ */}
      <section className="relative -mt-20 h-screen w-full overflow-hidden lg:-mt-24">
        {}
        <AnimatePresence initial={false} custom={direction} mode="popLayout">
          <motion.div
            key={slide}                                       
            custom={direction}
            initial={{ opacity: 0, x: direction * 60 }}       
            animate={{ opacity: 1, x: 0 }}                    
            exit={{ opacity: 0, x: direction * -60 }}         
            transition={{ duration: 0.7, ease: 'easeInOut' }}
            className="absolute inset-0"                      
          >
            {}
            <img
              src={HERO_SLIDES[slide].image}
              alt={HERO_SLIDES[slide].title}
              style={{ objectPosition: HERO_SLIDES[slide].position || 'center' }}
              className="h-full w-full object-cover"
            />
            {}
            <div className="absolute inset-0 flex items-center justify-center bg-black/35">
              <div className="text-center text-white px-6">
                {}
                <motion.h2
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.2, duration: 0.6 }}
                  className="font-serif text-5xl md:text-7xl font-light tracking-wider"
                >
                  {HERO_SLIDES[slide].title}
                </motion.h2>
                {}
                <motion.p
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.35, duration: 0.6 }}
                  className="mt-4 text-lg md:text-xl font-light"
                >
                  {HERO_SLIDES[slide].subtitle}
                </motion.p>
                {}
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.5, duration: 0.6 }}
                >
                  <Link
                    to={HERO_SLIDES[slide].link}
                    className="mt-8 inline-flex items-center gap-2 border-b border-white pb-2 text-sm uppercase tracking-widest hover:gap-4 transition-all"
                  >
                    {HERO_SLIDES[slide].cta} <ArrowRight size={16} />
                  </Link>
                </motion.div>
              </div>
            </div>
          </motion.div>
        </AnimatePresence>

        {}
        <button
          type="button"
          onClick={prevSlide}
          aria-label="Slide trước"
          className="absolute left-4 top-1/2 z-10 -translate-y-1/2 flex h-12 w-12 items-center justify-center rounded-full bg-white/20 backdrop-blur-sm text-white ring-1 ring-white/30 hover:bg-white hover:text-black transition-colors lg:left-8"
        >
          <ChevronLeft size={22} />
        </button>
        {}
        <button
          type="button"
          onClick={nextSlide}
          aria-label="Slide kế tiếp"
          className="absolute right-4 top-1/2 z-10 -translate-y-1/2 flex h-12 w-12 items-center justify-center rounded-full bg-white/20 backdrop-blur-sm text-white ring-1 ring-white/30 hover:bg-white hover:text-black transition-colors lg:right-8"
        >
          <ChevronRight size={22} />
        </button>

        {}
        <div className="absolute bottom-6 left-1/2 z-10 flex -translate-x-1/2 gap-2">
          {HERO_SLIDES.map((_, i) => (
            <button
              key={i}
              type="button"
              onClick={() => goSlide(i)}
              aria-label={`Chuyển đến slide ${i + 1}`}
              className={`h-2 rounded-full transition-all ${
                i === slide ? 'w-8 bg-white' : 'w-2 bg-white/50 hover:bg-white/80'
              }`}
            />
          ))}
        </div>
      </section>

      {/* ════════════════════════════════════════════════════════════
          RENDER: THANH TIỆN ÍCH / CAM KẾT
          ════════════════════════════════════════════════════════════ */}
      <section className="border-b border-gray-100 py-12">
        <div className="mx-auto grid max-w-7xl grid-cols-1 gap-8 px-4 sm:grid-cols-2 sm:px-6 lg:grid-cols-4 lg:px-8">
          {FEATURES.map((f) => (
            <div key={f.title} className="flex items-center gap-4">
              <f.icon size={28} strokeWidth={1.5} className="flex-shrink-0 text-gray-800" />
              <div>
                <h4 className="text-sm font-medium">{f.title}</h4>
                <p className="mt-0.5 text-xs text-gray-500">{f.desc}</p>
              </div>
            </div>
          ))}
        </div>
      </section>

      {/* ════════════════════════════════════════════════════════════
          RENDER: TILE DANH MỤC NAM / NỮ
          ════════════════════════════════════════════════════════════ */}
      <section className="py-16 md:py-24">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:gap-6">
            {CATEGORY_TILES.map((tile) => (
              <Link
                key={tile.label}
                to={tile.link}
                className="group relative block aspect-[4/5] overflow-hidden md:aspect-[4/3]"
              >
                <img
                  src={tile.image}
                  alt={tile.label}
                  style={{ objectPosition: tile.position || 'center' }}
                  className="h-full w-full object-cover transition-transform duration-700 group-hover:scale-105"
                />
                <div className="absolute inset-0 flex flex-col items-start justify-end bg-gradient-to-t from-black/55 via-black/10 to-transparent p-8 text-white">
                  <h3 className="font-serif text-3xl font-light tracking-wide md:text-4xl">{tile.label}</h3>
                  <p className="mt-1 text-sm font-light text-white/90">{tile.desc}</p>
                  <span className="mt-4 inline-flex items-center gap-2 border-b border-white pb-1 text-xs uppercase tracking-widest transition-all group-hover:gap-4">
                    Mua ngay <ArrowRight size={14} />
                  </span>
                </div>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {}
      {}
      {}
      {/* ════════════════════════════════════════════════════════════
          RENDER: CAROUSEL HÀNG MỚI
          ════════════════════════════════════════════════════════════ */}
      <section className="py-16 md:py-24 bg-gray-50">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          {}
          <div className="flex items-end justify-between">
            <div>
              <h2 className="font-serif text-3xl font-light tracking-wide">Hàng Mới</h2>
            </div>
            <Link to="/shop?newOnly=true" className="text-sm font-medium hover:underline">
              Xem tất cả
            </Link>
          </div>

          {}
          <div className="relative mt-12">
            {}
            {newArrivals.length > 0 && (
              <>
                <button
                  type="button"
                  onClick={() => scrollCarousel(-1)}            
                  aria-label="Sản phẩm trước đó"
                  className="absolute left-0 top-1/2 z-10 -translate-x-2 -translate-y-1/2 flex h-11 w-11 items-center justify-center rounded-full bg-white shadow-md ring-1 ring-black/5 hover:bg-black hover:text-white transition-colors lg:-translate-x-5"
                >
                  <ChevronLeft size={20} />
                </button>
                <button
                  type="button"
                  onClick={() => scrollCarousel(1)}             
                  aria-label="Sản phẩm tiếp theo"
                  className="absolute right-0 top-1/2 z-10 translate-x-2 -translate-y-1/2 flex h-11 w-11 items-center justify-center rounded-full bg-white shadow-md ring-1 ring-black/5 hover:bg-black hover:text-white transition-colors lg:translate-x-5"
                >
                  <ChevronRight size={20} />
                </button>
              </>
            )}

            {}
            <div
              ref={carouselRef}
              className="flex gap-4 overflow-x-auto scroll-smooth snap-x snap-mandatory pb-2 -mx-4 px-4 lg:gap-6 [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
            >
              {}
              {newArrivals.length === 0
                ? [...Array(6)].map((_, i) => (
                    <div
                      key={i}
                      className="flex-shrink-0 w-[60%] sm:w-[40%] md:w-[30%] lg:w-[23%] animate-pulse snap-start"
                    >
                      <div className="aspect-[3/4] bg-gray-200"></div>
                      <div className="mt-4 h-4 w-3/4 bg-gray-200"></div>
                      <div className="mt-2 h-4 w-1/2 bg-gray-200"></div>
                    </div>
                  ))
                : 
                  newArrivals.map((product) => (
                    <div
                      key={product.id}
                      className="flex-shrink-0 w-[60%] sm:w-[40%] md:w-[30%] lg:w-[23%] snap-start"
                    >
                      <ProductCard product={product} />
                    </div>
                  ))}
            </div>
          </div>
        </div>
      </section>

      {/* ════════════════════════════════════════════════════════════
          RENDER: BANNER EDITORIAL
          ════════════════════════════════════════════════════════════ */}
      <section className="grid grid-cols-1 md:grid-cols-2">
        <div className="relative min-h-[360px] overflow-hidden md:min-h-[520px]">
          <img
            src={EDITORIAL.image}
            alt={EDITORIAL.title}
            className="h-full w-full object-cover"
          />
        </div>
        <div className="flex flex-col items-start justify-center bg-gray-900 px-8 py-16 text-white md:px-16">
          <p className="text-xs uppercase tracking-[0.3em] text-white/60">{EDITORIAL.eyebrow}</p>
          <h2 className="mt-4 font-serif text-4xl font-light tracking-wide md:text-5xl">{EDITORIAL.title}</h2>
          <p className="mt-4 max-w-md text-sm font-light leading-relaxed text-white/80">{EDITORIAL.desc}</p>
          <Link
            to={EDITORIAL.link}
            className="mt-8 inline-flex items-center gap-2 bg-white px-8 py-3 text-xs font-medium uppercase tracking-widest text-black transition-colors hover:bg-gray-200"
          >
            {EDITORIAL.cta} <ArrowRight size={14} />
          </Link>
        </div>
      </section>

      {}
      {}
      {}
      {/* ════════════════════════════════════════════════════════════
          RENDER: LƯỚI SẢN PHẨM BÁN CHẠY
          ════════════════════════════════════════════════════════════ */}
      <section className="py-16 md:py-24">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <h2 className="font-serif text-3xl font-light tracking-wide">
              Bán Chạy Nhất
            </h2>
            {}
            <p className="mt-2 text-sm text-gray-500">
              Top {Math.min(bestSellers.length || 30, 30)} sản phẩm bán chạy nhất
            </p>
          </div>
          {}
          <div className="mt-12 grid grid-cols-2 gap-x-4 gap-y-8 md:grid-cols-3 lg:grid-cols-4 lg:gap-x-6 xl:grid-cols-5">
            {bestSellers.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
            {}
            {bestSellers.length === 0 &&
              [...Array(10)].map((_, i) => (
                <div key={i} className="animate-pulse">
                  <div className="aspect-[3/4] bg-gray-200"></div>
                  <div className="mt-4 h-4 w-3/4 bg-gray-200"></div>
                  <div className="mt-2 h-4 w-1/2 bg-gray-200"></div>
                </div>
              ))}
          </div>
        </div>
      </section>

      {/* ════════════════════════════════════════════════════════════
          RENDER: LƯỚI DANH MỤC NHANH
          ════════════════════════════════════════════════════════════ */}
      <section className="py-16 md:py-24 bg-gray-50">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <h2 className="font-serif text-3xl font-light tracking-wide">Mua sắm theo danh mục</h2>
          </div>
          <div className="mt-12 grid grid-cols-2 gap-4 lg:grid-cols-4 lg:gap-6">
            {QUICK_CATEGORIES.map((cat) => (
              <Link
                key={cat.label}
                to={cat.link}
                className="group relative block aspect-[3/4] overflow-hidden"
              >
                <img
                  src={cat.image}
                  alt={cat.label}
                  className="h-full w-full object-cover transition-transform duration-700 group-hover:scale-105"
                />
                <div className="absolute inset-0 flex items-end justify-center bg-gradient-to-t from-black/50 to-transparent p-4">
                  <span className="font-serif text-xl font-light tracking-wide text-white">{cat.label}</span>
                </div>
              </Link>
            ))}
          </div>
        </div>
      </section>

    </div>
  );
};

export default HomePage;
