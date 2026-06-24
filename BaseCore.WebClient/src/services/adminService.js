

import {
  productApi,
  categoryApi,
  variantApi,
  inventoryApi,
  couponApi,
  userApi,
  orderApi,
  reviewApi,
  statsApi,
  productImagesApi,
  sizeGuidesApi,
} from './api';

const data = (p) => p.then((r) => r.data);

// ════════════════════════════════════════════════════════════
// SERVICE API ADMIN
// ════════════════════════════════════════════════════════════
export const adminService = {

  // ════════════════════════════════════════════════════════════
  // API DASHBOARD/STATS
  // ════════════════════════════════════════════════════════════
  getStats:           () => data(statsApi.getStats()),
  getVariantTopSales: (limit) => data(statsApi.getVariantTopSales(limit)),

  // ════════════════════════════════════════════════════════════
  // API SẢN PHẨM
  // ════════════════════════════════════════════════════════════
  getProducts:    (params) => data(productApi.search(params)),

  getProductById: (id)     => data(productApi.getById(id)),
  createProduct:  (b)      => data(productApi.create(b)),
  updateProduct:  (id, b)  => data(productApi.update(id, b)),
  deleteProduct:  (id)     => data(productApi.delete(id)),

  // ════════════════════════════════════════════════════════════
  // API DANH MỤC
  // ════════════════════════════════════════════════════════════
  getCategories:  () => data(categoryApi.getAll()),
  createCategory: (b) => data(categoryApi.create(b)),
  updateCategory: (id, b) => data(categoryApi.update(id, b)),
  deleteCategory: (id) => data(categoryApi.delete(id)),

  // ════════════════════════════════════════════════════════════
  // API BIẾN THỂ
  // ════════════════════════════════════════════════════════════
  getVariantsByProduct: (productId) => data(variantApi.getByProduct(productId)),
  createVariant:        (b) => data(variantApi.create(b)),
  updateVariant:        (id, b) => data(variantApi.update(id, b)),

  
  
  adjustStock:          (id, delta) => data(variantApi.adjustStock(id, delta)),

  deleteVariant:        (id) => data(variantApi.delete(id)),

  // ════════════════════════════════════════════════════════════
  // API TỒN KHO
  // ════════════════════════════════════════════════════════════
  getInventory: (params) => data(inventoryApi.getAll(params)),

  
  getLowStock:  (threshold = 5) => data(inventoryApi.lowStock(threshold)),

  // ════════════════════════════════════════════════════════════
  // API MÃ GIẢM GIÁ
  // ════════════════════════════════════════════════════════════
  getCoupons:    () => data(couponApi.getAll()),
  createCoupon:  (b) => data(couponApi.create(b)),
  updateCoupon:  (id, b) => data(couponApi.update(id, b)),
  deleteCoupon:  (id) => data(couponApi.delete(id)),

  // ════════════════════════════════════════════════════════════
  // API NGƯỜI DÙNG
  // ════════════════════════════════════════════════════════════
  getUsers:    (params) => data(userApi.getAll(params)),
  getUserById: (id) => data(userApi.getById(id)),
  createUser:  (b) => data(userApi.create(b)),
  updateUser:  (id, b) => data(userApi.update(id, b)),
  deleteUser:  (id) => data(userApi.delete(id)),

  // ════════════════════════════════════════════════════════════
  // API ĐƠN HÀNG
  // ════════════════════════════════════════════════════════════
  getAllOrders:      (params) => data(orderApi.getAllAdmin(params)),
  getOrderById:      (id) => data(orderApi.getById(id)),

  
  updateOrderStatus: (id, status, note, trackingNumber) =>
      data(orderApi.updateStatus(id, status, note, trackingNumber)),

  // ════════════════════════════════════════════════════════════
  // API ĐÁNH GIÁ
  // ════════════════════════════════════════════════════════════
  getAllReviews: (page = 1, pageSize = 20) => data(reviewApi.getAllAdmin({ page, pageSize })),
  deleteReview:  (id) => data(reviewApi.delete(id)),

  // ════════════════════════════════════════════════════════════
  // API HÌNH ẢNH
  // ════════════════════════════════════════════════════════════
  getAllImages:        (params) => data(productImagesApi.getAll(params)),
  getImagesByProduct: (productId) => data(productImagesApi.getByProduct(productId)),
  createImage:        (b) => data(productImagesApi.create(b)),
  updateImage:        (id, b) => data(productImagesApi.update(id, b)),
  deleteImage:        (id) => data(productImagesApi.delete(id)),

  
  uploadImage:        (formData) => data(productImagesApi.upload(formData)),
  replaceImageFile:   (id, formData) => data(productImagesApi.replaceFile(id, formData)),

  setImagePrimary:    (id) => data(productImagesApi.setPrimary(id)),

  // ════════════════════════════════════════════════════════════
  // API BẢNG SIZE
  // ════════════════════════════════════════════════════════════
  getAllSizeGuides: () => data(sizeGuidesApi.getAll()),
  createSizeGuide:  (b) => data(sizeGuidesApi.create(b)),
  updateSizeGuide:  (id, b) => data(sizeGuidesApi.update(id, b)),
  deleteSizeGuide:  (id) => data(sizeGuidesApi.delete(id)),
};
