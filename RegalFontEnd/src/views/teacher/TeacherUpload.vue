<template>
    <el-dialog v-model="internalVisible" title="Upload danh sách giáo viên" width="600px" :close-on-click-modal="false">
        <el-upload drag action="" :http-request="handleUpload" accept=".xlsx,.xls,.csv" :auto-upload="false"
            :on-change="onFileChange" @close="onClose">
            <i class="el-icon-upload"></i>
            <div class="el-upload__text">
                Kéo thả file vào đây hoặc <em>bấm để chọn</em>
            </div>
            <div class="el-upload__tip" slot="tip">
                Chỉ chấp nhận file .xlsx / .csv. Dung lượng ≤ 5MB.
            </div>
        </el-upload>

        <el-table v-if="teachersPreview.length > 0" :data="teachersPreview" style="width: 100%; margin-top: 20px"
            height="300">
            <el-table-column prop="teacherCode" label="Mã GV" width="120" />
            <el-table-column prop="teacherName" label="Họ và tên" />
            <el-table-column prop="teacherEmail" label="Email" />
            <el-table-column prop="teacherPhone" label="SĐT" width="120" />
            <el-table-column prop="workType" label="Loại hình" :formatter="formatWorkType" width="120" />
        </el-table>

        <template #footer>
            <el-button @click="onCancel">Hủy</el-button>
            <el-button type="primary" :disabled="teachersPreview.length === 0" @click="onSubmit">
                Tải lên
            </el-button>
        </template>
    </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import * as XLSX from "xlsx";
import axios from "axios";
import { ElMessage } from "element-plus";
import type { TeacherModel } from "@/api/TeacherApi";

const props = defineProps<{
    visible: boolean;
}>();
const emits = defineEmits(['update:visible', 'imported']);

const internalVisible = ref(props.visible);

const file = ref<File | null>(null);
const teachersPreview = ref<TeacherModel[]>([]);

const formatWorkType = (_: any, __: any, value: number) => {
    switch (value) {
        case 0:
            return "Toàn thời gian";
        case 1:
            return "Bán thời gian";
        case 2:
            return "Hợp đồng";
        default:
            return "Không xác định";
    }
};

const onFileChange = (uploadFile: any) => {
    if (!uploadFile.raw) return;
    file.value = uploadFile.raw;
    readExcel(uploadFile.raw);
};

const readExcel = async (rawFile: File) => {
    const data = await rawFile.arrayBuffer();
    const workbook = XLSX.read(data);
    const sheetName = workbook.SheetNames[0];
    const sheet = workbook.Sheets[sheetName];
    const json = XLSX.utils.sheet_to_json(sheet, { header: 1 });

    // Giả định file có header: [Mã GV, Tên GV, Email, SĐT, Loại hình]
    teachersPreview.value = (json as any[])
        .slice(1)
        .map((row: any[]) => ({
            teacherCode: row[0] ?? "",
            teacherName: row[1] ?? "",
            teacherEmail: row[2] ?? "",
            teacherPhone: row[3] ?? "",
            workType: row[4] ?? 0,
            teacherNickname: "",
            teacherQualifications: "",
            teacherSpecialization: "",
            teacherBirthday: "",
            teacherGender: 0,
            teacherAddress: "",
            teacherNote: "",
            teacherAvatar: "",
            teacherDegree: "",
            teacherExperience: "",
            teacherIdentityNumber: "",
            teacherIdentityDate: "",
            teacherIdentityPlace: "",
            teacherTaxCode: "",
            teacherBankAccount: "",
            teacherBankName: "",
            teacherBankBranch: "",
            teacherBankOwner: "",
            teacherFacebook: "",
            teacherZalo: "",
            teacherSkype: "",
            teacherTelegram: "",
            teacherSalary: 0,
            teacherSalaryType: 0,
            teacherSalaryNote: "",
            teachingOutside: false,
            teacherAssistant: false,
            isOnline: false,
            applicationUserId: "",
            status: 1
        }));
};

const handleUpload = () => {
    // Element Plus yêu cầu hàm này nếu không dùng auto-upload
};

// Đồng bộ với v-model
watch(
    () => props.visible,
    (val) => (internalVisible.value = val)
);

function onClose() {
    emits('update:visible', false);
}

const onCancel = () => {
    // internalVisible.value = false;
    emits('update:visible', false);
    teachersPreview.value = [];
    file.value = null;
};

const onSubmit = async () => {
    try {
        await axios.post("/api/teachers/import", teachersPreview.value);
        ElMessage.success("Tải lên danh sách giáo viên thành công!");
        onCancel();
    } catch (err) {
        console.error(err);
        ElMessage.error("Có lỗi xảy ra khi tải lên.");
    }
};
</script>
