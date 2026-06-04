

import { useState, useEffect, useMemo } from 'react';

import { useNavigate } from 'react-router-dom';

import { CreditCard, MapPin, Tag, Check, Truck } from 'lucide-react';

import { useCart } from '../contexts/CartContext';
import { useAuth } from '../contexts/AuthContext';

import { orderService } from '../services/orderService';

import { addressApi, provincesApi, shippingCarriersApi, shippingRatesApi } from '../services/api';

import { fmt, IMAGE_PLACEHOLDER } from '../utils/format';

// ════════════════════════════════════════════════════════════
// CONSTANTS / SCHEMA
// ════════════════════════════════════════════════════════════
const inputClass =
  'w-full rounded-lg border border-slate-200 bg-white px-4 py-3 text-sm text-slate-900 placeholder-slate-400 outline-none transition focus:border-slate-900 focus:ring-2 focus:ring-slate-900/10';

// ─── Định dạng input thẻ ─────────────────────────────────────
// Số thẻ: nhóm 4 chữ số, tối đa 16 số → "1234 5678 9012 3456"
const formatCardNumber = (v) =>
  v.replace(/\D/g, '').slice(0, 16).replace(/(.{4})/g, '$1 ').trim();

// Ngày hết hạn: MM/YY
const formatExpiry = (v) => {
  const d = v.replace(/\D/g, '').slice(0, 4);
  return d.length >= 3 ? `${d.slice(0, 2)}/${d.slice(2)}` : d;
};

// ════════════════════════════════════════════════════════════
// RENDER: HELPERS
// ════════════════════════════════════════════════════════════
const Section = ({ icon: Icon, title, subtitle, children }) => (
  <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
    {}
    <div className="mb-5 flex items-start gap-3">
      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-slate-900 text-white">
        <Icon size={18} />
      </div>
      <div>
        <h2 className="text-base font-semibold text-slate-900">{title}</h2>
        {}
        {subtitle && <p className="text-xs text-slate-500">{subtitle}</p>}
      </div>
    </div>
    {}
    {children}
  </section>
);

// ════════════════════════════════════════════════════════════
// TRANG THANH TOÁN
// ════════════════════════════════════════════════════════════
const CheckoutPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const navigate = useNavigate();

  const { cartItems, totalPrice, totalItems, refresh } = useCart();

  const { isAuthenticated } = useAuth();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [addresses, setAddresses] = useState([]);
  const [addressId, setAddressId] = useState(null);        
  const [showNewAddress, setShowNewAddress] = useState(false); 
  const [provinces, setProvinces] = useState([]);          

  const [newAddress, setNewAddress] = useState({
    fullName: '', phone: '', street: '', ward: '', provinceId: '', isDefault: true,
  });

  const [carriers, setCarriers] = useState([]);            
  const [carrierId, setCarrierId] = useState(null);        

  
  const [quotes, setQuotes] = useState([]);
  const [quoteLoading, setQuoteLoading] = useState(false); 

  const [paymentMethod, setPaymentMethod] = useState('COD');

  const [card, setCard] = useState({ number: '', expiry: '', name: '', cvv: '' });
  const [note, setNote] = useState('');
  const [couponCode, setCouponCode] = useState('');          
  
  const [couponPreview, setCouponPreview] = useState({ message: '', discount: 0, valid: false });

  const [loading, setLoading] = useState(false);           
  const [error, setError] = useState('');                  

  const [orderPlaced, setOrderPlaced] = useState(false);

  // ════════════════════════════════════════════════════════════
  // EFFECT: TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  useEffect(() => {

    if (!isAuthenticated) {
      navigate('/login');
      return;
    }

    (async () => {

      const [a, p, c] = await Promise.all([
        addressApi.getMine(),              
        provincesApi.getAll(),             
        shippingCarriersApi.getAll(true),  
      ]);
      setAddresses(a.data || []);
      setProvinces(p.data || []);
      setCarriers(c.data || []);

      const def = (a.data || []).find((x) => x.isDefault) || a.data?.[0];
      if (def) setAddressId(def.id);
      
      else setShowNewAddress(true);

      if ((c.data || []).length) setCarrierId(c.data[0].id);
    })();
    
  }, [isAuthenticated, navigate]);

  

  

  useEffect(() => {
    if (!orderPlaced && cartItems.length === 0) navigate('/cart');
  }, [cartItems.length, navigate, orderPlaced]);

  // ════════════════════════════════════════════════════════════
  // TÍNH TOÁN
  // ════════════════════════════════════════════════════════════
  const selectedAddress = useMemo(
    () => addresses.find((a) => a.id === addressId),
    [addresses, addressId]
  );

  const subtotalAfterDiscount = totalPrice - couponPreview.discount;

  

  
  
  useEffect(() => {
    if (!selectedAddress) { setQuotes([]); return; }
    setQuoteLoading(true);
    shippingRatesApi.quote({
      provinceId: selectedAddress.provinceId,
      orderTotal: subtotalAfterDiscount,
    })
      .then(({ data }) => setQuotes(data || []))
      .catch(() => setQuotes([]))
      .finally(() => setQuoteLoading(false));
  }, [selectedAddress?.id, selectedAddress?.provinceId, subtotalAfterDiscount]);

  

  
  useEffect(() => {
    if (!quotes.length) return;
    const current = quotes.find((q) => q.carrierId === carrierId);
    if (!current || !current.available) {
      const firstAvailable = quotes.find((q) => q.available);
      if (firstAvailable) setCarrierId(firstAvailable.carrierId);
    }
  }, [quotes, carrierId]);

  

  const selectedQuote = quotes.find((q) => q.carrierId === carrierId);
  
  const estimatedShipping = selectedQuote?.fee ?? 0;
  
  const grandTotal = totalPrice - couponPreview.discount + estimatedShipping;

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const applyCoupon = async () => {
    if (!couponCode) return; 
    try {
      const res = await orderService.applyCoupon(couponCode, totalPrice);
      setCouponPreview({
        message: res.message || (res.isValid ? 'Áp mã thành công' : 'Mã không hợp lệ'),
        discount: res.discountAmount || 0,
        valid: res.isValid,
      });
    } catch {
      
      setCouponPreview({ message: 'Mã không hợp lệ', discount: 0, valid: false });
    }
  };

  

  

  const saveNewAddress = async () => {
    setError('');
    try {
      const payload = { ...newAddress, provinceId: Number(newAddress.provinceId) };
      const { data } = await addressApi.create(payload);
      const refreshed = (await addressApi.getMine()).data;
      setAddresses(refreshed);
      setAddressId(data.id);
      setShowNewAddress(false);
    } catch (e) {
      setError(e.response?.data?.message || 'Không lưu được địa chỉ.');
    }
  };

  
  
  const handleSubmit = async (e) => {
    e.preventDefault(); 
    setError('');

    if (!addressId) {
      setError('Vui lòng chọn hoặc thêm địa chỉ giao hàng.');
      return;
    }
    if (!carrierId || !selectedQuote?.available) {
      setError('Vui lòng chọn đơn vị vận chuyển khả dụng.');
      return;
    }

    setLoading(true);
    try {

      const order = await orderService.createOrder({
        addressId,
        shippingCarrierId: carrierId,
        
        couponCode: couponPreview.valid ? couponCode : undefined,
        paymentMethod,
        note,
      });

      setOrderPlaced(true);

      await refresh();

      navigate(`/order-confirmation/${order.id}`, {
        replace: true,
        state: { justPlaced: true }, 
      });
    } catch (err) {
      setError(err.response?.data?.message || 'Đặt hàng thất bại');
    } finally {
      setLoading(false); 
    }
  };

  

  if (!isAuthenticated || cartItems.length === 0) return null;

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="bg-slate-50 min-h-screen">
      <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        {}
        <div className="mb-10 text-center">
          <p className="text-xs uppercase tracking-[0.4em] text-slate-400">Thanh toán</p>
          <h1 className="mt-2 font-serif text-4xl font-light text-slate-900">Hoàn tất đơn hàng</h1>
        </div>

        {}
        <div className="lg:grid lg:grid-cols-12 lg:gap-8">

          {}
          <form onSubmit={handleSubmit} className="space-y-6 lg:col-span-7">
            {}
            {error && (
              <p className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700 ring-1 ring-rose-200">{error}</p>
            )}

            {}
            {/* ════════════════════════════════════════════════════════════
                RENDER: FORM ĐỊA CHỈ
                ════════════════════════════════════════════════════════════ */}
            <Section icon={MapPin} title="Địa chỉ giao hàng" subtitle="Chọn từ sổ địa chỉ hoặc thêm mới">
              {}
              {addresses.length > 0 && (
                <div className="space-y-2">
                  {addresses.map((a) => {
                    const active = a.id === addressId;
                    return (
                      
                      <label key={a.id}
                        className={`flex cursor-pointer items-start gap-3 rounded-xl border p-4 ${
                          active ? 'border-slate-900 bg-slate-50 ring-1 ring-slate-900' : 'border-slate-200'
                        }`}>
                        <input type="radio" name="addressId" checked={active}
                          onChange={() => setAddressId(a.id)} className="mt-1 accent-slate-900" />
                        <div className="flex-1">
                          <p className="font-medium text-slate-900">
                            {a.fullName} · {a.phone}
                            {}
                            {a.isDefault && <span className="ml-2 rounded bg-slate-900 px-2 py-0.5 text-[10px] text-white">MẶC ĐỊNH</span>}
                          </p>
                          <p className="text-sm text-slate-600">
                            {a.street}, {a.ward}, {a.province?.name || ''}
                          </p>
                        </div>
                      </label>
                    );
                  })}
                </div>
              )}

              {}
              <button type="button" onClick={() => setShowNewAddress(!showNewAddress)}
                className="mt-3 text-sm font-medium text-slate-700 underline">
                {showNewAddress ? 'Huỷ' : '+ Thêm địa chỉ mới'}
              </button>

              {}
              {showNewAddress && (
                <div className="mt-4 space-y-3 rounded-xl bg-slate-50 p-4">
                  {}
                  <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                    <input className={inputClass} placeholder="Họ tên người nhận"
                      value={newAddress.fullName}
                      onChange={(e) => setNewAddress({ ...newAddress, fullName: e.target.value })} />
                    <input className={inputClass} placeholder="Số điện thoại"
                      value={newAddress.phone}
                      onChange={(e) => setNewAddress({ ...newAddress, phone: e.target.value })} />
                  </div>
                  {}
                  <input className={inputClass} placeholder="Số nhà, tên đường"
                    value={newAddress.street}
                    onChange={(e) => setNewAddress({ ...newAddress, street: e.target.value })} />
                  {}
                  <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                    <input className={inputClass} placeholder="Phường/Xã"
                      value={newAddress.ward}
                      onChange={(e) => setNewAddress({ ...newAddress, ward: e.target.value })} />
                    <select className={inputClass} value={newAddress.provinceId}
                      onChange={(e) => setNewAddress({ ...newAddress, provinceId: e.target.value })}>
                      <option value="">Tỉnh/Thành phố</option>
                      {provinces.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
                    </select>
                  </div>
                  <button type="button" onClick={saveNewAddress}
                    className="rounded-lg bg-slate-900 px-4 py-2 text-sm text-white hover:bg-slate-700">
                    Lưu địa chỉ
                  </button>
                </div>
              )}
            </Section>

            {}
            <Section icon={Truck} title="Đơn vị vận chuyển" subtitle="Chọn đối tác giao hàng">
              {}
              {!selectedAddress ? (
                <p className="text-sm text-slate-500">Chọn địa chỉ giao hàng trước.</p>
              ) : quoteLoading ? (
                <p className="text-sm text-slate-500">Đang tính cước...</p>
              ) : quotes.length === 0 ? (
                <p className="text-sm text-rose-500">Không có đơn vị nào hỗ trợ địa chỉ này.</p>
              ) : (
                <div className="space-y-2">
                  {quotes.map((q) => {
                    const active = q.carrierId === carrierId;
                    return (
                      <label key={q.carrierId}
                        className={`flex cursor-pointer items-start gap-3 rounded-xl border p-4 ${
                          !q.available ? 'opacity-50 cursor-not-allowed' : ''
                        } ${active && q.available ? 'border-slate-900 bg-slate-50 ring-1 ring-slate-900' : 'border-slate-200'}`}>
                        <input type="radio" name="carrierId" checked={active}
                          disabled={!q.available}
                          onChange={() => setCarrierId(q.carrierId)} className="mt-1 accent-slate-900" />
                        <div className="flex-1">
                          <div className="flex items-center justify-between">
                            <p className="font-medium text-slate-900">{q.carrierName}</p>
                            {}
                            <p className="font-semibold">
                              {q.isFreeShip
                                ? <span className="text-emerald-600">Miễn phí</span>
                                : `${fmt(q.fee)} đ`}
                            </p>
                          </div>
                          {q.available ? (
                            <p className="mt-1 text-xs text-slate-500">
                              Dự kiến {q.estimatedDays} ngày
                              {}
                              {q.freeShipReason && <> · <span className="text-emerald-600">{q.freeShipReason}</span></>}
                            </p>
                          ) : (
                            <p className="mt-1 text-xs text-rose-500">{q.unavailableReason}</p>
                          )}
                        </div>
                      </label>
                    );
                  })}
                </div>
              )}
            </Section>

            {}
            <Section icon={Tag} title="Mã giảm giá" subtitle="Áp dụng coupon nếu có">
              <div className="flex gap-2">
                <input className={inputClass} placeholder="Nhập mã"
                  value={couponCode} onChange={(e) => setCouponCode(e.target.value)} />
                <button type="button" onClick={applyCoupon}
                  className="rounded-lg bg-slate-900 px-4 py-2 text-sm text-white hover:bg-slate-700">
                  Áp dụng
                </button>
              </div>
              {}
              {couponPreview.message && (
                <p className={`mt-2 text-sm ${couponPreview.valid ? 'text-green-600' : 'text-rose-500'}`}>
                  {couponPreview.valid && <Check size={14} className="mr-1 inline" />}
                  {couponPreview.message}
                  {couponPreview.valid && ` · -${fmt(couponPreview.discount)} đ`}
                </p>
              )}
            </Section>

            {}
            <Section icon={CreditCard} title="Thanh toán" subtitle="Chọn phương thức thanh toán">
              {}
              {[
                { value: 'COD', label: 'Tiền mặt khi nhận hàng (COD)', desc: 'Thanh toán bằng tiền mặt khi nhận hàng' },
                { value: 'Card', label: 'Thẻ tín dụng/Ghi nợ', desc: 'Visa, Mastercard, JCB...' },
                { value: 'Wallet', label: 'Chuyển khoản / Quét mã QR', desc: 'Quét mã VietQR để thanh toán' },
              ].map((opt) => (
                <div key={opt.value}>
                  <label
                    className={`mt-2 flex cursor-pointer items-start gap-3 rounded-xl border p-4 ${
                      paymentMethod === opt.value ? 'border-slate-900 bg-slate-50 ring-1 ring-slate-900' : 'border-slate-200'
                    }`}>
                    <input type="radio" name="payment" value={opt.value} checked={paymentMethod === opt.value}
                      onChange={(e) => setPaymentMethod(e.target.value)} className="mt-1 accent-slate-900" />
                    <span className="flex-1">
                      <span className="block font-medium text-slate-900">{opt.label}</span>
                      <span className="block text-xs text-slate-500">{opt.desc}</span>
                    </span>
                  </label>

                  {/* ─── Form nhập thẻ ─── */}
                  {opt.value === 'Card' && paymentMethod === 'Card' && (
                    <div className="mt-3 space-y-3 rounded-xl border border-slate-200 bg-slate-50 p-4">
                      <div>
                        <label className="mb-1 block text-xs font-medium uppercase tracking-wider text-slate-500">Số thẻ</label>
                        <input className={inputClass} placeholder="1234 5678 9012 3456" inputMode="numeric"
                          value={card.number}
                          onChange={(e) => setCard({ ...card, number: formatCardNumber(e.target.value) })} />
                      </div>
                      <div>
                        <label className="mb-1 block text-xs font-medium uppercase tracking-wider text-slate-500">Tên chủ thẻ</label>
                        <input className={`${inputClass} uppercase`} placeholder="NGUYEN VAN A"
                          value={card.name}
                          onChange={(e) => setCard({ ...card, name: e.target.value.toUpperCase() })} />
                      </div>
                      <div className="grid grid-cols-2 gap-3">
                        <div>
                          <label className="mb-1 block text-xs font-medium uppercase tracking-wider text-slate-500">Ngày hết hạn</label>
                          <input className={inputClass} placeholder="MM/YY" inputMode="numeric"
                            value={card.expiry}
                            onChange={(e) => setCard({ ...card, expiry: formatExpiry(e.target.value) })} />
                        </div>
                        <div>
                          <label className="mb-1 block text-xs font-medium uppercase tracking-wider text-slate-500">CVV</label>
                          <input className={inputClass} placeholder="123" inputMode="numeric" type="password" maxLength={4}
                            value={card.cvv}
                            onChange={(e) => setCard({ ...card, cvv: e.target.value.replace(/\D/g, '').slice(0, 4) })} />
                        </div>
                      </div>
                      <p className="text-[11px] text-slate-400">🔒 Thông tin thẻ được mã hoá và bảo mật tuyệt đối.</p>
                    </div>
                  )}

                  {/* ─── Mã QR ngân hàng ─── */}
                  {opt.value === 'Wallet' && paymentMethod === 'Wallet' && (
                    <div className="mt-3 flex flex-col items-center gap-3 rounded-xl border border-slate-200 bg-slate-50 p-5 text-center">
                      <p className="text-sm font-medium text-slate-900">Quét mã để chuyển khoản</p>
                      <img src="/hero/bank-qr.jpg" alt="Mã QR ngân hàng"
                        className="h-64 w-auto rounded-xl border border-slate-200 object-contain shadow-sm" />
                      <div className="text-sm text-slate-600">
                        <p className="font-semibold text-slate-900">HOANG PHAM MINH NHAT</p>
                        <p>STK: 0338107130</p>
                        <p className="mt-1 font-semibold text-slate-900">Số tiền: {fmt(grandTotal)} đ</p>
                      </div>
                      <p className="text-[11px] text-slate-400">
                        Vui lòng chuyển đúng số tiền & ghi chú là số điện thoại của bạn để đối soát.
                      </p>
                    </div>
                  )}
                </div>
              ))}

              {}
              <div className="mt-4">
                <label className="mb-1 block text-xs font-medium uppercase tracking-wider text-slate-500">Ghi chú</label>
                <textarea className={inputClass} rows="3" placeholder="Ghi chú cho shipper (không bắt buộc)"
                  value={note} onChange={(e) => setNote(e.target.value)} />
              </div>
            </Section>

            {}
            <button type="submit" disabled={loading || !addressId || !selectedQuote?.available}
              className="w-full rounded-xl bg-slate-900 py-4 text-sm font-semibold uppercase tracking-wider text-white transition hover:bg-slate-700 disabled:cursor-not-allowed disabled:bg-slate-300">
              {loading ? 'Đang xử lý...' : `Đặt hàng · ${fmt(grandTotal)} đ`}
            </button>
          </form>

          {}
          {/* ════════════════════════════════════════════════════════════
              RENDER: TÓM TẮT ĐƠN HÀNG
              ════════════════════════════════════════════════════════════ */}
          <div className="mt-10 lg:col-span-5 lg:mt-0">
            {}
            <div className="sticky top-28 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <div className="flex items-baseline justify-between">
                <h2 className="text-base font-semibold text-slate-900">Đơn hàng</h2>
                <span className="text-xs text-slate-500">{totalItems} sản phẩm</span>
              </div>

              {}
              <div className="mt-6 max-h-80 overflow-y-auto divide-y divide-slate-100">
                {cartItems.map((item) => (
                  
                  <div key={item.cartItemId ?? item.variantId} className="flex gap-4 py-4">
                    <div className="relative">
                      <img src={item.imageUrl || IMAGE_PLACEHOLDER}
                        alt={item.productName} className="h-20 w-16 rounded-lg object-cover bg-slate-100" />
                      {}
                      <span className="absolute -right-2 -top-2 flex h-5 min-w-5 items-center justify-center rounded-full bg-slate-900 px-1.5 text-[10px] font-semibold text-white">
                        {item.quantity}
                      </span>
                    </div>
                    <div className="flex-1">
                      <p className="text-sm font-medium text-slate-900 line-clamp-2">{item.productName}</p>
                      <p className="text-xs text-slate-500">
                        {item.colorName} · Size {item.sizeName}
                      </p>
                      <p className="text-xs text-slate-500">{fmt(item.price)} đ</p>
                    </div>
                    {}
                    <p className="text-sm font-semibold text-slate-900">{fmt(item.price * item.quantity)} đ</p>
                  </div>
                ))}
              </div>

              {}
              <div className="mt-4 space-y-2 border-t border-slate-200 pt-4 text-sm">
                <div className="flex justify-between text-slate-600">
                  <span>Tạm tính</span>
                  <span>{fmt(totalPrice)} đ</span>
                </div>
                {}
                {couponPreview.valid && (
                  <div className="flex justify-between text-green-600">
                    <span>Giảm giá ({couponCode})</span>
                    <span>-{fmt(couponPreview.discount)} đ</span>
                  </div>
                )}
                <div className="flex justify-between text-slate-600">
                  <span>Phí ship {selectedQuote?.carrierName && <span className="text-xs text-slate-400">({selectedQuote.carrierName})</span>}</span>
                  <span>{estimatedShipping === 0 ? 'Miễn phí' : `${fmt(estimatedShipping)} đ`}</span>
                </div>
                {}
                <div className="flex justify-between border-t border-slate-200 pt-3 text-base font-semibold text-slate-900">
                  <span>Tổng</span>
                  <span>{fmt(grandTotal)} đ</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CheckoutPage;
