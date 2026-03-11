<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="gift.headerTitle" headerDesc="gift.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('gift.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('gift.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filtergifts" :showIndex="true"
                    :loading="giftStore.loading" :showPagination="true" :page="giftStore.query.page"
                    :total="filtergifts.length" :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRowsData = val" :showView="true" @edit="editModelEvent"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" @delete="onDeleteClicked"
                    @view="viewModelEvent" @update:page="giftStore.setPage" @update:pageSize="giftStore.setPageSize" />
            </div>
        </div>

        <GiftDialog :visible="showFormModal" :mode="dialogMode" @update:visible="showFormModal = $event"
            @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useGiftStore } from '@/stores/giftStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import { formatDate, formatCurrency } from '@/utils/format';
import GiftDialog from './GiftDialog.vue'; // Sẽ tạo ở bước 1.3
import type { GiftModel } from '@/api/GiftApi';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const filter = ref({});
const page = ref(1);
const pageSize = ref(30);
const { t } = useI18n();
const giftStore = useGiftStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();
const filterComponentRef = ref<any>(null);
const showFormModal = ref(false);
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const selectedRowsData = ref<any[]>([]);
const filtergifts = computed(() => {
    let arr = giftStore.gifts;
    console.log("arr before filter:", arr);

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
// --- Lifecycle & Data Fetching ---
onMounted(() => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: ('promotionGroup.add'), type: 'add' },
            { event: 'delete', label: ('promotionGroup.delete'), type: 'delete' }
        ]
    });
    giftStore.fetchAll();
});

watch(
    () => [giftStore.query.page, giftStore.query.pageSize, filter],
    () => {
        giftStore.fetchAll();
    },
    { deep: true }
);
// --- Table Setup ---
function formatDateValue(value: any) {
    const anyCommon = commonStore as any;
    if (typeof anyCommon.formatDate === 'function') {
        return anyCommon.formatDate(value);
    }
    if (value == null || value === '') return '';
    try {
        return new Date(value).toLocaleString();
    } catch {
        return String(value);
    }
}
// --- Table Setup ---
const columns = computed(() => [
    { key: 'name', labelKey: 'gift.name', label: t('gift.name'), prop: 'name', minWidth: 200, sortable: true },
    { key: 'prices', labelKey: 'gift.prices', label: t('gift.prices'), prop: 'prices', minWidth: 120, align: 'right' as const, formatter: (v: number) => formatCurrency(v, true) },
    { key: 'description', labelKey: 'gift.description', label: t('gift.description'), prop: 'description', minWidth: 250 },
    { key: 'status', labelKey: 'common.status', label: t('common.status'), prop: 'status', minWidth: 100, formatter: (row: any) => row.status === 0 ? t('common.active') : t('common.inactive') },
    { key: 'createdAt', labelKey: 'common.createdAt', label: t('common.createdAt'), prop: 'createdAt', minWidth: 150, formatter: (v: string) => formatDate(v, 'DD/MM/YYYY') },
]);

// --- Actions & Handlers ---
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);
function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.department') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}
// function onTableFilter(newFilters: any) {
//     giftStore.query.filters = { ...giftStore.query.filters, ...newFilters };
//     giftStore.setPage(1);
// }
function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function addModelEvent() {
    dialogMode.value = 'create';
    // commonStore.generateCode('GI', 'Gift', 'GiftCode', 4); // Nếu có mã tự động
    giftStore.selectGift(null);
    showFormModal.value = true;
}

function editModelEvent(gift: any) {
    dialogMode.value = 'edit';
    giftStore.selectGift({ ...gift });
    showFormModal.value = true;
}

function viewModelEvent(gift: any) {
    dialogMode.value = 'view';
    giftStore.selectGift({ ...gift });
    showFormModal.value = true;
}

async function handleSave(gift: any) {
    try {
        // startLoading(); // Giả định có hàm startLoading/stopLoading
        await giftStore.saveGift(gift);
        showFormModal.value = false;
        const modelName = t('models.gift');
        const toastKey = gift.id ? 'toast.updateSuccess' : 'toast.createSuccess';
        notificationStore.showToast('success', { key: toastKey, params: { model: modelName } });
        await giftStore.fetchAll();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        // stopLoading();
        showFormModal.value = false;
    }
}

async function handleDelete(groups: GiftModel | GiftModel[]) {
    const list = Array.isArray(groups) ? groups : [groups];
    console.log("list", list);

    const ids = list.filter(item => typeof item.id === 'string' && item.id).map(item => item.id as string);

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.promotionGroup') } },
        async () => {
            try {
                startLoading();
                await giftStore.deleteGifts(ids);
                await giftStore.fetchAll();

            } catch (err: any) {
                console.error('Error deleting:', err);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}
</script>