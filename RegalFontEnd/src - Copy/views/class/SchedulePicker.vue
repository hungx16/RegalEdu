<template>
    <div class="schedule-picker">
        <el-row v-for="(item, index) in localSchedule" :key="index" :gutter="10" class="mb-2">
            <!-- Chọn thứ -->
            <el-col :span="10">
                <el-select v-model="item.dayOfWeek" placeholder="Chọn thứ" :disabled="disabled" style="width: 100%">
                    <el-option v-for="d in daysOfWeek" :key="d.value" :label="d.label" :value="d.value" />
                </el-select>
            </el-col>

            <!-- Chọn ca học -->
            <el-col :span="10">
                <el-select v-model="item.sessionId" placeholder="Chọn ca học" :disabled="disabled" style="width: 100%">
                    <el-option v-for="s in sessions" :key="s.id" :label="s.name" :value="s.id" />
                </el-select>
            </el-col>

            <!-- Nút xóa -->
            <el-col :span="4" class="flex items-center justify-center">
                <el-button v-if="!disabled" type="danger" icon="el-icon-delete" circle @click="removeSchedule(index)" />
            </el-col>
        </el-row>

        <div class="mt-2 flex justify-end" v-if="!disabled">
            <el-button type="primary" text icon="el-icon-plus" @click="addSchedule">
                {{ t('class.addSchedule') }}
            </el-button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface ScheduleItem {
    dayOfWeek: number | null
    sessionId: string | null
}

const props = defineProps<{
    modelValue: ScheduleItem[],
    disabled?: boolean,
    programType?: string
}>()

const emit = defineEmits(['update:modelValue'])

const localSchedule = ref<ScheduleItem[]>(props.modelValue || [])

watch(
    () => props.modelValue,
    (val) => {
        if (val) localSchedule.value = JSON.parse(JSON.stringify(val))
    },
    { deep: true }
)

watch(
    () => localSchedule.value,
    (val) => emit('update:modelValue', val),
    { deep: true }
)

const daysOfWeek = [
    { value: 1, label: 'Thứ 2' },
    { value: 2, label: 'Thứ 3' },
    { value: 3, label: 'Thứ 4' },
    { value: 4, label: 'Thứ 5' },
    { value: 5, label: 'Thứ 6' },
    { value: 6, label: 'Thứ 7' },
    { value: 0, label: 'Chủ nhật' }
]

// Giả sử ca học lấy từ API hoặc store
const sessions = ref([
    { id: 'AM', name: 'Ca sáng' },
    { id: 'PM', name: 'Ca chiều' },
    { id: 'EV', name: 'Ca tối' }
])

function addSchedule() {
    localSchedule.value.push({ dayOfWeek: null, sessionId: null })
}

function removeSchedule(index: number) {
    localSchedule.value.splice(index, 1)
}

// Nếu muốn tự động sinh số dòng theo loại chương trình
watch(
    () => props.programType,
    (val) => {
        if (val) {
            // ví dụ: mapping số buổi / tuần theo loại chương trình
            const defaultCount = programTypeToSessionCount(val)
            if (localSchedule.value.length < defaultCount) {
                for (let i = localSchedule.value.length; i < defaultCount; i++) {
                    localSchedule.value.push({ dayOfWeek: null, sessionId: null })
                }
            }
        }
    },
    { immediate: true }
)

function programTypeToSessionCount(type: string): number {
    // tạm hardcode, thực tế có thể lấy từ API
    const mapping: Record<string, number> = {
        BASIC: 3,
        ADVANCED: 5
    }
    return mapping[type] ?? 2
}
</script>

<style scoped>
.schedule-picker {
    width: 100%;
}
</style>
