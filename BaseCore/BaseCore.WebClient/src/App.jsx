

// ════════════════════════════════════════════════════════════
// COMPONENT GỐC & ĐỊNH TUYẾN (ROUTES)
// ════════════════════════════════════════════════════════════

import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';

import React from 'react';

import { AuthProvider } from './contexts/AuthContext';
import { CartProvider } from './contexts/CartContext';
import { WishlistProvider } from './contexts/WishlistContext';

import { ProtectedRoute, AdminRoute } from './components/ProtectedRoute';

import Layout from './components/layout/Layout';

// ════════════════════════════════════════════════════════════
// IMPORT PROVIDERS & PAGES
// ════════════════════════════════════════════════════════════

import HomePage from './pages/HomePage';
import ProductListPage from './pages/ProductListPage';
import ProductDetailPage from './pages/ProductDetailPage';
import CartPage from './pages/CartPage';
import CheckoutPage from './pages/CheckoutPage';
import OrderConfirmationPage from './pages/OrderConfirmationPage';
import OrderTrackingPage from './pages/OrderTrackingPage';
import WishlistPage from './pages/WishlistPage';
import AccountPage from './pages/AccountPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import AdminDashboardPage from './pages/AdminDashboardPage';
import NotFoundPage from './pages/NotFoundPage';

// ════════════════════════════════════════════════════════════
// ĐỊNH NGHĨA ROUTES
// ════════════════════════════════════════════════════════════
function AppRoutes() {
  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    <Routes>
      {}
      {}
      <Route element={<Layout />}>

        {/* ════════════════════════════════════════════════════════════ */}
        {/* ROUTES CÔNG KHAI                                              */}
        {/* ════════════════════════════════════════════════════════════ */}

        {}
        <Route path="/" element={<HomePage />} />

        {}
        <Route path="/shop" element={<ProductListPage />} />

        {}
        <Route path="/product/:id" element={<ProductDetailPage />} />

        {}
        <Route path="/cart" element={<CartPage />} />

        {/* ════════════════════════════════════════════════════════════ */}
        {/* ROUTES YÊU CẦU LOGIN                                          */}
        {/* ════════════════════════════════════════════════════════════ */}

        {}
        <Route
          path="/checkout"
          element={
            <ProtectedRoute>
              <CheckoutPage />
            </ProtectedRoute>
          }
        />

        {}
        <Route path="/order-confirmation/:orderId" element={<OrderConfirmationPage />} />

        {}
        <Route path="/my-orders" element={<OrderTrackingPage />} />

        {}
        <Route path="/wishlist" element={<WishlistPage />} />

        {}
        <Route
          path="/account"
          element={
            <ProtectedRoute>
              <AccountPage />
            </ProtectedRoute>
          }
        />

        {}
        <Route path="/login" element={<LoginPage />} />

        {}
        <Route path="/register" element={<RegisterPage />} />
      </Route>

      {}
      {}

      {/* ════════════════════════════════════════════════════════════ */}
      {/* ROUTES ADMIN                                                  */}
      {/* ════════════════════════════════════════════════════════════ */}

      {}
      <Route
        path="/admin"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/products"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/categories"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/coupons"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/orders"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/users"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/inventory"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/reviews"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/images"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/sizeguides"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/carriers"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      <Route
        path="/admin/rates"
        element={
          <AdminRoute>
            <AdminDashboardPage />
          </AdminRoute>
        }
      />

      {}
      {}
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

// ════════════════════════════════════════════════════════════
// COMPONENT: APP
// ════════════════════════════════════════════════════════════
function App() {
  // ════════════════════════════════════════════════════════════
  // RENDER: CÂY PROVIDER & ROUTER
  // ════════════════════════════════════════════════════════════
  return (

    <Router>
      {/* ════════════════════════════════════════════════════════════ */}
      {/* WRAPPER PROVIDERS                                             */}
      {/* ════════════════════════════════════════════════════════════ */}

      {}
      <AuthProvider>
        {}
        <CartProvider>
          {}
          <WishlistProvider>
            {}
            <AppRoutes />
          </WishlistProvider>
        </CartProvider>
      </AuthProvider>
    </Router>
  );
}

export default App;
