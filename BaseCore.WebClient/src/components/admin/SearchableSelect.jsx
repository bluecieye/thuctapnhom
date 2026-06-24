

import { useEffect, useMemo, useRef, useState } from 'react';
import { ChevronDown } from 'lucide-react';
import { inputClass } from './AdminModal';

import { includesAccentInsensitive } from '../../utils/slug';

// ════════════════════════════════════════════════════════════
// COMPONENT SELECT CÓ TÌM KIẾM
// ════════════════════════════════════════════════════════════
export const SearchableSelect = ({
  value,
  onChange,
  options = [],
  placeholder = 'Tất cả',
  allowEmpty = true,
  className = '',
}) => {

  // ════════════════════════════════════════════════════════════
  // STATE
  // ════════════════════════════════════════════════════════════
  const [query, setQuery] = useState('');
  const [open, setOpen] = useState(false);
  const [highlight, setHighlight] = useState(0);

  const wrapRef = useRef(null);
  
  const inputRef = useRef(null);

  
  const selected = options.find((o) => String(o.value) === String(value));

  // ════════════════════════════════════════════════════════════
  // HÀM PHỤ TRỢ
  // ════════════════════════════════════════════════════════════
  const filtered = useMemo(() => {
    if (!query.trim()) return options;
    return options.filter((o) => includesAccentInsensitive(o.label, query));
  }, [query, options]);

  // ════════════════════════════════════════════════════════════
  // EFFECT
  // ════════════════════════════════════════════════════════════
  useEffect(() => {
    if (!open) return;
    const onClickOutside = (e) => {
      if (wrapRef.current && !wrapRef.current.contains(e.target)) {
        setOpen(false);
        setQuery('');
      }
    };
    document.addEventListener('mousedown', onClickOutside);
    return () => document.removeEventListener('mousedown', onClickOutside);
  }, [open]);

  useEffect(() => { if (open) setHighlight(0); }, [open]);

  // ════════════════════════════════════════════════════════════
  // HÀM XỬ LÝ
  // ════════════════════════════════════════════════════════════
  const pick = (val) => {
    onChange(val);
    setOpen(false);
    setQuery('');
  };

  
  
  const onKeyDown = (e) => {
    if (e.key === 'ArrowDown') {
      e.preventDefault(); setOpen(true);
      setHighlight((h) => Math.min(h + 1, filtered.length - 1));
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      setHighlight((h) => Math.max(h - 1, 0));
    } else if (e.key === 'Enter') {
      e.preventDefault();
      const opt = filtered[highlight];
      if (opt) pick(opt.value);
    } else if (e.key === 'Escape') {
      setOpen(false); setQuery('');
    }
  };

  
  const displayValue = open ? query : (selected ? selected.label : '');

  const toggleOpen = () => {
    setOpen((o) => {
      const next = !o;
      
      if (!next) setQuery('');
      else inputRef.current?.focus();
      return next;
    });
  };

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (
    <div ref={wrapRef} className={`relative ${className}`}>
      {}
      {}
      <input
        ref={inputRef}
        type="text"
        className={`${inputClass} pr-9`}
        value={displayValue}
        placeholder={placeholder}
        onFocus={() => setOpen(true)}
        onChange={(e) => { setQuery(e.target.value); setOpen(true); }}
        onKeyDown={onKeyDown}
      />

      {}
      {}
      <button
        type="button"
        tabIndex={-1}
        aria-label="Mở danh sách"
        onMouseDown={(e) => { e.preventDefault(); toggleOpen(); }}
        className="absolute inset-y-0 right-0 flex items-center pr-2 text-slate-400 hover:text-slate-700"
      >
        <ChevronDown size={16} className={`transition-transform ${open ? 'rotate-180' : ''}`} />
      </button>

      {}
      {open && (
        <div className="absolute z-20 mt-1 max-h-64 w-full overflow-auto rounded-xl border border-slate-200 bg-white py-1 shadow-lg">
          {}
          {allowEmpty && (
            <button
              type="button"
              onMouseDown={(e) => { e.preventDefault(); pick(''); }}
              className={`block w-full px-3 py-1.5 text-left text-sm hover:bg-slate-100 ${
                value === '' || value === undefined || value === null
                  ? 'bg-slate-50 font-semibold text-slate-900'
                  : 'text-slate-600'
              }`}
            >
              {placeholder}
            </button>
          )}

          {filtered.length === 0 ? (
            <p className="px-3 py-2 text-sm text-slate-400">Không tìm thấy</p>
          ) : (
            filtered.map((o, idx) => (
              <button
                key={o.value}
                type="button"

                onMouseDown={(e) => { e.preventDefault(); pick(o.value); }}
                onMouseEnter={() => setHighlight(idx)}
                className={`block w-full px-3 py-1.5 text-left text-sm hover:bg-slate-100 ${
                  idx === highlight ? 'bg-slate-100' : ''
                } ${
                  String(o.value) === String(value) ? 'font-semibold text-slate-900' : 'text-slate-700'
                }`}
              >
                {o.label}
              </button>
            ))
          )}
        </div>
      )}
    </div>
  );
};

export default SearchableSelect;
