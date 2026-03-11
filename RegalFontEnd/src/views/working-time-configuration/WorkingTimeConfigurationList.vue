<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent"
            headerTitle="workingTimeConfiguration.headerTitle" headerDesc="workingTimeConfiguration.headerDesc"
            :disabledDelete="getDisableDelete" class="mb-6" />
        <!-- ROW cho 2 CARD -->
        <div class="card card-flush py-4 px-6 mb-8 shadow-sm rounded-4">
            <!-- Tiêu đề -->
            <div class="d-flex align-items-center gap-2 mb-4">
                <i class="bi bi-gear fs-2 text-primary"></i>
                <span class="fw-semibold fs-4">{{ t('workingTimeConfiguration.chooseConfiguration') }}</span>
            </div>

            <div class="row align-items-center mb-2">
                <div class="col-12 col-md-10 d-flex align-items-center gap-2 flex-wrap w-100-mobile">
                    <el-select v-model="selectedConfigId" placeholder="Chọn cấu hình" class="flex-grow-1"
                        style="min-width: 160px; width: 100%">
                        <el-option v-for="item in configurations" :key="item.id" :label="item.nameConfiguration"
                            :value="item.id">
                            <template #default>
                                <span class="d-flex align-items-center">
                                    <i v-if="item.applyToSystem" class="bi bi-globe-americas text-info me-1"></i>
                                    {{ item.nameConfiguration }}
                                </span>
                                <el-tag v-if="item.status === StatusType.Active" type="success" effect="dark"
                                    class="fs-8 px-4 py-1 rounded-pill">
                                    {{ t('workingTimeConfiguration.isActive') }}
                                </el-tag>
                            </template>
                        </el-option>
                    </el-select>


                </div>
                <div
                    class="col-12 col-md-2 d-flex justify-content-md-end align-items-center gap-2 mt-2 mt-md-0 w-100-mobile">
                    <el-button size="small" @click="onEdit"><i class="bi bi-pencil-square me-1"></i>{{ t('common.edit')
                    }}</el-button>
                    <el-dropdown @command="handleDropdownCommand">
                        <el-button size="small">
                            <i class="bi bi-three-dots-vertical"></i>
                        </el-button> <template #dropdown>
                            <el-dropdown-menu>
                                <el-dropdown-item command="copy">
                                    <i class="bi bi-clipboard me-2"></i> {{ t('workingTimeConfiguration.copyConfig') }}
                                </el-dropdown-item>
                                <el-dropdown-item command="save-template">
                                    <i class="bi bi-save2 me-2"></i> {{ t('workingTimeConfiguration.saveAsTemplate') }}
                                </el-dropdown-item>
                                <el-dropdown-item command="delete" divided>
                                    <i class="bi bi-trash3 me-2 text-danger"></i>
                                    <span class="text-danger">{{ t('workingTimeConfiguration.deleteConfig') }}</span>
                                </el-dropdown-item>
                            </el-dropdown-menu>
                        </template>
                    </el-dropdown>


                </div>
            </div>

            <!-- Thông tin mô tả -->
            <div class="bg-body-secondary rounded-3 p-4 mt-3">
                <div class="mb-1">
                    <span class="fw-bold">{{ t('workingTimeConfiguration.description') }}: </span>
                    <span>{{ selectedConfig?.description }}</span>
                </div>
                <div>
                    <span class="fw-bold">{{ t('workingTimeConfiguration.apply') }}: </span>
                    <span>
                        <template v-if="selectedConfig?.applyToSystem">
                            {{ t('workingTimeConfiguration.applyToSystem') }}
                            <i class="bi bi-globe-americas text-info ms-1"></i>
                        </template>
                        <template v-else>
                            {{ t('workingTimeConfiguration.applyDepartment') }}
                        </template>
                    </span>
                </div>
            </div>
        </div>
        <TabbedComponent :tabs="tabs" v-model="currentTab">
            <template #workingTime>
                <div class="card card-flush py-4 px-6 mb-8 shadow-sm rounded-4">
                    <!-- Tiêu đề -->
                    <div class="d-flex flex-column align-items-start gap-2 mb-4">
                        <span class="fw-semibold fs-4">{{ t('workingTime.headerTitle') }}</span>
                        <div class="text-muted fs-8">{{ t('workingTime.headerDesc') }}</div>
                    </div>
                    <WorkingTimeTab v-if="selectedConfigId" :configuration-id="selectedConfigId || ''" />
                </div>
            </template>
            <template #holiday>
                <HolidayTab v-if="selectedConfigId" :configuration-id="selectedConfigId || ''" />
            </template>
            <template #previewCalendar>
                <PreviewCalendarTab v-if="selectedConfigId" :configuration-id="selectedConfigId" />

            </template>
        </TabbedComponent>
        <WorkingTimeConfigurationDialog v-model:visible="showFormModal" :mode="dialogMode"
            :working-time-configuration-data="configurationStore.selectedConfiguration" :loading="formLoading"
            @submit="handleSave" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useWorkingTimeConfigurationStore } from '@/stores/workingTimeConfigurationStore';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import WorkingTimeConfigurationDialog from './WorkingTimeConfigurationDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { StatusType } from '@/types';
import TabbedComponent from '@/components/tabbed/TabbedComponent.vue';
import WorkingTimeTab from './WorkingTimeTab.vue';
import HolidayTab from './HolidayTab.vue';
import PreviewCalendarTab from './PreviewCalendarTab.vue';
const { t } = useI18n();
const configurationStore = useWorkingTimeConfigurationStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);
const tabs = [
    { name: 'workingTime', label: t('models.WorkingTime') },
    { name: 'holiday', label: t('models.Holiday') },
    { name: 'previewCalendar', label: t('models.PreviewCalendar') }
];
const currentTab = ref('workingTime');
const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');

const selectedRowsData = ref([]);
const configurations = computed(() => configurationStore.configurations);

const selectedConfigId = ref(configurations.value[0]?.id ?? null); // Chọn config đầu tiên hoặc null

const selectedConfig = computed(() =>
    configurations.value.find(cfg => cfg.id === selectedConfigId.value)
);
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);


function handleDelete(items: any | any[]) {
    const list = Array.isArray(items) ? items : [items];
    const ids = list.map(item => item.id).filter(Boolean);
    notificationStore.showConfirm(
        {
            key: 'toast.delete',
            params: { model: t('models.workingTimeConfiguration') },
        },
        async () => {
            try {
                startLoading();
                await configurationStore.deleteWorkingTimeConfigurations(ids);
                await configurationStore.fetchAllWorkingTimeConfigurations();
            } catch (error) {
                console.error(error);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    configurationStore.selectConfiguration(null);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit';
    configurationStore.selectConfiguration({ ...item });
    showFormModal.value = true;
}


async function handleSave(item: any) {
    startLoading();
    try {
        await configurationStore.saveWorkingTimeConfiguration(item);
        await configurationStore.fetchAllWorkingTimeConfigurations();
        showFormModal.value = false;
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
    }
}



onMounted(async () => {
    await configurationStore.fetchAllWorkingTimeConfigurations();

    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'workingTimeConfiguration.add', type: 'add' },
        ]
    });

});
// Theo dõi khi configurations thay đổi
watch(configurations, (configs) => {
    if (configs.length > 0) {
        // Ưu tiên chọn config áp dụng toàn hệ thống, rồi đến config đầu tiên
        const globalConfig = configs.find(cfg => cfg.applyToSystem && cfg.status === StatusType.Active);
        selectedConfigId.value = globalConfig?.id ?? configs[0]?.id ?? null;
    } else {
        selectedConfigId.value = null;
    }
}, { immediate: true });
function handleDropdownCommand(cmd: string) {
    switch (cmd) {
        case 'copy':
            // Gọi hàm sao chép cấu hình
            copyConfig();
            break;
        case 'save-template':
            // Gọi hàm lưu làm mẫu
            saveAsTemplate();
            break;
        case 'delete':
            // Gọi hàm xóa
            deleteConfig();
            break;
    }
}

function copyConfig() {
    // Xử lý logic sao chép
}
function saveAsTemplate() {
    // Xử lý logic lưu làm mẫu
}
function deleteConfig() {
    if (!selectedConfig.value) return;
    handleDelete([selectedConfig.value]);
}

function onEdit() {
    if (!selectedConfigId.value) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.workingTimeConfiguration') } });
        return;
    }
    editModelEvent(selectedConfig.value);
}
</script>
<style scoped>
.bg-body-secondary {
    background: #fafbfc;
}

.fs-8 {
    font-size: 14px;
}

.rounded-pill {
    border-radius: 999px !important;
}

@media (max-width: 768px) {
    .w-100-mobile {
        width: 100% !important;
        justify-content: flex-start !important;
        margin-bottom: 8px;
    }
}
</style>