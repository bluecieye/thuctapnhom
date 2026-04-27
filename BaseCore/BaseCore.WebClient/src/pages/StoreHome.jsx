import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { categoryApi, productApi } from '../services/api';
import { addToCart } from '../utils/cart';

const StoreHome = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [keyword, setKeyword] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState('');
  const [featured, setFeatured] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    loadCategories();
    loadFeatured();
  }, []);

  useEffect(() => {
    loadProducts();
  }, [keyword, categoryId]);

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
      const response = await productApi.search({ keyword, categoryId: categoryId || undefined, page: 1, pageSize: 12 });
      setProducts(response.data.items || []);
    } catch (error) {
      console.error('Failed to load products', error);
      setProducts([]);
    } finally {
      setLoading(false);
    }
  };

  const loadFeatured = async () => {
    try {
      const response = await productApi.search({ page: 1, pageSize: 4 });
      setFeatured(response.data.items || []);
    } catch (error) {
      console.error('Failed to load featured products', error);
    }
  };

  const handleAddToCart = (product) => {
    addToCart(product, 1);
    setMessage(`${product.name} đã được thêm vào giỏ hàng.`);
    setTimeout(() => setMessage(''), 2500);
  };

  return (
    <div className="space-y-12">
      <section className="overflow-hidden rounded-[2rem] bg-gradient-to-br from-slate-950 via-slate-900 to-slate-700 px-6 py-12 text-white shadow-soft sm:px-10">
        <div className="mx-auto max-w-7xl">
          <div className="grid gap-10 lg:grid-cols-[1.2fr_0.8fr] lg:items-center">
            <div className="space-y-6">
              <span className="inline-flex rounded-full bg-white/10 px-4 py-1 text-sm uppercase tracking-[0.35em] text-slate-200">
                New unisex collection
              </span>
              <h1 className="max-w-3xl text-4xl font-semibold leading-tight sm:text-5xl">
                Discover modern style for every body and every mood.
              </h1>
              <p className="max-w-2xl text-lg text-slate-200/90">
                SLAY STORE brings together contemporary essentials, sneakers, outerwear and accessories for a seamless unisex wardrobe.
              </p>
              <div className="flex flex-wrap gap-3">
                <button className="rounded-full bg-white px-6 py-3 text-sm font-semibold text-slate-950 shadow-lg shadow-slate-900/10 transition hover:bg-slate-100" onClick={() => navigate('/shop')}>
                  Shop collection
                </button>
                <button className="rounded-full border border-white/25 bg-white/10 px-6 py-3 text-sm font-semibold text-white transition hover:border-white hover:bg-white/15" onClick={() => setCategoryId('') }>
                  Explore all
                </button>
              </div>
            </div>

            <div className="grid gap-4 sm:grid-cols-2">
              <img src="https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&w=900&q=80" alt="Fashion hero" className="h-72 w-full rounded-[2rem] object-cover shadow-xl shadow-slate-950/20" />
              <div className="grid gap-4">
                <img src="https://images.unsplash.com/photo-1469334031218-e382a71b716b?auto=format&fit=crop&w=900&q=80" alt="Outfit" className="h-32 w-full rounded-[1.5rem] object-cover shadow-xl shadow-slate-950/20" />
                <img src="https://images.unsplash.com/photo-1516762689617-e1cffcef479d?auto=format&fit=crop&w=900&q=80" alt="Accessories" className="h-32 w-full rounded-[1.5rem] object-cover shadow-xl shadow-slate-950/20" />
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="grid gap-8 lg:grid-cols-[0.7fr_1.3fr]">
        <div className="space-y-6 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
          <div>
            <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Collections</p>
            <h2 className="mt-3 text-2xl font-semibold text-slate-900">Shop by category</h2>
          </div>
          <div className="grid gap-4">
            <button
              className={`rounded-3xl px-4 py-3 text-left text-sm font-medium transition ${categoryId === '' ? 'bg-slate-900 text-white' : 'bg-slate-100 text-slate-700 hover:bg-slate-200'}`}
              onClick={() => setCategoryId('')}
            >
              All categories
            </button>
            {categories.map((category) => (
              <button
                key={category.id}
                className={`rounded-3xl px-4 py-3 text-left text-sm font-medium transition ${categoryId === category.id.toString() ? 'bg-slate-900 text-white' : 'bg-slate-100 text-slate-700 hover:bg-slate-200'}`}
                onClick={() => setCategoryId(category.id.toString())}
              >
                {category.name}
              </button>
            ))}
          </div>
        </div>

        <div className="space-y-8">
          <div className="rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
            <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Search</p>
                <h2 className="mt-2 text-2xl font-semibold text-slate-900">Find your next outfit</h2>
              </div>
              <div className="flex flex-1 flex-col gap-3 sm:max-w-md sm:flex-row">
                <input
                  type="text"
                  value={keyword}
                  onChange={(e) => setKeyword(e.target.value)}
                  placeholder="Search by name, style or category"
                  className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-900 outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                />
                <button
                  onClick={() => loadProducts()}
                  className="rounded-3xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white transition hover:bg-slate-800"
                >
                  Search
                </button>
              </div>
            </div>
          </div>

          {message && (
            <div className="rounded-3xl bg-emerald-50 p-4 text-sm text-emerald-700 ring-1 ring-emerald-200">
              {message}
            </div>
          )}

          <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
            {loading ? (
              <div className="col-span-full rounded-[2rem] bg-white p-10 text-center shadow-soft ring-1 ring-slate-200">
                Loading products...
              </div>
            ) : products.length === 0 ? (
              <div className="col-span-full rounded-[2rem] bg-white p-10 text-center shadow-soft ring-1 ring-slate-200">
                <p className="text-lg font-semibold text-slate-900">No products found</p>
                <p className="mt-2 text-sm text-slate-500">Try another keyword or category filter.</p>
              </div>
            ) : (
              products.map((product) => (
                <article key={product.id} className="overflow-hidden rounded-[2rem] bg-white shadow-soft ring-1 ring-slate-200">
                  <div className="relative h-72 overflow-hidden bg-slate-100">
                    <img
                      src={product.imageUrl || 'https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&w=900&q=80'}
                      alt={product.name}
                      className="h-full w-full object-cover"
                    />
                  </div>
                  <div className="space-y-4 p-5">
                    <div className="flex items-center justify-between gap-3">
                      <div>
                        <p className="text-sm uppercase tracking-[0.3em] text-slate-500">{product.category?.name || 'Unisex'}</p>
                        <h3 className="mt-2 text-xl font-semibold text-slate-900">{product.name}</h3>
                      </div>
                      {product.isNew && <span className="rounded-full bg-slate-900 px-3 py-1 text-xs font-semibold uppercase text-white">New</span>}
                    </div>
                    <p className="max-h-16 overflow-hidden text-sm leading-6 text-slate-600">{product.description || 'A timeless piece for a clean and comfortable everyday wardrobe.'}</p>
                    <div className="flex items-center justify-between gap-3">
                      <div>
                        <p className="text-2xl font-semibold text-slate-900">{Number(product.price).toLocaleString()} đ</p>
                        {product.originalPrice && product.originalPrice > product.price && (
                          <p className="text-sm text-slate-500 line-through">{Number(product.originalPrice).toLocaleString()} đ</p>
                        )}
                      </div>
                      <div className="flex gap-2">
                        <button
                          onClick={() => navigate(`/product/${product.id}`)}
                          className="rounded-full border border-slate-200 bg-white px-4 py-2 text-sm font-semibold text-slate-900 transition hover:bg-slate-50"
                        >
                          View
                        </button>
                        <button
                          onClick={() => handleAddToCart(product)}
                          className="rounded-full bg-slate-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-slate-800"
                        >
                          Add to cart
                        </button>
                      </div>
                    </div>
                  </div>
                </article>
              ))
            )}
          </div>
        </div>
      </section>

      <section className="grid gap-6 rounded-[2rem] bg-white p-6 shadow-soft ring-1 ring-slate-200">
        <div className="flex items-center justify-between gap-4">
          <div>
            <p className="text-sm uppercase tracking-[0.3em] text-slate-500">Featured</p>
            <h2 className="mt-2 text-2xl font-semibold text-slate-900">Popular picks</h2>
          </div>
          <button className="rounded-full border border-slate-200 bg-slate-50 px-4 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-100" onClick={() => navigate('/shop')}>
            Browse all products
          </button>
        </div>
        <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-4">
          {featured.map((product) => (
            <div key={product.id} className="rounded-[2rem] border border-slate-200 p-5">
              <img
                src={product.imageUrl || 'https://images.unsplash.com/photo-1520975928447-d5e2a8bb2d18?auto=format&fit=crop&w=900&q=80'}
                alt={product.name}
                className="mb-4 h-44 w-full rounded-[1.5rem] object-cover"
              />
              <p className="text-sm uppercase tracking-[0.25em] text-slate-500">{product.category?.name || 'Unisex'}</p>
              <h3 className="mt-2 text-lg font-semibold text-slate-900">{product.name}</h3>
              <p className="mt-2 text-sm text-slate-600 max-h-12 overflow-hidden">{product.description}</p>
              <p className="mt-4 text-base font-semibold text-slate-900">{Number(product.price).toLocaleString()} đ</p>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
};

export default StoreHome;
