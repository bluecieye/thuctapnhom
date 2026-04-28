import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import React from 'react';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import MainLayout from './components/MainLayout';
import StoreLayout from './components/StoreLayout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Products from './pages/Products';
import Users from './pages/Users';
import Categories from './pages/Categories';
import StoreHome from './pages/StoreHome';
import StoreProducts from './pages/StoreProducts';
import StoreProductDetail from './pages/StoreProductDetail';
import StoreCart from './pages/StoreCart';
import StoreOrders from './pages/StoreOrders';
import Manufacturers from './pages/Manufacturers';

function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<StoreLayout><StoreHome /></StoreLayout>} />
            <Route path="/shop" element={<StoreLayout><StoreProducts /></StoreLayout>} />
            <Route path="/product/:id" element={<StoreLayout><StoreProductDetail /></StoreLayout>} />
            <Route path="/cart" element={<StoreLayout><StoreCart /></StoreLayout>} />
            <Route path="/my-orders" element={<StoreLayout><StoreOrders /></StoreLayout>} />
            <Route path="/login" element={<Login />} />
            <Route
                path="/admin"
                element={
                    <ProtectedRoute>
                        <MainLayout>
                            <Dashboard />
                        </MainLayout>
                    </ProtectedRoute>
                }
            />
            <Route
                path="/admin/products"
                element={
                    <ProtectedRoute>
                        <MainLayout>
                            <Products />
                        </MainLayout>
                    </ProtectedRoute>
                }
            />
            <Route
                path="/admin/categories"
                element={
                    <ProtectedRoute>
                        <MainLayout>
                            <Categories />
                        </MainLayout>
                    </ProtectedRoute>
                }
            />
            <Route
                path="/admin/users"
                element={
                    <ProtectedRoute adminOnly={true}>
                        <MainLayout>
                            <Users />
                        </MainLayout>
                    </ProtectedRoute>
                }
            />
            <Route
                path="/admin/manufacturers"
                element={
                    <ProtectedRoute>
                        <MainLayout>
                            <Manufacturers />
                        </MainLayout>
                    </ProtectedRoute>
                }
            />
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    );
}

function App() {
    return (
        <Router>
            <AuthProvider>
                <AppRoutes />
            </AuthProvider>
        </Router>
    );
}

export default App;
