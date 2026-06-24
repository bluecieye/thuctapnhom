

import { productApi, categoryApi, lookupsApi } from './api';

import { slugify } from '../utils/slug';

// ════════════════════════════════════════════════════════════
// SERVICE API SẢN PHẨM
// ════════════════════════════════════════════════════════════
export const productService = {

  

  

  

  // ════════════════════════════════════════════════════════════
  // TÌM KIẾM & DANH SÁCH
  // ════════════════════════════════════════════════════════════
  getProducts: async (filters = {}) => {

    const params = {};

    
    if (filters.keyword) params.keyword = filters.keyword;

    if (filters.categoryId) params.categoryId = filters.categoryId;

    if (filters.categoryIds) {

      
      
      const ids = Array.isArray(filters.categoryIds)
        ? filters.categoryIds                                  
        : String(filters.categoryIds)                          
            .split(',')                                        
            .map((s) => s.trim())                              
            .filter(Boolean);                                  

      
      if (ids.length > 0) params.categoryIds = ids;
    }

    if (filters.gender) params.gender = filters.gender;

    if (filters.season) params.season = filters.season;

    
    
    if (filters.minPrice) params.minPrice = filters.minPrice;
    if (filters.maxPrice) params.maxPrice = filters.maxPrice;

    if (filters.sizeId) params.sizeId = filters.sizeId;

    if (filters.colorId) params.colorId = filters.colorId;

    
    if (filters.inStockOnly) params.inStockOnly = true;

    if (filters.newOnly) params.newOnly = true;

    if (filters.sortBy) params.sortBy = filters.sortBy;

    if (filters.page) params.page = filters.page;

    
    params.pageSize = filters.pageSize || 12;

    
    const response = await productApi.search(params);

    const data = response.data;

    

    if (Array.isArray(data)) return { items: data, totalCount: data.length };
    return data;
  },

  
  
  getProductById: async (id) => {
    const response = await productApi.getById(id);
    return response.data;
  },

  

  
  
  // ════════════════════════════════════════════════════════════
  // LOOKUP (DANH MỤC / MÀU / SIZE)
  // ════════════════════════════════════════════════════════════
  getCategories: async () => (await categoryApi.getAll()).data,
  getColors:     async () => (await lookupsApi.colors()).data,
  getSizes:      async () => (await lookupsApi.sizes()).data,
  getSizeGuide:  async () => (await lookupsApi.sizeGuide()).data,

  

  // ════════════════════════════════════════════════════════════
  // SẢN PHẨM NỔI BẬT
  // ════════════════════════════════════════════════════════════
  getNewArrivals: async (limit = 12) => (await productApi.newArrivals(limit)).data || [],

  
  
  getBestSellers: async (limit = 30) => (await productApi.bestSellers(limit)).data || [],

  

  
  
  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  imagePath: (slug, fileName) =>
    slug && fileName ? `/images/products/${slug}/${fileName}` : '',

  

  
  
  getPrimaryImage: (product) => {

    const images = product?.images || [];

    
    const primary = images.find((i) => i.isPrimary) || images[0];

    if (!primary || !product) return '';

    
    const slug = product.slug || slugify(product.name);
    if (!slug) return '';

    return `/images/products/${slug}/${primary.fileName}`;
  },

  

  
  getDiscountedPrice: (product) => {
    const disc = product?.discountPercent || 0;
    return (product?.basePrice ?? 0) * (1 - disc / 100);
  },

  getMinVariantPrice: (product) => {
    const variants = product?.variants || [];
    const disc = product?.discountPercent || 0;
    const baseP = product?.basePrice ?? 0;
    const factor = 1 - disc / 100;
    if (variants.length === 0) return baseP * factor;
    return Math.min(...variants.map((v) => v.price ?? baseP)) * factor;
  },

  getMaxVariantPrice: (product) => {
    const variants = product?.variants || [];
    const disc = product?.discountPercent || 0;
    const baseP = product?.basePrice ?? 0;
    const factor = 1 - disc / 100;
    if (variants.length === 0) return baseP * factor;
    return Math.max(...variants.map((v) => v.price ?? baseP)) * factor;
  },

  

  

  
  
  getTotalStock: (product) =>
    (product?.variants || []).reduce(
      (s, v) => s + Math.max(0, (v.stock || 0) - (v.reservedStock || 0)),
      0
    ),
};
