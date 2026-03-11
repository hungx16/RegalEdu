<template>
    <div>
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h5 class="fw-bold mb-0">{{ t('custome.contactListTitle') }}</h5>
            <el-button v-if="!isView" type="primary" :icon="Plus" @click="emit('addContact')" circle />
        </div>

        <div v-for="(contact, index) in contacts" :key="index" class="contact-card p-4 mb-3 rounded-3 shadow-sm border">

            <el-row :gutter="20">
                <el-col :span="6">
                    <span class="text-body-secondary d-block">{{ t('custome.contactName') }}</span>
                    <el-form-item :prop="'contacts.' + index + '.fullName'" :rules="contactRules.fullName">
                        <el-input v-model="contacts[index].fullName" :disabled="isView"
                            :placeholder="t('student.contactNamePlaceholder')" />
                    </el-form-item>
                    <span class="text-body-secondary d-block mt-2">{{ t('custome.contactEmail') }}</span>
                    <el-form-item>
                        <el-input v-model="contacts[index].email" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="6">
                    <span class="text-body-secondary d-block">{{ t('custome.contactRelationship') }}</span>
                    <el-form-item :prop="'contacts.' + index + '.relationship'" :rules="contactRules.relationship">
                        <el-select v-model="contacts[index].relationship" :disabled="isView" class="w-100">
                            <el-option :label="t('contact.relationship.father')" :value="0" />
                            <el-option :label="t('contact.relationship.mother')" :value="1" />
                        </el-select>
                    </el-form-item>
                    <span class="text-body-secondary d-block mt-2">{{ t('custome.username') }}</span>
                    <el-form-item>
                        <el-input v-model="contacts[index].username" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="10">
                    <span class="text-body-secondary d-block">{{ t('custome.contactPhone') }}</span>
                    <el-form-item :prop="'contacts.' + index + '.phone'" :rules="contactRules.phone">
                        <el-input v-model="contacts[index].phone" :disabled="isView"
                            :placeholder="t('custome.contactPhonePlaceholder')" />
                    </el-form-item>
                    <span class="text-body-secondary d-block mt-2">{{ t('custome.contactAddress') }}</span>
                    <el-form-item>
                        <el-input v-model="contacts[index].address" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="2" v-if="!isView" class="d-flex justify-content-end align-items-start">
                    <el-button @click="removeContact(index)" type="danger" :icon="Delete" circle />
                </el-col>
            </el-row>
        </div>

        <div v-if="contacts.length === 0 && isView" class="text-center text-body-secondary mt-5">
            {{ t('custome.noContactsFound') }}
        </div>
    </div>
</template>

<script setup lang="ts">
import type { ContactModel } from '@/api/StudentApi';
import { Plus, Delete } from '@element-plus/icons-vue';

// Props nhận dữ liệu và trạng thái từ StudentDialog
const props = defineProps<{
    /** Danh sách Contacts hiện tại của Student (được truyền từ formData.contacts) */
    contacts: ContactModel[];
    /** Chế độ xem (true: không cho chỉnh sửa) */
    isView: boolean;
    /** Hàm dịch i18n */
    t: Function;
}>();

const emit = defineEmits(['addContact', 'update:contacts']);

// Rules cho các trường bắt buộc của Contact (dùng cho validation lồng nhau)
const contactRules = {
    fullName: [{ required: true, message: props.t('validation.required'), trigger: 'blur' }],
    phone: [{ required: true, message: props.t('validation.required'), trigger: 'blur' }],
    relationship: [{ required: true, message: props.t('validation.required'), trigger: 'change' }],
};

const removeContact = (index: number) => {
    // Xóa contact khỏi mảng và emit sự kiện cập nhật lên component cha
    const newContacts = [...props.contacts];
    newContacts.splice(index, 1);
    emit('update:contacts', newContacts);
};
</script>