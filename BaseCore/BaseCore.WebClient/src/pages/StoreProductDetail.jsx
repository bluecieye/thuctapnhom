import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { productApi } from '../services/api';
import { addToCart } from '../utils/cart';

const StoreProductDetail = () => {
  const { id } = useParams();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    if (id) {
      loadProduct(id);
    }
  }, [id]);

  const loadProduct = async (productId) => {
    setLoading(true);
    try {
      const response = await productApi.getById(productId);
      setProduct(response.data);
    } catch (error) {
      console.error('Failed to load product', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = () => {
    if (!product) return;
    addToCart(product, 1);
    setMessage('Sản phẩm đã được thêm vào giỏ hàng.');
    setTimeout(() => setMessage(''), 2500);
  };

  if (loading) {
    return (
      <div className="rounded-[2rem] bg-white p-10 text-center shadow-soft ring-1 ring-slate-200">
        Loading product details...
      </div>
    );
  }

  if (!product) {
    return (
      <div className="rounded-[2rem] bg-white p-10 text-center shadow-soft ring-1 ring-slate-200">
        <p className="text-lg font-semibold text-slate-900">Product not found</p>
        <p className="mt-2 text-sm text-slate-600">The product you are looking for is unavailable.</p>
      </div>
    );
  }

  return (
    <div className="grid gap-10 lg:grid-cols-[0.95fr_0.55fr]">
      <div className="rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
        {message && (
          <div className="rounded-3xl bg-emerald-50 p-4 text-sm text-emerald-700 ring-1 ring-emerald-200">
            {message}
          </div>
        )}
        <div className="overflow-hidden rounded-[2rem] bg-slate-100">
          <img
            src={product.imageUrl || 'https://images.unsplash.com/photo-1520975928447-d5e2a8bb2d18?auto=format&fit=crop&w=900&q=80'}
            alt={product.name}
            className="h-full w-full object-cover"
          />
        </div>
        <div className="mt-6 grid gap-3 sm:grid-cols-3">
          <div className="rounded-3xl bg-slate-50 p-4 text-center">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-500">Available</p>
            <p className="mt-2 text-base font-semibold text-slate-900">{product.stock} pieces</p>
          </div>
          <div className="rounded-3xl bg-slate-50 p-4 text-center">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-500">Category</p>
            <p className="mt-2 text-base font-semibold text-slate-900">{product.category?.name || 'Unisex'}</p>
          </div>
          <div className="rounded-3xl bg-slate-50 p-4 text-center">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-500">Style</p>
            <p className="mt-2 text-base font-semibold text-slate-900">{product.gender || 'Unisex'}</p>
          </div>
        </div>
      </div>

      <div className="rounded-[2rem] bg-white p-8 shadow-soft ring-1 ring-slate-200">
        <div className="flex items-center justify-between gap-4">
          <div>
            <p className="text-sm uppercase tracking-[0.3em] text-slate-500">{product.brand || 'SLAY Store'}</p>
            <h1 className="mt-2 text-3xl font-semibold text-slate-900">{product.name}</h1>
          </div>
          {product.isNew && <span className="rounded-full bg-slate-900 px-4 py-2 text-sm font-semibold uppercase text-white">New arrival</span>}
        </div>

        <div className="mt-6 space-y-4">
          <div className="flex items-center gap-4">
            <p className="text-4xl font-semibold text-slate-900">{Number(product.price).toLocaleString()} đ</p>
            {product.originalPrice && product.originalPrice > product.price && (
              <p className="text-sm text-slate-500 line-through">{Number(product.originalPrice).toLocaleString()} đ</p>
            )}
          </div>
          <p className="text-sm leading-7 text-slate-600">{product.description || 'A versatile statement piece crafted for both style and comfort.'}</p>
          <div className="grid gap-4 rounded-3xl bg-slate-50 p-5 text-sm text-slate-600">
            <div className="flex items-center justify-between">
              <span>Brand</span>
              <span className="font-semibold text-slate-900">{product.brand || 'SLAY Store'}</span>
            </div>
            <div className="flex items-center justify-between">
              <span>Gender</span>
              <span className="font-semibold text-slate-900">{product.gender || 'Unisex'}</span>
            </div>
          </div>
        </div>

        <div className="mt-8 flex flex-wrap gap-3">
          <button onClick={handleAddToCart} className="rounded-full bg-slate-900 px-6 py-3 text-sm font-semibold text-white transition hover:bg-slate-800">
            Add to cart
          </button>
          <button onClick={() => navigate('/cart')} className="rounded-full border border-slate-200 bg-white px-6 py-3 text-sm font-semibold text-slate-900 transition hover:bg-slate-50">
            View cart
          </button>
        </div>
      </div>
    </div>
  );
};

export default StoreProductDetail;
