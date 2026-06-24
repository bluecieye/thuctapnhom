

import { useState, useEffect } from 'react';

import { Link, useNavigate, useSearchParams } from 'react-router-dom';

import { useAuth } from '../contexts/AuthContext';

// ════════════════════════════════════════════════════════════
// TRANG ĐĂNG NHẬP
// ════════════════════════════════════════════════════════════
const LoginPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const navigate = useNavigate();

  const [searchParams] = useSearchParams();


  const { login, isAdmin, user } = useAuth();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [formData, setFormData] = useState({ username: '', password: '' });

  const [error, setError] = useState('');

  const [loading, setLoading] = useState(false);

  const [notice, setNotice] = useState('');

  // ════════════════════════════════════════════════════════════
  // EFFECT: TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    if (searchParams.get('registered')) {
      setNotice('Đăng ký thành công. Vui lòng đăng nhập.');
    }
    if (sessionStorage.getItem('sessionExpired')) {
      sessionStorage.removeItem('sessionExpired');
      setNotice('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.');
    }
  }, [searchParams]);



  useEffect(() => {
    if (user) navigate(isAdmin() ? '/admin' : '/');
  }, [user, isAdmin, navigate]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const handleSubmit = async (e) => {

    e.preventDefault();
    setError('');
    setLoading(true);

    const result = await login(formData.username, formData.password);
    setLoading(false);

    if (result.success) {


      navigate(result.user?.role === 'Admin' ? '/admin' : '/');
    } else {

      setError(result.message || 'Tên đăng nhập hoặc mật khẩu không đúng');
    }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="mx-auto max-w-md px-4 py-20">
      <h1 className="font-serif text-3xl font-light text-center">Đăng Nhập</h1>

      {}
      {notice && (
        <p className="mt-4 rounded bg-emerald-50 px-3 py-2 text-sm text-emerald-700 ring-1 ring-emerald-200 text-center">
          {notice}
        </p>
      )}

      {/* ════════════════════════════════════════════════════════════
          RENDER: FORM
          ════════════════════════════════════════════════════════════ */}
      <form onSubmit={handleSubmit} className="mt-8 space-y-6">
        {}
        {error && <p className="text-red-500 text-sm text-center">{error}</p>}

        {}
        <div>
          <label className="block text-sm font-medium mb-1">Tên đăng nhập</label>
          <input
            type="text"
            value={formData.username}
            onChange={(e) => setFormData({ ...formData, username: e.target.value })}
            required
            autoFocus
            className="w-full border border-gray-200 px-4 py-3 text-sm focus:border-black outline-none"
          />
        </div>

        {}
        <div>
          <label className="block text-sm font-medium mb-1">Mật khẩu</label>
          <input
            type="password"
            value={formData.password}
            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
            required
            className="w-full border border-gray-200 px-4 py-3 text-sm focus:border-black outline-none"
          />
        </div>

        {}
        <button
          type="submit"
          disabled={loading}
          className="w-full bg-black py-3 text-white hover:bg-gray-800 disabled:bg-gray-400"
        >
          {loading ? 'Đang đăng nhập...' : 'Đăng Nhập'}
        </button>

        {}
        <p className="text-center text-sm text-gray-500">
          Chưa có tài khoản?{' '}
          <Link to="/register" className="text-black underline">
            Đăng ký ngay
          </Link>
        </p>
      </form>
    </div>
  );
};

export default LoginPage;
