

import { createContext, useContext, useState, useEffect, useCallback } from 'react';

import { wishlistApi } from '../services/api';

import { useAuth } from './AuthContext';

// ════════════════════════════════════════════════════════════
// CONTEXT DANH SÁCH YÊU THÍCH
// ════════════════════════════════════════════════════════════

// ════════════════════════════════════════════════════════════
// CONTEXT INSTANCE
// ════════════════════════════════════════════════════════════
const WishlistContext = createContext();

// ════════════════════════════════════════════════════════════
// HOOK SỬ DỤNG
// ════════════════════════════════════════════════════════════
export const useWishlist = () => useContext(WishlistContext);

const GUEST_KEY = 'wishlist';

const readGuest = () => {
  try {
    return JSON.parse(localStorage.getItem(GUEST_KEY) || '[]');
  } catch {
    
    return [];
  }
};

const writeGuest = (items) => localStorage.setItem(GUEST_KEY, JSON.stringify(items));

// ════════════════════════════════════════════════════════════
// COMPONENT: WISHLIST PROVIDER
// ════════════════════════════════════════════════════════════
export const WishlistProvider = ({ children }) => {

  const { isAuthenticated } = useAuth();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [items, setItems] = useState([]);

  const [loading, setLoading] = useState(false);

  

  


  // ════════════════════════════════════════════════════════════
  // HÀM API
  // ════════════════════════════════════════════════════════════
  const refresh = useCallback(async () => {
    
    if (!isAuthenticated) {
      setItems(readGuest());
      return;
    }

    setLoading(true);
    try {
      
      const { data } = await wishlistApi.getMine();

      
      setItems(data.map((w) => ({ ...w.product, wishlistId: w.id })));
    } catch (e) {
      
      console.error('Wishlist fetch failed', e);
    } finally {
      
      setLoading(false);
    }
  }, [isAuthenticated]);

  

  

  

  

  // ════════════════════════════════════════════════════════════
  // EFFECT: KHỞI TẠO
  // ════════════════════════════════════════════════════════════
  useEffect(() => { refresh(); }, [refresh]);

  

  

  

  

  
  useEffect(() => {
    
    if (!isAuthenticated) return;

    const guest = readGuest();

    if (guest.length === 0) return;

    (async () => {

      for (const p of guest) {

        try { await wishlistApi.add(p.id); } catch {}
      }
      
      writeGuest([]);
      
      refresh();
    })();
  }, [isAuthenticated, refresh]);

  

  

  

  
  const addToWishlist = async (product) => {
    
    if (isAuthenticated) {
      
      await wishlistApi.add(product.id);

      await refresh();
      return;
    }

    
    const guest = readGuest();
    
    if (!guest.find((i) => i.id === product.id)) {
      
      const next = [...guest, product];
      
      writeGuest(next);
      setItems(next);
    }
  };

  
  
  const removeFromWishlist = async (productId) => {
    
    if (isAuthenticated) {
      await wishlistApi.remove(productId);
      await refresh();  
      return;
    }

    const next = readGuest().filter((i) => i.id !== productId);
    writeGuest(next);
    setItems(next);
  };

  

  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const isInWishlist = (productId) => items.some((i) => i.id === productId);


  // ════════════════════════════════════════════════════════════
  // VALUE PROVIDER & RETURN
  // ════════════════════════════════════════════════════════════
  return (
    <WishlistContext.Provider
      value={{
        wishlistItems: items,        
        loading,                     
        addToWishlist,               
        removeFromWishlist,          
        isInWishlist,                
        refresh,                     
      }}
    >
      {children}
    </WishlistContext.Provider>
  );
};
