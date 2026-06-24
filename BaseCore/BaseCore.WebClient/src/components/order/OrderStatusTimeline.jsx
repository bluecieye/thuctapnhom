

import { Check, Clock, Package, Truck, Home, XCircle } from 'lucide-react';

// ════════════════════════════════════════════════════════════
// TIMELINE TRẠNG THÁI ĐƠN HÀNG (stepper trực quan)
// ════════════════════════════════════════════════════════════
// Luồng chuẩn: Pending → Processing → Shipping → Delivered.
// "Cancelled" hiển thị riêng (không nằm trong tiến trình).
const STEPS = [
  { key: 'Pending',    label: 'Chờ xác nhận', icon: Clock },
  { key: 'Processing', label: 'Đang xử lý',   icon: Package },
  { key: 'Shipping',   label: 'Đang giao',    icon: Truck },
  { key: 'Delivered',  label: 'Đã giao',      icon: Home },
];

export const OrderStatusTimeline = ({ status }) => {
  // Đơn đã huỷ: hiển thị thông báo riêng thay vì tiến trình.
  if (status === 'Cancelled') {
    return (
      <div className="flex items-center gap-2 rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700 ring-1 ring-rose-200">
        <XCircle size={18} />
        <span className="font-medium">Đơn hàng đã huỷ</span>
      </div>
    );
  }

  const currentIdx = Math.max(0, STEPS.findIndex((s) => s.key === status));

  return (
    <div className="flex items-center">
      {STEPS.map((step, idx) => {
        const done = idx < currentIdx;
        const active = idx === currentIdx;
        const Icon = step.icon;
        return (
          <div key={step.key} className="flex flex-1 items-center last:flex-none">
            {/* Nút tròn trạng thái */}
            <div className="flex flex-col items-center">
              <div
                className={`flex h-9 w-9 items-center justify-center rounded-full ring-1 transition-colors ${
                  done
                    ? 'bg-emerald-500 text-white ring-emerald-500'
                    : active
                      ? 'bg-slate-900 text-white ring-slate-900'
                      : 'bg-white text-slate-300 ring-slate-200'
                }`}
              >
                {done ? <Check size={16} /> : <Icon size={16} />}
              </div>
              <span
                className={`mt-1.5 max-w-[64px] text-center text-[11px] leading-tight ${
                  active ? 'font-semibold text-slate-900' : 'text-slate-400'
                }`}
              >
                {step.label}
              </span>
            </div>
            {/* Đường nối giữa các bước */}
            {idx < STEPS.length - 1 && (
              <div className={`mx-1 h-0.5 flex-1 ${idx < currentIdx ? 'bg-emerald-500' : 'bg-slate-200'}`} />
            )}
          </div>
        );
      })}
    </div>
  );
};

export default OrderStatusTimeline;
