

import { useEffect, useState } from 'react';
import { adminService } from '../../services/adminService';

import { AdminTable, AdminPagination } from './AdminTable';
import { AdminModal, FormField, inputClass } from './AdminModal';
import { SearchableSelect } from './SearchableSelect';

const ROLES = ['Customer', 'WarehouseStaff', 'Marketing', 'Admin'];

const empty = {
  username: '',
  email: '',
  phone: '',
  password: '',
  role: 'Customer',
  isActive: true,
};

const ROLE_BADGE = {
  Customer:       'bg-slate-100 text-slate-700',
  WarehouseStaff: 'bg-blue-100 text-blue-700',
  Marketing:      'bg-purple-100 text-purple-700',
  Admin:          'bg-emerald-100 text-emerald-700',
};

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ NGƯỜI DÙNG
// ════════════════════════════════════════════════════════════
export const UsersSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);

  const [keyword, setKeyword] = useState('');           
  const [filterRole, setFilterRole] = useState('');     
  const [filterStatus, setFilterStatus] = useState(''); 

  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);                       
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);       

  const [editing, setEditing] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState(empty);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => { load(); }, [page, keyword, filterRole, filterStatus]);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = async () => {
    setLoading(true);
    try {
      const data = await adminService.getUsers({
        keyword,
        page,
        pageSize,
        
        role: filterRole || undefined,

        isActive: filterStatus !== '' ? filterStatus === 'true' : undefined,
      });
      
      setItems(data.data || []);
      setTotalPages(data.totalPages || 0);
      setTotalCount(data.totalCount || 0);
    } catch {
      
      setItems([]);
    } finally {
      setLoading(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const openCreate = () => { setEditing(null); setFormData(empty); setError(''); setShowModal(true); };

  
  const openEdit = (u) => {
    setEditing(u);
    setFormData({
      username: u.username,
      email: u.email || '',
      phone: u.phone || '',
      password: '',
      role: u.role,
      isActive: u.isActive,
    });
    setError('');
    setShowModal(true);
  };

  

  
  
  const submit = async (e) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);
    try {
      if (editing) {
        
        const body = {
          email: formData.email,
          phone: formData.phone,
          role: formData.role,
          isActive: formData.isActive,
        };

        if (formData.password) body.password = formData.password;
        await adminService.updateUser(editing.id, body);
      } else {
        
        await adminService.createUser({
          username: formData.username,
          email: formData.email,
          phone: formData.phone,
          password: formData.password,
          role: formData.role,
        });
      }
      setShowModal(false);
      load();
    } catch (err) {
      setError(err.response?.data?.message || 'Lưu thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (u) => {
    if (!window.confirm(`Vô hiệu hoá user "${u.username}"?`)) return;
    try {
      await adminService.deleteUser(u.id);
      load();
    } catch { alert('Thao tác thất bại.'); }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Người dùng</h2>
        <button onClick={openCreate} className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800">
          + Thêm user
        </button>
      </div>

      {}
      <div className="flex flex-wrap gap-2 rounded-2xl border border-slate-200 bg-white p-3">
        {}
        <input className={`${inputClass} flex-1 min-w-[180px]`}
          placeholder="Tìm theo username hoặc email..." value={keyword}
          onChange={(e) => { setPage(1); setKeyword(e.target.value); }} />
        <SearchableSelect
          className="w-44"
          value={filterRole}
          onChange={(val) => { setPage(1); setFilterRole(val); }}
          options={ROLES.map((r) => ({ value: r, label: r }))}
          placeholder="Tất cả vai trò"
        />
        <SearchableSelect
          className="w-40"
          value={filterStatus}
          onChange={(val) => { setPage(1); setFilterStatus(val); }}
          options={[{ value: 'true', label: 'Active' }, { value: 'false', label: 'Inactive' }]}
          placeholder="Tất cả"
        />
      </div>

      {}
      {loading ? (
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">Đang tải...</p>
      ) : (
        <>
          {}
          <p className="text-sm text-slate-500">Tổng: {totalCount} user</p>
          <AdminTable
            columns={[
              { key: 'id', label: 'ID', style: { width: 70 } },
              
              { key: 'username', label: 'Username', render: (u) => <span className="font-medium">@{u.username}</span> },
              { key: 'email', label: 'Email' },
              
              { key: 'phone', label: 'SĐT', render: (u) => u.phone || '—' },
              {
                key: 'role', label: 'Vai trò',
                
                render: (u) => (
                  <span className={`rounded-full px-2 py-1 text-xs font-semibold ${ROLE_BADGE[u.role] || 'bg-slate-100'}`}>
                    {u.role}
                  </span>
                ),
              },
              {
                key: 'status', label: 'Trạng thái',
                
                render: (u) => (
                  <span className={`rounded-full px-2 py-1 text-xs font-semibold ${
                    u.isActive ? 'bg-emerald-50 text-emerald-700' : 'bg-rose-50 text-rose-700'
                  }`}>
                    {u.isActive ? 'Active' : 'Inactive'}
                  </span>
                ),
              },
              {
                key: 'actions', label: 'Thao tác',
                render: (u) => (
                  <div className="flex gap-2">
                    <button onClick={() => openEdit(u)} className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800">
                      Sửa
                    </button>
                    <button onClick={() => handleDelete(u)} className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100">
                      Vô hiệu
                    </button>
                  </div>
                ),
              },
            ]}
            rows={items}
            emptyText="Không có người dùng nào"
          />
          {}
          <AdminPagination page={page} totalPages={totalPages} onChange={setPage} />
        </>
      )}

      {}
      <AdminModal
        open={showModal}
        title={editing ? 'Sửa user' : 'Thêm user'}
        onClose={() => setShowModal(false)}
        footer={
          <>
            <button type="button" onClick={() => setShowModal(false)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50">
              Huỷ
            </button>
            <button type="submit" form="user-form" disabled={submitting}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400">
              {submitting ? 'Đang lưu...' : editing ? 'Cập nhật' : 'Thêm'}
            </button>
          </>
        }
      >
        <form id="user-form" onSubmit={submit}>
          {error && <p className="mb-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>}

          {}
          {!editing && (
            <FormField label="Tên đăng nhập" required>
              <input className={inputClass} required value={formData.username}
                onChange={(e) => setFormData({ ...formData, username: e.target.value })} />
            </FormField>
          )}

          <FormField label="Email" required>
            <input type="email" className={inputClass} required value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })} />
          </FormField>

          <FormField label="Số điện thoại">
            <input className={inputClass} value={formData.phone}
              onChange={(e) => setFormData({ ...formData, phone: e.target.value })} />
          </FormField>

          {}
          <FormField label={editing ? 'Mật khẩu mới (để trống nếu không đổi)' : 'Mật khẩu'} required={!editing}>
            <input type="password" className={inputClass} required={!editing} minLength={editing ? 0 : 6}
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })} />
          </FormField>

          {}
          <div className="grid grid-cols-2 gap-4">
            <FormField label="Vai trò" required>
              <select className={inputClass} value={formData.role}
                onChange={(e) => setFormData({ ...formData, role: e.target.value })}>
                {ROLES.map((r) => <option key={r} value={r}>{r}</option>)}
              </select>
            </FormField>

            {}
            {editing && (
              <FormField label="Trạng thái">
                <label className="flex items-center gap-2 pt-3">
                  <input type="checkbox" checked={formData.isActive}
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })} />
                  <span className="text-sm text-slate-700">Đang hoạt động</span>
                </label>
              </FormField>
            )}
          </div>
        </form>
      </AdminModal>
    </div>
  );
};

export default UsersSection;
