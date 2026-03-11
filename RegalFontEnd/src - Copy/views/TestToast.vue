<template>
  <div class="p-5">
    <button class="btn btn-primary" @click="triggerToast">Show Toast</button>
    <button class="btn btn-primary" @click="triggerToast1">Show Toast</button>
    <button class="btn btn-primary" @click="dialogVisible = true">Open Dialog</button>
    <MonthSwitcher v-model="currentMonth" :min="minDate" :max="maxDate" @update:modelValue="onMonthChange" />

    <TabbedFormDialog v-model:visible="dialogVisible" title="Cập nhật người dùng" :form-data="form" :rules="rules"
      :tabs="[
        { label: 'Thông tin cá nhân', name: 'info' },
        { label: 'Tài khoản', name: 'account' }
      ]" @submit="handleSubmit">
      <template #info>
        <el-form-item label="Họ tên" prop="name">
          <el-input v-model="form.name" />
        </el-form-item>
      </template>
      <template #account>
        <el-form-item label="Email" prop="email">
          <el-input v-model="form.email" />
        </el-form-item>
      </template>
    </TabbedFormDialog>

    <el-button circle class="action-button">
      <el-icon>
        <View />
      </el-icon>
    </el-button>
    <el-button circle class="action-button">
      <el-icon>
        <Edit />
      </el-icon>
    </el-button>
    <el-button circle class="action-button">
      <el-icon>
        <Delete />
      </el-icon>
    </el-button>


    <BaseBadge :label="2" color="blue" displayType="level" />

    <BaseBadge :label="403" color="red" displayType="studentCountAlt" bold />

    <BaseBadge label="Khóa học" color="purple" icon="bi bi-book" />
    <BaseBadge :badges="[
      { label: 1, color: 'green', displayType: 'level' },
      { label: 220, color: 'purple', displayType: 'studentCountAlt' },
      { label: 'Khóa học', color: 'blue', icon: 'bi bi-book' }
    ]" />
    <BaseBadge color="orange">
      <template #icon>
        <i class="bi bi-repeat"></i>
      </template>
      Ghi danh lại
    </BaseBadge>
    <BaseBadge :label="'Chờ xác nhận'" color="gray" :rawLabel="true" />
    <BaseTable :columns="columns" :items="divisions" :showCheckboxColumn="false" :showPagination="false"
      :showIndex="true">
      <template #cell-divisionLevel="{ item }">
        <BaseBadge :label="item.divisionLevel"
          :color="item.divisionLevel === 1 ? 'green' : item.divisionLevel === 2 ? 'blue' : 'purple'"
          displayType="level" />
      </template>
      <template #cell-studentCount="{ item }">
        <BaseBadge :label="item.studentCount" :color="item.studentCount > 400
          ? 'red'
          : item.studentCount > 200
            ? 'purple'
            : 'green'
          " displayType="studentCountAlt" :bold="item.studentCount > 200" />
      </template>
      <template #cell-tags="{ item }">
        <div class="badge-list">
          <BaseBadge :badges="item.tags" />
        </div>
      </template>
    </BaseTable>
    <BaseBadge label="Đã duyệt" color="cyan" outline bold />
    <BaseBadge label="Quản trị viên" color="blue" outline icon="bi-person-fill" />
    <BaseBadge label="5 phòng ban" color="deepPurple" outline />
    <BaseBadge label="Đang hoạt động" color="green" outline />
    <BaseBadge label="5 phòng ban" color="cyan" outline soft bordered />
    <TabbedComponent :tabs="tabs" v-model="currentTab">
      <template #info>
        <div>Đây là nội dung thông tin cá nhân</div>
      </template>
      <template #account>
        <div>Đây là nội dung tài khoản</div>
      </template>
    </TabbedComponent>


    <div class="card card-flush p-6 rounded-4 shadow-sm">
      <h3 class="mb-4">{{ t('tag.demoTitle') }}</h3>
      <div class="mb-6">
        <BaseTag text="Metronic" icon="bi bi-bootstrap" shape="pill" variant="primary" class="me-2" />
        <BaseTag text="Vue 3" variant="success" />
        <BaseTag text="Element Plus" variant="info" :dismissible="true" @dismiss="onDismiss" class="ms-2" />
      </div>
      <TagList v-model="formTags.tags" :maxVisible="10" :maxTags="8" :dismissible="true" :distinct-colors="true"
        :suggestions="['Vue', 'React', 'Vite', 'TypeScript', 'Metronic']" :autoColor="true" :hideAddWhenLimit="true" />

      <div class="text-muted fs-8 mt-2">Raw: {{ formTags.tags }}</div>
    </div>

  </div>

</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { View, Edit, Delete } from '@element-plus/icons-vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import TabbedFormDialog from '@/components/tabbed-form-dialog/TabbedFormDialog.vue';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import MonthSwitcher from '@/components/month-switcher/MonthSwitcher.vue';
import TabbedComponent from '@/components/tabbed/TabbedComponent.vue';
import BaseTag from '@/components/tag/BaseTag.vue'
import TagList from '@/components/tag/TagList.vue'

import { useI18n } from 'vue-i18n';
import { joinTags, splitTags } from '@/utils/tags';
const { t } = useI18n()

const currentMonth = ref(new Date());
const minDate = new Date(2022, 0, 1); // Tùy chỉnh
const maxDate = new Date(2026, 11, 31);
function onMonthChange(val: Date) {
  // Xử lý khi đổi tháng
}
const tabs = [
  { name: 'info', label: 'Thông tin cá nhân' },
  { name: 'account', label: 'Tài khoản' },
];
const currentTab = ref('info');
const store = useNotificationStore();
const tagsStr = ref<string>('Design#$#Frontend#$#Vue#$#Metronic')
const allSuggestions = ['Vue', 'React', 'Svelte', 'Nuxt', 'Vite', 'TypeScript', 'Element Plus', 'Bootstrap']
const addable = ref<boolean>(true)
const formTags = ref({
  // ví dụ từ API
  tags: 'Design#$#Frontend#$#Vue#$#Metronic',
})
/** Sự kiện xóa từ BaseTags: index là index của VÙNG HIỂN THỊ (<= max), text là nội dung tag */
function onRemoveTag(visibleIndex: number, text: string) {
  const arr = splitTags(formTags.value.tags)          // ['Design','Frontend','Vue','Metronic']
  // Khuyến nghị xóa theo 'text' để không lệch khi có max/+N:
  const idx = arr.findIndex(s => s === text)
  if (idx !== -1) arr.splice(idx, 1)
  formTags.value.tags = joinTags(arr)                 // 'Design#$#Frontend#$#Metronic' (ví dụ)
}
function onDismiss() { /* demo */ }
function onBlocked(reason: 'disabled' | 'limit' | 'not-addable') {
  // tuỳ anh, có thể toast
  console.debug('Blocked add:', reason)
}
const dialogVisible = ref(false);
const form = reactive({
  name: '',
  email: ''
});
const divisions = ref([
  {
    id: 1,
    divisionCode: 'DV001',
    divisionName: 'Phòng Kỹ thuật',
    divisionLevel: 1,
    studentCount: 85,
    tags: [
      { label: 1, color: 'green', displayType: 'level' },
      { label: 85, color: 'cyan', displayType: 'studentCountAlt' }
    ]
  },
  {
    id: 2,
    divisionCode: 'DV002',
    divisionName: 'Phòng Marketing',
    divisionLevel: 2,
    studentCount: 250,
    tags: [
      { label: 'Marketing', color: 'blue', i18nKey: 'division.marketing' },
      { label: 250, color: 'purple', displayType: 'studentCountAlt', bold: true }
    ]
  },
  {
    id: 3,
    divisionCode: 'DV003',
    divisionName: 'Phòng Đào tạo',
    divisionLevel: 3,
    studentCount: 430,
    tags: [
      { label: 3, color: 'purple', displayType: 'level' },
      { label: 430, color: 'red', displayType: 'studentCountAlt', bold: true },
      { label: 'Hot', color: 'orange', icon: 'bi bi-fire', rawLabel: true }
    ]
  },
  {
    id: 4,
    divisionCode: 'DV004',
    divisionName: 'Phòng IT',
    divisionLevel: 1,
    studentCount: 120,
    tags: [
      { label: 'IT', color: 'violet', i18nKey: 'division.it' },
      { label: 120, color: 'green', displayType: 'studentCountAlt' }
    ]
  }
]);

const columns: BaseTableColumn[] = [
  { key: '__index__', labelKey: 'common.index', width: 80, align: 'center', headerAlign: 'center', sortable: false },
  { key: 'divisionCode', labelKey: 'division.code', sortable: true, width: 120, isBold: true },
  { key: 'divisionName', labelKey: 'division.name', sortable: true, width: 180 },
  { key: 'divisionLevel', labelKey: 'division.level', sortable: true, width: 120 },
  { key: 'studentCount', labelKey: 'division.studentCount', sortable: true, width: 180 },
  { key: 'tags', labelKey: 'division.tags', width: 280 }
];
const rules = {
  name: [{ required: true, message: 'Vui lòng nhập họ tên', trigger: 'blur' }],
  email: [{ required: true, message: 'Vui lòng nhập email', trigger: 'blur' }]
};

function triggerToast() {
  store.showToast('success', { key: 'toast.success', params: { model: 'user' } });
  store.showToast('error', { key: 'toast.saveError', params: { model: 'user' } });
  store.showToast('warning', { key: 'toast.warning', params: { model: 'user' } });
  store.showToast('info', { key: 'toast.info', params: { model: 'user' } });
}

function triggerToast1() {
  store.showConfirm(
    { key: 'toast.delete', params: { model: 'user' } },
    () => { alert('Xác nhận thành công!') },
    () => { alert('Đã hủy xác nhận!') }
  )
}

function handleSubmit(formData: any) {
  console.log('Form submitted:', formData);
}
</script>
<!-- <template>
  <div>
    <el-button type="primary" @click="openDialog('create')">Thêm mới</el-button>
    <el-button type="success" @click="openDialog('edit', sampleDivision)">Sửa mẫu</el-button>
    <el-button type="info" @click="openDialog('view', sampleDivision)">Xem chi tiết</el-button>

    <!-- BaseDialog dùng slot truyền mode -->
<!-- <BaseDialog v-model:visible="visible" :title="dialogTitle" :mode="dialogMode" :formData="formData" :rules="rules"
      :loading="loading" @submit="handleSubmit" @delete="handleDelete">
      <template #form="{ mode, formData = {} }">
        <el-form-item label="Mã phòng ban" v-if="mode !== 'create'">
          <el-input v-model="formData.id" disabled />
        </el-form-item>
        <el-form-item label="Tên phòng ban" prop="name">
          <el-input v-model="formData.name" :disabled="mode === 'view'" placeholder="Nhập tên phòng ban" />
        </el-form-item>
        <el-form-item label="Mô tả">
          <el-input v-model="formData.description" type="textarea" :disabled="mode === 'view'" placeholder="Nhập mô tả"
            rows="3" />
        </el-form-item>
        <!-- Chỉ hiện ở chế độ view -->
<!-- <el-form-item label="Ngày tạo" v-if="mode === 'view'">
          <span>{{ formData.createdAt ? formData.createdAt : 'Chưa có thông tin' }}</span>
        </el-form-item>
</template>
</BaseDialog>
</div> -->
<!-- </template> -->

<!-- <script setup lang="ts">
import { ref, reactive } from 'vue'
import BaseDialog from '@/components/base-dialog-form/BaseDialogForm.vue'

// Dữ liệu form cho dialog
const visible = ref(false)
const dialogMode = ref<'create' | 'edit' | 'view'>('create')
const dialogTitle = ref('')
const loading = ref(false)

const formData = reactive({
  id: '',
  name: '',
  description: '',
  createdAt: '',
})

const sampleDivision = {
  id: 'DV001',
  name: 'Phòng Kinh Doanh',
  description: 'Quản lý hoạt động kinh doanh',
  createdAt: '2024-07-05 10:00:00',
}

const rules = {
  name: [{ required: true, message: 'Vui lòng nhập tên phòng ban', trigger: 'blur' }],
}

// Hàm mở dialog theo chế độ
type Division = {
  id: string
  name: string
  description: string
  createdAt: string
}

function openDialog(mode: 'create' | 'edit' | 'view', data: Division | null = null) {
  dialogMode.value = mode
  dialogTitle.value =
    mode === 'create'
      ? 'Thêm phòng ban'
      : mode === 'edit'
        ? 'Chỉnh sửa phòng ban'
        : 'Chi tiết phòng ban'

  if (data) {
    Object.assign(formData, data)
  } else {
    Object.assign(formData, { id: '', name: '', description: '', createdAt: '' })
  }
  visible.value = true
}

function handleSubmit(data: Division) {
  loading.value = true
  setTimeout(() => {
    loading.value = false
    visible.value = false
    alert(
      dialogMode.value === 'create'
        ? 'Tạo mới thành công!'
        : 'Cập nhật thành công!'
    )
  }, 800)
}

function handleDelete(data: Division) {
  visible.value = false
  alert('Đã xoá phòng ban!')
}
</script> -->




<style scoped lang="scss">
/* Cấu hình cho các nút action, bỏ màu nền và viền */
.el-button.is-circle {
  background: transparent !important;
  /* Không có màu nền */
  border: none !important;
  /* Không có đường viền */
  box-shadow: none !important;
  /* Không có bóng */
  border-radius: 12px !important;
  /* Bo góc */
  min-width: 36px;
  height: 36px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0 !important;
  margin: 0 4px;
  transition: background 0.2s, color 0.2s;
  position: relative;
  overflow: visible;
}

/* Hiệu ứng hover: nền vàng khi hover với bo góc */
.el-button.is-circle::after {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  background: #ffd740;
  /* Màu nền vàng */
  border-radius: 12px;
  /* Bo góc */
  z-index: 0;
  opacity: 0;
  transition: opacity 0.2s;
  pointer-events: none;
  /* Không ảnh hưởng đến sự kiện hover */
  display: block;
}

/* Khi hover/focus vào nút, hiện nền vàng bo góc */
.el-button.is-circle:hover::after,
.el-button.is-circle:focus::after {
  opacity: 1;
  /* Nền vàng sẽ hiện lên */
}

/* Icon sẽ nằm trên nền vàng */
.el-button.is-circle .el-icon,
.el-button.is-circle i {
  position: relative;
  z-index: 1;
  /* Đảm bảo icon nằm trên nền vàng */
  color: #525151 !important;
  /* Màu đen cho icon */
  font-size: 14px;

}

/* Icon Delete màu đỏ riêng biệt */
.el-button.is-circle:last-child .el-icon,
.el-button.is-circle:last-child i {
  color: #f44336 !important;
  /* Màu đỏ cho icon Delete */
}

.badge-list {
  display: flex;
  flex-wrap: wrap;
  gap: 6px 8px;
  /* gap ngang/dọc cho badge */
  align-items: center;
  min-height: 32px;
  /* hoặc auto tuỳ chiều cao badge */
}
</style>