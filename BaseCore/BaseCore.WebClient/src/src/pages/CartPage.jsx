

import { Link } from 'react-router-dom';

import { Minus, Plus, ArrowRight } from 'lucide-react';

import { useCart } from '../contexts/CartContext';

import { useAuth } from '../contexts/AuthContext';

import { fmt, IMAGE_PLACEHOLDER } from '../utils/format';

// ════════════════════════════════════════════════════════════
// TRANG GIỎ HÀNG
// ════════════════════════════════════════════════════════════
const CartPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const { cartItems, removeFromCart, updateQuantity, totalPrice, loading } = useCart();


  const { isAuthenticated } = useAuth();

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  if (loading) {
    return <div className="mx-auto max-w-7xl px-4 py-20 text-center text-gray-500">Đang tải giỏ hàng...</div>;
  }

  
  
  if (cartItems.length === 0) {
    return (
      <div className="mx-auto max-w-7xl px-4 py-20 text-center">
        <h2 className="font-serif text-2xl">Giỏ hàng trống</h2>
        <p className="mt-2 text-gray-500">Hãy thêm sản phẩm để bắt đầu mua sắm</p>
        <Link to="/shop" className="mt-8 inline-block bg-black px-8 py-3 text-white">
          Tiếp tục mua sắm
        </Link>
      </div>
    );
  }

  
  
  return (
    <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
      <h1 className="font-serif text-3xl font-light">Giỏ hàng</h1>

      {}
      <div className="mt-10 lg:grid lg:grid-cols-12 lg:gap-12">
        {}
        {/* ════════════════════════════════════════════════════════════
            RENDER: DANH SÁCH SẢN PHẨM TRONG GIỎ
            ════════════════════════════════════════════════════════════ */}
        <div className="lg:col-span-8">
          {}
          <div className="hidden border-b border-gray-200 pb-2 text-sm font-medium md:grid md:grid-cols-12">
            <div className="col-span-6">Sản phẩm</div>
            <div className="col-span-2 text-center">Giá</div>
            <div className="col-span-2 text-center">Số lượng</div>
            <div className="col-span-2 text-right">Tổng</div>
          </div>

          {}
          <div className="divide-y divide-gray-200">
            {cartItems.map((item) => {

              const itemKey = item.cartItemId ?? item.variantId;
              return (
                <div key={itemKey} className="py-6 md:grid md:grid-cols-12 md:items-center">
                  {}
                  <div className="flex gap-4 md:col-span-6">
                    <img
                      src={item.imageUrl || IMAGE_PLACEHOLDER}
                      alt={item.productName}
                      className="h-24 w-20 object-cover bg-gray-100"
                    />
                    <div>
                      {}
                      <Link to={`/product/${item.productId}`} className="font-medium hover:underline">
                        {item.productName}
                      </Link>
                      {}
                      <p className="mt-1 text-xs text-gray-500">
                        SKU: {item.sku} {item.colorName && `· ${item.colorName}`} {item.sizeName && `· Size ${item.sizeName}`}
                      </p>
                      {}
                      <button
                        onClick={() => removeFromCart(itemKey)}
                        className="mt-2 block text-sm text-gray-400 hover:text-red-500"
                      >
                        Xoá
                      </button>
                    </div>
                  </div>

                  {}
                  <div className="mt-4 flex justify-between md:col-span-2 md:mt-0 md:block md:text-center">
                    <span className="md:hidden">Giá</span>
                    <span>{fmt(item.price)} đ</span>
                  </div>

                  {}
                  <div className="mt-4 flex justify-between md:col-span-2 md:mt-0 md:justify-center">
                    <span className="md:hidden">Số lượng</span>
                    <div className="flex items-center border border-gray-200">
                      {}
                      <button
                        onClick={() => updateQuantity(itemKey, Math.max(1, item.quantity - 1))}
                        className="p-2 hover:bg-gray-50"
                      >
                        <Minus size={14} />
                      </button>
                      <span className="w-8 text-center">{item.quantity}</span>
                      {}
                      <button
                        onClick={() => updateQuantity(itemKey, item.quantity + 1)}
                        className="p-2 hover:bg-gray-50"
                      >
                        <Plus size={14} />
                      </button>
                    </div>
                  </div>

                  {}
                  <div className="mt-4 flex justify-between md:col-span-2 md:mt-0 md:block md:text-right">
                    <span className="md:hidden">Tổng</span>
                    <span className="font-medium">{fmt(item.price * item.quantity)} đ</span>
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {}
        {}
        {/* ════════════════════════════════════════════════════════════
            RENDER: TÓM TẮT ĐƠN HÀNG
            ════════════════════════════════════════════════════════════ */}
        <div className="mt-10 lg:col-span-4 lg:mt-0">
          <div className="bg-gray-50 p-6">
            <h2 className="text-lg font-medium">Tóm tắt đơn hàng</h2>
            <div className="mt-6 space-y-4">
              <div className="flex justify-between">
                <span>Tạm tính</span>
                <span>{fmt(totalPrice)} đ</span>
              </div>
              {}
              <div className="flex justify-between">
                <span>Phí ship</span>
                <span className="text-gray-500">Tính ở bước thanh toán</span>
              </div>
              <div className="border-t border-gray-200 pt-4">
                <div className="flex justify-between font-medium">
                  <span>Tổng</span>
                  <span>{fmt(totalPrice)} đ</span>
                </div>
              </div>
            </div>

            {}
            {}
            {isAuthenticated ? (
              <Link to="/checkout" className="mt-6 flex w-full items-center justify-center gap-2 bg-black py-3 text-white hover:bg-gray-800">
                Thanh toán <ArrowRight size={16} />
              </Link>
            ) : (
              <Link to="/login" className="mt-6 flex w-full items-center justify-center gap-2 bg-black py-3 text-white hover:bg-gray-800">
                Đăng nhập để thanh toán
              </Link>
            )}
            <p className="mt-4 text-center text-xs text-gray-500">
              Free ship cho đơn hàng từ 500.000 đ
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CartPage;
