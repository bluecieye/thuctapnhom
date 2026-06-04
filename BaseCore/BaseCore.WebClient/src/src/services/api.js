

import axios from 'axios';

const API_BASE_URL = '/api';

// ════════════════════════════════════════════════════════════
// INSTANCE AXIOS & INTERCEPTOR
// ════════════════════════════════════════════════════════════

// ════════════════════════════════════════════════════════════
// KHỞI TẠO INSTANCE
// ════════════════════════════════════════════════════════════
const api = axios.create({
    
    baseURL: API_BASE_URL,

    
    headers: { 'Content-Type': 'application/json' },

    
    timeout: 10000,

    paramsSerializer: {

        indexes: null,
    },
});

// ════════════════════════════════════════════════════════════
// INTERCEPTOR REQUEST
// ════════════════════════════════════════════════════════════
api.interceptors.request.use(
    (config) => {

        const token = localStorage.getItem('token');

        
        if (token) config.headers.Authorization = `Bearer ${token}`;

        return config;
    },
    
    (error) => Promise.reject(error)
);

// ════════════════════════════════════════════════════════════
// INTERCEPTOR RESPONSE
// ════════════════════════════════════════════════════════════
api.interceptors.response.use(
    
    (response) => response,

    (error) => {

        if (error.response?.status === 401) {
            
            localStorage.removeItem('token');
            localStorage.removeItem('user');

            
            if (!window.location.pathname.startsWith('/login')) {

                
                window.location.href = '/login';
            }
        }

        return Promise.reject(error);
    }
);

// ════════════════════════════════════════════════════════════
// AUTH API
// ════════════════════════════════════════════════════════════
export const authApi = {

    login: (username, password) => api.post('/auth/login', { username, password }),

    
    register: (data) => api.post('/auth/register', data),
};

// ════════════════════════════════════════════════════════════
// USER API
// ════════════════════════════════════════════════════════════
export const userApi = {

    getAll: (params) => api.get('/users', { params }),

    getById: (id) => api.get(`/users/${id}`),

    create: (data) => api.post('/users', data),

    update: (id, data) => api.put(`/users/${id}`, data),

    delete: (id) => api.delete(`/users/${id}`),
};

// ════════════════════════════════════════════════════════════
// PRODUCT API
// ════════════════════════════════════════════════════════════
export const productApi = {
    
    getAll:        (params) => api.get('/products', { params }),

    search:        (params) => api.get('/products', { params }),

    getById:       (id) => api.get(`/products/${id}`),

    
    newArrivals:   (limit = 12) => api.get('/products/new-arrivals', { params: { limit } }),

    bestSellers:   (limit = 30) => api.get('/products/best-sellers', { params: { limit } }),

    create:        (data) => api.post('/products', data),
    update:        (id, data) => api.put(`/products/${id}`, data),
    delete:        (id) => api.delete(`/products/${id}`),
};

// ════════════════════════════════════════════════════════════
// CATEGORY API
// ════════════════════════════════════════════════════════════
export const categoryApi = {
    getAll:  () => api.get('/categories'),
    getById: (id) => api.get(`/categories/${id}`),
    create:  (data) => api.post('/categories', data),
    update:  (id, data) => api.put(`/categories/${id}`, data),
    delete:  (id) => api.delete(`/categories/${id}`),
};

// ════════════════════════════════════════════════════════════
// VARIANT API
// ════════════════════════════════════════════════════════════
export const variantApi = {
    getById:      (id) => api.get(`/variants/${id}`),

    getBySku:     (sku) => api.get(`/variants/sku/${sku}`),

    
    getByProduct: (productId) => api.get(`/variants/product/${productId}`),

    create:       (data) => api.post('/variants', data),
    update:       (id, data) => api.put(`/variants/${id}`, data),

    
    adjustStock:  (id, delta) => api.put(`/variants/${id}/stock`, { delta }),

    delete:       (id) => api.delete(`/variants/${id}`),
};

// ════════════════════════════════════════════════════════════
// INVENTORY API
// ════════════════════════════════════════════════════════════
export const inventoryApi = {
    getAll:   (params) => api.get('/inventory', { params }),

    
    lowStock: (threshold = 5) => api.get('/inventory/low-stock', { params: { threshold } }),
};

// ════════════════════════════════════════════════════════════
// CART API
// ════════════════════════════════════════════════════════════
export const cartApi = {
    
    get:        () => api.get('/cart'),

    
    addItem:    (variantId, quantity = 1) => api.post('/cart/items', { variantId, quantity }),

    updateItem: (itemId, quantity) => api.put(`/cart/items/${itemId}`, { quantity }),

    removeItem: (itemId) => api.delete(`/cart/items/${itemId}`),

    clear:      () => api.delete('/cart'),
};

// ════════════════════════════════════════════════════════════
// COUPON API
// ════════════════════════════════════════════════════════════
export const couponApi = {
    // Admin lấy tất cả coupon (cả hết hạn / inactive) để quản lý.
    getAll:    () => api.get('/coupons'),

    active:    () => api.get('/coupons/active'),

    getByCode: (code) => api.get(`/coupons/${code}`),

    create:    (data) => api.post('/coupons', data),
    update:    (id, data) => api.put(`/coupons/${id}`, data),
    delete:    (id) => api.delete(`/coupons/${id}`),
};

// ════════════════════════════════════════════════════════════
// ORDER API
// ════════════════════════════════════════════════════════════
export const orderApi = {
    
    create:       (data) => api.post('/orders', data),

    getMyOrders:  () => api.get('/orders/me'),

    getById:      (id) => api.get(`/orders/${id}`),

    cancel:       (id) => api.put(`/orders/${id}/cancel`),

    
    applyCoupon:  (code, subtotal) => api.post('/orders/apply-coupon', { code, subtotal }),

    
    getAllAdmin:  (params) => api.get('/orders/admin', { params }),

    
    updateStatus: (id, status, note) => api.put(`/orders/${id}/status`, { status, note }),
};

// ════════════════════════════════════════════════════════════
// REVIEW API
// ════════════════════════════════════════════════════════════
export const reviewApi = {
    
    getByProduct: (productId) => api.get(`/reviews/product/${productId}`),

    getAllAdmin:   (params) => api.get('/reviews/admin', { params }),

    create:       (data) => api.post('/reviews', data),

    delete:       (id) => api.delete(`/reviews/${id}`),
};

// ════════════════════════════════════════════════════════════
// ADDRESS API
// ════════════════════════════════════════════════════════════
export const addressApi = {
    
    getMine:    () => api.get('/addresses'),

    create:     (data) => api.post('/addresses', data),
    update:     (id, data) => api.put(`/addresses/${id}`, data),

    
    setDefault: (id) => api.put(`/addresses/${id}/default`),

    delete:     (id) => api.delete(`/addresses/${id}`),
};

// ════════════════════════════════════════════════════════════
// WISHLIST API
// ════════════════════════════════════════════════════════════
export const wishlistApi = {
    getMine:  () => api.get('/wishlist'),

    add:      (productId) => api.post(`/wishlist/${productId}`),

    remove:   (productId) => api.delete(`/wishlist/${productId}`),
};

// ════════════════════════════════════════════════════════════
// LOOKUPS API
// ════════════════════════════════════════════════════════════
export const lookupsApi = {
    colors:    () => api.get('/lookups/colors'),
    sizes:     () => api.get('/lookups/sizes'),
    sizeGuide: () => api.get('/lookups/size-guide'),  
    banners:   () => api.get('/lookups/banners'),     
};

// ════════════════════════════════════════════════════════════
// PRODUCT IMAGES API
// ════════════════════════════════════════════════════════════
export const productImagesApi = {
    getAll:       (params) => api.get('/product-images', { params }),
    getByProduct: (productId) => api.get(`/product-images/product/${productId}`),
    create:       (data) => api.post('/product-images', data),
    update:       (id, data) => api.put(`/product-images/${id}`, data),
    delete:       (id) => api.delete(`/product-images/${id}`),

    

    upload:       (formData) => api.post('/product-images/upload', formData, {
                       headers: { 'Content-Type': 'multipart/form-data' },
                   }),

    replaceFile:  (id, formData) => api.put(`/product-images/${id}/upload`, formData, {
                       headers: { 'Content-Type': 'multipart/form-data' },
                   }),

    
    setPrimary:   (id) => api.put(`/product-images/${id}/set-primary`),
};

// ════════════════════════════════════════════════════════════
// SIZE GUIDES API
// ════════════════════════════════════════════════════════════
export const sizeGuidesApi = {
    getAll:  () => api.get('/size-guides'),
    create:  (data) => api.post('/size-guides', data),
    update:  (id, data) => api.put(`/size-guides/${id}`, data),
    delete:  (id) => api.delete(`/size-guides/${id}`),
};

// ════════════════════════════════════════════════════════════
// STATS API
// ════════════════════════════════════════════════════════════
export const statsApi = {
    getStats: () => api.get('/stats'),
};

// ════════════════════════════════════════════════════════════
// PROVINCES API
// ════════════════════════════════════════════════════════════
export const provincesApi = {
    getAll:  () => api.get('/provinces'),
    getById: (id) => api.get(`/provinces/${id}`),
};

// ════════════════════════════════════════════════════════════
// SHIPPING CARRIERS API
// ════════════════════════════════════════════════════════════
export const shippingCarriersApi = {

    getAll: (activeOnly) => api.get('/shipping-carriers', { params: activeOnly ? { activeOnly: true } : {} }),
    getById: (id) => api.get(`/shipping-carriers/${id}`),
    create: (data) => api.post('/shipping-carriers', data),
    update: (id, data) => api.put(`/shipping-carriers/${id}`, data),
    delete: (id) => api.delete(`/shipping-carriers/${id}`),
};

// ════════════════════════════════════════════════════════════
// SHIPPING RATES API
// ════════════════════════════════════════════════════════════
export const shippingRatesApi = {
    getAll: (params) => api.get('/shipping-rates', { params }),
    getById: (id) => api.get(`/shipping-rates/${id}`),
    create: (data) => api.post('/shipping-rates', data),
    update: (id, data) => api.put(`/shipping-rates/${id}`, data),
    delete: (id) => api.delete(`/shipping-rates/${id}`),

    
    quote:  (data) => api.post('/shipping-rates/quote', data),
};

// ════════════════════════════════════════════════════════════
// EXPORT
// ════════════════════════════════════════════════════════════
export default api;
