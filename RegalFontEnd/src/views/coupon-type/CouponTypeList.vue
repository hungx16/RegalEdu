<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="couponType.headerTitle" headerDesc="couponType.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('couponType.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('couponType.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filtercouponTypes" :showIndex="true"
                    :loading="couponTypeStore.loading" :showPagination="true" :page="couponTypeStore.query.page"
                    :total="filtercouponTypes.length" :pageSize="couponTypeStore.query.pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" :showView="true"
                    @edit="editModelEvent" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    @delete="handleDelete" @view="viewModelEvent" @update:page="val => couponTypeStore.setPage(val)"
                    @update:pageSize="val => couponTypeStore.setPageSize(val)">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.couponTypeStatusInActive') : item.status === 1 ? t('common.couponTypeStatusActive') : t('common.couponTypeStatusDiActive')" />
                    </template>
                    <template #cell-startDate="{ item }">
                        <span>{{ formatDate(item.startDate, 'DD/MM/YYYY') }}</span>
                    </template>
                    <template #cell-endDate="{ item }">
                        <span>{{ formatDate(item.endDate, 'DD/MM/YYYY') }}</span>
                    </template>
                    <template #cell-method="{ item }">
                        <span>{{ getLabelMethod(item.couponTypeDiscounts[0]?.method) }}</span>
                    </template>
                </BaseTable>
            </div>
        </div>

        <CouponTypeDialog v-model:visible="showFormModal" :mode="dialogMode" :loading="formLoading"
            :showActived="showActived" :showOpenNewDialog="showOpenNewDialog"
            :coupon-type-data="couponTypeStore.selectedCouponType" @submit="handleSave" @delete="handleDelete"
            @activated="handleActivated" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useCouponTypeStore } from '@/stores/couponTypeStore';
import { useNotificationStore } from '@/stores/notificationStore';
import { useCommonStore } from '@/stores/commonStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import CouponTypeDialog from './CouponTypeDialog.vue'; // Sẽ tạo ở bước 4.2
import { formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import type { CouponTypeModel } from '@/api/CouponTypeApi';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';

const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const filter = ref({});
const page = ref(1);
const { t } = useI18n();
const couponTypeStore = useCouponTypeStore();
const notificationStore = useNotificationStore();
const commonStore = useCommonStore();
const showFormModal = ref(false);
const showActived = ref(true);
const showOpenNewDialog = ref(true);

const filterComponentRef = ref();

let dialogMode = ref<'create' | 'edit' | 'view'>('create');
const selectedRowsData = ref<Array<CouponTypeModel>>([]);

const columns: BaseTableColumn[] = [
    { key: 'code', labelKey: 'couponType.code', filterType: 'text', sortable: true, sticky: true, width: 160 },
    { key: 'name', labelKey: 'couponType.name', filterType: 'text', sortable: true, sticky: true },
    { key: 'method', labelKey: 'couponType.type', filterType: 'text', sortable: true, width: 180 },
    { key: 'startDate', labelKey: 'couponType.startDate', filterType: 'text', sortable: true, width: 150 },
    { key: 'endDate', labelKey: 'couponType.endDate', filterType: 'text', sortable: true, width: 150 },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.couponTypeStatusInActive', value: 0 },
            { label: 'common.couponTypeStatusActive', value: 1 },
            { label: 'common.couponTypeStatusDiActive', value: 2 },
        ]
    },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
];

const getLabelMethod = (method: any) => {
    // Return a translated label based on the method value; adjust keys if your i18n keys differ
    if (method === 0) return t('discount.onOrderTotal');
    if (method === 1) return t('discount.onQuantity');
    return String(method ?? '');
};

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);
const filtercouponTypes = computed(() => {
    let arr = couponTypeStore.couponTypes;
    console.log("Giá trị của couponTypes hiển thị:", arr);

    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            if (key === 'createdAt') {
                arr = arr.filter(item => {
                    if (!item[key]) return false;
                    const dateOnly = String(item[key]).substring(0, 10);
                    return dateOnly === val;
                });
            } else {
                arr = arr.filter(item =>
                    String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
                );
            }
        }
    });
    return arr;
});
onMounted(() => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: ('couponType.add'), type: 'add' },
            { event: 'delete', label: ('couponType.delete'), type: 'delete' }
        ]
    });
    couponTypeStore.fetchAll();


});

watch(
    () => [couponTypeStore.query.page, couponTypeStore.query.pageSize, filter],
    () => {
        couponTypeStore.fetchAll();
    },
    { deep: true }

);

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.couponType') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(couponTypes: CouponTypeModel | CouponTypeModel[]) {
    const list = Array.isArray(couponTypes) ? couponTypes : [couponTypes];
    const ids = list.filter(item => typeof item.id === 'string' && item.id).map(item => item.id as string);
    // If any selected coupon type is active (status === 1), prevent deletion
    if (list.some(item => item.status === 1)) {
        notificationStore.showToast('warning', { key: 'toast.deleteActiveError', params: { model: t('models.couponType') } });
        return;
    }
    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.couponType') } },
        async () => {
            try {
                startLoading();
                await couponTypeStore.deleteCouponTypes(ids);
                await couponTypeStore.fetchAll();

            } catch (err: any) {
                console.error('Error deleting:', err);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    // await commonStore.generateCode('CT', 'CouponType', 'Code', 4); // Nếu có mã tự động
    couponTypeStore.selectCouponType(null);
    showFormModal.value = true;
}

function editModelEvent(couponType: any) {
    dialogMode.value = 'edit';
    couponTypeStore.selectCouponType({ ...couponType });
    // console.log("couponType edit:", couponType);

    showFormModal.value = true;
}
function viewModelEvent(couponType: any) {
    dialogMode.value = 'view';
    // console.log("couponType view:", couponType);
    couponTypeStore.selectCouponType({ ...couponType });
    if (couponType.status === 0) {
        showActived.value = true;
        showOpenNewDialog.value = false;
    } else if (couponType.status === 1) {
        showActived.value = false;
        showOpenNewDialog.value = true;
    } else {
        showActived.value = false;
        showOpenNewDialog.value = false;
    }
    showFormModal.value = true;
}
async function handleSave(couponType: any) {
    try {
        startLoading();
        console.log("Form data trước khi lưu:", couponType);

        await couponTypeStore.saveCouponType(couponType);
        if (couponType.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.couponType') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.couponType') } });
        }
        await couponTypeStore.fetchAll();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
        showFormModal.value = false;
    }
}

async function handleActivated(couponType: any) {
    try {
        startLoading();
        // Toggle status between active (1) and inactive (0) if status exists, otherwise set active
        const newStatus = typeof couponType.status === 'number' ? (couponType.status === 1 ? 0 : 1) : 1;
        const updated = { ...couponType, status: newStatus };

        // Save updated status via store (reuse saveCouponType)
        //  await couponTypeStore.saveCouponType(updated);
        console.log("Form data trước khi kích hoạt/hủy kích hoạt:", updated.id);

        notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.couponType') } });
        await couponTypeStore.fetchAll();
    } catch (err: any) {
        console.error('Error activating/deactivating:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
        showFormModal.value = false;
    }
}
</script>