<template>
    <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
        headerTitle="supportingDocument.headerTitle" headerDesc="supportingDocument.headerDesc"
        :disabledDelete="getDisableDelete" class="mb-6" />
    <!-- ROW cho 2 CARD -->
    <div class="row g-4 mb-8">
        <div class="col-6 col-md-6">
            <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <span class="fw-semibold fs-5">{{ t('item.totalItems') }}</span>
                    <i class="bi bi-journal-text fs-4 text-body-secondary"></i>
                </div>
                <div class="fs-2 fw-bold mb-1">{{ supportingDocumentStore.supportingDocuments.length }}</div>
                <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
            </div>
        </div>
        <div class="col-6 col-md-6">
            <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <span class="fw-semibold fs-5">{{ t('item.totalQuantity') }}</span>
                    <i class="bi bi-list-ol fs-4 text-body-secondary"></i>
                </div>
                <div class="fs-2 fw-bold mb-1">{{supportingDocumentStore.supportingDocuments.filter(doc =>
                    doc.status == StatusType.Active).length}}</div>
                <div class="fs-7 text-body-secondary">{{ t('common.active', {
                    number: 0
                }) }}</div>
            </div>
        </div>
    </div>
    <!-- BẢNG DỮ LIỆU -->
    <div class="card mb-10 w-100 mt-5">
        <div class="card-header card-header-stretch">
            <div class="card-title d-flex flex-column">
                <h3 class="fw-bold fs-4 mb-1">{{ t('supportingDocument.listTitle') }}</h3>
                <span class="text-body-secondary fw-light fs-8">{{ t('supportingDocument.listFunction') }}</span>
            </div>
        </div>
        <div class="card-body py-6 px-2 px-md-6">
            <BaseTable :showCheckboxColumn="true" :columns="columns"
                :items="supportingDocumentStore.supportingDocuments" :loading="supportingDocumentStore.loading"
                :showPagination="true" :page="page" :total="supportingDocumentStore.total" :pageSize="pageSize"
                :filter="filter" @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val"
                @edit="editModelEvent" @view="viewModelEvent" :showActionsColumn="true" :showEdit="true"
                :showDelete="true" :showView="true" @delete="handleDelete" @update:page="val => page = val"
                @update:pageSize="onPageSizeChange">
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
                    <BaseBadge type="boolean" :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                </template>
            </BaseTable>
        </div>
        <SupportingDocumentDialog v-model:visible="showFormModal" :mode="dialogMode"
            :supportingDocumentData="supportingDocumentStore.selectedSupportingDocument" :loading="formLoading"
            @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useSupportingDocumentStore } from '@/stores/supportingDocumentStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import SupportingDocumentDialog from '@/views/supporting-document/SupportingDocumentDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { useFileStore } from '@/stores/fileStore';
import { useCommonStore } from '@/stores/commonStore';
import { StatusType } from '@/types';
const commonStore = useCommonStore();
const fileStore = useFileStore();
const { t } = useI18n();
const supportingDocumentStore = useSupportingDocumentStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRowsData = ref([]);

const columns: BaseTableColumn[] = [
    { key: 'documentName', labelKey: 'supportingDocument.name', filterType: 'text', sortable: true },
    { key: 'description', labelKey: 'supportingDocument.description', filterType: 'text', sortable: false },
    {
        key: 'fileUrl',
        labelKey: 'lectureType.fileAttached',
        width: 200,
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
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.supportingDocument') } });
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
        { key: 'toast.delete', params: { model: t('models.supportingDocument') } },
        async () => {

            startLoading();
            try {
                // Lấy đủ document để đọc websiteKeys (trường hợp rows chỉ có id)
                const docs = list.map(it => it?.websiteKeys !== undefined ? it
                    : supportingDocumentStore.supportingDocuments.find(d => d.id === it.id))
                    .filter(Boolean) as Array<{ id: string; websiteKeys: string }>;

                // 1) Tính delta tần số: mỗi key xuất hiện trong mỗi doc bị xoá => -1
                const delta: Record<string, number> = {};
                for (const doc of docs) {
                    const keys = commonStore.splitRawKeys(doc.websiteKeys);
                    for (const k of keys) {
                        const lower = commonStore.normalizeKey(k).toLowerCase();
                        delta[lower] = (delta[lower] ?? 0) - 1;
                    }
                }

                // 2) Dự phóng websiteKeys mới (để gửi lên BE nếu BE muốn danh sách mới)
                const projected = commonStore.projectWebsiteKeysWithDelta(commonStore.websiteKeys, delta);
                const payloadWebsiteKeys = projected.map(x => ({ key: x.key, frequency: x.frequency }));
                await supportingDocumentStore.deleteSupportingDocuments({
                    ids: ids,
                    websiteKeys: payloadWebsiteKeys,
                    enWebsiteKeys: payloadWebsiteKeys
                });


                await supportingDocumentStore.fetchAllSupportingDocuments();
                showFormModal.value = false;
                commonStore.rebuildWebsiteKeysFromDocuments(
                    supportingDocumentStore.supportingDocuments.map(doc => ({
                        websiteKeys: doc.websiteKeys ?? null
                    }))
                );

            } catch (err: any) {
                console.error('Error deleting:', err?.response?.data?.errors || err);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    supportingDocumentStore.selectSupportingDocument(null);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit';
    supportingDocumentStore.selectSupportingDocument({ ...item });
    showFormModal.value = true;
}

function viewModelEvent(item: any) {
    dialogMode.value = 'view';
    supportingDocumentStore.selectSupportingDocument({ ...item });
    showFormModal.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await supportingDocumentStore.saveSupportingDocument(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.supportingDocument') } });
        supportingDocumentStore.fetchAllSupportingDocuments();
        await commonStore.fetchWebsiteKeys();

        showFormModal.value = false;
    } catch (err: any) {
        console.error('Error saving:', err);
    } finally {
        stopLoading();
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'supportingDocument.add', type: 'add' },
            { event: 'delete', label: 'supportingDocument.delete', type: 'delete' },
        ]
    });
    await commonStore.fetchWebsiteKeys();
    await commonStore.fetchEnWebsiteKeys();
    await supportingDocumentStore.fetchAllSupportingDocuments();
});

async function downloadFile(row) {
    await fileStore.downloadFile(row.attachment.path, row.attachment.fileName);
}
</script>
