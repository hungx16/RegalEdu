// src/utils/tags.ts
export const DEFAULT_TAG_DELIMITER = '#$#'

export type Variant =
    | 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'dark'
    | 'purple' | 'pink' | 'orange' | 'cyan'

export function splitTags(
    value: string | string[] | undefined | null,
    delimiter = DEFAULT_TAG_DELIMITER
): string[] {
    if (!value) return []
    if (Array.isArray(value)) return value.map(x => x?.trim()).filter(Boolean)
    return String(value)
        .split(delimiter)
        .map(s => s.trim())
        .filter(s => s.length > 0)
}

export function joinTags(
    arr: string[] | undefined | null,
    delimiter = DEFAULT_TAG_DELIMITER
): string {
    if (!arr || arr.length === 0) return ''
    return arr.map(s => s.trim()).filter(Boolean).join(delimiter)
}

export function isStringMode(value: unknown): boolean {
    return typeof value === 'string'
}

// Bảng màu mở rộng ổn định
const TAG_COLORS: Variant[] = [
    'primary', 'danger', 'success', 'warning', 'purple', 'info', 'orange', 'pink', 'cyan',
]

/** Map text -> variant ổn định */
export function tagToVariant(text: string): Variant {
    const index = text.split('').reduce((acc, ch) => acc + ch.charCodeAt(0), 0)
    return TAG_COLORS[index % TAG_COLORS.length]
}

/**
 * Chuẩn hoá class màu cho Bootstrap/Metronic.
 * tone = 'solid' dùng .text-bg-*
 * tone = 'light' dùng .badge-light-* (Metronic có sẵn); với màu mở rộng, ta dùng class custom (khai báo CSS bên dưới).
 */
export function variantToClass(variant: Variant, tone: 'solid' | 'light' = 'solid'): string {
    const std: Variant[] = ['primary', 'secondary', 'success', 'danger', 'warning', 'info', 'light', 'dark']
    const isStd = std.includes(variant)

    if (tone === 'solid') {
        return isStd ? `text-bg-${variant}` : `text-bg-${variant}` // màu mở rộng sẽ dùng CSS custom
    }
    // tone light
    return isStd ? `badge-light-${variant}` : `badge-light-${variant}`
}
// === ADD: Palette chuẩn dùng cho distinct colors (khớp với SCSS soft) ===
export const TAG_PALETTE: Variant[] = [
    'primary', 'success', 'danger', 'warning', 'info', 'secondary', 'purple', 'pink', 'orange', 'cyan'
]

// === ADD: hash ổn định để ưu tiên vị trí trong palette (không bắt buộc) ===
export function stableHash(s: string): number {
    let h = 0
    for (let i = 0; i < s.length; i++) {
        h = (h << 5) - h + s.charCodeAt(i)
        h |= 0
    }
    return Math.abs(h)
}
