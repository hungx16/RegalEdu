<template>
    <div class="working-time-config">
        <div v-for="day in groupedWorkingTimes" :key="day.dayOfWeek"
            class="day-config mb-6 bg-white rounded-4 shadow-xs left-rounded-border">
            <div class="p-4 content-inner">
                <div class="d-flex align-items-center gap-3 mb-2 position-relative day-header-row">
                    <span class="fw-semibold fs-5" style="min-width: 85px;">{{ t(getDayOfWeekKey(day.dayOfWeek))
                        }}</span>
                    <el-switch :model-value="daysActiveMap[day.dayOfWeek]"
                        :loading="dayLoadingMap[day.dayOfWeek] === true"
                        @change="val => onToggleSwitch(day.dayOfWeek, val)" />
                    <el-tag size="small" class="ms-2 working-status-tag"
                        :style="day.active ? 'background:#7352A0;color:#fff;border-radius:8px;' : 'background:#e4e6ef;color:#999;border-radius:8px;'">
                        {{ day.active ? t('workingTime.working') : t('workingTime.off') }}
                    </el-tag>
                    <span v-if="day.active" class="ms-2 working-time-hour text-primary-emphasis">
                        {{ t('workingTime.hours', { hours: day.totalHours }) }}
                    </span>

                    <!-- Nút Thêm ca (chỉ hiện khi active) -->
                    <el-button v-if="day.active" size="small" class="add-shift-btn ms-auto" type="primary" plain
                        @click="onAddShift(day.dayOfWeek)">
                        <i class="bi bi-plus-lg"></i>
                        {{ t('workingTime.addShift') }}
                    </el-button>
                </div>

            </div>

            <!-- Nội dung ca -->
            <template v-if="day.active">
                <div v-if="day.shifts.length > 0" class="shifts-container px-3 px-md-4 pb-3 pb-md-4">
                    <div v-for="(shift, idx) in day.shifts" :key="shift.id"
                        class="shift-card mb-2 px-4 py-2 border rounded-3 d-flex align-items-center"
                        :class="{ 'mb-0': idx === day.shifts.length - 1 }">
                        <div class="flex-grow-1">
                            <div class="fw-semibold">{{ shift.name }}</div>
                            <div class="text-muted">{{ shift.startTime }} - {{ shift.endTime }}</div>
                        </div>
                        <div class="d-flex align-items-center gap-2 ms-3">
                            <el-button size="small" plain @click="onEditShift(shift)">
                                <i class="bi bi-pencil-square me-1"></i> {{ t('workingTime.edit') }}
                            </el-button>
                            <el-button size="small" plain @click="onDeleteShift(shift)">
                                <i class="bi bi-trash3 me-1"></i> {{ t('workingTime.delete') }}
                            </el-button>
                        </div>
                    </div>
                </div>
                <div v-else class="text-muted px-4 py-3">
                    {{ t('workingTime.noShift') }}
                </div>
            </template>
        </div>

        <WorkingTimeDialog v-model:visible="showAddShiftDialog" :workingTimeData="shiftForm" :loading="formLoading"
            :configurationName="selectedConfigName" :configurationId="props.configurationId" :mode="dialogMode"
            @submit="handleSave" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useWorkingTimeStore } from '@/stores/workingTimeStore';
import { DayOfWeek, getDayOfWeekKey } from '@/types/daysOfWeek';
import { useWorkingTimeConfigurationStore } from '@/stores/workingTimeConfigurationStore';
import WorkingTimeDialog from '@/views/working-time-configuration/WorkingTimeDialog.vue';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { useNotificationStore } from '@/stores/notificationStore';
import { ActionType } from '@/types';
const props = defineProps<{ configurationId: string }>();
const { t } = useI18n();
const workingTimeStore = useWorkingTimeStore();
const workingTimeConfigurationsStore = useWorkingTimeConfigurationStore();
const selectedConfig = computed(() => workingTimeConfigurationsStore.configurations.find(c => c.id === props.configurationId));
const selectedConfigName = computed(() => selectedConfig.value?.nameConfiguration || '');
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);
const notificationStore = useNotificationStore();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');

const dayLoadingMap = ref<{ [key: number]: boolean }>({});
const showAddShiftDialog = ref(false);
const shiftForm = ref({
    name: '',
    startTime: '',
    endTime: '',
    dayOfWeek: 0,
    isWorkingDay: true,
    workingTimeConfigurationId: props.configurationId,
});

// Helpers cho localStorage
function getStorageKey(configId: string) {
    return `wt_active_days_${configId}`;
}
function loadDaysActiveMap(configId: string): Record<number, boolean> {
    try {
        const raw = localStorage.getItem(getStorageKey(configId));
        return raw ? JSON.parse(raw) : {};
    } catch {
        return {};
    }
}
function saveDaysActiveMap(configId: string, map: Record<number, boolean>) {
    localStorage.setItem(getStorageKey(configId), JSON.stringify(map));
}

const daysActiveMap = ref<{ [key: number]: boolean }>({});

// Khi mount hoặc đổi cấu hình, nạp trạng thái từ localStorage, rồi sync với dữ liệu BE
async function setupAll() {
    await loadData();
    daysActiveMap.value = loadDaysActiveMap(props.configurationId);
    syncDaysActive();
}
onMounted(setupAll);
watch(() => props.configurationId, setupAll);

async function loadData() {
    if (!props.configurationId) return;
    dayLoadingMap.value = {};
    await workingTimeStore.fetchAllWorkingTimesByConfigId(props.configurationId);
}

// Đồng bộ trạng thái ngày làm việc giữa BE và localStorage
function syncDaysActive() {
    for (let i = DayOfWeek.Monday; i <= DayOfWeek.Sunday; i++) {
        const shifts = workingTimeStore.workingTimes.filter(item => item.dayOfWeek === i);
        if (shifts.length > 0) {
            daysActiveMap.value[i] = shifts.some(s => s.isWorkingDay);
        } else {
            daysActiveMap.value[i] = daysActiveMap.value[i] || false; // Lấy từ FE/localStorage
        }
    }
    saveDaysActiveMap(props.configurationId, daysActiveMap.value);
}

// Dùng daysActiveMap cho computed
const groupedWorkingTimes = computed(() => {
    const days: {
        dayOfWeek: DayOfWeek;
        active: boolean;
        shifts: any[];
        totalHours: number;
    }[] = [];
    for (let i = DayOfWeek.Monday; i <= DayOfWeek.Sunday; i++) {
        const shifts = workingTimeStore.workingTimes.filter(item => item.dayOfWeek === i);
        const active = daysActiveMap.value[i];
        const totalHours = shifts.reduce((sum, s) => {
            if (s.startTime && s.endTime) {
                const [h1, m1] = s.startTime.split(':').map(Number);
                const [h2, m2] = s.endTime.split(':').map(Number);
                return sum + ((h2 + m2 / 60) - (h1 + m1 / 60));
            }
            return sum;
        }, 0);
        days.push({
            dayOfWeek: i,
            active,
            shifts,
            totalHours: Number(totalHours.toFixed(2)),
        });
    }
    return days;
});

// Khi toggle switch, cập nhật daysActiveMap và backend nếu có ca
function onToggleSwitch(dayOfWeek: number, isActive: boolean) {
    // Update UI ngay lập tức
    daysActiveMap.value[dayOfWeek] = isActive;
    saveDaysActiveMap(props.configurationId, daysActiveMap.value);

    // Nếu có ca làm việc thì đồng bộ với backend
    const shifts = workingTimeStore.workingTimes.filter(item => item.dayOfWeek === dayOfWeek);
    if (shifts.length) {
        dayLoadingMap.value[dayOfWeek] = true;
        Promise.all(shifts.map(shift =>
            workingTimeStore.saveWorkingTime({
                ...shift,
                isWorkingDay: isActive,
            })
        )).then(() => {
            workingTimeStore.fetchAllWorkingTimesByConfigId(props.configurationId).then(() => {
                syncDaysActive();
                dayLoadingMap.value[dayOfWeek] = false;
            });
        });
    }
}


function onAddShift(dayOfWeek) {
    dialogMode.value = ActionType.Create
    showAddShiftDialog.value = true;
    shiftForm.value = {
        name: '',
        startTime: '',
        endTime: '',
        dayOfWeek,
        isWorkingDay: true,
        workingTimeConfigurationId: props.configurationId,
    }
}


async function handleSave(item: any) {
    startLoading();
    try {
        await workingTimeStore.saveWorkingTime(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.workingTimeConfiguration') } });
        showAddShiftDialog.value = false;
        await loadData();
        syncDaysActive();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
        //notificationStore.showToast('error', { key: 'toast.saveError', params: { model: t('models.workingTimeConfiguration') } });
    } finally {
        stopLoading();
    }
}
function onEditShift(shift: any) {
    // Mở dialog edit
    dialogMode.value = ActionType.Edit
    shiftForm.value = { ...shift };
    showAddShiftDialog.value = true;
}

function onDeleteShift(shift: any) {
    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.WorkingTime') } },
        async () => {
            startLoading();
            try {
                await workingTimeStore.deleteWorkingTimes([shift.id]);
                await loadData();
                syncDaysActive();
            } catch (err: any) {
                console.error('Error deleting:', err?.response?.data?.errors || err);
            } finally {
                stopLoading();
            }
        }
    );
}

</script>
<style lang="scss" scoped>
.left-rounded-border {
    border-left: 4px solid #60287a;
    border-top-left-radius: 20px;
    border-bottom-left-radius: 20px;
    background: #fff;
}

.day-config {
    box-sizing: border-box;
    // width: 100%;  // => BỎ width này!
    min-width: 0;
}

.day-config,
.left-rounded-border {
    width: 100%;
    box-sizing: border-box;
}

.border-active {
    border-left: 8px solid #60287a !important;
    /* màu tím khi active */
}

.working-time-config {
    width: 100%;
    max-width: 100vw;
}

.day-header-row>* {
    display: inline-flex;
    align-items: center;
    height: var(--button-height);
    /* hoặc đúng bằng height của el-switch */
}

.day-label {
    min-width: 85px;
    font-size: 1.1rem;
    /* CHÚ Ý: phải set height và align-items */
    height: 32px;
    display: inline-flex;
    align-items: center;
}

.working-status-tag,
.el-tag--small {
    height: var(--button-height);
}

.working-time-hour {
    height: var(--button-height);
}

.add-shift-btn {
    height: var(--button-height) !important;
}

.day-label {
    line-height: var(--button-height) !important;
    padding-top: 0 !important;
    padding-bottom: 0 !important;
}

.content-inner {
    padding: 1.5rem;
}


@media (max-width: 576px) {
    .day-header-row {
        flex-wrap: wrap !important;
        align-items: flex-start !important;
    }

    .add-shift-btn {
        width: 100%;
        margin-left: 0 !important;
        margin-top: 8px !important;
        text-align: left;
        order: 10;
    }

    .shifts-container,
    .text-muted.px-3 {
        padding-left: 12px !important;
        padding-right: 12px !important;
    }

    .shift-card {
        padding-left: 12px !important;
        padding-right: 12px !important;
    }
}
</style>