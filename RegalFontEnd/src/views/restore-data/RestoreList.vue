<template>
    <div>
        <!-- FilterComponent (đặt ngoài row/card) -->
        <FilterComponent ref="filterComponentRef" headerTitle="restore.headerTitle" headerDesc="restore.headerDesc"
            :disabledDelete="getDisableDelete" class="mb-6" />

        <div class="card">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex align-items-center">
                    <el-select v-model="selectedModelKey" placeholder="Chọn loại dữ liệu" size="large" class="me-4"
                        style="width: 250px;">
                        <el-option v-for="(config, key) in modelConfigs" :key="key" :label="t(config.labelKey)"
                            :value="key" />
                    </el-select>

                    <el-button type="primary" :disabled="isRestoreDisabled" @click="handleRestore"
                        :loading="currentStore?.loading">
                        <el-icon class="me-2">
                            <Refresh />
                        </el-icon>
                        Khôi phục ({{ selectedRows.length }})
                    </el-button>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <template v-if="selectedModelKey">
                    <BaseTable :key="selectedModelKey" :columns="currentColumns" :items="filteredItems"
                        :loading="currentStore?.loading" :show-checkbox-column="true" :show-pagination="true"
                        :page="page" :pageSize="pageSize" :total="filteredItems.length" :filter="filter"
                        @update:filter="onTableFilter" @update:rows="val => selectedRows = val"
                        @update:page="val => page = val" @update:pageSize="onPageSizeChange"
                        :show-actions-column="false" height="600" />
                </template>
                <div v-else class="text-center text-muted py-10">
                    {{ t('restore.noData') }}
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElSelect, ElOption, ElButton, ElIcon } from 'element-plus';
import { Refresh } from '@element-plus/icons-vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';

// Import components
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { formatDate } from '@/utils/format';

// Import Stores
import { useCompanyStore } from '@/stores/companyStore';
import { useDivisionStore } from '@/stores/divisionStore';
import { useDegreeStore } from '@/stores/degreeStore';
import { usePositionStore } from '@/stores/positionStore';
/** ===== KHAI BÁO CẤU HÌNH ===== */
const listHeaderParams = {
    listParams: [],
    listBtn: [
    ],
};
const filterComponentRef = ref();

/** ===== LIFECYCLE ===== */
onMounted(() => {
    filterComponentRef.value?.initListHeaderParams(listHeaderParams);
});
/** ===== KHAI BÁO BIẾN & REF ===== */
const { t } = useI18n();
const notificationStore = useNotificationStore();

// Instantiate stores
const companyStore = useCompanyStore();
const divisionStore = useDivisionStore();
const degreeStore = useDegreeStore();
const positionStore = usePositionStore();

// Component state
const selectedModelKey = ref<string>('');
const selectedRows = ref<any[]>([]);

// 1. THÊM STATE ĐỂ QUẢN LÝ FILTER VÀ PAGINATION (GIỐNG HỆT DIVISIONLIST)
const page = ref(1);
const pageSize = ref(30);
const filter = ref<Record<string, any>>({});

const getDisableDelete = computed(() => selectedRows.value.length === 0);

interface ModelConfig {
    labelKey: string;
    store: any;
    fetchAction: () => Promise<void>;
    restoreAction: (ids: string[]) => Promise<void>;
    itemsStateKey: string;
    columns: BaseTableColumn[];
}

/** ===== ĐỊNH NGHĨA CẤU HÌNH CHO TỪNG MODEL ===== */
const modelConfigs: Record<string, ModelConfig> = {
    company: {
        labelKey: 'models.Company',
        store: companyStore,
        fetchAction: companyStore.fetchDeletedCompanies,
        restoreAction: companyStore.restoreCompanies,
        itemsStateKey: 'companies',
        columns: [
            { key: 'companyCode', labelKey: 'company.code', sticky: true, width: 150, filterType: 'text' },
            { key: 'companyName', labelKey: 'company.name', sticky: true, width: 250, isBold: true, filterType: 'text' },
            { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 250 },
            {
                key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true,
                formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
            },
        ]
    },
    division: {
        labelKey: 'models.Division',
        store: divisionStore,
        fetchAction: divisionStore.fetchDeletedPositions,
        restoreAction: divisionStore.restoreDivisions,
        itemsStateKey: 'divisions',
        columns: [
            { key: 'divisionCode', labelKey: 'division.code', sticky: true, width: 160, filterType: 'text' },
            { key: 'divisionName', labelKey: 'division.name', sticky: true, width: 250, isBold: true, filterType: 'text' },
            { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 250 },
            {
                key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true,
                formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
            },
        ]
    },
    degree: {
        labelKey: 'models.Degree',
        store: degreeStore,
        fetchAction: degreeStore.fetchDeletedDegrees,
        restoreAction: degreeStore.restoreDegrees,
        itemsStateKey: 'degrees',
        columns: [
            { key: 'degreeName', labelKey: 'degree.name', isBold: true, width: 300, filterType: 'text' },
            { key: 'description', labelKey: 'degree.description', filterType: 'text' },
            { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 250 },
            {
                key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true,
                formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
            },
        ]
    },
    position: {
        labelKey: 'models.Position',
        store: positionStore,
        fetchAction: positionStore.fetchDeletedPositions,
        restoreAction: positionStore.restorePositions,
        itemsStateKey: 'positions',
        columns: [
            { key: 'positionCode', labelKey: 'position.code', width: 160, filterType: 'text' },
            { key: 'positionName', labelKey: 'position.name', isBold: true, width: 250, filterType: 'text' },
            { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 250 },
            {
                key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true,
                formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
            },
        ]
    }
};


/** ===== COMPUTED ===== */
const currentConfig = computed<ModelConfig | null>(() => {
    return selectedModelKey.value ? modelConfigs[selectedModelKey.value] : null;
});

const currentStore = computed(() => currentConfig.value?.store);

const currentColumns = computed<BaseTableColumn[]>(() => {
    return currentConfig.value?.columns ?? [];
});

const rawDeletedItems = computed(() => {
    if (!currentStore.value || !currentConfig.value) return [];
    return currentStore.value[currentConfig.value.itemsStateKey];
});

// 2. THÊM COMPUTED ĐỂ LỌC DỮ LIỆU (GIỐNG HỆT filteredDivisionsAll)
const filteredItems = computed(() => {
    let arr = [...rawDeletedItems.value];
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            // Logic lọc cho ngày tháng
            const columnDef = currentColumns.value.find(c => c.key === key);
            if (columnDef?.filterType === 'date') {
                arr = arr.filter(item => {
                    if (!item[key]) return false;
                    const dateOnly = String(item[key]).substring(0, 10);
                    return dateOnly === val;
                });
            } else { // Logic lọc cho các kiểu text, select...
                arr = arr.filter(item =>
                    String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
                );
            }
        }
    });
    return arr;
});

const isRestoreDisabled = computed(() => selectedRows.value.length === 0);

/** ===== FUNCTION HANDLER ===== */

// 3. THÊM CÁC HANDLER ĐỂ CẬP NHẬT STATE (GIỐNG HỆT DIVISIONLIST)
function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1; // reset page khi filter
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1; // reset page khi đổi page size
}

async function fetchDeletedData() {
    if (!currentConfig.value) return;
    // Reset state khi fetch dữ liệu mới
    selectedRows.value = [];
    filter.value = {};
    page.value = 1;
    await currentConfig.value.fetchAction();
}

async function handleRestore() {
    if (!currentConfig.value || selectedRows.value.length === 0) return;

    const ids = selectedRows.value.map(row => row.id).filter((id): id is string => !!id);
    if (ids.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noValidId' });
        return;
    }

    const modelName = t(currentConfig.value.labelKey).toLowerCase();

    notificationStore.showConfirm(
        { key: 'toast.restoreConfirm', params: { count: ids.length, model: modelName } },
        async () => {
            try {
                await currentConfig.value!.restoreAction(ids);
                notificationStore.showToast('success', {
                    key: 'toast.restoreSuccess',
                    params: { model: modelName }
                });
                await fetchDeletedData();
            } catch (error) {
                console.error("Restore failed:", error);
                notificationStore.showToast('error', { key: 'toast.restoreError' });
            }
        }
    );
}

/** ===== WATCHERS ===== */
watch(selectedModelKey, (newKey) => {
    if (newKey) {
        fetchDeletedData();
    }
});

</script>