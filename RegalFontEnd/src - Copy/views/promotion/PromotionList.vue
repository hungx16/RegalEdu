<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="promotion.headerTitle" headerDesc="promotion.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('promotion.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('promotion.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredPromotion"
                    :loading="promotionStore.loading" :showPagination="true" :page="promotionStore.query.page"
                    :total="promotionStore.total" :pageSize="promotionStore.query.pageSize"
                    @update:filter="onTableFilter" :filter="filter" @update:rows="val => selectedRowsData = val"
                    @edit="editModelEvent" @view="viewModelEvent" :showActionsColumn="true" :showEdit="true"
                    :showDelete="true" :showView="true" @delete="handleDelete"
                    @update:page="val => promotionStore.setPage(val)"
                    @update:pageSize="val => promotionStore.setPageSize(val)">
                    <template #cell-promoCode="{ item }">
                        <span class="link-primary fw-bold">{{ item.promoCode }}</span>
                    </template>
                    <template #cell-startDate="{ item }">
                        <span>{{ formatDate(item.startDate, 'DD/MM/YYYY') }}</span>
                    </template>
                    <template #cell-endDate="{ item }">
                        <span>{{ formatDate(item.endDate, 'DD/MM/YYYY') }}</span>
                    </template>
                    <template #cell-type="{ item }">
                        <BaseBadge :label="getPromotionType(item.type)" />
                    </template>
                </BaseTable>
            </div>
        </div>

        <PromotionDialog v-model:visible="showFormModal" :mode="dialogMode"
            :promotion-data="promotionStore.selectedPromotion" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import PromotionDialog from './PromotionDialog.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { formatDate } from '@/utils/format';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { usePromotionStore } from '@/stores/promotionStore';
import { useNotificationStore } from '@/stores/notificationStore';
import type { PromotionModel } from '@/api/PromotionApi';

const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);
const { t } = useI18n();
const promotionStore = usePromotionStore();
const notificationStore = useNotificationStore();
const filter = ref({}); // Bộ lọc hiện tại của bảng
const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const selectedRowsData = ref<Array<PromotionModel>>([]);

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

const getPromotionType = (type: number) => {
    switch (type) {
        case 0: return t('promotiontype.discount');
        case 1: return t('promotiontype.gift');
        case 2: return t('promotiontype.coupon');
        case 3: return t('promotiontype.fixedPrice');
        default: return t('promotiontype.other');
    }
};
//tạo fitter cho bảng
// --- Computed Property cho Lọc (tương tự CouponTypeList.vue) ---
const filteredPromotion = computed(() => {
    let arr = promotionStore.promotions.slice(); // Sao chép mảng gốc

    // Áp dụng bộ lọc từ BaseTable
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            const valStr = String(val).toLowerCase();

            if (key === 'createdAt') {
                // Lọc theo ngày (giả định val là chuỗi YYYY-MM-DD)
                arr = arr.filter(item => item.createdAt?.substring(0, 10) === val);
            } else if (['type', 'status', 'paymentStatus'].includes(key)) {
                // Lọc theo giá trị số/enum
                arr = arr.filter(item => String(item[key]) === valStr);
            } else {
                // Lọc theo chuỗi (code, name, companyName, etc.)
                arr = arr.filter(item =>
                    String(item[key] ?? '').toLowerCase().includes(valStr)
                );
            }
        }
    });
    return arr;
});
const columns: BaseTableColumn[] = [
    { key: 'promoCode', labelKey: 'promotion.code', filterType: 'text', sortable: true, sticky: true, width: 130, isBold: true, align: 'center' },
    { key: 'name', labelKey: 'promotion.name', filterType: 'text', sortable: true, sticky: true, width: 160, isBold: true },
    {
        key: 'startDate', labelKey: 'promotion.startDate', filterType: 'date', sortable: true, width: 150, align: 'center',
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    {
        key: 'endDate', labelKey: 'promotion.endDate', filterType: 'date', sortable: true, width: 150, align: 'center',
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'description', labelKey: 'promotion.description', filterType: 'text', sortable: false },
    { key: 'type', labelKey: 'promotion.type', filterType: 'select', sortable: true, width: 180, align: 'center' },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
];

function onTableFilter(val: Record<string, any>) {
    filter.value = val;

}

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.promotion') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function handleDelete(promotions) {
    const list = Array.isArray(promotions) ? promotions : [promotions];
    const ids = list.filter(item => typeof item.id === 'string' && item.id).map(item => item.id as string);

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.promotion') } },
        async () => {
            try {
                startLoading();
                await promotionStore.deletePromotions(ids);
                await promotionStore.fetchPagedPromotions();
                notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.promotion') } });
            } catch (err) {
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
    promotionStore.selectPromotion(null);
    showFormModal.value = true;
}

function editModelEvent(promotion) {
    dialogMode.value = 'edit';
    promotionStore.selectPromotion({ ...promotion });
    showFormModal.value = true;
}

function viewModelEvent(promotion) {
    dialogMode.value = 'view';
    promotionStore.selectPromotion({ ...promotion });
    showFormModal.value = true;
}

async function handleSave(promotion) {
    console.log('Dữ liệu promotion:', promotion);
    try {
        startLoading();
        await promotionStore.savePromotion(promotion);
        showFormModal.value = false;
        if (promotion.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.promotion') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.promotion') } });
        }
        await promotionStore.fetchPagedPromotions();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
    }
}

watch(
    () => [promotionStore.query.page, promotionStore.query.pageSize],
    () => {
        promotionStore.fetchAllPromotions();
    },
);

onMounted(() => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'promotion.add', type: 'add' },
            { event: 'delete', label: 'promotion.delete', type: 'delete' },
        ],
    });
    promotionStore.fetchAllPromotions();
});
</script>