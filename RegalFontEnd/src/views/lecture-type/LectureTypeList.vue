<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="lectureType.headerTitle" headerDesc="lectureType.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />
        <!-- ROW cho 2 CARD -->
        <div class="row g-4 mb-8">
            <!-- Tổng loại bài giảng -->
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('lectureType.totalLectureTypes') }}</span>
                        <i class="bi bi-collection fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalLectureTypes }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('lectureType.allTypes') }}</div>
                </div>
            </div>

            <!-- Đang sử dụng -->
            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('lectureType.activeTypes') }}</span>
                        <i class="bi bi-check2-circle fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ activeLectureTypes }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('lectureType.activeTypesDesc') }}</div>
                </div>
            </div>
        </div>
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('lectureType.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('lectureType.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">

                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="lectureTypeStore.lectureTypes"
                    :loading="lectureTypeStore.loading" :showPagination="true" :page="page"
                    :total="lectureTypeStore.total" :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRowsData = val" @edit="editModelEvent" @view="viewModelEvent"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true"
                    @delete="handleDelete" @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <template #cell-fileUrl="{ item }">
                        <div v-if="item.attachment">
                            <a href="javascript:void(0);" class="text-primary fw-semibold" @click="downloadFile(item)">
                                <i class="bi bi-paperclip me-1"></i>
                                {{ item.attachment.fileName }}
                            </a>
                        </div>
                        <span v-else class="text-muted">{{ t('lectureType.noFileAttached') }}</span>
                    </template>

                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                </BaseTable>
            </div>
        </div>

        <LectureTypeDialog v-model:visible="showFormModal" :mode="dialogMode"
            :lecture-type-data="lectureTypeStore.selectedLectureType" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useLectureTypeStore } from '@/stores/lectureTypeStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import LectureTypeDialog from './LectureTypeDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { useFileStore } from '@/stores/fileStore';
const fileStore = useFileStore();

const { t } = useI18n();
const lectureTypeStore = useLectureTypeStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRowsData = ref([]);
// ===== STAT CARDS =====
const totalLectureTypes = computed(() => lectureTypeStore.lectureTypes.length);
const activeLectureTypes = computed(
    () => lectureTypeStore.lectureTypes.filter(x => x.status === 0).length
);

const columns: BaseTableColumn[] = [
    { key: 'lectureName', labelKey: 'lectureType.name', filterType: 'text', sortable: true },
    { key: 'description', labelKey: 'lectureType.description', filterType: 'text', sortable: false },
    {
        key: 'fileUrl',
        labelKey: 'lectureType.fileAttached',
        width: 200,
        // Không cần filterType vì chỉ là cột hiển thị
    },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },

    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 200 },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 200,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'actions', labelKey: 'common.actions', width: 160 }
];

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.lectureType') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(items: any | any[]) {
    const list = Array.isArray(items) ? items : [items];
    const ids = list.map(item => item.id).filter(Boolean);
    notificationStore.showConfirm(
        {
            key: 'toast.delete',
            params: { model: t('models.lectureType') },
        },
        async () => {
            try {
                startLoading();
                await lectureTypeStore.deleteLectureTypes(ids);
                await lectureTypeStore.fetchAllLectureTypes();
                showFormModal.value = false;
            } catch (error) {
                console.error(error);
            } finally {
                stopLoading();
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    lectureTypeStore.selectLectureType(null);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit';
    lectureTypeStore.selectLectureType({ ...item });
    showFormModal.value = true;
}

function viewModelEvent(item: any) {
    dialogMode.value = 'view';
    lectureTypeStore.selectLectureType({ ...item });
    showFormModal.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await lectureTypeStore.saveLectureType(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.lectureType') } });
        await lectureTypeStore.fetchAllLectureTypes();
        showFormModal.value = false;
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

onMounted(() => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'lectureType.add', type: 'add' },
            { event: 'delete', label: 'lectureType.delete', type: 'delete' },
        ]
    });
    lectureTypeStore.fetchAllLectureTypes();
});
async function downloadFile(row) {
    await fileStore.downloadFile(row.attachment.path, row.attachment.fileName);

}

</script>
