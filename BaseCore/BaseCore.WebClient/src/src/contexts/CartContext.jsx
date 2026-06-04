

import { createContext, useContext, useState, useEffect, useCallback } from 'react';

import { cartApi } from '../services/api';

import { slugify } from '../utils/slug';

import { useAuth } from './AuthContext';

// ════════════════════════════════════════════════════════════
// CONTEXT GIỎ HÀNG
// ════════════════════════════════════════════════════════════

// ════════════════════════════════════════════════════════════
// CONTEXT INSTANCE
// ════════════════════════════════════════════════════════════
const CartContext = createContext();

// ════════════════════════════════════════════════════════════
// HOOK SỬ DỤNG
// ════════════════════════════════════════════════════════════
export const useCart = () => useContext(CartContext);

const GUEST_KEY = 'basecore_cart_guest';

const readGuestCart = () => {
  try {
    return JSON.parse(localStorage.getItem(GUEST_KEY) || '[]');
  } catch {
    
    return [];
  }
};

const writeGuestCart = (items) => localStorage.setItem(GUEST_KEY, JSON.stringify(items));

// ════════════════════════════════════════════════════════════
// COMPONENT: CART PROVIDER
// ════════════════════════════════════════════════════════════
export const CartProvider = ({ children }) => {

  const { isAuthenticated } = useAuth();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [cart, setCart] = useState(null);

  

  const [guestItems, setGuestItems] = useState(readGuestCart);

  const [loading, setLoading] = useState(false);

  

  

  

  


  // ════════════════════════════════════════════════════════════
  // HÀM API
  // ════════════════════════════════════════════════════════════
  const refresh = useCallback(async () => {

    if (!isAuthenticated) {
      setCart(null);
      return;
    }
    
    setLoading(true);
    try {
      
      const { data } = await cartApi.get();
      
      setCart(data);
    } catch (e) {
      
      console.error('Cart fetch failed', e);
    } finally {
      
      setLoading(false);
    }
  }, [isAuthenticated]);

  

  

  


  // ════════════════════════════════════════════════════════════
  // EFFECT: KHỞI TẠO
  // ════════════════════════════════════════════════════════════
  useEffect(() => { refresh(); }, [refresh]);

  

  

  

  
  
  useEffect(() => {

    
    if (!isAuthenticated || guestItems.length === 0) return;

    
    
    (async () => {

      for (const it of guestItems) {

        try { await cartApi.addItem(it.variantId, it.quantity); } catch {}
      }
      
      writeGuestCart([]);
      setGuestItems([]);
      
      refresh();
    })();
  }, [isAuthenticated, guestItems, refresh]);

  

  
  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const persistGuest = (items) => {
    setGuestItems(items);
    writeGuestCart(items);
  };

  

  

  

  

  

  
  const addToCart = async (variant, product, quantity = 1) => {
    
    if (isAuthenticated) {

      const { data } = await cartApi.addItem(variant.id, quantity);
      setCart(data);
      return;
    }

    
    const existing = guestItems.find((i) => i.variantId === variant.id);

    const newItems = existing
      
      ? guestItems.map((i) =>
          i.variantId === variant.id ? { ...i, quantity: i.quantity + quantity } : i
        )

      
      : [
          ...guestItems,
          {
            variantId: variant.id,
            quantity,
            
            sku: variant.sku || variant.SKU,
            price: variant.price || variant.Price,
            productId: product?.id,
            productName: product?.name,
            
            colorName: variant.color?.name || variant.colorName,
            sizeName: variant.size?.name || variant.sizeName,

            imageUrl: (() => {
              const fileName = product?.images?.[0]?.fileName;
              if (!fileName || !product) return '';
              const slug = product.slug || slugify(product.name);
              return slug ? `/images/products/${slug}/${fileName}` : '';
            })(),
            productSlug: product?.slug,
          },
        ];

    persistGuest(newItems);
  };

  

  
  
  const updateQuantity = async (cartItemIdOrVariantId, quantity) => {
    
    if (quantity < 1) return;

    if (isAuthenticated) {
      const { data } = await cartApi.updateItem(cartItemIdOrVariantId, quantity);
      setCart(data);
      return;
    }

    persistGuest(
      guestItems.map((i) =>
        i.variantId === cartItemIdOrVariantId ? { ...i, quantity } : i
      )
    );
  };

  
  
  const removeFromCart = async (cartItemIdOrVariantId) => {
    
    if (isAuthenticated) {
      const { data } = await cartApi.removeItem(cartItemIdOrVariantId);
      setCart(data);
      return;
    }

    persistGuest(guestItems.filter((i) => i.variantId !== cartItemIdOrVariantId));
  };

  
  
  const clearCart = async () => {
    
    if (isAuthenticated) {
      
      await cartApi.clear();

      setCart({ ...cart, cartItems: [] });
      return;
    }
    
    persistGuest([]);
  };

  

  

  

  

  // ════════════════════════════════════════════════════════════
  // VALUE PROVIDER & RETURN
  // ════════════════════════════════════════════════════════════
  const items = isAuthenticated

    
    ? (cart?.cartItems || []).map((i) => ({
        cartItemId: i.id,
        variantId: i.variantId,
        quantity: i.quantity,
        sku: i.variant?.sku,
        price: i.variant?.price,

        stock: i.variant?.stock - i.variant?.reservedStock,
        productId: i.variant?.product?.id,
        productName: i.variant?.product?.name,
        colorName: i.variant?.color?.name,
        sizeName: i.variant?.size?.name,
        
        imageUrl: i.variant?.product?.images?.[0]?.fileName && i.variant?.product?.slug
          ? `/images/products/${i.variant.product.slug}/${i.variant.product.images[0].fileName}` : '',
        productSlug: i.variant?.product?.slug,
      }))
    
    : guestItems.map((i) => ({ ...i, cartItemId: null }));

  
  const totalPrice = items.reduce((sum, i) => sum + (i.price || 0) * i.quantity, 0);

  const totalItems = items.reduce((sum, i) => sum + (i.quantity || 0), 0);

  return (
    <CartContext.Provider
      value={{
        cart,                  
        cartItems: items,      
        loading,               
        addToCart,             
        updateQuantity,        
        removeFromCart,        
        clearCart,             
        refresh,               
        totalPrice,            
        totalItems,            
      }}
    >
      {children}
    </CartContext.Provider>
  );
};
