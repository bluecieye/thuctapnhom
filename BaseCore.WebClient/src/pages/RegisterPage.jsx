

import { useState } from 'react';

import { Link, useNavigate } from 'react-router-dom';

import { useAuth } from '../contexts/AuthContext';

// ════════════════════════════════════════════════════════════
// TRANG ĐĂNG KÝ
// ════════════════════════════════════════════════════════════
const RegisterPage = () => {
  // ════════════════════════════════════════════════════════════
  // CONTEXT & HOOK
  // ════════════════════════════════════════════════════════════
  const navigate = useNavigate();

  const { register } = useAuth();

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════

  const [formData, setFormData] = useState({
    username: '',
    password: '',
    confirmPassword: '',
    email: '',
    phone: '',
  });

  const [error, setError] = useState('');

  const [loading, setLoading] = useState(false);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const handleSubmit = async (e) => {
    
    e.preventDefault();
    setError('');

    if (formData.password.length < 6) {
      setError('Mật khẩu phải có ít nhất 6 ký tự.');
      return;
    }

    if (formData.password !== formData.confirmPassword) {
      setError('Mật khẩu không khớp.');
      return;
    }

    setLoading(true);

    
    
    if (!formData.email) {
      setError('Email là bắt buộc.');
      setLoading(false);
      return;
    }

    
    const result = await register({
      username: formData.username,
      password: formData.password,
      email: formData.email,
      phone: formData.phone,
    });
    setLoading(false);

    if (result.success) {
      
      navigate('/login?registered=true');
    } else {
      
      setError(result.message || 'Đăng ký thất bại.');
    }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="mx-auto max-w-md px-4 py-20">
      <h1 className="font-serif text-3xl font-light text-center">Tạo Tài Khoản</h1>

      {/* ════════════════════════════════════════════════════════════
          RENDER: FORM
          ════════════════════════════════════════════════════════════ */}
      <form onSubmit={handleSubmit} className="mt-8 space-y-4">
        {}
        {error && <p className="text-red-500 text-sm text-center">{error}</p>}

        {}
        <div>
          <label className="block text-sm font-medium mb-1">Tên đăng nhập *</label>
          <input
            type="text"
            value={formData.username}
            onChange={(e) => setFormData({ ...formData, username: e.target.value })}
            required
            className="w-full border border-gray-200 px-4 py-3 text-sm focus:border-black outline-none"
          />
        </div>

        {}
        <div>
          <label className="block text-sm font-medium mb-1">Email *</label>
          <input
            type="email"
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
            required
            className="w-full border border-gray-200 px-4 py-3 text-sm focus:border-black outline-none"
          />
        </div>

        {}
        <div>
          <label className="block text-sm font-medium mb-1">Số điện thoại</label>
          <input
            type="text"
            value={formData.phone}
            onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
            className="w-full border border-gray-200 px-4 py-3 text-sm focus:border-black outline-none"
          />
        </div>

        {}
        <div>
          <label className="block text-sm font-medium mb-1">Mật khẩu *</label>
          <input
            type="password"
            value={formData.password}
            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
            required
            minLength={6}
            className="w-full border border-gray-200 px-4 py-3 text-sm focus:border-black outline-none"
          />
        </div>

        {}
        <div>
          <label className="block text-sm font-medium mb-1">Xác nhận mật khẩu *</label>
          <input
            type="password"
            value={formData.confirmPassword}
            onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
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
          {loading ? 'Đang tạo tài khoản...' : 'Đăng Ký'}
        </button>

        {}
        <p className="text-center text-sm text-gray-500">
          Đã có tài khoản?{' '}
          <Link to="/login" className="text-black underline">
            Đăng nhập
          </Link>
        </p>
      </form>
    </div>
  );
};

export default RegisterPage;
