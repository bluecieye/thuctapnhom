

// ════════════════════════════════════════════════════════════
// COMPONENT MODAL ADMIN (DÙNG CHUNG)
// ════════════════════════════════════════════════════════════
export const AdminModal = ({ open, title, onClose, children, footer, size = 'md' }) => {

  if (!open) return null;




  const widths = { sm: 'max-w-md', md: 'max-w-lg', lg: 'max-w-2xl', xl: 'max-w-4xl' };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    

    
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/50 p-4">
      {}
      {}
      {}
      <div className={`w-full ${widths[size] || widths.md} rounded-2xl bg-white shadow-2xl`}>

        {}
        {}
        {}
        <div className="flex items-center justify-between border-b border-slate-200 px-5 py-3">
          <h3 className="text-lg font-semibold text-slate-900">{title}</h3>

          {}
          <button
            type="button"
            onClick={onClose}
            className="rounded-full p-1.5 text-slate-400 transition hover:bg-slate-100 hover:text-slate-700"
          >
            ✕
          </button>
        </div>

        {}
        {}
        {}
        <div className="max-h-[70vh] overflow-y-auto px-5 py-4">{children}</div>

        {}
        {}
        {}
        {footer && (
          <div className="flex justify-end gap-2 border-t border-slate-200 px-5 py-3">{footer}</div>
        )}
      </div>
    </div>
  );
};

// ════════════════════════════════════════════════════════════
// COMPONENT TRƯỜNG FORM (DÙNG CHUNG)
// ════════════════════════════════════════════════════════════
export const FormField = ({ label, required, children, hint }) => (
  
  <div className="mb-4">
    {}
    <label className="mb-1.5 block text-sm font-medium text-slate-700">
      {label}
      {}
      {required && <span className="text-rose-500"> *</span>}
    </label>

    {}
    {children}

    {}
    {hint && <p className="mt-1 text-xs text-slate-500">{hint}</p>}
  </div>
);

// ════════════════════════════════════════════════════════════
// HẰNG SỐ DÙNG CHUNG
// ════════════════════════════════════════════════════════════
export const inputClass =
  'w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:border-slate-400 focus:ring-2 focus:ring-slate-200';
