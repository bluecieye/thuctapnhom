

import { Link } from 'react-router-dom';

// ════════════════════════════════════════════════════════════
// TRANG 404
// ════════════════════════════════════════════════════════════
const NotFoundPage = () => {
  return (
    
    <div className="flex h-[60vh] flex-col items-center justify-center text-center">
      {}
      <h1 className="font-serif text-6xl font-light">404</h1>

      {}
      <p className="mt-4 text-gray-500">Không tìm thấy trang</p>

      {}
      <Link to="/" className="mt-8 underline">
        Về trang chủ
      </Link>
    </div>
  );
};

export default NotFoundPage;
