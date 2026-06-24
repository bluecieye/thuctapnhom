

import { orderApi } from './api';

// ════════════════════════════════════════════════════════════
// SERVICE API ĐƠN HÀNG
// ════════════════════════════════════════════════════════════
export const orderService = {

  

  

  

  
  // ════════════════════════════════════════════════════════════
  // CHECKOUT
  // ════════════════════════════════════════════════════════════
  createOrder: async ({
    addressId, shippingCarrierId, couponCode, paymentMethod = 'COD', note, items,
    email, guestName, guestPhone, guestStreet, guestWard, guestProvinceId,
  }) => {

    const response = await orderApi.create({
      addressId,
      shippingCarrierId,
      couponCode,
      paymentMethod,
      note,
      items,
      // Các trường dưới đây chỉ dùng khi đặt hàng với tư cách khách vãng lai.
      email,
      guestName,
      guestPhone,
      guestStreet,
      guestWard,
      guestProvinceId,
    });


    return response.data;
  },



  // Tra cứu đơn (khách vãng lai) theo mã đơn + email/SĐT.
  trackOrder: async (code, contact) => (await orderApi.track(code, contact)).data,

  

  
  
  applyCoupon: async (code, subtotal) => {
    const response = await orderApi.applyCoupon(code, subtotal);
    return response.data;
  },

  // ════════════════════════════════════════════════════════════
  // ĐƠN HÀNG NGƯỜI DÙNG
  // ════════════════════════════════════════════════════════════
  // ════════════════════════════════════════════════════════════
  // ĐƠN HÀNG NGƯỜI DÙNG
  // ════════════════════════════════════════════════════════════
  getMyOrders: async () => (await orderApi.getMyOrders()).data,

  

  getOrderById: async (id) => (await orderApi.getById(id)).data,

  

  
  cancelOrder:  async (id) => (await orderApi.cancel(id)).data,

  // ════════════════════════════════════════════════════════════
  // ĐƠN HÀNG ADMIN
  // ════════════════════════════════════════════════════════════
  getAllOrders:      async (params = {}) => (await orderApi.getAllAdmin(params)).data,

  

  
  
  updateOrderStatus: async (id, status, note) => (await orderApi.updateStatus(id, status, note)).data,
};
