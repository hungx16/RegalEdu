<template>
    <BaseDialogForm :visible="visible" mode="view" :title="titleComputed" :loading="loading" :form-data="{}"
        :submit-disabled="true" @update:visible="v => emit('update:visible', v)" @close="emit('update:visible', false)">
        <template #form>
            <BaseTable :columns="columns" :items="rows" :loading="false" :showPagination="false"
                :showCheckboxColumn="false" :showActionsColumn="false" :height="isMobile ? 320 : 420">
                <template #cell-totalQuotaBefore="{ item }">
                    {{ formatCurrency(item.totalQuotaBefore) }}
                </template>
                <template #cell-totalQuotaAfter="{ item }">
                    {{ formatCurrency(item.totalQuotaAfter) }}
                </template>
                <template #cell-delta="{ item }">
                    <span :class="item.delta >= 0 ? 'text-success' : 'text-danger'">
                        {{ item.delta >= 0 ? '+' : '−' }}{{ formatCurrency(Math.abs(item.delta)) }}
                    </span>
                </template>
                <template #cell-reason="{ item }">
                    <span class="reason">{{ item.reason || '—' }}</span>
                </template>
            </BaseTable>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import { formatCurrency, formatDate } from '@/utils/format'

interface Adjustment {
    id?: string
    scope: number
    admissionsQuotaRegionId?: string
    admissionsQuotaCompanyId?: string
    totalQuotaBefore?: number | null
    totalQuotaAfter?: number | null
    reason?: string | null
    createdAt?: string | Date | null
}

const props = withDefaults(defineProps<{
    visible: boolean
    loading?: boolean
    title?: string
    targetName?: string
    adjustments: Adjustment[]
    locale?: string
    currency?: string
}>(), {
    visible: false,
    loading: false,
    adjustments: () => [],
    locale: 'vi-VN',
    currency: 'VND',
})

const emit = defineEmits(['update:visible'])
const { t } = useI18n()

const isMobile = window.innerWidth <= 767

const titleComputed = computed(() =>
    props.title
        ? props.targetName ? `${props.title} • ${props.targetName}` : props.title
        : props.targetName ? `${t('admissionsQuota.adjustments')} • ${props.targetName}`
            : t('admissionsQuota.adjustments')
)

const rows = computed(() =>
    (props.adjustments || [])
        .map(a => {
            const before = Number(a.totalQuotaBefore ?? 0)
            const after = Number(a.totalQuotaAfter ?? 0)
            return { ...a, totalQuotaBefore: before, totalQuotaAfter: after, delta: after - before }
        })
        .sort((a, b) => new Date(b.createdAt ?? 0).getTime() - new Date(a.createdAt ?? 0).getTime())
)

const columns: BaseTableColumn[] = [
    { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, formatter: (v: string) => formatDate(v, 'DD/MM/YYYY') },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 220 },
    { key: 'totalQuotaBefore', labelKey: 'admissionsQuota.totalBefore', width: 180, align: 'right' },
    { key: 'totalQuotaAfter', labelKey: 'admissionsQuota.totalAfter', width: 180, align: 'right' },
    { key: 'delta', labelKey: 'admissionsQuota.delta', width: 150, align: 'right' },
    { key: 'reason', labelKey: 'common.reason', minWidth: 220, align: 'left' },
]


</script>

<style scoped>
.reason {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    display: inline-block;
    max-width: 420px;
}

.text-success {
    color: #22c55e;
}

.text-danger {
    color: #ef4444;
}
</style>
