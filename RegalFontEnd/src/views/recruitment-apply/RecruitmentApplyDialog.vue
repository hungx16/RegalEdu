<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="mode" @submit="onSubmit" @delete="onDelete" @update:visible="emit('update:visible', $event)">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required">{{ t('recruitmentApply.candidateName') }}</label>
                    <el-form-item prop="candidateName">
                        <el-input v-model="formData.candidateName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required">{{ t('recruitmentApply.candidateEmail') }}</label>
                    <el-form-item prop="candidateEmail">
                        <el-input v-model="formData.candidateEmail" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required">{{ t('recruitmentApply.candidatePhone') }}</label>
                    <el-form-item prop="candidatePhone">
                        <el-input v-model="formData.candidatePhone" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label>{{ t('recruitmentApply.recruitmentInfo') }}</label>
                    <el-form-item prop="recruitmentInfoId">
                        <el-select v-model="formData.recruitmentInfoId" :disabled="isView">
                            <el-option v-for="info in recruitmentInfos" :key="info.id" :label="info.recruitmentInfoName"
                                :value="info.id" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label>{{ t('recruitmentApply.candidateExperience') }}</label>
                    <el-form-item prop="candidateExperience">
                        <el-input type="textarea" v-model="formData.candidateExperience" rows="3" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label>{{ t('recruitmentApply.candidateCV') }}</label>
                    <el-form-item prop="candidateCV">
                        <el-input v-model="formData.candidateCV" :disabled="isView" placeholder="Link or file name" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { RecruitmentApplyModel } from '@/api/RecruitmentApplyApi'
import { useRecruitmentInfoStore } from '@/stores/recruitmentInfoStore'

const props = defineProps<{ visible: boolean; mode?: 'create' | 'edit' | 'view'; recruitmentApplyData: Partial<RecruitmentApplyModel> | null }>()
const emit = defineEmits(['update:visible', 'submit', 'delete'])
const { t } = useI18n()
const recruitmentInfoStore = useRecruitmentInfoStore()
const recruitmentInfos = ref<any[]>([])

const formData = ref<Partial<RecruitmentApplyModel>>({
    candidateName: '',
    candidateEmail: '',
    candidatePhone: '',
    candidateExperience: '',
    candidateCV: '',
    recruitmentInfoId: ''
})

const isView = computed(() => props.mode === 'view')
const modeTitle = computed(() => {
    if (isView.value) return t('recruitmentApply.detailTitle')
    if (props.mode === 'edit') return t('recruitmentApply.editTitle')
    return t('recruitmentApply.addTitle')
})

const rules = {
    candidateName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    candidateEmail: [{ required: true, type: 'email', message: t('validation.invalidEmail'), trigger: 'blur' }],
    candidatePhone: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    recruitmentInfoId: [{ required: true, message: t('validation.required'), trigger: 'change' }]
}

watch(() => props.recruitmentApplyData, (val) => {
    if (val) formData.value = { ...formData.value, ...val }
}, { immediate: true })

onMounted(async () => {
    await recruitmentInfoStore.fetchAllRecruitmentInfo()
    recruitmentInfos.value = recruitmentInfoStore.recruitmentInfoList
})

function onSubmit() {
    emit('submit', formData.value)
}

function onDelete() {
    emit('delete', formData.value)
}
</script>
