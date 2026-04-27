const CART_KEY = 'fashion_store_cart';

export const getCartItems = () => {
    try {
        return JSON.parse(localStorage.getItem(CART_KEY) || '[]');
    } catch (error) {
        return [];
    }
};

export const saveCartItems = (items) => {
    localStorage.setItem(CART_KEY, JSON.stringify(items));
    window.dispatchEvent(new Event('storage'));
};

export const getCartCount = () => {
    return getCartItems().reduce((sum, item) => sum + (item.quantity || 0), 0);
};

export const addToCart = (product, quantity = 1) => {
    const items = getCartItems();
    const existingItem = items.find((item) => item.id === product.id);

    if (existingItem) {
        existingItem.quantity += quantity;
    } else {
        items.push({
            id: product.id,
            name: product.name,
            price: product.price,
            imageUrl: product.imageUrl,
            category: product.category,
            quantity,
        });
    }

    saveCartItems(items);
    return items;
};

export const updateCartQuantity = (productId, quantity) => {
    const items = getCartItems().map((item) =>
        item.id === productId ? { ...item, quantity } : item
    );
    saveCartItems(items);
    return items;
};

export const removeCartItem = (productId) => {
    const items = getCartItems().filter((item) => item.id !== productId);
    saveCartItems(items);
    return items;
};

export const clearCart = () => {
    saveCartItems([]);
};
