

import { useEffect, useMemo, useState } from 'react';

import { X, ChevronDown, ChevronRight } from 'lucide-react';

import { motion, AnimatePresence } from 'framer-motion';

import { productService } from '../../services/productService';

// ════════════════════════════════════════════════════════════
// CONSTANTS / SCHEMA
// ════════════════════════════════════════════════════════════
const PRICE_MIN = 0;
const PRICE_STEP = 50_000;            
const DEFAULT_PRICE_MAX = 5_000_000;  

const roundUpToStep = (n) => Math.ceil(n / PRICE_STEP) * PRICE_STEP;

const SORT_OPTIONS = [
  { value: 'newest',    label: 'Mới nhất' },
  { value: 'priceAsc',  label: 'Giá thấp đến cao' },
  { value: 'priceDesc', label: 'Giá cao đến thấp' },
];

const fmtPrice = (n) => `đ ${Number(n || 0).toLocaleString('vi-VN')}`;

// ════════════════════════════════════════════════════════════
// COMPONENT: SECTION
// ════════════════════════════════════════════════════════════
const Section = ({ title, expanded, onToggle, children }) => (
  <div className="border-b border-gray-100">
    {}
    <button
      type="button"
      onClick={onToggle}
      className="flex w-full items-center justify-between py-5 text-left"
    >
      <span className="text-sm font-semibold uppercase tracking-[0.15em]">{title}</span>
      {}
      {expanded ? <ChevronDown size={18} /> : <ChevronRight size={18} />}
    </button>

    {}
    {expanded && <div className="pb-5">{children}</div>}
  </div>
);

// ════════════════════════════════════════════════════════════
// COMPONENT: PRICE RANGE SLIDER
// ════════════════════════════════════════════════════════════
const PriceRangeSlider = ({ min, max, bound, onChange }) => {

  // ════════════════════════════════════════════════════════════
  // DERIVED VALUES / MEMOS
  // ════════════════════════════════════════════════════════════
  const pct = (v) => bound === PRICE_MIN ? 0 : ((v - PRICE_MIN) / (bound - PRICE_MIN)) * 100;

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div>
      {}
      <div className="mb-4 flex items-center justify-between text-sm">
        <span className="underline">{fmtPrice(min)}</span>
        <span className="underline">{fmtPrice(max)}</span>
      </div>

      {}
      <div className="relative h-1 w-full bg-gray-200">
        {}
        <div
          className="absolute h-1 bg-black"
          style={{ left: `${pct(min)}%`, right: `${100 - pct(max)}%` }}
        />

        {}
        <input
          type="range"
          min={PRICE_MIN}
          max={bound}
          step={PRICE_STEP}
          value={min}
          
          onChange={(e) => onChange({ min: Math.min(Number(e.target.value), max - PRICE_STEP), max })}

          className="range-thumb pointer-events-none absolute inset-0 h-1 w-full appearance-none bg-transparent"
        />

        {}
        <input
          type="range"
          min={PRICE_MIN}
          max={bound}
          step={PRICE_STEP}
          value={max}
          
          onChange={(e) => onChange({ min, max: Math.max(Number(e.target.value), min + PRICE_STEP) })}
          className="range-thumb pointer-events-none absolute inset-0 h-1 w-full appearance-none bg-transparent"
        />
      </div>

      {}
      <style>{`
        .range-thumb::-webkit-slider-thumb {
          -webkit-appearance: none;
          appearance: none;
          pointer-events: auto;
          width: 14px;
          height: 14px;
          background: black;
          border: 0;
          border-radius: 0;
          cursor: pointer;
        }
        .range-thumb::-moz-range-thumb {
          pointer-events: auto;
          width: 14px;
          height: 14px;
          background: black;
          border: 0;
          border-radius: 0;
          cursor: pointer;
        }
      `}</style>
    </div>
  );
};

// ════════════════════════════════════════════════════════════
// COMPONENT SIDEBAR BỘ LỌC
// ════════════════════════════════════════════════════════════
export const FilterSidebar = ({ isOpen, onClose, updateFilters, searchParams, totalCount = 0, priceMax = 0 }) => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [colors, setColors] = useState([]);
  const [sizes, setSizes] = useState([]);

  // ════════════════════════════════════════════════════════════
  // EFFECTS
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    if (!isOpen) return;
    productService.getColors().then(setColors).catch(() => setColors([]));
    productService.getSizes().then(setSizes).catch(() => setSizes([]));
  }, [isOpen]);

  // Khóa cuộn trang nền khi sidebar mở (tránh cuộn lan ra ngoài).
  useEffect(() => {
    if (!isOpen) return;
    const prev = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    return () => { document.body.style.overflow = prev; };
  }, [isOpen]);



  // ════════════════════════════════════════════════════════════
  // DERIVED VALUES / MEMOS
  // ════════════════════════════════════════════════════════════
  const priceBound = useMemo(
    () => (priceMax > 0 ? roundUpToStep(priceMax) : DEFAULT_PRICE_MAX),
    [priceMax]
  );

  // Bộ lọc dạng "nháp" (draft): thay đổi KHÔNG áp dụng ngay,
  // chỉ áp dụng khi bấm nút "Xem".
  const [price, setPrice] = useState({
    min: Number(searchParams.get('minPrice')) || PRICE_MIN,
    max: Number(searchParams.get('maxPrice')) || priceBound,
  });
  const [draftSort, setDraftSort]     = useState(searchParams.get('sortBy')  || '');
  const [draftColor, setDraftColor]   = useState(searchParams.get('colorId') || '');
  const [draftSize, setDraftSize]     = useState(searchParams.get('sizeId')  || '');
  const [draftGender, setDraftGender] = useState(searchParams.get('gender')  || '');

  // Số sản phẩm khớp bộ lọc nháp — cập nhật ngay khi đổi lựa chọn.
  const [previewCount, setPreviewCount] = useState(totalCount);

  const [expanded, setExpanded] = useState({ price: true, sort: false, gender: false, color: false, size: false });

  // Mỗi lần mở sidebar: đồng bộ draft theo bộ lọc đang áp dụng (URL).
  useEffect(() => {
    if (!isOpen) return;
    setPrice({
      min: Number(searchParams.get('minPrice')) || PRICE_MIN,
      max: Number(searchParams.get('maxPrice')) || priceBound,
    });
    setDraftSort(searchParams.get('sortBy')   || '');
    setDraftColor(searchParams.get('colorId') || '');
    setDraftSize(searchParams.get('sizeId')   || '');
    setDraftGender(searchParams.get('gender') || '');
    setPreviewCount(totalCount);
  }, [isOpen]);

  // Tính số kết quả khớp bộ lọc nháp (debounce) để hiện trong nút "Xem (X)".
  useEffect(() => {
    if (!isOpen) return;
    const t = setTimeout(async () => {
      try {
        const data = await productService.getProducts({
          keyword:     searchParams.get('keyword')     || '',
          categoryId:  searchParams.get('categoryId')  || '',
          categoryIds: searchParams.get('categoryIds') || '',
          gender:      draftGender || '',
          newOnly:     searchParams.get('newOnly') === 'true',
          minPrice:    price.min > PRICE_MIN ? price.min : '',
          maxPrice:    price.max < priceBound ? price.max : '',
          colorId:     draftColor || '',
          sizeId:      draftSize || '',
          page: 1,
          pageSize: 1,
        });
        setPreviewCount(data.totalCount || 0);
      } catch {
        /* lỗi mạng — giữ nguyên số cũ */
      }
    }, 300);
    return () => clearTimeout(t);

  }, [isOpen, price, draftColor, draftSize, draftGender, priceBound, searchParams]);

  const sortBy  = draftSort;
  const colorId = draftColor;
  const sizeId  = draftSize;
  const gender  = draftGender;

  const activeChips = useMemo(() => {
    const chips = [];
    if (price.min !== PRICE_MIN || price.max !== priceBound) chips.push({ key: 'price',  label: 'TẦM GIÁ' });
    if (sortBy)  chips.push({ key: 'sort',   label: 'SẮP XẾP' });
    if (gender)  chips.push({ key: 'gender', label: gender === 'Male' ? 'NAM' : 'NỮ' });
    if (colorId) chips.push({ key: 'color',  label: 'MÀU SẮC' });
    if (sizeId)  chips.push({ key: 'size',   label: 'KÍCH CỠ' });
    return chips;
  }, [price, sortBy, gender, colorId, sizeId, priceBound]);

  const toggleSection = (k) => setExpanded((e) => ({ ...e, [k]: !e[k] }));

  

  // ════════════════════════════════════════════════════════════
  // HANDLERS
  // ════════════════════════════════════════════════════════════
  const removeChip = (key) => {
    if (key === 'price')       setPrice({ min: PRICE_MIN, max: priceBound });
    else if (key === 'sort')   setDraftSort('');
    else if (key === 'gender') setDraftGender('');
    else if (key === 'color')  setDraftColor('');
    else if (key === 'size')   setDraftSize('');
  };

  
  
  const clearAll = () => {
    setPrice({ min: PRICE_MIN, max: priceBound });
    setDraftSort('');
    setDraftGender('');
    setDraftColor('');
    setDraftSize('');
  };

  

  
  // Áp dụng TOÀN BỘ bộ lọc nháp một lần khi bấm "Xem".
  const applyAndClose = () => {
    updateFilters({
      minPrice: price.min > PRICE_MIN ? price.min : null,
      maxPrice: price.max < priceBound ? price.max : null,
      sortBy:   draftSort   || null,
      gender:   draftGender || null,
      colorId:  draftColor  || null,
      sizeId:   draftSize   || null,
      page: null,
    });
    onClose();
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    <AnimatePresence>
      {isOpen && (
        <>
          {}
          <motion.div
            className="fixed inset-0 z-40 bg-black/40"
            
            initial={{ opacity: 0 }}
            
            animate={{ opacity: 1 }}
            
            exit={{ opacity: 0 }}
            transition={{ duration: 0.25 }}
            
            onClick={onClose}
          />

          {}
          <motion.div
            className="fixed right-0 top-0 bottom-0 z-50 flex w-full max-w-md flex-col bg-white shadow-2xl lg:w-1/3"
            
            initial={{ x: '100%' }}
            
            animate={{ x: 0 }}
            
            exit={{ x: '100%' }}
            
            transition={{ type: 'tween', duration: 0.3, ease: 'easeOut' }}
          >
            {}
            <div className="relative flex items-center justify-center border-b border-gray-100 px-4 py-5">
              <h2 className="text-sm font-semibold uppercase tracking-[0.2em]">Bộ Lọc</h2>
              {}
              <button
                type="button"
                onClick={onClose}
                aria-label="Đóng"
                className="absolute right-4 top-1/2 -translate-y-1/2"
              >
                <X size={22} />
              </button>
            </div>

            {}
            <div className="flex-1 overflow-y-auto overscroll-contain px-4 sm:px-6">
        {}
        {activeChips.length > 0 && (
          <div className="flex flex-wrap gap-2 py-4">
            {activeChips.map((c) => (
              <button
                key={c.key}
                onClick={() => removeChip(c.key)}
                className="flex items-center gap-2 border border-black px-3 py-1.5 text-xs uppercase tracking-wider"
              >
                {c.label} <X size={14} />
              </button>
            ))}
          </div>
        )}

        {}
        <Section title="Tầm giá" expanded={expanded.price} onToggle={() => toggleSection('price')}>
          <PriceRangeSlider min={price.min} max={price.max} bound={priceBound} onChange={setPrice} />
        </Section>

        {}
        <Section title="Sắp xếp theo" expanded={expanded.sort} onToggle={() => toggleSection('sort')}>
          <div className="space-y-2">
            {SORT_OPTIONS.map((opt) => {
              const active = sortBy === opt.value;
              return (
                <button
                  key={opt.value}
                  type="button"
                  
                  onClick={() => setDraftSort(active ? '' : opt.value)}
                  className={`block w-full border px-3 py-2 text-left text-sm ${
                    active ? 'border-black bg-black text-white' : 'border-gray-200 hover:border-black'
                  }`}
                >
                  {opt.label}
                </button>
              );
            })}
          </div>
        </Section>

        {}
        <Section title="Giới tính" expanded={expanded.gender} onToggle={() => toggleSection('gender')}>
          <div className="flex gap-2">
            {[{ value: 'Male', label: 'Nam' }, { value: 'Female', label: 'Nữ' }].map((opt) => {
              const active = gender === opt.value;
              return (
                <button
                  key={opt.value}
                  type="button"
                  onClick={() => setDraftGender(active ? '' : opt.value)}
                  className={`flex-1 border py-2 text-sm ${
                    active ? 'border-black bg-black text-white' : 'border-gray-200 hover:border-black'
                  }`}
                >
                  {opt.label}
                </button>
              );
            })}
          </div>
        </Section>

        {}
        <Section title="Màu sắc" expanded={expanded.color} onToggle={() => toggleSection('color')}>
          <div className="flex flex-wrap gap-2">
            {colors.map((c) => {
              
              const active = colorId === String(c.id);
              return (
                <button
                  key={c.id}
                  type="button"
                  onClick={() => setDraftColor(active ? '' : String(c.id))}
                  
                  title={c.name}
                  className={`flex items-center gap-2 border px-3 py-1.5 text-xs ${
                    active ? 'border-black' : 'border-gray-200 hover:border-black'
                  }`}
                >
                  {}
                  <span
                    className="inline-block h-3 w-3 rounded-full border"
                    style={{ backgroundColor: c.hexCode }}
                  />
                  {c.name}
                </button>
              );
            })}
          </div>
        </Section>

        {}
        <Section title="Kích cỡ" expanded={expanded.size} onToggle={() => toggleSection('size')}>
          <div className="flex flex-wrap gap-2">
            {sizes.map((s) => {
              const active = sizeId === String(s.id);
              return (
                <button
                  key={s.id}
                  type="button"
                  onClick={() => setDraftSize(active ? '' : String(s.id))}
                  
                  className={`min-w-[3rem] border px-3 py-1.5 text-sm ${
                    active ? 'border-black bg-black text-white' : 'border-gray-200 hover:border-black'
                  }`}
                >
                  {s.name}
                </button>
              );
            })}
          </div>
        </Section>
      </div>

      {}
      <div className="grid grid-cols-2 gap-0 border-t border-gray-100">
        <button
          type="button"
          onClick={clearAll}
          className="border-r border-gray-100 px-6 py-5 text-sm font-semibold uppercase tracking-wider"
        >
          Xoá
        </button>
        <button
          type="button"
          onClick={applyAndClose}
          className="bg-black px-6 py-5 text-sm font-semibold uppercase tracking-wider text-white"
        >
          {}
          Xem ({previewCount})
        </button>
      </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
};

export default FilterSidebar;
