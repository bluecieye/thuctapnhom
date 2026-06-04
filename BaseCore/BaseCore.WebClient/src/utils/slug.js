

// ════════════════════════════════════════════════════════════
// TIỆN ÍCH SLUG
// ════════════════════════════════════════════════════════════

// ════════════════════════════════════════════════════════════
// HÀM SLUGIFY
// ════════════════════════════════════════════════════════════
export const slugify = (input) => {

    if (!input) return '';

    
    return String(input)                  
        .trim()                           
        .toLowerCase()                    

        
        
        .normalize('NFD')

        
        
        .replace(/[̀-ͯ]/g, '')

        

        .replace(/[đĐ]/g, 'd')

        
        
        .replace(/[^a-z0-9\s-]/g, '')

        
        
        .replace(/[\s-]+/g, '-')

        
        
        .replace(/^-+|-+$/g, '');
};

// ════════════════════════════════════════════════════════════
// XỬ LÝ DẤU TIẾNG VIỆT
// ════════════════════════════════════════════════════════════
export const stripAccents = (input) => {
    if (input == null) return '';
    return String(input)
        .toLowerCase()
        .normalize('NFD')              
        .replace(/[̀-ͯ]/g, '')           
        .replace(/[đĐ]/g, 'd');        
};

export const includesAccentInsensitive = (haystack, needle) => {
    const n = stripAccents(needle).trim();
    if (!n) return true;
    return stripAccents(haystack).includes(n);
};

// ════════════════════════════════════════════════════════════
// TẠO MÃ SKU
// ════════════════════════════════════════════════════════════
export const buildSku = (productSlug, colorName, sizeName) => {

    

    const parts = [productSlug, slugify(colorName), slugify(sizeName)].filter(Boolean);

    
    
    return parts.join('-');
};
