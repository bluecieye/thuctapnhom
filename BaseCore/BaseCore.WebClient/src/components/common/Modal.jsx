

import { X } from 'lucide-react';

// ════════════════════════════════════════════════════════════
// COMPONENT MODAL (DÙNG CHUNG)
// ════════════════════════════════════════════════════════════
export const Modal = ({ isOpen, onClose, title, children }) => {

  

  


  if (!isOpen) return null;



  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    

    

    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      <div className="max-h-[90vh] w-full max-w-lg overflow-y-auto bg-white p-6">
        {}
        {}
        {}
        {}
        {}
        <div className="flex items-center justify-between">
          {}
          <h3 className="text-xl font-medium">{title}</h3>

          {}
          {}
          {}
          {}
          <button onClick={onClose}><X size={20} /></button>
        </div>

        {}
        {}
        {}
        {}
        {}
        <div className="mt-4">{children}</div>
      </div>
    </div>
  );
};

export default Modal;
