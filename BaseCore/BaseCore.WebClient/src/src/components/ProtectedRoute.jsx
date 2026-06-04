

import { Navigate } from 'react-router-dom';

import { useAuth } from '../contexts/AuthContext';

// ════════════════════════════════════════════════════════════
// COMPONENT ROUTE BẢO VỆ (PHÂN QUYỀN)
// ════════════════════════════════════════════════════════════

export const ProtectedRoute = ({ children }) => {

    
    
    const { isAuthenticated, loading } = useAuth();

    

    if (loading) {
        return (

            

            <div className="flex h-[60vh] items-center justify-center">
                {}
                <div className="h-8 w-8 animate-spin rounded-full border-2 border-black border-t-transparent" />
            </div>
        );
    }

    

    
    if (!isAuthenticated) return <Navigate to="/login" replace />;

    
    return children;
};

// ════════════════════════════════════════════════════════════
// COMPONENT: ADMIN ROUTE
// ════════════════════════════════════════════════════════════
export const AdminRoute = ({ children }) => {

    
    const { isAuthenticated, isStaff, loading } = useAuth();

    
    
    if (loading) {
        return (
            <div className="flex h-[60vh] items-center justify-center">
                <div className="h-8 w-8 animate-spin rounded-full border-2 border-black border-t-transparent" />
            </div>
        );
    }

    if (!isAuthenticated) return <Navigate to="/login" replace />;

    

    if (!isStaff()) return <Navigate to="/" replace />;

    return children;
};

export default ProtectedRoute;
