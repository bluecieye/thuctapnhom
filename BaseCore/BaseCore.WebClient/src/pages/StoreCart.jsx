import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { orderApi } from '../services/api';
import { clearCart, getCartItems, removeCartItem, updateCartQuantity } from '../utils/cart';

const StoreCart = () => {
  const [cartItems, setCartItems] = useState([]);
  const [shippingAddress, setShippingAddress] = useState('');
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

  const handleCheckout = async () => {
    setError('');
    if (cartItems.length === 0) {
      setError('Your cart is empty.');
      return;
    }

    if (!isAuthenticated) {
      navigate('/login');
      return;
    }

    if (!shippingAddress.trim()) {
      setError('Please enter a shipping address.');
      return;
    }

    setLoading(true);
    try {
      const orderPayload = {
        items: cartItems.map((item) => ({ productId: item.id, quantity: item.quantity })),
        shippingAddress,
      };
      const response = await orderApi.create(orderPayload);
      clearCart();
      setCartItems([]);
      setSuccess(`Đặt hàng thành công! Mã đơn hàng: #${response.data.order.id}`);
      setShippingAddress('');
      setTimeout(() => navigate('/my-orders'), 2000);
    } catch (err) {
      setError(err.response?.data?.message || 'Checkout failed.');
    } finally {
      setLoading(false);
    }
  };

  const totalAmount = cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);

  return (
    <div className="space-y-8">
      <div className="flex flex-col gap-4 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Cart</p>
          <h1 className="mt-2 text-3xl font-semibold text-slate-900">Review your order</h1>
          <p className="mt-1 text-sm text-slate-600">Update quantities and complete checkout securely.</p>
        </div>
        <button className="rounded-full border border-slate-200 bg-slate-50 px-5 py-3 text-sm font-semibold text-slate-900 transition hover:bg-slate-100" onClick={() => navigate('/shop')}>
          Continue shopping
        </button>
      </div>

      {(error || success) && (
        <div className={`rounded-3xl p-4 text-sm ring-1 ${error ? 'bg-rose-50 text-rose-700 ring-rose-200' : 'bg-emerald-50 text-emerald-700 ring-emerald-200'}`}>
          {error || success}
        </div>
      )}

      {cartItems.length === 0 ? (
        <div className="rounded-[2rem] bg-white p-10 text-center shadow-soft ring-1 ring-slate-200">
          <p className="text-lg font-semibold text-slate-900">Your cart is empty</p>
          <p className="mt-2 text-sm text-slate-600">Add a few items to get started.</p>
        </div>
      ) : (
        <div className="grid gap-6 lg:grid-cols-[1.4fr_0.6fr]">
          <div className="space-y-4 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
            {cartItems.map((item) => (
              <div key={item.id} className="flex flex-col gap-4 rounded-[1.75rem] border border-slate-200 p-4 sm:flex-row sm:items-center">
                <img src={item.imageUrl || 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=400&q=80'} alt={item.name} className="h-28 w-full rounded-3xl object-cover sm:w-32" />
                <div className="flex-1 space-y-2">
                  <div className="flex items-center justify-between gap-3">
                    <div>
                      <h2 className="text-lg font-semibold text-slate-900">{item.name}</h2>
                      <p className="text-sm text-slate-500">{item.category?.name || 'Unisex'}</p>
                    </div>
                    <p className="text-lg font-semibold text-slate-900">{Number(item.price).toLocaleString()} đ</p>
                  </div>
                  <div className="flex flex-wrap items-center gap-3">
                    <div className="flex items-center gap-2 rounded-full border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-700">
                      <button type="button" onClick={() => handleQuantityChange(item.id, item.quantity - 1)} className="rounded-full bg-white px-2 py-1 text-base text-slate-700 transition hover:bg-slate-100">
                        -
                      </button>
                      <span>{item.quantity}</span>
                      <button type="button" onClick={() => handleQuantityChange(item.id, item.quantity + 1)} className="rounded-full bg-white px-2 py-1 text-base text-slate-700 transition hover:bg-slate-100">
                        +
                      </button>
                    </div>
                    <button type="button" onClick={() => handleRemove(item.id)} className="rounded-full bg-rose-50 px-4 py-2 text-sm font-semibold text-rose-700 transition hover:bg-rose-100">
                      Remove
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          <aside className="rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
            <div className="space-y-4">
              <div>
                <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Order summary</p>
                <p className="mt-2 text-3xl font-semibold text-slate-900">{totalAmount.toLocaleString()} đ</p>
              </div>
              <div className="space-y-3 rounded-3xl bg-slate-50 p-4 text-sm text-slate-600">
                <div className="flex justify-between">
                  <span>Items</span>
                  <span>{cartItems.length}</span>
                </div>
                <div className="flex justify-between">
                  <span>Shipping</span>
                  <span>Calculated at checkout</span>
                </div>
              </div>
              <div>
                <label className="mb-2 block text-sm font-medium text-slate-700">Shipping address</label>
                <textarea
                  rows="4"
                  value={shippingAddress}
                  onChange={(e) => setShippingAddress(e.target.value)}
                  className="w-full rounded-3xl border border-slate-200 bg-white px-4 py-3 text-sm text-slate-900 outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                />
              </div>
              <button
                onClick={handleCheckout}
                disabled={loading}
                className="w-full rounded-full bg-slate-900 px-6 py-3 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:bg-slate-400"
              >
                {loading ? 'Processing...' : 'Checkout'}
              </button>
              {!isAuthenticated && (
                <p className="rounded-3xl bg-slate-100 px-4 py-3 text-sm text-slate-600">
                  Please <button onClick={() => navigate('/login')} className="font-semibold text-slate-900 underline">sign in</button> to place your order.
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
