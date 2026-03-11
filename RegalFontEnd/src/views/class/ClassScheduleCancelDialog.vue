<template>
    <el-dialog :model-value="visible" width="920px" :close-on-click-modal="false" :destroy-on-close="true"
        @close="emit('update:visible', false)">
        <template #header>
            <div class="dlg-header">
                <div class="dlg-title">{{ t('classSchedule.cancelDialog.title') }}</div>
                <div class="dlg-sub">{{ t('classSchedule.cancelDialog.subTitle') }}</div>
            </div>
        </template>

        <el-form ref="formRef" :model="form" :rules="rules" label-position="top">
            <el-row :gutter="16">
                <!-- Loại huỷ -->
                <el-col :span="12">
                    <el-form-item :label="t('classSchedule.cancelDialog.cancelTypeLabel')" prop="cancelType">
                        <el-select v-model="form.cancelType" class="w-100"
                            :placeholder="t('classSchedule.cancelDialog.cancelTypePlaceholder')">
                            <el-option :value="CancelType.Shifting"
                                :label="t('classSchedule.cancelDialog.cancelType.shift')" />
                            <el-option :value="CancelType.Substitution"
                                :label="t('classSchedule.cancelDialog.cancelType.substitute')" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <!-- Thời gian học bù (NGÀY + GIỜ) -->
                <el-col :span="12">
                    <el-form-item :label="t('classSchedule.cancelDialog.substitutionDateLabel')"
                        prop="substitutionDateTime">
                        <el-date-picker v-model="form.substitutionDateTime" type="datetime" class="w-100"
                            :placeholder="t('classSchedule.cancelDialog.substitutionDateTimePlaceholder')"
                            format="DD/MM/YYYY HH:mm" value-format="YYYY-MM-DDTHH:mm:ss" :disabled="!isSubstitution" />
                        <div v-if="isSubstitution" class="hint">
                            {{ t('classSchedule.cancelDialog.durationHint') }}
                        </div>
                    </el-form-item>
                </el-col>
            </el-row>

            <el-row :gutter="16">
                <!-- GV dạy thay -->
                <el-col :span="12">
                    <el-form-item :label="t('classSchedule.cancelDialog.substituteTeacherLabel')"
                        prop="substituteTeacherId">
                        <el-select v-model="form.substituteTeacherId" class="w-100" filterable
                            :placeholder="t('classSchedule.cancelDialog.substituteTeacherPlaceholder')"
                            :loading="teacherLoading" :disabled="!isSubstitution">
                            <el-option v-for="gv in teacherOptions" :key="gv.id" :value="gv.id"
                                :label="gv.applicationUser.fullName" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12" />
            </el-row>

            <!-- Lý do -->
            <el-form-item :label="t('classSchedule.cancelDialog.reasonLabel')" prop="cancelReason">
                <el-input v-model="form.cancelReason" type="textarea" :rows="4"
                    :placeholder="t('classSchedule.cancelDialog.reasonPlaceholder')" maxlength="500" show-word-limit />
            </el-form-item>
        </el-form>

        <template #footer>
            <div class="dlg-footer">
                <el-button @click="emit('update:visible', false)">
                    {{ t('common.cancel') }}
                </el-button>
                <el-button type="success" :loading="submitting" @click="submit">
                    {{ t('classSchedule.cancelDialog.confirm') }}
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'

import type { ClassScheduleModel } from '@/api/ClassScheduleApi'
import { useClassScheduleStore } from '@/stores/classScheduleStore'
import { useTeacherStore } from '@/stores/teacherStore'

enum CancelType {
    Shifting = 1,
    Substitution = 2
}

const { t } = useI18n()

const props = defineProps<{
    visible: boolean
    classId: string | null | undefined
    schedule: ClassScheduleModel | null
}>()

const emit = defineEmits<{
    (e: 'update:visible', v: boolean): void
    (e: 'cancelled'): void
}>()

const formRef = ref<FormInstance>()
const submitting = ref(false)

const classScheduleStore = useClassScheduleStore()
const teacherStore = useTeacherStore()

const { teachers, loading: teacherLoading } = storeToRefs(teacherStore)
const teacherOptions = computed(() => teachers.value || [])

const form = reactive({
    cancelType: CancelType.Shifting as CancelType,
    // ✅ datetime: YYYY-MM-DDTHH:mm:ss
    substitutionDateTime: null as string | null,
    substituteTeacherId: null as string | null,
    cancelReason: ''
})

const isSubstitution = computed(() => form.cancelType === CancelType.Substitution)

/** tách date + time theo đúng backend */
function extractDateAndTime(value?: string | null) {
    if (!value) return { date: null as string | null, time: null as string | null }
    // value-format: YYYY-MM-DDTHH:mm:ss
    return {
        date: value.substring(0, 10),   // YYYY-MM-DD
        time: value.substring(11, 19)  // HH:mm:ss
    }
}

const rules = computed<FormRules>(() => ({
    cancelType: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    cancelReason: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    substitutionDateTime: isSubstitution.value
        ? [{ required: true, message: t('validation.required'), trigger: 'change' }]
        : [],
    substituteTeacherId: isSubstitution.value
        ? [{ required: true, message: t('validation.required'), trigger: 'change' }]
        : []
}))

watch(
    () => props.visible,
    (v) => {
        if (!v) return
        form.cancelType = CancelType.Shifting
        form.substitutionDateTime = null
        form.substituteTeacherId = null
        form.cancelReason = ''
        formRef.value?.clearValidate()
    }
)

watch(
    () => form.cancelType,
    async (type) => {
        if (type !== CancelType.Substitution) {
            form.substitutionDateTime = null
            form.substituteTeacherId = null
            formRef.value?.clearValidate(['substitutionDateTime', 'substituteTeacherId'])
            return
        }
        if (!props.classId) return
        await teacherStore.fetchAllTeacher()
    }
)

async function submit() {
    const ok = await formRef.value?.validate().catch(() => false)
    if (!ok) return

    if (!props.classId || !props.schedule?.id) {
        ElMessage.error(t('classSchedule.cancelDialog.errors.missingInfo'))
        return
    }

    submitting.value = true
    try {
        // 1) Huỷ không bù
        if (form.cancelType === CancelType.Shifting) {
            await classScheduleStore.cancelAndShift({
                classId: props.classId,
                classScheduleId: props.schedule.id,
                cancelReason: form.cancelReason.trim()
            })
        }

        // 2) Huỷ có bù (date + time)
        if (form.cancelType === CancelType.Substitution) {
            const { date, time } = extractDateAndTime(form.substitutionDateTime)

            if (!date || !time) {
                ElMessage.error(t('validation.required'))
                return
            }

            await classScheduleStore.cancelAndSubstitute({
                classId: props.classId,
                classScheduleId: props.schedule.id,
                cancelReason: form.cancelReason.trim(),
                substitutionDate: date,          // DateTime (controller parse)
                substitutionStartTime: time,     // TimeSpan: HH:mm:ss
                substituteTeacherId: form.substituteTeacherId
            })
        }

        ElMessage.success(t('common.success'))
        emit('update:visible', false)
        emit('cancelled')
    } catch (e) {
        console.error(e)
        ElMessage.error(t('common.error'))
    } finally {
        submitting.value = false
    }
}
</script>

<style scoped>
.dlg-header {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.dlg-title {
    font-size: 16px;
    font-weight: 700;
}

.dlg-sub {
    font-size: 13px;
    color: #8f8f8f;
}

.dlg-footer {
    display: flex;
    justify-content: flex-end;
    gap: 12px;
}

.w-100 {
    width: 100%;
}

.hint {
    margin-top: 6px;
    font-size: 12px;
    color: #8f8f8f;
}
</style>
