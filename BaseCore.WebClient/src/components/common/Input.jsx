

// ════════════════════════════════════════════════════════════
// COMPONENT INPUT (DÙNG CHUNG)
// ════════════════════════════════════════════════════════════
export const Input = ({ label, error, className = '', ...props }) => {
  return (

    <div className="w-full">
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {label && (
        <label className="mb-1 block text-sm font-medium text-gray-700">{label}</label>
      )}

      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      {}
      <input
        className={`w-full border border-gray-200 px-3 py-2 text-sm outline-none focus:border-black ${className}`}
        {...props}
      />

      {}
      {}
      {}
      {}
      {}
      {}
      {error && <p className="mt-1 text-xs text-red-500">{error}</p>}
    </div>
  );
};

export default Input;
