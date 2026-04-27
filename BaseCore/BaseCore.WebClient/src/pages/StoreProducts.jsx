import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { categoryApi, productApi } from '../services/api';
import { addToCart } from '../utils/cart';

const StoreProducts = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [keyword, setKeyword] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [loading, setLoading] = useState(true);
  const [notification, setNotification] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    loadCategories();
  }, []);

  useEffect(() => {
    loadProducts();
  }, [keyword, categoryId, page]);

  const loadCategories = async () => {
    try {
      const response = await categoryApi.getAll();
      setCategories(response.data || []);
    } catch (error) {
      console.error('Failed to load categories', error);
    }
  };

  const loadProducts = async () => {
    setLoading(true);
    try {
      const response = await productApi.search({ keyword, categoryId: categoryId || undefined, page, pageSize: 12 });
      setProducts(response.data.items || []);
      setTotalPages(response.data.totalPages || 0);
    } catch (error) {
      console.error('Failed to load products', error);
      setProducts([]);
      setTotalPages(0);
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = (product) => {
    addToCart(product, 1);
    setNotification(`${product.name} đã được thêm vào giỏ hàng.`);
    setTimeout(() => setNotification(''), 2500);
  };

  const renderPages = () => {
    const pages = [];
    for (let i = 1; i <= totalPages; i += 1) {
      pages.push(
        <li key={i}>
          <button
            type="button"
            className={`rounded-full px-4 py-2 text-sm font-medium transition ${page === i ? 'bg-slate-900 text-white' : 'bg-slate-100 text-slate-700 hover:bg-slate-200'}`}
            onClick={() => setPage(i)}
          >
            {i}
          </button>
        </li>
      );
    }
    return pages;
  };

  return (
    <div className="space-y-8">
      <div className="flex flex-col gap-4 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Product catalog</p>
          <h1 className="mt-2 text-3xl font-semibold text-slate-900">Explore the store</h1>
        </div>
        <button className="rounded-full border border-slate-200 bg-slate-50 px-5 py-3 text-sm font-semibold text-slate-900 transition hover:bg-slate-100" onClick={() => navigate('/cart')}>
          View cart
        </button>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_0.55fr]">
        <section className="space-y-6 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
          <div className="grid gap-4 sm:grid-cols-[1fr_auto] sm:items-center">
            <div>
              <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Search filters</p>
              <h2 className="mt-2 text-xl font-semibold text-slate-900">Refine your selection</h2>
            </div>
            <div className="flex flex-col gap-3 sm:flex-row">
              <input
                type="text"
                value={keyword}
                onChange={(e) => setKeyword(e.target.value)}
                placeholder="Search products..."
                className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-900 outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
              />
              <button
                className="rounded-3xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white transition hover:bg-slate-800"
                onClick={() => setPage(1)}
              >
                Apply
              </button>
            </div>
          </div>

          <div className="flex flex-wrap gap-3">
            <button
              className={`rounded-full px-4 py-2 text-sm font-medium transition ${categoryId === '' ? 'bg-slate-900 text-white' : 'bg-slate-100 text-slate-700 hover:bg-slate-200'}`}
              onClick={() => { setCategoryId(''); setPage(1); }}
            >
              All categories
            </button>
            {categories.map((category) => (
              <button
                key={category.id}
                className={`rounded-full px-4 py-2 text-sm font-medium transition ${categoryId === category.id.toString() ? 'bg-slate-900 text-white' : 'bg-slate-100 text-slate-700 hover:bg-slate-200'}`}
                onClick={() => { setCategoryId(category.id.toString()); setPage(1); }}
              >
                {category.name}
              </button>
            ))}
          </div>

          {notification && (
            <div className="rounded-3xl bg-emerald-50 p-4 text-sm text-emerald-700 ring-1 ring-emerald-200">
              {notification}
            </div>
          )}

          {loading ? (
            <div className="rounded-[2rem] bg-slate-50 p-10 text-center text-slate-500 shadow-soft ring-1 ring-slate-200">
              Loading products...
            </div>
          ) : products.length === 0 ? (
            <div className="rounded-[2rem] bg-slate-50 p-10 text-center text-slate-500 shadow-soft ring-1 ring-slate-200">
              No products match the current filters.
            </div>
          ) : (
            <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
              {products.map((product) => (
                <article key={product.id} className="overflow-hidden rounded-[2rem] bg-white shadow-soft ring-1 ring-slate-200">
                  <div className="relative h-64 overflow-hidden bg-slate-100">
                    <img
                      src={product.imageUrl || 'https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&w=900&q=80'}
                      alt={product.name}
                      className="h-full w-full object-cover"
                    />
                  </div>
                  <div className="space-y-4 p-5">
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <p className="text-sm uppercase tracking-[0.3em] text-slate-500">{product.category?.name || 'Unisex'}</p>
                        <h3 className="mt-2 text-lg font-semibold text-slate-900">{product.name}</h3>
                      </div>
                      {product.isNew && <span className="rounded-full bg-slate-900 px-3 py-1 text-xs font-semibold uppercase text-white">New</span>}
                    </div>
                    <p className="text-sm leading-6 text-slate-600">{product.description}</p>
                    <div className="flex items-center justify-between gap-3">
                      <div>
                        <p className="text-xl font-semibold text-slate-900">{Number(product.price).toLocaleString()} đ</p>
                        {product.originalPrice && product.originalPrice > product.price && (
                          <p className="text-sm text-slate-500 line-through">{Number(product.originalPrice).toLocaleString()} đ</p>
                        )}
                      </div>
                      <div className="flex gap-2">
                        <button
                          className="rounded-full border border-slate-200 bg-white px-4 py-2 text-sm font-semibold text-slate-900 transition hover:bg-slate-50"
                          onClick={() => navigate(`/product/${product.id}`)}
                        >
                          View
                        </button>
                        <button
                          className="rounded-full bg-slate-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-slate-800"
                          onClick={() => handleAddToCart(product)}
                        >
                          Add
                        </button>
                      </div>
                    </div>
                  </div>
                </article>
              ))}
            </div>
          )}

          {totalPages > 1 && (
            <div className="flex flex-wrap items-center justify-center gap-2 pt-4">
              <button
                className="rounded-full border border-slate-200 bg-slate-100 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-200"
                onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
                disabled={page === 1}
              >
                Previous
              </button>
              <ul className="flex flex-wrap items-center gap-2">{renderPages()}</ul>
              <button
                className="rounded-full border border-slate-200 bg-slate-100 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-200"
                onClick={() => setPage((prev) => Math.min(prev + 1, totalPages))}
                disabled={page === totalPages}
              >
                Next
              </button>
            </div>
          )}
        </section>

        <aside className="space-y-6 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
          <div>
            <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Need help?</p>
            <h2 className="mt-2 text-xl font-semibold text-slate-900">Order support</h2>
            <p className="mt-3 text-sm leading-6 text-slate-600">Have a question about fit, stock or tracking? Our support team is ready to help.</p>
          </div>
          <div className="rounded-3xl bg-slate-50 p-5">
            <p className="text-sm text-slate-500">Email</p>
            <p className="mt-2 text-sm font-semibold text-slate-900">support@slaystore.com</p>
            <p className="mt-4 text-sm text-slate-500">Phone</p>
            <p className="mt-2 text-sm font-semibold text-slate-900">+84 123 456 789</p>
          </div>
        </aside>
      </div>
    </div>
  );
};

export default StoreProducts;
