

import { createContext, useContext, useCallback, useState } from 'react';

import { createPortal } from 'react-dom';

import { CheckCircle2, AlertCircle, X } from 'lucide-react';

// ════════════════════════════════════════════════════════════
// CONTEXT THÔNG BÁO NHANH (TOAST)
// ════════════════════════════════════════════════════════════
const ToastContext = createContext(null);

// Trả về no-op nếu dùng ngoài provider (an toàn, không vỡ app).
export const useToast = () => useContext(ToastContext) || { show: () => {} };

let nextId = 1;

export const ToastProvider = ({ children }) => {
  const [toasts, setToasts] = useState([]);

  const remove = useCallback((id) => {
    setToasts((list) => list.filter((t) => t.id !== id));
  }, []);

  // show(message, type) — type: 'success' | 'error'
  const show = useCallback((message, type = 'success') => {
    const id = nextId++;
    setToasts((list) => [...list, { id, message, type }]);
    setTimeout(() => remove(id), 2600);
  }, [remove]);

  return (
    <ToastContext.Provider value={{ show }}>
      {children}
      {createPortal(
        <div className="pointer-events-none fixed bottom-6 left-1/2 z-[100] flex -translate-x-1/2 flex-col items-center gap-2">
          {toasts.map((t) => (
            <div
              key={t.id}
              className={`pointer-events-auto flex items-center gap-3 rounded-full px-5 py-3 text-sm text-white shadow-lg ring-1 ${
                t.type === 'error'
                  ? 'bg-rose-600 ring-rose-700/30'
                  : 'bg-slate-900 ring-black/10'
              }`}
            >
              {t.type === 'error' ? <AlertCircle size={18} /> : <CheckCircle2 size={18} className="text-emerald-400" />}
              <span className="font-medium">{t.message}</span>
              <button
                type="button"
                onClick={() => remove(t.id)}
                aria-label="Đóng"
                className="-mr-1 rounded-full p-0.5 hover:bg-white/15"
              >
                <X size={14} />
              </button>
            </div>
          ))}
        </div>,
        document.body
      )}
    </ToastContext.Provider>
  );
};

export default ToastContext;
