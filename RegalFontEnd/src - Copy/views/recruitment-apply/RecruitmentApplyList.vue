<template>
    <div>
        <!-- Bộ lọc -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="recruitmentApply.headerTitle" headerDesc="recruitmentApply.headerDesc"
            :disabledDelete="getDisableDelete" class="mb-6" />

        <!-- Thống kê nhanh -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('recruitmentApply.totalApplicants') }}</span>
                        <i class="bi bi-person-lines-fill fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ recruitmentApplyStore.recruitmentApplies.length }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('recruitmentApply.applicant') }}</div>
                </div>
            </div>

            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('recruitmentApply.totalVacancies') }}</span>
                        <i class="bi bi-briefcase fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ uniqueRecruitmentInfos.length }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('recruitmentApply.position') }}</div>
                </div>
            </div>
        </div>

        <!-- Bảng dữ liệu -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('recruitmentApply.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('recruitmentApply.listDesc') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredRecruitments"
                    :loading="recruitmentApplyStore.loading" :showPagination="true" :page="page"
                    :total="filteredRecruitments.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" @edit="editModelEvent"
                    @view="viewModelEvent" @delete="handleDelete" :showActionsColumn="true" :showEdit="true"
                    :showDelete="true" :showView="true" @update:page="val => page = val"
                    @update:pageSize="onPageSizeChange">
                    <template #cell-recruitmentInfoName="{ item }">
                        <BaseBadge :label="item.recruitmentInfo?.recruitmentInfoName" color="purple" soft bordered
                            bold />
                    </template>
                    <template #cell-candidateName="{ item }">
                        <BaseBadge :label="item.candidateName" color="blue" soft bordered bold />
                    </template>
                </BaseTable>
            </div>
        </div>

        <!-- Dialog thêm/sửa/xem -->
        <RecruitmentApplyDialog v-model:visible="showFormModal" :mode="dialogMode"
            :recruitmentApplyData="recruitmentApplyStore.selectedRecruitmentApply" :loading="formLoading"
            @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
/** ===== IMPORT ===== */
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRecruitmentApplyStore } from '@/stores/recruitmentApplyStore'
import { useRecruitmentInfoStore } from '@/stores/recruitmentInfoStore'
import { useNotificationStore } from '@/stores/notificationStore'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import RecruitmentApplyDialog from './RecruitmentApplyDialog.vue'
import type { RecruitmentApplyModel } from '@/api/RecruitmentApplyApi'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import { StatusType } from '@/types'

/** ===== INIT ===== */
const { t } = useI18n()
const recruitmentApplyStore = useRecruitmentApplyStore()
const recruitmentInfoStore = useRecruitmentInfoStore()
const notificationStore = useNotificationStore()
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

const filterComponentRef = ref()
const showFormModal = ref(false)
const dialogMode = ref<'create' | 'edit' | 'view'>('create')
const filter = ref({})
const page = ref(1)
const pageSize = ref(20)
const selectedRowsData = ref<RecruitmentApplyModel[]>([])

/** ===== COLUMNS ===== */
const columns: BaseTableColumn[] = [
    { key: 'candidateName', labelKey: 'recruitmentApply.candidateName', sortable: true, filterType: 'text', width: 180, isBold: true },
    { key: 'candidateEmail', labelKey: 'recruitmentApply.candidateEmail', sortable: true, filterType: 'text', width: 200 },
    { key: 'candidatePhone', labelKey: 'recruitmentApply.candidatePhone', sortable: true, filterType: 'text', width: 150, align: 'center' },
    { key: 'candidateExperience', labelKey: 'recruitmentApply.candidateExperience', filterType: 'text' },
    { key: 'recruitmentInfoName', labelKey: 'recruitmentApply.recruitmentInfo', filterType: 'text', width: 220 },
    { key: 'actions', labelKey: 'common.actions', width: 200 }
]

/** ===== COMPUTED ===== */
const getDisableDelete = computed(() => selectedRowsData.value.length === 0)

const filteredRecruitments = computed(() => {
    let arr = recruitmentApplyStore.recruitmentApplies
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            arr = arr.filter(item => String(item[key as keyof RecruitmentApplyModel] ?? '').toLowerCase().includes(String(val).toLowerCase()))
        }
    })
    return arr
})

const uniqueRecruitmentInfos = computed(() => {
    return recruitmentInfoStore.recruitmentInfoList.filter((info,) =>
        info.status === StatusType.Active
    )
})

/** ===== HANDLER ===== */
function onTableFilter(val: Record<string, any>) {
    filter.value = val
    page.value = 1
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize
    page.value = 1
}

async function addModelEvent() {
    dialogMode.value = 'create'
    recruitmentApplyStore.selectRecruitmentApply(null)
    showFormModal.value = true
}

function editModelEvent(apply: RecruitmentApplyModel) {
    dialogMode.value = 'edit'
    recruitmentApplyStore.selectRecruitmentApply({ ...apply })
    showFormModal.value = true
}

function viewModelEvent(apply: RecruitmentApplyModel) {
    dialogMode.value = 'view'
    recruitmentApplyStore.selectRecruitmentApply({ ...apply })
    showFormModal.value = true
}

async function handleSave(apply: RecruitmentApplyModel) {
    startLoading()
    try {
        await recruitmentApplyStore.saveRecruitmentApply(apply)
        await recruitmentApplyStore.fetchAllRecruitmentApplies()
        notificationStore.showToast('success', {
            key: apply.id ? 'toast.updateSuccess' : 'toast.createSuccess',
            params: { model: t('models.recruitmentApply') }
        })
        stopLoading()
        showFormModal.value = false
    } catch (err: any) {
        console.error(err)
        stopLoading()
    }
}

function onDeleteClicked() {
    if (!selectedRowsData.value.length) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.recruitmentApply') } })
        return
    }
    handleDelete(selectedRowsData.value)
}

async function handleDelete(apply: RecruitmentApplyModel | RecruitmentApplyModel[]) {
    const list = Array.isArray(apply) ? apply : [apply]
    const ids = list.filter(i => i.id).map(i => i.id as string)

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.recruitmentApply') } },
        async () => {
            startLoading()
            try {
                await recruitmentApplyStore.deleteRecruitmentApply(ids)
                await recruitmentApplyStore.fetchAllRecruitmentApplies()
                stopLoading()
                showFormModal.value = false
            } catch (err) {
                console.error('Delete error:', err)
                stopLoading()
            }
        }
    )
}

/** ===== LIFECYCLE ===== */
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            // { event: 'add', label: 'recruitmentApply.add', type: 'add' },
            { event: 'delete', label: 'recruitmentApply.delete', type: 'delete' }
        ]
    })

    await recruitmentApplyStore.fetchAllRecruitmentApplies()
    await recruitmentInfoStore.fetchAllRecruitmentInfo()
})
</script>
