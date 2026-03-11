<template>
    <el-dialog :model-value="visible" @update:model-value="emit('update:visible', $event)" width="650px" align-center
        :close-on-click-modal="false" :show-close="false" class="metronic-dialog responsive-padding"
        :style="{ '--el-dialog-border-radius': '1rem' }">
        <!-- Header -->
        <template #header>
            <div class="dialog-header d-flex align-items-center justify-content-center position-relative "
                :class="isEdit ? 'bg-light-warning' : 'bg-light-primary'" style="min-height: 30px;">
                <h3 class="modal-title text-gray-700 m-0 py-3 text-left w-100">
                    {{ title }}
                </h3>
                <button class="btn btn-sm btn-icon btn-active-color-light position-absolute"
                    style="top: 50%; right: 1rem; transform: translateY(-50%);" @click="emit('update:visible', false)">
                    <i class="bi bi-x fs-1"></i>
                </button>
            </div>
        </template>

        <!-- Tabs -->
        <div v-if="tabs?.length" class="custom-tab-wrapper pt-3">
            <div class="custom-tab-list">
                <div v-for="(tab, index) in tabs" :key="tab.name"
                    :class="['custom-tab-item', activeTab === tab.name ? 'active' : '']" @click="activeTab = tab.name">
                    {{ t(tab.label) }}
                </div>
            </div>
        </div>

        <!-- Body with grid -->
        <el-row :gutter="20" class="modal-body pt-12 pb-5">
            <el-col :span="24">
                <el-form ref="formRef" :model="formData" :rules="rules" @submit.prevent="onSubmit"
                    class="form form-wide">
                    <slot :name="activeTab || 'form'" />
                </el-form>
            </el-col>
        </el-row>

        <!-- Footer -->
        <template #footer>
            <div class="text-right pb-4">
                <el-button @click="emit('update:visible', false)" class="btn btn-light me-3">
                    {{ t('common.cancel') }}
                </el-button>
                <el-button type="primary" class="btn btn-primary" @click="onSubmit" :loading="loading">
                    <template v-if="!loading">{{ t('common.save') }}</template>
                    <template v-else>
                        <span class="spinner-border spinner-border-sm align-middle me-2"></span>
                        {{ t('common.loading') }}
                    </template>
                </el-button>
                <el-button v-if="isEdit" type="danger" class="btn btn-danger ms-3" @click="onDelete"
                    :disabled="loading">
                    {{ t('common.delete') }}
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script setup lang="ts">
import { ref, toRaw } from 'vue'
import { useI18n } from 'vue-i18n'
import type { PropType } from 'vue'

const { t } = useI18n()

const props = defineProps({
    visible: Boolean,
    title: String,
    isEdit: Boolean,
    formData: Object,
    rules: Object,
    loading: Boolean,
    tabs: Array as PropType<{ label: string; name: string }[]>
})

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])

const formRef = ref()
const activeTab = ref(props.tabs?.[0]?.name || '')

defineExpose({ formRef })

function onSubmit() {
    emit('submit', toRaw(props.formData))
}
function onDelete() {
    emit('delete', toRaw(props.formData))
}
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
    border-radius: 0.5rem;
    padding-left: 10px;
}

.modal-title {
    font-size: 1.25rem;
    font-weight: 600;
}

.bg-primary {
    background-color: var(--bs-primary, #009EF7) !important;
}

.bg-success {
    background-color: var(--bs-success, #50CD89) !important;
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

.custom-tab-wrapper {
    width: 100%;
}

.custom-tab-list {
    display: flex;
    width: 100%;
    background-color: #f2f2f2;
    border-radius: 999px;
    overflow: hidden;
    border: 1px solid #e4e7ed;
    justify-content: center;
}

.custom-tab-item {
    flex: 1 1 0;
    text-align: center;
    padding: 10px 16px;
    font-size: 14px;
    font-weight: 500;
    color: #606266;
    cursor: pointer;
    transition: background-color 0.2s ease;
    white-space: nowrap;
    display: flex;
    justify-content: center;
    align-items: center;
}

.custom-tab-item.active {
    background-color: #fff;
    color: #303133;
    font-weight: 600;
    border-radius: 999px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

@media (max-width: 768px) {
    .custom-tab-item {
        font-size: 13px;
        padding: 8px 10px;
    }

    .responsive-padding {
        margin-left: 12px !important;
        margin-right: 12px !important;
    }
}
</style>
