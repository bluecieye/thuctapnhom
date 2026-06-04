

import { orderApi } from './api';

// ════════════════════════════════════════════════════════════
// SERVICE API ĐƠN HÀNG
// ════════════════════════════════════════════════════════════
export const orderService = {

  

  

  

  
  // ════════════════════════════════════════════════════════════
  // CHECKOUT
  // ════════════════════════════════════════════════════════════
  createOrder: async ({ addressId, shippingCarrierId, couponCode, paymentMethod = 'COD', note, items }) => {

    const response = await orderApi.create({
      addressId,           
      shippingCarrierId,   
      couponCode,          
      paymentMethod,       
      note,                
      items,               
    });

    
    return response.data;
  },

  

  
  
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
