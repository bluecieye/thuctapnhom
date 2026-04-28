import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { orderApi } from '../services/api';
import { clearCart, getCartItems, removeCartItem, updateCartQuantity } from '../utils/cart';

const PROVINCES = [
  'An Giang','Bà Rịa - Vũng Tàu','Bắc Giang','Bắc Kạn','Bạc Liêu','Bắc Ninh','Bến Tre',
  'Bình Định','Bình Dương','Bình Phước','Bình Thuận','Cà Mau','Cao Bằng','Cần Thơ',
  'Đà Nẵng','Đắk Lắk','Đắk Nông','Điện Biên','Đồng Nai','Đồng Tháp','Gia Lai',
  'Hà Giang','Hà Nam','Hà Nội','Hà Tĩnh','Hải Dương','Hải Phòng','Hậu Giang',
  'Hòa Bình','Hưng Yên','Khánh Hòa','Kiên Giang','Kon Tum','Lai Châu','Lâm Đồng',
  'Lạng Sơn','Lào Cai','Long An','Nam Định','Nghệ An','Ninh Bình','Ninh Thuận',
  'Phú Thọ','Phú Yên','Quảng Bình','Quảng Nam','Quảng Ngãi','Quảng Ninh','Quảng Trị',
  'Sóc Trăng','Sơn La','Tây Ninh','Thái Bình','Thái Nguyên','Thanh Hóa',
  'Thừa Thiên Huế','Tiền Giang','TP. Hồ Chí Minh','Trà Vinh','Tuyên Quang',
  'Vĩnh Long','Vĩnh Phúc','Yên Bái',
];

const StoreCart = () => {
  const [cartItems, setCartItems] = useState([]);
  const [address, setAddress] = useState({ province: '', district: '', ward: '', street: '' });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    setCartItems(getCartItems());
  }, []);

  const handleQuantityChange = (id, quantity) => {
    if (quantity < 1) return;
    updateCartQuantity(id, quantity);
    setCartItems(getCartItems());
  };

  const handleRemove = (id) => {
    removeCartItem(id);
    setCartItems(getCartItems());
  };

  const buildShippingAddress = () => {
    return [address.street, address.ward, address.district, address.province]
      .filter(Boolean)
      .join(', ');
  };

  const handleCheckout = async () => {
    setError('');
    setSuccess('');

    if (cartItems.length === 0) {
      setError('Giỏ hàng đang trống.');
      return;
    }

    if (!isAuthenticated) {
      navigate('/login');
      return;
    }

    if (!address.province || !address.district || !address.street) {
      setError('Vui lòng điền đầy đủ tỉnh/thành phố, quận/huyện và địa chỉ cụ thể.');
      return;
    }

    setLoading(true);
    try {
      const shippingAddress = buildShippingAddress();
      const orderPayload = {
        items: cartItems.map((item) => ({ productId: item.id, quantity: item.quantity })),
        shippingAddress,
      };
      const response = await orderApi.create(orderPayload);
      const orderId = response?.data?.order?.id ?? response?.data?.id ?? '—';
      clearCart();
      setCartItems([]);
      setSuccess(`Đặt hàng thành công! Mã đơn hàng: #${orderId}`);
      setAddress({ province: '', district: '', ward: '', street: '' });
      setTimeout(() => navigate('/my-orders'), 2500);
    } catch (err) {
      setError(err.response?.data?.message || 'Đặt hàng thất bại. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  const totalAmount = cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
  const totalQty = cartItems.reduce((sum, item) => sum + item.quantity, 0);

  return (
    <div className="space-y-8">
      <div className="flex flex-col gap-4 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Cart</p>
          <h1 className="mt-2 text-3xl font-semibold text-slate-900">Review your order</h1>
          <p className="mt-1 text-sm text-slate-600">Update quantities and complete checkout securely.</p>
        </div>
        <button
          className="rounded-full border border-slate-200 bg-slate-50 px-5 py-3 text-sm font-semibold text-slate-900 transition hover:bg-slate-100"
          onClick={() => navigate('/shop')}
        >
          Continue shopping
        </button>
      </div>

      {(error || success) && (
        <div className={`rounded-3xl p-4 text-sm ring-1 ${error ? 'bg-rose-50 text-rose-700 ring-rose-200' : 'bg-emerald-50 text-emerald-700 ring-emerald-200'}`}>
          {error || success}
        </div>
      )}

      {cartItems.length === 0 && !success ? (
        <div className="rounded-[2rem] bg-white p-10 text-center shadow-soft ring-1 ring-slate-200">
          <p className="text-lg font-semibold text-slate-900">Your cart is empty</p>
          <p className="mt-2 text-sm text-slate-600">Add a few items to get started.</p>
          <button
            className="mt-6 rounded-full bg-slate-900 px-6 py-3 text-sm font-semibold text-white hover:bg-slate-800"
            onClick={() => navigate('/shop')}
          >
            Browse shop
          </button>
        </div>
      ) : (
        <div className="grid gap-6 lg:grid-cols-[1.4fr_0.6fr]">
          {/* Cart items */}
          <div className="space-y-4 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
            {cartItems.map((item) => (
              <div key={item.id} className="flex flex-col gap-4 rounded-[1.75rem] border border-slate-200 p-4 sm:flex-row sm:items-center">
                <img
                  src={item.imageUrl || 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=400&q=80'}
                  alt={item.name}
                  className="h-28 w-full rounded-3xl object-cover sm:w-32"
                />
                <div className="flex-1 space-y-2">
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <h2 className="text-lg font-semibold text-slate-900">{item.name}</h2>
                      <p className="text-sm text-slate-500">{item.category?.name || 'Unisex'}</p>
                    </div>
                    <p className="text-lg font-semibold text-slate-900 whitespace-nowrap">{Number(item.price).toLocaleString()} đ</p>
                  </div>
                  <div className="flex flex-wrap items-center gap-3">
                    <div className="flex items-center gap-2 rounded-full border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-700">
                      <button
                        type="button"
                        onClick={() => handleQuantityChange(item.id, item.quantity - 1)}
                        className="rounded-full bg-white px-2 py-1 text-base text-slate-700 transition hover:bg-slate-100"
                      >
                        −
                      </button>
                      <span className="w-6 text-center">{item.quantity}</span>
                      <button
                        type="button"
                        onClick={() => handleQuantityChange(item.id, item.quantity + 1)}
                        className="rounded-full bg-white px-2 py-1 text-base text-slate-700 transition hover:bg-slate-100"
                      >
                        +
                      </button>
                    </div>
                    <span className="text-sm text-slate-500">
                      = {Number(item.price * item.quantity).toLocaleString()} đ
                    </span>
                    <button
                      type="button"
                      onClick={() => handleRemove(item.id)}
                      className="rounded-full bg-rose-50 px-4 py-2 text-sm font-semibold text-rose-700 transition hover:bg-rose-100"
                    >
                      Xóa
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Order summary + shipping */}
          <aside className="rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
            <div className="space-y-5">
              {/* Summary */}
              <div>
                <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Order summary</p>
                <p className="mt-2 text-3xl font-semibold text-slate-900">{totalAmount.toLocaleString()} đ</p>
              </div>
              <div className="space-y-2 rounded-3xl bg-slate-50 p-4 text-sm text-slate-600">
                <div className="flex justify-between">
                  <span>Sản phẩm</span>
                  <span>{cartItems.length} loại</span>
                </div>
                <div className="flex justify-between">
                  <span>Số lượng</span>
                  <span>{totalQty} sản phẩm</span>
                </div>
                <div className="flex justify-between">
                  <span>Phí vận chuyển</span>
                  <span>Tính khi giao</span>
                </div>
              </div>

              {/* Structured shipping address */}
              <div className="space-y-3">
                <p className="text-sm font-semibold text-slate-700">Địa chỉ giao hàng</p>

                <div>
                  <label className="mb-1 block text-xs font-medium text-slate-500">Tỉnh / Thành phố *</label>
                  <select
                    value={address.province}
                    onChange={(e) => setAddress({ ...address, province: e.target.value })}
                    className="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                  >
                    <option value="">-- Chọn tỉnh / thành phố --</option>
                    {PROVINCES.map((p) => (
                      <option key={p} value={p}>{p}</option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="mb-1 block text-xs font-medium text-slate-500">Quận / Huyện *</label>
                  <input
                    type="text"
                    placeholder="Ví dụ: Quận 1, Huyện Bình Chánh..."
                    value={address.district}
                    onChange={(e) => setAddress({ ...address, district: e.target.value })}
                    className="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                  />
                </div>

                <div>
                  <label className="mb-1 block text-xs font-medium text-slate-500">Phường / Xã</label>
                  <input
                    type="text"
                    placeholder="Ví dụ: Phường Bến Nghé..."
                    value={address.ward}
                    onChange={(e) => setAddress({ ...address, ward: e.target.value })}
                    className="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                  />
                </div>

                <div>
                  <label className="mb-1 block text-xs font-medium text-slate-500">Số nhà / Tên đường *</label>
                  <input
                    type="text"
                    placeholder="Ví dụ: 123 Nguyễn Huệ..."
                    value={address.street}
                    onChange={(e) => setAddress({ ...address, street: e.target.value })}
                    className="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                  />
                </div>

                {address.province && address.district && address.street && (
                  <div className="rounded-2xl bg-slate-50 px-3 py-2.5 text-xs text-slate-500">
                    <span className="font-medium text-slate-700">Địa chỉ đầy đủ: </span>
                    {buildShippingAddress()}
                  </div>
                )}
              </div>

              <button
                onClick={handleCheckout}
                disabled={loading}
                className="w-full rounded-full bg-slate-900 px-6 py-3 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:bg-slate-400"
              >
                {loading ? 'Đang xử lý...' : 'Đặt hàng'}
              </button>

              {!isAuthenticated && (
                <p className="rounded-3xl bg-slate-100 px-4 py-3 text-sm text-slate-600">
                  Vui lòng{' '}
                  <button
                    onClick={() => navigate('/login')}
                    className="font-semibold text-slate-900 underline"
                  >
                    đăng nhập
                  </button>{' '}
                  để đặt hàng.
                </p>
              )}
            </div>
          </aside>
        </div>
      )}
    </div>
  );
};

export default StoreCart;
