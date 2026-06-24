

import { createContext, useContext, useState, useEffect, useCallback, useRef } from 'react';

import { cartApi } from '../services/api';

import { slugify } from '../utils/slug';

import { useAuth } from './AuthContext';

import { useToast } from './ToastContext';

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

  const { show: showToast } = useToast();

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

  

  

  

  
  // Capture latest guestItems without making it a sync effect dependency.
  const guestItemsRef = useRef(guestItems);
  useEffect(() => { guestItemsRef.current = guestItems; });

  useEffect(() => {
    if (!isAuthenticated) return;
    const itemsToSync = guestItemsRef.current;
    if (itemsToSync.length === 0) return;

    let cancelled = false;
    (async () => {
      const synced = [];
      let lastCart = null;
      for (const it of itemsToSync) {
        if (cancelled) return;
        try {
          const { data } = await cartApi.addItem(it.variantId, it.quantity);
          synced.push(it.variantId);
          lastCart = data;
        } catch (e) {
          const msg = e.response?.data?.message
            || `Không thể đồng bộ "${it.productName || 'sản phẩm'}" vào giỏ hàng.`;
          showToast(msg, 'error');
        }
      }
      if (cancelled) return;
      const remaining = itemsToSync.filter((i) => !synced.includes(i.variantId));
      writeGuestCart(remaining);
      setGuestItems(remaining);
      if (lastCart) setCart(lastCart);
      else refresh();
    })();

    return () => { cancelled = true; };
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAuthenticated, refresh]);

  

  
  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const persistGuest = (items) => {
    setGuestItems(items);
    writeGuestCart(items);
  };

  

  

  

  

  

  
  const addToCart = async (variant, product, quantity = 1) => {

    if (isAuthenticated) {
      try {
        const { data } = await cartApi.addItem(variant.id, quantity);
        setCart(data);
        showToast('Đã thêm vào giỏ hàng');
      } catch (e) {
        const msg = e.response?.data?.message || 'Không thêm được vào giỏ hàng.';
        showToast(msg, 'error');
        throw e;
      }
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
            price: (variant.price || variant.Price || 0) * (1 - (product?.discountPercent || 0) / 100),
            productId: product?.id,
            productName: product?.name,
            colorName: variant.color?.name || variant.colorName,
            sizeName: variant.size?.name || variant.sizeName,
            imageUrl: (() => {
              const imgs = product?.images || [];
              const slug = product?.slug || slugify(product?.name || '');
              const primary = imgs.find((img) => img.isPrimary) || imgs[0];
              if (!primary?.fileName || !slug) return '';
              return `/images/products/${slug}/${primary.fileName}`;
            })(),
            productSlug: product?.slug,
          },
        ];

    persistGuest(newItems);
    showToast('Đã thêm vào giỏ hàng');
  };

  

  
  
  const updateQuantity = async (cartItemIdOrVariantId, quantity) => {

    if (quantity < 1) return;

    if (isAuthenticated) {
      // If this is a pending guest item (not yet synced), update locally.
      if (guestItems.some((i) => i.variantId === cartItemIdOrVariantId)) {
        persistGuest(guestItems.map((i) =>
          i.variantId === cartItemIdOrVariantId ? { ...i, quantity } : i
        ));
        return;
      }
      try {
        const { data } = await cartApi.updateItem(cartItemIdOrVariantId, quantity);
        setCart(data);
      } catch (e) {
        showToast(e.response?.data?.message || 'Cập nhật số lượng thất bại.', 'error');
      }
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
      // If this is a pending guest item (not yet synced), remove locally.
      if (guestItems.some((i) => i.variantId === cartItemIdOrVariantId)) {
        persistGuest(guestItems.filter((i) => i.variantId !== cartItemIdOrVariantId));
        return;
      }
      try {
        const { data } = await cartApi.removeItem(cartItemIdOrVariantId);
        setCart(data);
      } catch (e) {
        showToast(e.response?.data?.message || 'Xóa sản phẩm thất bại.', 'error');
      }
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
  const serverItems = isAuthenticated
    ? (cart?.cartItems || []).map((i) => {
        const disc = i.variant?.product?.discountPercent ?? 0;
        const factor = 1 - disc / 100;
        const imgs = i.variant?.product?.images || [];
        const slug = i.variant?.product?.slug;
        const primary = imgs.find((img) => img.isPrimary) || imgs[0];
        return {
          cartItemId: i.id,
          variantId: i.variantId,
          quantity: i.quantity,
          sku: i.variant?.sku,
          price: (i.variant?.price ?? 0) * factor,
          stock: i.variant?.stock - i.variant?.reservedStock,
          productId: i.variant?.product?.id,
          productName: i.variant?.product?.name,
          colorName: i.variant?.color?.name,
          sizeName: i.variant?.size?.name,
          imageUrl: primary?.fileName && slug
            ? `/images/products/${slug}/${primary.fileName}` : '',
          productSlug: slug,
        };
      })
    : [];

  // Guest items that haven't synced yet (shown while sync is in progress or failed).
  const syncedVariantIds = new Set(serverItems.map((i) => i.variantId));
  const pendingGuestItems = guestItems
    .filter((i) => !syncedVariantIds.has(i.variantId))
    .map((i) => ({ ...i, cartItemId: null }));

  const items = isAuthenticated
    ? [...serverItems, ...pendingGuestItems]
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
