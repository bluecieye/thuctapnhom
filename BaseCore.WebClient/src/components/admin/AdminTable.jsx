

// ════════════════════════════════════════════════════════════
// COMPONENT BẢNG DỮ LIỆU ADMIN
// ════════════════════════════════════════════════════════════
export const AdminTable = ({ columns, rows, emptyText = 'Không có dữ liệu', rowKey = 'id' }) => (

  <div className="overflow-x-auto rounded-2xl border border-slate-200 bg-white">
    <table className="w-full text-sm">
      {}
      {}
      <thead className="bg-slate-50 text-left text-xs uppercase tracking-wider text-slate-500">
        <tr>
          {}
          {}
          {}
          {columns.map((c) => (
            <th key={c.key} className="px-4 py-3 font-semibold" style={c.style}>
              {c.label}
            </th>
          ))}
        </tr>
      </thead>

      {}
      {}
      <tbody className="divide-y divide-slate-100">
        {rows.length === 0 ? (

          <tr>
            <td colSpan={columns.length} className="px-4 py-10 text-center text-slate-400">
              {emptyText}
            </td>
          </tr>
        ) : (
          
          rows.map((row, idx) => (

            
            <tr key={row[rowKey] ?? idx} className="hover:bg-slate-50">
              {}
              {}
              {}
              {columns.map((c) => (
                <td key={c.key} className="px-4 py-3 align-top text-slate-700">
                  {c.render ? c.render(row) : row[c.key]}
                </td>
              ))}
            </tr>
          ))
        )}
      </tbody>
    </table>
  </div>
);

// ════════════════════════════════════════════════════════════
// COMPONENT PHÂN TRANG ADMIN
// ════════════════════════════════════════════════════════════
export const AdminPagination = ({ page, totalPages, onChange }) => {

  if (!totalPages || totalPages <= 1) return null;

  // ════════════════════════════════════════════════════════════
  // RENDER
  // ════════════════════════════════════════════════════════════
  return (

    <div className="mt-4 flex items-center justify-end gap-2 text-sm">
      {}
      {}
      {}
      <button
        onClick={() => onChange(Math.max(1, page - 1))}
        disabled={page === 1}
        className="rounded-lg border border-slate-200 px-3 py-1.5 disabled:opacity-50"
      >
        Trước
      </button>

      {}
      <span className="text-slate-500">
        Trang {page} / {totalPages}
      </span>

      {}
      {}
      <button
        onClick={() => onChange(Math.min(totalPages, page + 1))}
        disabled={page === totalPages}
        className="rounded-lg border border-slate-200 px-3 py-1.5 disabled:opacity-50"
      >
        Sau
      </button>
    </div>
  );
};
