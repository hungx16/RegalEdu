<template>
    <template v-if="Array.isArray(badges)">
        <span v-bind="attrs" v-for="(badge, idx) in badges" :key="idx"
            :class="['base-badge', computeClass(badge), attrs.class]" :style="computeStyle(badge, idx)"
            :title="badge.title || String(getDisplayLabel(badge.label ?? '', badge))">
            <slot name="icon" v-if="badge.icon">
                <i :class="badge.icon" class="me-1" />
            </slot>
            {{ getDisplayLabel(badge.label ?? '', badge) }}
        </span>
    </template>

    <template v-else>
        <span v-bind="attrs" :class="['base-badge', computedClass, attrs.class]" :style="computedStyle"
            :title="title || String(getDisplayLabel(label ?? '', undefined))">
            <slot name="icon" v-if="icon">
                <i :class="icon" class="me-1" />
            </slot>
            <slot>
                {{ getDisplayLabel(label ?? '', undefined) }}
            </slot>
        </span>
    </template>
</template>

<script setup lang="ts">
import { computed, useAttrs } from 'vue';
import { useI18n } from 'vue-i18n';
const { t } = useI18n();
const attrs = useAttrs();

const COLOR_MAP = {
    green: { color: '#1abc9c', bg: '#e7fcf4', border: '#90e5c7' },
    blue: { color: '#3498db', bg: '#eaf6ff', border: '#b9e7fe' },
    purple: { color: '#a259d9', bg: '#f6edfd', border: '#d1b3f3' },
    red: { color: '#fff', bg: '#f44336', border: '#e57373' },
    orange: { color: '#e67e22', bg: '#ffe3cb', border: '#f4b183' },
    yellow: { color: '#b7950b', bg: '#fff9e6', border: '#ffe08c' },
    violet: { color: '#5d267d', bg: '#efe1f8', border: '#c1a1d3' },
    cyan: { color: '#00bcd4', bg: '#e0f7fa', border: '#6ed3e8' },
    gray: { color: '#888', bg: '#f3f3f3', border: '#ddd' },
    white: { color: '#555', bg: '#ffffff', border: '#bbb' },
    deepPurple: { color: '#fff', bg: '#5D2C7D', border: '#5D2C7D' },
    lightBlue: { color: '#fff', bg: '#00A3FF', border: '#00A3FF' },
    lightGreen: { color: '#fff', bg: '#00B96B', border: '#00B96B' },
    lightRed: { color: '#fff', bg: '#FF4D4F', border: '#FF4D4F' },
    lightOrange: { color: '#fff', bg: '#FF8C00', border: '#FF8C00' },
    lightYellow: { color: '#000', bg: '#F7D03C', border: '#F7D03C' },
    lightCyan: { color: '#fff', bg: '#00B8D9', border: '#00B8D9' },
};

interface BadgeItem {
    label?: string | number;
    color?: string;
    outline?: boolean;
    soft?: boolean;
    bold?: boolean;
    pill?: boolean;
    icon?: string;
    custom?: { color?: string; bg?: string; border?: string };
    disabled?: boolean;
    title?: string;
    displayType?: string;
    i18nKey?: string;
    bordered?: boolean;
}

const props = defineProps({
    label: [String, Number],
    color: String,
    outline: Boolean,
    soft: Boolean,
    bold: Boolean,
    pill: { type: Boolean, default: true },
    icon: String,
    disabled: Boolean,
    custom: Object,
    title: String,
    displayType: String,
    i18nKey: String,
    badges: { type: Array as () => BadgeItem[] | null, default: null },
    bordered: Boolean,
    rawLabel: Boolean,
    colorByLabelMap: {
        type: Object as () => Record<string, string>,
        default: () => ({}),
    },
    excludeFromRandom: { type: String, default: '' },
    containerBackgroundColor: { type: String, default: '' },
    noRandomColor: Boolean,
});
function parseColor(color: string): { r: number; g: number; b: number } | null {
    if (color.startsWith('#')) {
        const hex = color.replace('#', '');
        if (hex.length === 6) {
            const r = parseInt(hex.substring(0, 2), 16);
            const g = parseInt(hex.substring(2, 4), 16);
            const b = parseInt(hex.substring(4, 6), 16);
            return { r, g, b };
        }
    } else if (color.startsWith('rgb')) {
        const match = color.match(/(\d+),\s*(\d+),\s*(\d+)/);
        if (match) {
            return {
                r: parseInt(match[1]),
                g: parseInt(match[2]),
                b: parseInt(match[3]),
            };
        }
    }
    return null;
}

function isColorSimilar(color1: string, color2: string): boolean {
    const rgb1 = parseColor(color1);
    const rgb2 = parseColor(color2);
    if (!rgb1 || !rgb2) return false;

    const distance = Math.sqrt(
        Math.pow(rgb1.r - rgb2.r, 2) +
        Math.pow(rgb1.g - rgb2.g, 2) +
        Math.pow(rgb1.b - rgb2.b, 2)
    );
    return distance < 90;
}


function getSafeColorKey(label: string, salt = ''): string {
    const combined = label + salt;
    let hash = 0;
    for (let i = 0; i < combined.length; i++) {
        hash = combined.charCodeAt(i) + ((hash << 5) - hash);
    }
    let keys = Object.keys(COLOR_MAP);
    if (props.excludeFromRandom) {
        keys = keys.filter(k => k !== props.excludeFromRandom);
    }
    if (props.containerBackgroundColor) {
        const target = props.containerBackgroundColor.toLowerCase();
        keys = keys.filter(k => {
            const bg = COLOR_MAP[k]?.bg?.toLowerCase();
            return bg && !isColorSimilar(bg, target);
        });
    }
    if (keys.length === 0) keys = Object.keys(COLOR_MAP);
    const index = Math.abs(hash) % keys.length;
    return keys[index] || 'gray';
}

function getDisplayLabel(label: string | number, badge?: BadgeItem) {
    const raw = String(label);
    if (props.rawLabel) return raw;
    if (badge?.i18nKey) return t(badge.i18nKey, { value: raw });
    if (props.i18nKey) return t(props.i18nKey, { value: raw });
    const type = badge?.displayType || props.displayType;
    if (type === 'level') return t('badge.level', { value: raw });
    if (type === 'studentCount') return t('badge.studentCount', { value: raw });
    if (type === 'studentCountAlt') return t('badge.studentCountAlt', { value: raw });
    if (type === 'department') return t('badge.department', { value: raw });
    return raw;
}

const computedClass = computed(() => [
    `base-badge--${props.color}`,
    {
        'is-outline': props.outline,
        'is-soft': props.soft,
        'is-pill': props.pill,
        'is-bold': props.bold,
        'is-disabled': props.disabled,
    },
]);

const computedStyle = computed(() =>
    getStyle({
        label: props.label,
        color: props.color,
        outline: props.outline,
        soft: props.soft,
        custom: props.custom,
        disabled: props.disabled,
        bordered: props.bordered,
        i18nKey: props.i18nKey,
    })
);

function computeClass(badge: BadgeItem) {
    return [
        `base-badge--${badge.color || 'gray'}`,
        {
            'is-outline': badge.outline,
            'is-soft': badge.soft,
            'is-pill': badge.pill ?? true,
            'is-bold': badge.bold,
            'is-disabled': badge.disabled,
        },
    ];
}

function computeStyle(badge: BadgeItem, index = 0) {
    return getStyle({
        ...badge,
        label: badge.label,
        bordered: badge.bordered ?? false,
        i18nKey: badge.i18nKey,
        salt: String(index),
    });
}

function getStyle(opt: any) {
    const labelStr = String(opt.label || '');
    const explicitColor = props.colorByLabelMap?.[labelStr];
    const resolvedColorKey = explicitColor
        || (props.noRandomColor ? opt.color
            : getSafeColorKey(labelStr, opt.salt));
    const conf = COLOR_MAP[resolvedColorKey] || COLOR_MAP.gray;

    if (opt.custom) {
        return {
            color: opt.custom.color ?? conf.color,
            background: opt.custom.bg ?? conf.bg,
            border: opt.bordered ? `1px solid ${opt.custom.border ?? conf.border ?? '#ccc'}` : 'none',
            opacity: opt.disabled ? 0.55 : 1,
            boxShadow: '0 1px 2px rgba(0,0,0,0.1)',
        };
    }

    return {
        color: opt.outline ? conf.color : (opt.disabled ? '#aaa' : conf.color),
        background: opt.outline
            ? opt.soft
                ? lightenColor(conf.bg, 0.4)
                : '#fff'
            : opt.soft
                ? lightenColor(conf.bg, 0.12)
                : conf.bg,
        border: opt.bordered ? `1px solid ${conf.border}` : 'none',
        opacity: opt.disabled ? 0.55 : 1,
        boxShadow: '0 1px 2px rgba(0,0,0,0.1)',
    };
}

function lightenColor(color: string, percent = 0.1) {
    if (!color.startsWith('#')) return color;
    let num = parseInt(color.slice(1), 16);
    let amt = Math.round(2.55 * percent * 100);
    let R = (num >> 16) + amt;
    let G = ((num >> 8) & 0x00ff) + amt;
    let B = (num & 0x0000ff) + amt;

    return (
        '#' +
        (
            0x1000000 +
            (R < 255 ? (R < 1 ? 0 : R) : 255) * 0x10000 +
            (G < 255 ? (G < 1 ? 0 : G) : 255) * 0x100 +
            (B < 255 ? (B < 1 ? 0 : B) : 255)
        ).toString(16).slice(1)
    );
}
</script>

<style scoped>
.base-badge {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    min-width: 44px;
    min-height: 22px;
    padding: 0 10px;
    border-radius: 14px;
    border: none;
    background: #fff;
    margin: 2px 4px 2px 0;
    line-height: normal;
    font-size: 12px;
    transition: all 0.18s;
    user-select: none;
    cursor: default;
    white-space: nowrap;
    box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.base-badge.is-pill {
    border-radius: 14px;
}

.base-badge.is-bold {
    font-weight: 700;
}

.base-badge.is-outline {
    background: #fff !important;
}

.base-badge.is-soft {
    opacity: 0.85;
}

.base-badge.is-disabled {
    opacity: 0.55;
    pointer-events: none;
}

.base-badge .me-1 {
    margin-right: 5px;
}
</style>
