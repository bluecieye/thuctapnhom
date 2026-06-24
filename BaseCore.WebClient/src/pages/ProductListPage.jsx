

import { useState, useEffect } from 'react';

import { useSearchParams, Link } from 'react-router-dom';

import { SlidersHorizontal } from 'lucide-react';

import { ProductCard } from '../components/product/ProductCard';

import { FilterSidebar } from '../components/filter/FilterSidebar';

import { productService } from '../services/productService';

// ════════════════════════════════════════════════════════════
// TRANG DANH SÁCH SẢN PHẨM (SHOP)
// ════════════════════════════════════════════════════════════
const ProductListPage = () => {
  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [searchParams, setSearchParams] = useSearchParams();

  const [products, setProducts] = useState([]);

  const [totalCount, setTotalCount] = useState(0);

  const [totalPages, setTotalPages] = useState(0);

  const [priceMax, setPriceMax] = useState(0);

  const [loading, setLoading] = useState(true);

  const [filterOpen, setFilterOpen] = useState(false);


  const page = parseInt(searchParams.get('page') || '1');

  // ════════════════════════════════════════════════════════════
  // EFFECT: TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    const fetchProducts = async () => {
      setLoading(true);                    
      try {
        
        const data = await productService.getProducts({
          keyword:     searchParams.get('keyword')     || '',
          categoryId:  searchParams.get('categoryId')  || '', 
          categoryIds: searchParams.get('categoryIds') || '', 
          gender:      searchParams.get('gender')      || '', 
          sizeId:      searchParams.get('sizeId')      || '',
          colorId:     searchParams.get('colorId')     || '',
          minPrice:    searchParams.get('minPrice')    || '',
          maxPrice:    searchParams.get('maxPrice')    || '',
          sortBy:      searchParams.get('sortBy')      || '',
          newOnly:     searchParams.get('newOnly') === 'true', 
          page,
          pageSize: 12,                                          
        });

        setProducts(data.items || []);
        setTotalCount(data.totalCount || 0);
        setTotalPages(data.totalPages || 0);
        setPriceMax(data.priceMax || 0);
      } catch (err) {
        
        console.error(err);
        setProducts([]);
        setTotalCount(0);
        setPriceMax(0);
      } finally {
        
        setLoading(false);
      }
    };
    fetchProducts();
  }, [searchParams, page]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const updateFilters = (newParams) => {
    setSearchParams((prev) => {
      const updated = new URLSearchParams(prev);          
      Object.entries(newParams).forEach(([key, value]) => {
        if (value === null || value === '' || value === undefined) updated.delete(key);
        else updated.set(key, value);
      });
      return updated;
    });
  };

  
  const goToPage = (n) => {
    updateFilters({ page: n > 1 ? n : null });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  // Dãy số trang rút gọn dạng: 1 … 4 [5] 6 … 20 (tránh tràn khi nhiều trang).
  const getPageNumbers = (current, total) => {
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    const pages = new Set([1, total, current, current - 1, current + 1]);
    const sorted = [...pages].filter((n) => n >= 1 && n <= total).sort((a, b) => a - b);
    const result = [];
    let prev = 0;
    for (const n of sorted) {
      if (n - prev > 1) result.push('…');
      result.push(n);
      prev = n;
    }
    return result;
  };

  // ════════════════════════════════════════════════════════════
  // TÍNH TOÁN
  // ════════════════════════════════════════════════════════════
  const CATEGORY_LABELS = {
    '1':  'Áo khoác Nam',
    '2':  'Áo Nam',
    '3':  'Quần dài Nam',
    '4':  'Quần short Nam',
    '5':  'Áo Nữ',
    '6':  'Quần dài Nữ',
    '7':  'Quần short Nữ',
    '8':  'Áo khoác Nữ',
    '9':  'Váy Nữ',
    '10': 'Chân váy Nữ',
  };
  
  const GROUPED_LABELS = {
    '6,7': 'Quần Nữ',
  };
  
  const GENDER_LABELS = { Male: 'Nam', Female: 'Nữ' };

  const keyword     = searchParams.get('keyword');
  const categoryId  = searchParams.get('categoryId');
  const categoryIds = searchParams.get('categoryIds');
  const gender      = searchParams.get('gender');

  

  

  

  
  const collectionLabel = keyword
    ? `Tìm kiếm: ${keyword}`
    : searchParams.get('newOnly') === 'true'
      ? gender ? `Sản phẩm mới ${GENDER_LABELS[gender] || ''}`.trim() : 'Hàng Mới'
      : searchParams.get('sale') === 'true'
        ? 'Khuyến Mãi'
        : categoryId && CATEGORY_LABELS[categoryId]
          ? CATEGORY_LABELS[categoryId]
          : categoryIds && GROUPED_LABELS[categoryIds]
            ? GROUPED_LABELS[categoryIds]
            : gender
              ? GENDER_LABELS[gender] || gender
              : 'Tất Cả Sản Phẩm';

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
            <li><Link to="/" className="hover:text-black">Trang Chủ</Link></li>
            <li>/</li>
            <li className="text-black">{collectionLabel}</li>
          </ol>
        </nav>

        {}
        {/* ════════════════════════════════════════════════════════════
            RENDER: FILTER & SORT BAR
            ════════════════════════════════════════════════════════════ */}
        <div className="flex items-center justify-between border-b border-gray-100 pb-6">
          <h1 className="font-serif text-3xl font-light">{collectionLabel}</h1>
          <button
            type="button"
            onClick={() => setFilterOpen(true)}                  
            className="flex items-center gap-2 border border-gray-200 px-4 py-2 text-sm hover:border-black"
          >
            <SlidersHorizontal size={16} />
            Bộ lọc
          </button>
        </div>

        <div className="py-8">
          {}
          {loading ? (
            <div className="grid grid-cols-2 gap-x-4 gap-y-8 md:grid-cols-3 lg:grid-cols-4 lg:gap-x-6">
              {[...Array(8)].map((_, i) => (
                <div key={i} className="animate-pulse">
                  <div className="aspect-[3/4] bg-gray-200"></div>
                  <div className="mt-4 h-4 w-3/4 bg-gray-200"></div>
                  <div className="mt-2 h-4 w-1/2 bg-gray-200"></div>
                </div>
              ))}
            </div>
          ) : products.length === 0 ? (
            <div className="py-20 text-center">
              <p className="text-gray-500">Không tìm thấy sản phẩm.</p>
            </div>
          ) : (
            <>
              {}
              {/* ════════════════════════════════════════════════════════════
                  RENDER: LƯỚI SẢN PHẨM
                  ════════════════════════════════════════════════════════════ */}
              <div className="grid grid-cols-2 gap-x-4 gap-y-8 md:grid-cols-3 lg:grid-cols-4 lg:gap-x-6">
                {products.map((product) => (
                  <ProductCard key={product.id} product={product} />
                ))}
              </div>

              {}
              {}
              {}
              {totalPages > 1 && (
                <div className="mt-12 flex items-center justify-center gap-2">
                  {}
                  <button
                    disabled={page === 1}
                    onClick={() => goToPage(page - 1)}
                    className="border border-gray-200 px-4 py-2 text-sm disabled:opacity-50"
                  >
                    Trước
                  </button>
                  {}
                  {getPageNumbers(page, totalPages).map((n, idx) =>
                    n === '…' ? (
                      <span key={`ellipsis-${idx}`} className="px-2 py-2 text-sm text-gray-400">…</span>
                    ) : (
                      <button
                        key={n}
                        onClick={() => goToPage(n)}
                        className={`border px-3 py-2 text-sm ${
                          n === page
                            ? 'border-black bg-black text-white'
                            : 'border-gray-200 hover:border-black'
                        }`}
                      >
                        {n}
                      </button>
                    )
                  )}
                  {}
                  <button
                    disabled={page === totalPages}
                    onClick={() => goToPage(page + 1)}
                    className="border border-gray-200 px-4 py-2 text-sm disabled:opacity-50"
                  >
                    Sau
                  </button>
                </div>
              )}
            </>
          )}
        </div>
      </div>

      {}
      {}
      {}
      {}
      <FilterSidebar
        isOpen={filterOpen}
        onClose={() => setFilterOpen(false)}
        updateFilters={updateFilters}
        searchParams={searchParams}
        totalCount={totalCount}
        priceMax={priceMax}
      />
    </div>
  );
};

export default ProductListPage;
