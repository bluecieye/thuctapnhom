

// ════════════════════════════════════════════════════════════
// TIỆN ÍCH FORMAT HIỂN THỊ
// ════════════════════════════════════════════════════════════

// ════════════════════════════════════════════════════════════
// FORMAT SỐ
// ════════════════════════════════════════════════════════════
export const fmt = (n) => Number(n || 0).toLocaleString('vi-VN');

// ════════════════════════════════════════════════════════════
// FORMAT NGÀY
// ════════════════════════════════════════════════════════════
export const fmtDate = (d) => (d ? new Date(d).toLocaleDateString('vi-VN') : '—');

export const fmtDateTime = (d) => (d ? new Date(d).toLocaleString('vi-VN') : '—');

// ════════════════════════════════════════════════════════════
// ẢNH PLACEHOLDER
// ════════════════════════════════════════════════════════════
export const IMAGE_PLACEHOLDER =
  'data:image/svg+xml;charset=utf-8,' +
  encodeURIComponent(
    '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 500" preserveAspectRatio="xMidYMid slice">' +
    '<rect width="400" height="500" fill="#f1f5f9"/>' +
    '<text x="50%" y="50%" font-family="Georgia, serif" font-size="20" fill="#94a3b8" text-anchor="middle" dominant-baseline="middle">Chưa có ảnh</text>' +
    '</svg>'
  );

// ════════════════════════════════════════════════════════════
// SẮP XẾP
// ════════════════════════════════════════════════════════════
export const sortByIdDesc = (arr) =>
  (arr || []).slice().sort((a, b) => (b.id || 0) - (a.id || 0));
