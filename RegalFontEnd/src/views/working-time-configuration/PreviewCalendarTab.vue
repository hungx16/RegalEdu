<template>
    <div class="card card-flush py-4 px-6 mb-8 shadow-sm rounded-4">
        <div class="d-flex flex-wrap gap-4">


            <!-- Calendar -->
            <div style="min-width:320px;">
                <div class="fw-semibold fs-5 mb-1">{{ t('previewCalendar.headerTitle') }}</div>
                <div class="text-muted fs-8 mb-3">{{ t('previewCalendar.headerDesc') }}</div>

                <div class="calendar-preview-box rounded-3 p-3">
                    <el-calendar v-model="calendarDate" :range="monthRange" class="preview-calendar">
                        <template #header>
                            <!-- Bộ chuyển tháng -->
                            <div class="d-flex align-items-center gap-3 mb-3">
                                <el-button size="small" @click="prevMonth" plain>
                                    <i class="bi bi-chevron-left me-1"></i>
                                    {{ t('previewCalendar.prevMonth') }}
                                </el-button>
                                <div class="fw-semibold">
                                    {{ t('previewCalendar.month') }} {{ calendarDate.getMonth() + 1 }}/{{
                                        calendarDate.getFullYear() }}
                                </div>
                                <el-button size="small" @click="nextMonth" plain>
                                    {{ t('previewCalendar.nextMonth') }}
                                    <i class="bi bi-chevron-right ms-1"></i>
                                </el-button>
                            </div>
                        </template>
                        <template #date-cell="{ data }">
                            <div class="cell-inner" :class="getDayClass(data.day)">
                                <span>{{ Number(data.day.split('-')[2]) }}</span>
                            </div>
                        </template>
                    </el-calendar>
                </div>
            </div>

            <!-- Legend + Stats -->
            <div class="flex-grow-1" style="min-width:240px;">
                <div class="fw-semibold fs-6 mb-2">{{ t('previewCalendar.legend') }}</div>
                <div class="d-flex flex-column gap-1 mb-4">
                    <div class="d-flex align-items-center gap-2">
                        <span class="legend-box bg-work"></span> {{ t('previewCalendar.workingDay') }}
                    </div>
                    <div class="d-flex align-items-center gap-2">
                        <span class="legend-box bg-holiday"></span> {{ t('previewCalendar.holiday') }}
                    </div>
                    <div class="d-flex align-items-center gap-2">
                        <span class="legend-box bg-off"></span> {{ t('previewCalendar.offDay') }}
                    </div>
                </div>

                <div class="fw-semibold fs-6 mb-2">{{ t('previewCalendar.statistics') }}</div>
                <div class="fs-7">
                    <div>{{ t('previewCalendar.workingDayPerWeek') }}: {{ stat.workingDayPerWeek }}</div>
                    <div>{{ t('previewCalendar.totalWorkingHoursPerWeek') }}: {{ stat.totalWorkingHoursPerWeek }}</div>
                    <div>{{ t('previewCalendar.totalHoliday') }}: {{ stat.totalHoliday }}</div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useHolidayStore } from '@/stores/useHolidayStore';
import { useWorkingTimeStore } from '@/stores/workingTimeStore';

const props = defineProps<{ configurationId: string }>();
const { t } = useI18n();
const holidayStore = useHolidayStore();
const workingTimeStore = useWorkingTimeStore();

const now = new Date();
const calendarDate = ref(new Date(now.getFullYear(), now.getMonth(), 1));

function getMonthRange(date: Date) {
    const start = new Date(date.getFullYear(), date.getMonth(), 1);
    const end = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    return [start, end];
}
const monthRange = computed(() => getMonthRange(calendarDate.value));

// Lọc dữ liệu theo configurationId

const filteredHolidays = computed(() =>
    holidayStore.holidays.filter(h => h.workingTimeConfigurationId === props.configurationId)
);
onMounted(async () => {
    // Kiểm tra xem có holidays và workingTimes đã được tải chưa
    await useHolidayStore().fetchAllHolidays();
    await useWorkingTimeStore().fetchAllWorkingTimes();
});
const holidaysSet = computed(() => {
    const y = calendarDate.value.getFullYear();
    const m = String(calendarDate.value.getMonth() + 1).padStart(2, '0');
    return new Set(
        filteredHolidays.value
            .filter(h => {
                const baseDate = h.date.slice(0, 10); // lấy đúng YYYY-MM-DD
                if (h.frequency === 0) {
                    // Lặp lại hàng năm: tháng phải giống, ngày phải thuộc tháng
                    const [, hMonth] = baseDate.split('-');
                    return hMonth === m;
                } else {
                    // Nghỉ 1 lần: đúng năm-tháng
                    return baseDate.slice(0, 7) === `${y}-${m}`;
                }
            })
            .map(h => {
                const baseDate = h.date.slice(0, 10); // luôn lấy YYYY-MM-DD
                if (h.frequency === 0) {
                    // Xuất ra ngày đúng tháng đang xem, năm nào cũng được
                    const [, , hDay] = baseDate.split('-');
                    return `${y}-${m}-${String(hDay).padStart(2, '0')}`;
                }
                return baseDate;
            })
    );
});




const filteredWorkingTimes = computed(() =>
    workingTimeStore.workingTimes.filter(wt => wt.workingTimeConfigurationId === props.configurationId)
);
function prevMonth() {
    calendarDate.value = new Date(calendarDate.value.getFullYear(), calendarDate.value.getMonth() - 1, 1);
}
function nextMonth() {
    calendarDate.value = new Date(calendarDate.value.getFullYear(), calendarDate.value.getMonth() + 1, 1);
}

// Danh sách ngày trong tháng
function getAllDaysInMonth(year: number, month: number): string[] {
    const days: string[] = [];
    const d = new Date(year, month, 1);
    while (d.getMonth() === month) {
        days.push(d.toISOString().slice(0, 10));
        d.setDate(d.getDate() + 1);
    }
    return days;
}
const allDays = computed(() =>
    getAllDaysInMonth(calendarDate.value.getFullYear(), calendarDate.value.getMonth())
);



// Ngày làm việc (theo thứ trong tuần)
const workingDayOfWeek = computed(() =>
    filteredWorkingTimes.value.filter(wt => wt.isWorkingDay).map(wt => wt.dayOfWeek)
);

const workingHoursPerDay = computed(() =>
    filteredWorkingTimes.value.filter(wt => wt.isWorkingDay).reduce((sum, wt) => {
        if (wt.startTime && wt.endTime) {
            const [h1, m1] = wt.startTime.split(':').map(Number);
            const [h2, m2] = wt.endTime.split(':').map(Number);
            return sum + ((h2 + m2 / 60) - (h1 + m1 / 60));
        }
        return sum;
    }, 0)
);

// Phân loại từng ngày
function getDayClass(dateStr: string) {
    // Ưu tiên nghỉ lễ cao nhất
    if (holidaysSet.value.has(dateStr)) return 'cell-holiday';

    // Nghỉ thường: KHÔNG phải ngày làm việc
    const d = new Date(dateStr);
    const dow = d.getDay();

    // Nếu KHÔNG thuộc workingDayOfWeek => nghỉ thường (cell-off)
    if (!workingDayOfWeek.value.includes(dow)) return 'cell-off';

    // Nếu không phải nghỉ lễ, không phải nghỉ thường => là ngày làm việc
    return 'cell-work';
}


// Thống kê
const stat = computed(() => {
    return {
        workingDayPerWeek: workingDayOfWeek.value.length,
        totalWorkingHoursPerWeek: (workingHoursPerDay.value * workingDayOfWeek.value.length).toFixed(1),
        totalHoliday: Array.from(holidaysSet.value).length,
    }
});
</script>

<style scoped>
.calendar-preview-box {
    background: #fff;
    border: 1px solid #e4e6ef;
    min-width: 320px;
}

.preview-calendar {
    --el-calendar-cell-width: 38px;
    --el-calendar-cell-height: 38px;
}

/* Căn giữa tuyệt đối số ngày trong mọi ô lịch */
.preview-calendar .el-calendar-table .el-calendar-day {
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
    height: 100% !important;
    padding: 0 !important;
}

/* Đảm bảo mỗi ô lịch chiếm toàn bộ chiều cao và căn giữa tuyệt đối */
.preview-calendar .el-calendar-table td {
    vertical-align: middle !important;
    height: 48px !important;
    /* hoặc giá trị bạn muốn */
    padding: 0 !important;
}

/* Số ngày luôn ở giữa, bo tròn, nền đẹp */
.cell-inner {
    width: 32px;
    height: 90%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: auto;
    border-radius: 8px;
    font-weight: 500;
    text-align: center;
}

.cell-work {
    background: #e8f3ff;
    color: #3471c9;
}

.cell-holiday {
    background: #ffeaea;
    color: #e35d6a;
}

.cell-off {
    background: #f8f9fa;
    color: #999;
}

/* Legend style */
.legend-box {
    display: inline-block;
    width: 18px;
    height: 18px;
    border-radius: 6px;
    border: 1px solid #d6dee8;
}

.bg-work {
    background: #e8f3ff;
}

.bg-holiday {
    background: #ffeaea;
}

.bg-off {
    background: #f8f9fa;
}

/* -------- Responsive: căn giữa toàn bộ trên mobile -------- */
@media (max-width: 900px) {
    .d-flex.flex-wrap.gap-4 {
        flex-direction: column !important;
        align-items: center !important;
        gap: 16px !important;
    }

    .calendar-preview-box {
        min-width: 0;
        width: 100%;
        padding: 6px !important;
    }

    .flex-grow-1 {
        min-width: 0 !important;
        width: 100% !important;
        max-width: 100vw;
    }

    .preview-calendar .el-calendar__header,
    .calendar-preview-box .d-flex.align-items-center {
        justify-content: center !important;
    }

    .preview-calendar .d-flex.align-items-center.gap-3.mb-3 {
        justify-content: center !important;
        width: 100%;
    }

    .cell-inner {
        width: 30px;
        height: 30px;
        font-size: 1rem;
    }
}

@media (max-width: 576px) {
    .calendar-preview-box {
        padding: 3px !important;
        min-width: 0 !important;
    }

    .cell-inner {
        width: 28px;
        height: 28px;
        font-size: 0.95rem;
    }
}
</style>
