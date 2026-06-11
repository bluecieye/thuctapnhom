

// ════════════════════════════════════════════════════════════
// COMPONENT BUTTON (DÙNG CHUNG)
// ════════════════════════════════════════════════════════════
export const Button = ({
  children,
  variant = 'primary',
  size = 'md',
  className = '',
  ...props
}) => {

  

  

  
  const baseClasses =
    'inline-flex items-center justify-center font-medium transition-colors focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed';

  

  
  
  const variants = {
    
    primary: 'bg-black text-white hover:bg-gray-800',
    
    secondary: 'border border-gray-300 bg-white text-gray-700 hover:bg-gray-50',
    
    outline: 'border border-black text-black hover:bg-black hover:text-white',
  };

  

  
  const sizes = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2 text-sm',
    lg: 'px-6 py-3 text-base',
  };







  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <button
      className={`${baseClasses} ${variants[variant]} ${sizes[size]} ${className}`}
      {...props}
    >
      {}
      {children}
    </button>
  );
};

export default Button;
