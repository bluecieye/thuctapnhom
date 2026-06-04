

import { useEffect, useState } from 'react';

import { adminService } from '../../services/adminService';

import { AdminTable } from './AdminTable';
import { AdminModal, FormField, inputClass } from './AdminModal';

import { includesAccentInsensitive } from '../../utils/slug';

import { sortByIdDesc } from '../../utils/format';

// ════════════════════════════════════════════════════════════
// SECTION QUẢN LÝ DANH MỤC
// ════════════════════════════════════════════════════════════
export const CategoriesSection = () => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════

  
  const [items, setItems] = useState([]);

  const [keyword, setKeyword] = useState('');

  const [loading, setLoading] = useState(true);

  
  const [editing, setEditing] = useState(null);

  const [showModal, setShowModal] = useState(false);

  const [formData, setFormData] = useState({ name: '', description: '' });

  const [error, setError] = useState('');

  const [submitting, setSubmitting] = useState(false);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    load();
  }, []);

  // ════════════════════════════════════════════════════════════
  // TẢI DỮ LIỆU
  // ════════════════════════════════════════════════════════════
  const load = async () => {
    setLoading(true);
    try {
      const data = await adminService.getCategories();
      setItems(sortByIdDesc(data));
    } catch (e) {
      setItems([]);
    } finally {
      setLoading(false);
    }
  };

  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const filtered = items.filter(
    (c) =>
      !keyword ||
      includesAccentInsensitive(c.name, keyword) ||
      (c.description && includesAccentInsensitive(c.description, keyword))
  );

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const openCreate = () => {
    setEditing(null);
    setFormData({ name: '', description: '' });
    setError('');
    setShowModal(true);
  };

  

  
  const openEdit = (c) => {
    setEditing(c);
    setFormData({ name: c.name, description: c.description || '' });
    setError('');
    setShowModal(true);
  };

  

  const submit = async (e) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);
    try {
      if (editing) {

        await adminService.updateCategory(editing.id, { id: editing.id, ...formData });
      } else {
        
        await adminService.createCategory(formData);
      }
      
      setShowModal(false);
      load();
    } catch (err) {
      
      setError(err.response?.data?.message || 'Thao tác thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  
  
  const handleDelete = async (c) => {

    if (!window.confirm(`Delete category "${c.name}"? Linked products may block this.`)) return;
    try {
      await adminService.deleteCategory(c.id);
      load(); 
    } catch {
      
      alert('Failed to delete. The category may have associated products.');
    }
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div className="space-y-4">
      {}
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold text-slate-900">Danh mục</h2>
        <button
          onClick={openCreate}
          className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800"
        >
          + Thêm danh mục
        </button>
      </div>

      {}
      {}
      <div className="rounded-2xl border border-slate-200 bg-white p-3">
        <input
          className={`${inputClass} w-full`}
          placeholder="Tìm theo tên hoặc mô tả..."
          value={keyword}
          onChange={(e) => setKeyword(e.target.value)}
        />
      </div>

      {}
      {loading ? (
        
        <p className="rounded-2xl border border-slate-200 bg-white p-6 text-center text-slate-500">
          Loading...
        </p>
      ) : (

        <>
          <p className="text-sm text-slate-500">
            Showing {filtered.length} of {items.length} categories
          </p>

          {}
          {}
          <AdminTable
            columns={[
              { key: 'id', label: 'ID', style: { width: 80 } },
              {
                key: 'name',
                label: 'Name',
                
                render: (c) => <span className="font-medium">{c.name}</span>,
              },
              
              { key: 'description', label: 'Description', render: (c) => c.description || '—' },
              {
                key: 'actions',
                label: 'Actions',
                
                render: (c) => (
                  <div className="flex gap-2">
                    <button
                      onClick={() => openEdit(c)}
                      className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(c)}
                      className="rounded-lg bg-rose-50 px-3 py-1.5 text-xs font-semibold text-rose-700 ring-1 ring-rose-200 hover:bg-rose-100"
                    >
                      Delete
                    </button>
                  </div>
                ),
              },
            ]}
            rows={filtered}
            emptyText="Không có danh mục nào"
          />
        </>
      )}

      {}
      <AdminModal
        open={showModal}
        
        title={editing ? 'Edit category' : 'Create category'}
        onClose={() => setShowModal(false)}
        footer={
          
          <>
            <button
              type="button"
              onClick={() => setShowModal(false)}
              className="rounded-full border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-700 hover:bg-slate-50"
            >
              Cancel
            </button>

            {}
            {}
            {}
            <button
              type="submit"
              form="category-form"
              disabled={submitting}
              className="rounded-full bg-slate-900 px-5 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:bg-slate-400"
            >
              {submitting ? 'Saving...' : editing ? 'Update' : 'Create'}
            </button>
          </>
        }
      >
        {}
        <form id="category-form" onSubmit={submit}>
          {}
          {error && (
            <p className="mb-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{error}</p>
          )}

          {}
          <FormField label="Tên" required>
            <input
              className={inputClass}
              required
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            />
          </FormField>

          {}
          <FormField label="Mô tả">
            <textarea
              rows="3"
              className={inputClass}
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            />
          </FormField>
        </form>
      </AdminModal>
    </div>
  );
};

export default CategoriesSection;
