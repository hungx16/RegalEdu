<template>
    <div class="p-3">
        <div class="card p-4 mb-4">
            <h6 class="fw-bold mb-3">{{ t('custome.customerNote') }}</h6>
            <el-input type="textarea" v-model="internalNote" :rows="3" />
            <div class="d-flex justify-content-end mt-3">
                <el-button type="primary" @click="saveMainNote">{{ t('custome.addNote') }}</el-button>
            </div>
        </div>

        <el-divider />

        <h6 class="fw-bold mb-3">{{ t('custome.historyNotes') }}</h6>
        <div v-for="(note, idx) in internalNotes" :key="note.id ?? idx" class="card p-3 mb-3 bg-light-subtle">
            <p>{{ note.noteContext }}</p>
            <div class="d-flex justify-content-between text-body-secondary mt-2">
                <span>{{ t('custome.noteBy') }} {{ note.employeeName || 'System' }}</span>
                <span>{{ formatDate(note.noteDate) }}</span>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { StudentNoteModel } from '@/api/StudentApi';

const props = defineProps<{ notes: StudentNoteModel[]; t: Function; }>();
const emit = defineEmits(['update:notes']);

// Giả định ghi chú chính của KH nằm trong Student.Reason
const internalNote = ref("KH rất quan tâm, có khả năng chuyển đổi cao");
const internalNotes = ref([...props.notes]); // Lịch sử ghi chú

watch(() => props.notes, (newNotes) => {
    internalNotes.value = [...newNotes];
});

/**
 * Safely format a date that may be a string or a Date instance.
 * Returns an empty string for falsy values.
 */
const formatDate = (d?: string | Date | null): string => {
    if (!d) return '';
    if (d instanceof Date) return d.toISOString().substring(0, 10);
    return String(d).substring(0, 10);
};

const saveMainNote = () => {
    const newNote: StudentNoteModel = {
        noteContext: internalNote.value,
        noteDate: new Date().toISOString(),
        employeeName: '' // Giả định lấy tên người dùng hiện tại
    };
    internalNotes.value.unshift(newNote);
    // Logic lưu ghi chú chính (cần emit để cập nhật Student.Reason)
    emit('update:notes', internalNotes.value);
};
</script>