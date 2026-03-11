// ===== Helpers ngày tháng an toàn (LOCAL, chống lệch timezone) =====

import { WorkStage } from "@/types";

// Chuyển mọi kiểu về Date LOCAL. Ưu tiên parse 'YYYY-MM-DD' theo LOCAL.
export function toDateLocal(input: any): Date | null {
    if (!input) return null;
    if (input instanceof Date) return isNaN(input.getTime()) ? null : input;

    const s = String(input);
    // 'YYYY-MM-DD' hoặc 'YYYY-MM-DDTHH:mm...' => lấy phần ngày đầu
    const m = s.match(/^(\d{4})-(\d{2})-(\d{2})/);
    if (m) {
        const Y = Number(m[1]), M = Number(m[2]), D = Number(m[3]);
        const d = new Date(Y, M - 1, D); // LOCAL date
        return isNaN(d.getTime()) ? null : d;
    }

    const d = new Date(s); // fallback
    return isNaN(d.getTime()) ? null : d;
}

export function firstDayOfMonth(y: number, m: number) { return new Date(y, m - 1, 1); }
export function lastDayOfMonth(y: number, m: number) { return new Date(y, m, 0); }
export function addDays(d: Date, n: number) { const x = new Date(d); x.setDate(x.getDate() + n); return x; }

// NHẬN MỌI KIỂU, tự toDateLocal bên trong
export function isWithinMonth(input: any, year: number | string, month: number | string) {
    const d = toDateLocal(input);
    if (!d) return false;
    return d.getFullYear() === Number(year) && (d.getMonth() + 1) === Number(month);
}
export function toDate(v: any): Date | null { if (!v) return null; const d = new Date(v); return isNaN(+d) ? null : d; }
function daysBetweenInclusive(s: Date, e: Date) {
    const ss = Date.UTC(s.getFullYear(), s.getMonth(), s.getDate());
    const ee = Date.UTC(e.getFullYear(), e.getMonth(), e.getDate());
    const diff = Math.floor((ee - ss) / (24 * 60 * 60 * 1000));
    return diff + 1;
}
function workingDaysByRule(days: number) {
    if (!days || days <= 0) return 0;
    // 6-1 schedule: INT(N/7) * 6 + MIN(6, MOD(N, 7))
    const fullWeeks = Math.floor(days / 7);
    const remainder = days % 7;
    return fullWeeks * 6 + Math.min(6, remainder);
}
export function countWorkingDaysBetween(s: Date, e: Date) {
    if (!s || !e || e < s) return 0;
    return workingDaysByRule(daysBetweenInclusive(s, e));
}

// D_std
export function countWorkingDaysInMonth(y: number, m: number) {
    const daysInMonth = lastDayOfMonth(y, m).getDate();
    return workingDaysByRule(daysInMonth);
}
// D_prob
export function countWorkingDaysAccumulated(startInput: any, endInput: any = new Date()) {
    const start = toDateLocal(startInput);
    const end = toDateLocal(endInput);
    if (!start || !end) return 0;
    const s = new Date(start.getFullYear(), start.getMonth(), start.getDate());
    const e = new Date(end.getFullYear(), end.getMonth(), end.getDate());
    if (e < s) return 0;

    let total = 0;
    const startMonth = s.getMonth() + 1;
    const endMonth = e.getMonth() + 1;
    const endYear = e.getFullYear();
    let y = s.getFullYear();
    let m = startMonth;

    while (y < endYear || (y === endYear && m <= endMonth)) {
        const monthStart = new Date(y, m - 1, 1);
        const monthEnd = lastDayOfMonth(y, m);
        const rangeStart = (y === s.getFullYear() && m === startMonth) ? s : monthStart;
        const rangeEnd = (y === endYear && m === endMonth) ? e : monthEnd;
        total += countWorkingDaysBetween(rangeStart, rangeEnd);
        if (m === 12) {
            y += 1;
            m = 1;
        } else {
            m += 1;
        }
    }
    return total;
}
export function getEmployeeStatus(
    item: { joinedAt?: any; endAt?: any; probationEnd?: any },
    year?: number,
    month?: number
): WorkStage {
    // nếu không truyền year/month thì dùng tháng hiện tại
    const now = new Date();
    const y = Number(year ?? now.getFullYear());
    const m = Number(month ?? (now.getMonth() + 1));
    if (item?.endAt && isWithinMonth(item.endAt, y, m)) return WorkStage.EndingThisMonth;
    if (item?.probationEnd && isWithinMonth(item.probationEnd, y, m) && item?.joinedAt && isWithinMonth(item.joinedAt, y, m)) return WorkStage.ProbationStartEnd;
    if (item?.probationEnd && isWithinMonth(item.probationEnd, y, m)) return WorkStage.ProbationEnd;
    if (item?.joinedAt && isWithinMonth(item.joinedAt, y, m)) return WorkStage.ProbationStart;
    return WorkStage.Normal;
}
// helper: nhân sự active trong (y, m)?
export function isActiveInMonth(y: number, m: number, join?: Date | string | null, end?: Date | string | null) {
    const ms = firstDayOfMonth(y, m);
    const me = lastDayOfMonth(y, m);
    const j = toDate(join);
    const e = toDate(end);

    const startOk = !j || j <= me; // bắt đầu không được sau cuối tháng
    const endOk = !e || e >= ms; // nghỉ không được trước đầu tháng
    return startOk && endOk;
}
/** Đổi Date -> số YYYYMMDD để so sánh theo ngày */
function toYMDNum(d: Date): number {
    return d.getFullYear() * 10000 + (d.getMonth() + 1) * 100 + d.getDate();
}
export function isResigned(
    employeeEndDate?: string | Date | null,
    now: Date = new Date(),
    inclusiveLastDay: boolean = true
): boolean {
    const end = toDate(employeeEndDate);
    if (!end) return false;
    const todayNum = toYMDNum(now);
    const endNum = toYMDNum(end);
    return inclusiveLastDay ? todayNum > endNum : todayNum >= endNum;
}
