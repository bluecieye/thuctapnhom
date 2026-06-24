

import React, { createContext, useContext, useState, useEffect } from 'react';

import { authApi } from '../services/api';

// ════════════════════════════════════════════════════════════
// CONTEXT XÁC THỰC
// ════════════════════════════════════════════════════════════

// ════════════════════════════════════════════════════════════
// CONTEXT INSTANCE
// ════════════════════════════════════════════════════════════
const AuthContext = createContext(null);

// ════════════════════════════════════════════════════════════
// HOOK SỬ DỤNG
// ════════════════════════════════════════════════════════════
export const useAuth = () => {
    
    const context = useContext(AuthContext);

    
    if (!context) throw new Error('useAuth must be used within an AuthProvider');

    return context;
};

// ════════════════════════════════════════════════════════════
// COMPONENT: AUTH PROVIDER
// ════════════════════════════════════════════════════════════
export const AuthProvider = ({ children }) => {

    // ════════════════════════════════════════════════════════════
    // STATE
    // ════════════════════════════════════════════════════════════
    const [user, setUser] = useState(null);



    const [loading, setLoading] = useState(true);

    // ════════════════════════════════════════════════════════════
    // EFFECT: KHỞI TẠO
    // ════════════════════════════════════════════════════════════
    useEffect(() => {

        const storedUser = localStorage.getItem('user');

        const token = localStorage.getItem('token');

        if (storedUser && token) {
            try {
                
                const parsed = JSON.parse(storedUser);

                
                setUser({ ...parsed, token });
            } catch {

                localStorage.removeItem('user');
                localStorage.removeItem('token');
            }
        }

        
        setLoading(false);
    }, []); 

    

    // ════════════════════════════════════════════════════════════
    // HÀM API
    // ════════════════════════════════════════════════════════════
    const login = async (username, password) => {
        try {

            const response = await authApi.login(username, password);

            
            const data = response.data;

            
            
            const userData = {
                id: data.userId,           
                userId: data.userId,       
                username: data.username,   
                email: data.email,         
                role: data.role,           
            };

            
            
            localStorage.setItem('token', data.token);

            localStorage.setItem('user', JSON.stringify(userData));

            setUser({ ...userData, token: data.token });

            return { success: true, user: userData };
        } catch (error) {

            const message = error.response?.data?.message
                || (error.code === 'ECONNABORTED' || !error.response
                    ? 'Không kết nối được đến máy chủ. Vui lòng kiểm tra backend đang chạy.'
                    : 'Đăng nhập thất bại.');

            return { success: false, message };
        }
    };

    

    
    const register = async (data) => {
        try {

            await authApi.register(data);

            return { success: true };
        } catch (error) {
            
            const message = error.response?.data?.message || 'Registration failed';
            return { success: false, message };
        }
    };

    

    
    const logout = () => {
        localStorage.removeItem('token');  
        localStorage.removeItem('user');   
        setUser(null);                     
    };

    

    
    // ════════════════════════════════════════════════════════════
    // HÀM PHỤ TRỢ
    // ════════════════════════════════════════════════════════════
    const isAdmin = () => user?.role === 'Admin';

    

    
    
    const isStaff = () => ['Admin', 'WarehouseStaff', 'Marketing'].includes(user?.role);

    

    // ════════════════════════════════════════════════════════════
    // VALUE PROVIDER & RETURN
    // ════════════════════════════════════════════════════════════
    const value = {
        user,
        login,
        register,
        logout,                     
        isAdmin,                    
        isStaff,                    
        isAuthenticated: !!user,    
                                    
        loading,                    
    };

    

    

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export default AuthContext;
