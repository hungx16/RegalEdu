// utils/format.ts
export function formatDate(
  value: string | Date,
  format: string = 'YYYY-MM-DD HH:mm:ss'
): string {
  if (!value) return ''
  const date = typeof value === 'string' ? new Date(value) : value
  if (isNaN(date.getTime())) return ''

  // Padding helper
  const pad = (num: number, size: number = 2) => num.toString().padStart(size, '0')

  const map: Record<string, string> = {
    YYYY: date.getFullYear().toString(),
    MM: pad(date.getMonth() + 1),
    DD: pad(date.getDate()),
    HH: pad(date.getHours()),
    mm: pad(date.getMinutes()),
    ss: pad(date.getSeconds()),
  }

  // Replace format tokens
  return format.replace(/YYYY|MM|DD|HH|mm|ss/g, match => map[match] || match)
}


export function capitalizeFirst(str: string): string {
  if (!str) return '';
  return str.charAt(0).toUpperCase() + str.slice(1);
}

export function formatTimeToSpan(val) {
  // Nếu đã có giây thì giữ nguyên
  if (/^\d{2}:\d{2}:\d{2}$/.test(val)) return val;
  // Nếu dạng HH:mm thì thêm :00
  if (/^\d{2}:\d{2}$/.test(val)) return val + ":00";
  // Nếu dạng khác, tự xử lý thêm
  return val;
}

export function thousandFormatter(val: number | string): string {
  if (val === null || val === undefined || val === '') return '';
  const s = String(val).replace(/[^\d.-]/g, ''); // giữ số, dấu - và .
  const [intPart, decPart] = s.split('.');
  const intFmt = intPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  return decPart !== undefined ? `${intFmt}.${decPart}` : intFmt;
}

export function thousandParser(val: string): number {
  if (!val) return 0;
  const s = val.replace(/,/g, '');
  const n = Number(s);
  return Number.isNaN(n) ? 0 : n;
}
// utils/number.ts
// utils/number.ts
export function formatCurrency(
  value: number | string,
  showUnit: boolean = true,
  locale: string = 'vi-VN',
  currency: string = 'VND',
  maximumFractionDigits?: number,

  unitPosition: 'right' | 'left' = 'right' // mặc định bên phải
): string {
  if (value === null || value === undefined || value === '') return '';

  const num = toNumber(value);
  if (num === null) return '';

  // 1) Format số (không đơn vị) theo locale
  const defFrac = currency === 'VND' ? 0 : 2;
  const maxFrac = typeof maximumFractionDigits === 'number' ? maximumFractionDigits : defFrac;
  const numberStr = new Intl.NumberFormat(locale, {
    style: 'decimal',
    maximumFractionDigits: maxFrac,
    minimumFractionDigits: maxFrac,
  }).format(num);

  if (!showUnit) return numberStr;

  // 2) Lấy ký hiệu tiền tệ theo locale (VD: ₫, $, €…)
  const symbol = getCurrencySymbol(locale, currency);

  // 3) Ghép lại theo yêu cầu vị trí đơn vị (mặc định bên phải)
  const nbsp = '\u00A0'; // tránh xuống dòng giữa số và đơn vị
  return unitPosition === 'right'
    ? `${numberStr}${nbsp}${symbol}`
    : `${symbol}${nbsp}${numberStr}`;
}

function getCurrencySymbol(locale: string, currency: string): string {
  // dùng formatToParts để lấy đúng symbol theo locale
  // formatToParts có thể không có trong các lib TypeScript cũ, nên cast sang any để tránh lỗi biên dịch
  const parts = (new Intl.NumberFormat(locale, {
    style: 'currency',
    currency,
    currencyDisplay: 'symbol',
    maximumFractionDigits: 0,
  }) as any).formatToParts(1) as Array<{ type: string; value: string }>;
  const sym = parts.find((p: { type: string; value: string }) => p.type === 'currency')?.value;
  return sym || currency; // fallback: mã tiền tệ
}

/** Chuẩn hóa chuỗi số: hỗ trợ '1.234.567,89' hoặc '1,234,567.89' */
function toNumber(v: number | string): number | null {
  if (typeof v === 'number') return isNaN(v) ? null : v;
  let s = String(v).trim();
  if (!s) return null;
  s = s.replace(/[^\d.,-]/g, '');
  const hasDot = s.includes('.');
  const hasComma = s.includes(',');
  if (hasDot && hasComma) {
    if (s.lastIndexOf(',') > s.lastIndexOf('.')) {
      s = s.replace(/\./g, '').replace(',', '.'); // vi: 1.234.567,89
    } else {
      s = s.replace(/,/g, ''); // en: 1,234,567.89
    }
  } else if (hasComma && !hasDot) {
    s = s.replace(',', '.');
  }
  const n = Number(s);
  return isNaN(n) ? null : n;
}

