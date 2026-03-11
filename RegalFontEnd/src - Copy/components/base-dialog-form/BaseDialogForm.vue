<template>
    <el-dialog :model-value="visible" @update:model-value="emit('update:visible', $event)" :width="dialogWidth"
        @submit.prevent align-center :close-on-click-modal="false" :show-close="false"
        class="metronic-dialog responsive-padding" :style="{ '--el-dialog-border-radius': '1rem' }" :key="mode"
        @close="emit('close')">
        <!-- Header -->
        <template #header>
            <div class="dialog-header px-4 pt-3 pb-2 position-relative" :class="headerClass">
                <div class="d-flex align-items-center mb-0">
                    <span class="dialog-icon me-2">
                        <slot name="icon"></slot>
                    </span>
                    <h3 class="modal-title m-0">{{ title }}</h3>
                </div>
                <div v-if="description" class="dialog-desc mt-1 ms-4">{{ description }}</div>
                <slot name="description" />
                <button type="button" class="btn btn-sm btn-icon btn-active-color-light position-absolute"
                    style="top: 50%; right: 1rem; transform: translateY(-50%);" @click="emit('update:visible', false)">
                    <i class="bi bi-x fs-1"></i>
                </button>
            </div>
        </template>

        <!-- Body -->
        <div v-loading="loading">
            <el-row :gutter="20" class="modal-body px-5 pt-5 pb-5" :style="{ minHeight: bodyMaxHeight }">
                <el-col :span="24">
                    <el-form ref="formRef" :model="formData" :rules="rules" @submit.prevent="onSubmit"
                        class="form form-wide">
                        <slot name="form" :mode="mode" :formData="formData" />
                    </el-form>
                </el-col>
            </el-row>
        </div>
        <!-- Footer -->
        <template #footer>
            <div class="text-center pb-4">
                <el-button @click="emit('update:visible', false)" class="btn btn-light me-3" :disabled="loading"
                    native-type="button">
                    {{ mode === 'view' ? t('common.close') : t('common.cancel') }}
                </el-button>
                <el-button v-if="mode !== 'view' && showActionButtons" type="primary" class="btn btn-primary" @click="onSubmit"
                    native-type="button" :disabled="loading || submitDisabled">
                    {{ t('common.save') }}
                </el-button>
                <el-button v-if="mode === 'edit' && showDelete && showActionButtons" type="danger" class="btn btn-danger ms-3"
                    @click="onDelete" :disabled="loading || deleteDisabled">
                    {{ t('common.delete') }}
                </el-button>
                <el-button v-if="mode == 'view' && showActive" type="primary" class="btn btn-primary"
                    @click="onActivated" native-type="button" :disabled="loading">
                    {{ t('common.activate') }}
                </el-button>
                <el-button v-if="mode == 'view' && showOpenNewDialog" type="primary" class="btn btn-primary"
                    @click="onOpenNewDialog" native-type="button" :disabled="loading">
                    {{ t('common.issueCreate') }}
                </el-button>
                <slot name="footer-extra" :mode="mode" :formData="formData" :loading="loading" />
            </div>
        </template>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, toRaw, computed, h } from 'vue'
import { useI18n } from 'vue-i18n'
import type { PropType } from 'vue'
const { t } = useI18n()
// trong <script setup>
const bodyMaxHeight = computed(() => {
    const v = props.height
    if (v === undefined || v === null || v === '') return '65vh'      // mặc định
    if (typeof v === 'number') return `${v}px`                        // số -> px
    return /^\d+$/.test(String(v)) ? `${v}px` : String(v)             // '400' -> '400px'; '70vh' giữ nguyên
})

const props = defineProps({
    visible: Boolean,
    title: String,
    mode: {
        type: String as PropType<'create' | 'edit' | 'view' | 'activated' | 'opennewdialog'>,
        default: 'create'
    },
    description: { type: String, default: '' },
    formData: Object,
    rules: Object,
    loading: Boolean,
    submitDisabled: { type: Boolean, default: false },
    showDelete: { type: Boolean, default: true },
    showActionButtons: { type: Boolean, default: true },
    deleteDisabled: { type: Boolean, default: false },
    height: { type: [String, Number], default: '400' },
    showActive: { type: Boolean, default: false },
    showOpenNewDialog: { type: Boolean, default: false },
    width: { type: String, default: '' }
})

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close', 'activated', 'opennewdialog'])

const formRef = ref()
defineExpose({ formRef })

async function onSubmit() {
    if (props.mode === 'view') return
    if (formRef.value?.validate) {
        await formRef.value.validate((valid: boolean) => {
            if (valid) emit('submit', toRaw(props.formData))
        })
    } else {
        emit('submit', toRaw(props.formData))
    }
}
async function onActivated() {
    if (props.mode !== 'view') return
    if (formRef.value?.validate) {
        await formRef.value.validate((valid: boolean) => {
            if (valid) emit('activated', toRaw(props.formData))
        })
    } else {
        emit('activated', toRaw(props.formData))
    }
}
async function onOpenNewDialog() {
    if (props.mode !== 'view') return
    if (formRef.value?.validate) {
        await formRef.value.validate((valid: boolean) => {
            if (valid) emit('opennewdialog', toRaw(props.formData))
        })
    } else {
        emit('opennewdialog', toRaw(props.formData))
    }
}
function onDelete() {
    emit('delete', toRaw(props.formData))
}

const dialogWidth = computed(() => {
    if (props.width) return props.width;
    return window.innerWidth < 600 ? '98vw' : '650px';
})

const headerClass = computed(() => {
    if (props.mode === 'create') return 'bg-light-primary'
    if (props.mode === 'edit') return 'bg-light-warning'
    return 'bg-light-info'
})
</script>

<style scoped>
.metronic-dialog {
    --el-dialog-border-radius: 1rem;
    --el-dialog-padding-primary: 0;
    --el-dialog-bg-color: white;
    --el-overlay-background-color: rgba(0, 0, 0, 0.5);
    border-radius: 1rem !important;
    overflow: hidden;
    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.1);
}

.dialog-header {
    border-radius: .5rem !important;
    /* bo 4 góc */
    position: relative;
    min-height: 30px;
}

.modal-title {
    font-size: 1.25rem;
    font-weight: 600;
    color: #1a2442;
}

.bg-light-primary {
    background-color: #e1f0ff !important;
}

.bg-light-warning {
    background-color: #fff8dd !important;
}

.bg-light-info {
    background-color: #f1faff !important;
}

.dialog-desc {
    font-size: 0.98em;
    font-weight: 400;
    color: #8f95b2;
    margin-top: 2px;
    margin-left: 26px;
    /* căn cùng với title+icon */
}

.modal-body {
    max-height: 65vh;
    overflow-y: auto;
    padding-left: 0 !important;
    padding-right: 0 !important;
}

.btn {
    border-radius: 0.5rem !important;
    padding: 0.5rem 1.25rem !important;
    font-weight: 500 !important;
}

.btn .spinner-border {
    width: 1rem;
    height: 1rem;
}

.form-wide .el-form-item {
    width: 100%;
    margin-bottom: 18px;
}

.form-wide .el-form-item__content {
    width: 100%;
    display: flex;
}



.form-wide .el-input,
.form-wide .el-input__wrapper,
.form-wide .el-radio-group {
    width: 100%;
    min-width: 0;
    box-sizing: border-box;
}

.dialog-icon {
    display: inline-flex;
    align-items: center;
    margin-right: 8px;
    font-size: 1.25em;
}

@media (max-width: 600px) {
    .metronic-dialog {
        width: calc(100vw - 10px) !important;
        min-width: unset !important;
        max-width: 100vw !important;
        margin-left: 5px !important;
        margin-right: 5px !important;
        left: 0 !important;
        right: 0 !important;
        padding: 0 !important;
    }

    .el-dialog__body {
        padding-left: 0 !important;
        padding-right: 0 !important;
    }
}

@media (max-width: 768px) {
    .responsive-padding {
        margin-left: 12px !important;
        margin-right: 12px !important;
    }
}
</style>
