// export interface BadgeMeta {
//     label: string
//     className: string
//     icon?: string
//     tooltip?: string
//     i18nKey?: string
//     tooltipI18nKey?: string
//     showFullValue?: boolean
//     editable?: boolean
//     options?: { label: string; value: string }[]
// }

// type BadgeMap = Record<string, BadgeMeta>

// export const badgeMaps: Record<string, BadgeMap> = {
//     email: {
//         'gmail.com': {
//             label: 'Gmail',
//             className: 'badge-gmail',
//             icon: 'fas fa-envelope',
//             tooltip: 'Email cá nhân Gmail',
//             i18nKey: 'badge.email.gmail',
//             tooltipI18nKey: 'tooltip.email.gmail',
//             showFullValue: true
//         },
//         'example.com': {
//             label: 'Example Corp',
//             className: 'badge-example',
//             icon: 'fas fa-building',
//             tooltip: 'Email công ty Example',
//             i18nKey: 'badge.email.example',
//             tooltipI18nKey: 'tooltip.email.example',
//             showFullValue: true
//         },
//         'regaledu.vn': {
//             label: 'RegalEdu',
//             className: 'badge-regal',
//             icon: 'fas fa-university',
//             tooltip: 'Email nội bộ RegalEdu',
//             i18nKey: 'badge.email.regaledu',
//             tooltipI18nKey: 'tooltip.email.regaledu',
//             showFullValue: true
//         }
//     },
//     status: Object.fromEntries(
//         ['new', 'potential', 'success', 'lost'].map((key) => [
//             key,
//             {
//                 label: '',
//                 className: `badge-status-${key}`,
//                 icon:
//                     key === 'new'
//                         ? 'fas fa-user-plus'
//                         : key === 'potential'
//                             ? 'fas fa-clock'
//                             : key === 'success'
//                                 ? 'fas fa-check-circle'
//                                 : 'fas fa-user-times',
//                 i18nKey: `badge.status.${key}`,
//                 tooltipI18nKey: `tooltip.status.${key}`,
//                 editable: true
//             }
//         ])
//     ),
//     priority: Object.fromEntries(
//         ['high', 'medium', 'low'].map((key) => [
//             key,
//             {
//                 label: '',
//                 className: `badge-priority-${key}`,
//                 i18nKey: `badge.priority.${key}`,
//                 tooltipI18nKey: `tooltip.priority.${key}`,
//                 editable: true
//             }
//         ])
//     ),
//     boolean: {
//         true: {
//             label: 'Đã xóa',
//             className: 'badge-deleted',
//             icon: 'fas fa-trash',
//             tooltip: 'Người dùng đã bị đánh dấu là xoá',
//             i18nKey: 'badge.boolean.true',
//             tooltipI18nKey: 'tooltip.boolean.true',
//             editable: true
//         },
//         false: {
//             label: 'Hoạt động',
//             className: 'badge-active',
//             icon: 'fas fa-check',
//             tooltip: 'Người dùng đang hoạt động',
//             i18nKey: 'badge.boolean.false',
//             tooltipI18nKey: 'tooltip.boolean.false',
//             editable: true
//         }
//     },
//     enable: {
//         true: {
//             label: 'Enable',
//             className: 'badge-enabled',
//             icon: 'fas fa-check-circle',
//             i18nKey: 'badge.enable.true',
//             tooltipI18nKey: 'tooltip.enable.true',
//             editable: true
//         },
//         false: {
//             label: 'Disable',
//             className: 'badge-disabled',
//             icon: 'fas fa-times-circle',
//             i18nKey: 'badge.enable.false',
//             tooltipI18nKey: 'tooltip.enable.false',
//             editable: true
//         }
//     }
// }

// export function getBadgeMeta(
//     type: string,
//     value: string | boolean | number,
//     options: { showFullValueOverride?: boolean } = {}
// ): BadgeMeta {
//     value = String(value)
//     const map = badgeMaps[type]
//     if (!map) return fallbackMeta(value)

//     if (type === 'email') {
//         const domain = value?.split('@')[1]?.toLowerCase() || ''
//         const base = map[domain]
//         const showFull = options.showFullValueOverride ?? base?.showFullValue
//         return {
//             ...base,
//             label: showFull ? value : base?.label || value,
//             className: base?.className || 'badge-default',
//             icon: base?.icon,
//             tooltip: base?.tooltip,
//             i18nKey: base?.i18nKey,
//             tooltipI18nKey: base?.tooltipI18nKey,
//             editable: base?.editable,
//             options: base?.options
//         }
//     }

//     const base = map[value]
//     if (base && base.editable && !base.options) {
//         base.options = Object.keys(map).map((key) => ({
//             label: map[key].i18nKey || map[key].label,
//             value: key
//         }))
//     }

//     return base
//         ? {
//             ...base,
//             label: base.label,
//             className: base.className,
//             icon: base.icon,
//             tooltip: base.tooltip,
//             i18nKey: base.i18nKey,
//             tooltipI18nKey: base.tooltipI18nKey,
//             editable: base.editable,
//             options: base.options
//         }
//         : fallbackMeta(value)
// }

// function fallbackMeta(label: string): BadgeMeta {
//     return {
//         label,
//         className: 'badge-default',
//         icon: 'fas fa-question-circle',
//         tooltip: 'tooltip.fallback',
//         i18nKey: 'badge.fallback',
//         tooltipI18nKey: 'tooltip.fallback'
//     }
// }
