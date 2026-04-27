import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Login = () => {
  const [tab, setTab] = useState('login');

  // Login state
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  // Register state
  const [regUsername, setRegUsername] = useState('');
  const [regName, setRegName] = useState('');
  const [regEmail, setRegEmail] = useState('');
  const [regPhone, setRegPhone] = useState('');
  const [regPassword, setRegPassword] = useState('');
  const [regConfirm, setRegConfirm] = useState('');

  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();
  const { login, register } = useAuth();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    const result = await login(username, password);

    if (result.success) {
      setSuccess('Đăng nhập thành công! Đang chuyển hướng...');
      setTimeout(() => {
        const storedUser = JSON.parse(localStorage.getItem('user') || '{}');
        if (storedUser?.role === 'Admin') {
          navigate('/admin');
        } else {
          navigate('/');
        }
      }, 1000);
    } else {
      setError(result.message || 'Tên đăng nhập hoặc mật khẩu không đúng.');
    }

    setLoading(false);
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (regPassword !== regConfirm) {
      setError('Mật khẩu xác nhận không khớp.');
      return;
    }
    if (regPassword.length < 6) {
      setError('Mật khẩu phải có ít nhất 6 ký tự.');
      return;
    }

    setLoading(true);

    const result = await register({
      username: regUsername,
      name: regName,
      email: regEmail,
      phone: regPhone,
      password: regPassword,
    });

    if (result.success) {
      setSuccess('Đăng ký thành công! Vui lòng đăng nhập.');
      setRegUsername(''); setRegName(''); setRegEmail('');
      setRegPhone(''); setRegPassword(''); setRegConfirm('');
      setTimeout(() => {
        setTab('login');
        setSuccess('');
      }, 2000);
    } else {
      setError(result.message || 'Đăng ký thất bại. Vui lòng thử lại.');
    }

    setLoading(false);
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-950/5 px-4 py-10">
      <div className="w-full max-w-xl rounded-[2rem] bg-white p-10 shadow-soft ring-1 ring-slate-200">

        <div className="mb-8 text-center">
          <p className="text-sm uppercase tracking-[0.3em] text-slate-500">SLAY STORE</p>
          <h1 className="mt-4 text-3xl font-semibold text-slate-900">
            {tab === 'login' ? 'Chào mừng trở lại' : 'Tạo tài khoản'}
          </h1>
          <p className="mt-2 text-sm text-slate-500">
            {tab === 'login'
              ? 'Đăng nhập để tiếp tục mua sắm.'
              : 'Tạo tài khoản để trải nghiệm mua sắm tốt hơn.'}
          </p>
        </div>

        {/* Tabs */}
        <div className="mb-6 flex rounded-2xl bg-slate-100 p-1">
          <button
            className={`flex-1 rounded-xl py-2.5 text-sm font-semibold transition ${tab === 'login' ? 'bg-white text-slate-900 shadow-sm' : 'text-slate-500 hover:text-slate-700'}`}
            onClick={() => { setTab('login'); setError(''); setSuccess(''); }}
          >
            Đăng nhập
          </button>
          <button
            className={`flex-1 rounded-xl py-2.5 text-sm font-semibold transition ${tab === 'register' ? 'bg-white text-slate-900 shadow-sm' : 'text-slate-500 hover:text-slate-700'}`}
            onClick={() => { setTab('register'); setError(''); setSuccess(''); }}
          >
            Đăng ký
          </button>
        </div>

        {/* Notifications */}
        {error && (
          <div className="mb-5 rounded-2xl bg-red-50 px-5 py-4 text-sm text-red-700 ring-1 ring-red-200">
            {error}
          </div>
        )}
        {success && (
          <div className="mb-5 rounded-2xl bg-emerald-50 px-5 py-4 text-sm text-emerald-700 ring-1 ring-emerald-200">
            {success}
          </div>
        )}

        {/* Login Form */}
        {tab === 'login' && (
          <form className="space-y-5" onSubmit={handleLogin}>
            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">Tên đăng nhập</label>
              <input
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Nhập tên đăng nhập"
                className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                required
              />
            </div>
            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">Mật khẩu</label>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Nhập mật khẩu"
                className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                required
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="inline-flex w-full items-center justify-center rounded-3xl bg-slate-900 px-4 py-3 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:bg-slate-400"
            >
              {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
            </button>
            <p className="text-center text-sm text-slate-500">
              Chưa có tài khoản?{' '}
              <button type="button" className="font-semibold text-slate-900 hover:underline" onClick={() => { setTab('register'); setError(''); setSuccess(''); }}>
                Đăng ký ngay
              </button>
            </p>
          </form>
        )}

        {/* Register Form */}
        {tab === 'register' && (
          <form className="space-y-4" onSubmit={handleRegister}>
            <div className="grid gap-4 sm:grid-cols-2">
              <div>
                <label className="mb-2 block text-sm font-medium text-slate-700">Tên đăng nhập <span className="text-red-500">*</span></label>
                <input
                  type="text"
                  value={regUsername}
                  onChange={(e) => setRegUsername(e.target.value)}
                  placeholder="vd: john_doe"
                  className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                  required
                />
              </div>
              <div>
                <label className="mb-2 block text-sm font-medium text-slate-700">Họ tên</label>
                <input
                  type="text"
                  value={regName}
                  onChange={(e) => setRegName(e.target.value)}
                  placeholder="vd: Nguyễn Văn A"
                  className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                />
              </div>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              <div>
                <label className="mb-2 block text-sm font-medium text-slate-700">Email</label>
                <input
                  type="email"
                  value={regEmail}
                  onChange={(e) => setRegEmail(e.target.value)}
                  placeholder="vd: email@gmail.com"
                  className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                />
              </div>
              <div>
                <label className="mb-2 block text-sm font-medium text-slate-700">Số điện thoại</label>
                <input
                  type="tel"
                  value={regPhone}
                  onChange={(e) => setRegPhone(e.target.value)}
                  placeholder="vd: 0901234567"
                  className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                />
              </div>
            </div>
            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">Mật khẩu <span className="text-red-500">*</span></label>
              <input
                type="password"
                value={regPassword}
                onChange={(e) => setRegPassword(e.target.value)}
                placeholder="Ít nhất 6 ký tự"
                className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                required
              />
            </div>
            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">Xác nhận mật khẩu <span className="text-red-500">*</span></label>
              <input
                type="password"
                value={regConfirm}
                onChange={(e) => setRegConfirm(e.target.value)}
                placeholder="Nhập lại mật khẩu"
                className="w-full rounded-3xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200"
                required
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="inline-flex w-full items-center justify-center rounded-3xl bg-slate-900 px-4 py-3 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:bg-slate-400"
            >
              {loading ? 'Đang đăng ký...' : 'Tạo tài khoản'}
            </button>
            <p className="text-center text-sm text-slate-500">
              Đã có tài khoản?{' '}
              <button type="button" className="font-semibold text-slate-900 hover:underline" onClick={() => { setTab('login'); setError(''); setSuccess(''); }}>
                Đăng nhập
              </button>
            </p>
          </form>
        )}
      </div>
    </div>
  );
};

export default Login;
