<template>
  <div v-if="listParamsFilter != null" :class="[
    'event-filter d-flex align-items-center justify-content-between',
    listParamsFilter.listParams.length > 3 ? 'so-many' : '',
  ]">
    <!-- PHẦN TIÊU ĐỀ & MÔ TẢ BÊN TRÁI -->
    <div class="filter-header-info pt-5">
      <h2 class="filter-title">{{ t(props.headerTitle) }}</h2>
      <div class="filter-desc">{{ t(props.headerDesc) }}</div>
    </div>
    <!-- Bộ lọc nâng cao (ẩn/hiện nếu cần dùng) -->
    <!--
    <el-select
      v-if="arrFilterData.length > 0"
      v-model="filterSelected"
      class="input-border-bottom el-select--filter"
      :placeholder="t('Filter')"
      @change="filterChanged"
      :no-data-text="t('NoData')"
      style="width: 220px; font-size: 14px"
    >
      <el-option
        v-for="item in arrFilterData"
        :key="item.id"
        :label="item.label"
        :value="item.id"
      />
    </el-select>
    <el-input
      placeholder="Search"
      prefix-icon="el-icon-search"
      @keydown.enter.native="keyEnter"
      v-model="txtSearch"
    />
    -->
    <!-- <div v-if="showNoRecord" class="d-flex align-items-center">
      <span class="textRow">{{ t('No of rows:') }}</span>
      <el-select v-model="noOfRow" class="input-border-bottom el-select--filter" :placeholder="t('Filter')"
        @change="numberOfRowsChanged" :no-data-text="t('NoData')" style="width: 120px; font-size: 14px">
        <el-option v-for="item in arrNumberOfRows" :key="item.id" :label="item.label" :value="item.id" />
      </el-select>
    </div> -->
    <div class="btnfunction_wrap">
      <el-button v-for="btnItem in listParamsFilter.listBtn" :key="btnItem.label" @click="btnFunction(btnItem.event)"
        :class="'btn ' + calBtnClass(btnItem.type)" :disabled="getDisableButton(btnItem.type)" round>
        <template #icon>
          <el-icon v-if="getElIcon(btnItem.type)">
            <component :is="getElIcon(btnItem.type)" />
          </el-icon>
        </template>
        {{ t(btnItem.label) }}
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { PermissionAction, type FormPermissionDTO } from '@/api/accountGroupPermissionApi';
import type { ListHeaderParams, BtnFunction } from '@/types';
import { useAccountGroupPermissionStore } from '@/stores/accountGroupPermissionStore';
import { useRoute } from 'vue-router';
import { ElMessageBox } from 'element-plus';
import {
  Plus,
  Delete,
  Edit,
  Document,
  Printer,
  Download,
  Upload,
  Refresh,
  Lock,
  User,
  View,
  DataLine,
} from '@element-plus/icons-vue';

// Store, route, i18n
const store = useAccountGroupPermissionStore();
const route = useRoute();
const { t } = useI18n();

// Props
type FilterDataItem = { id: string | number; label: string };
const props = defineProps({
  disabledDelete: { type: Boolean, default: false },
  disabledViewDetail: { type: Boolean, default: true },
  disabledAssign: { type: Boolean, default: true },
  arrFilterData: { type: Array as () => FilterDataItem[], default: () => [] },
  disabledUpdateDebt: { type: Boolean, default: true },
  disabledCopy: { type: Boolean, default: true },
  disabledPrint: { type: Boolean, default: true },
  disabledImport: { type: Boolean, default: false },
  disabledExport: { type: Boolean, default: false },
  disabledExportPayment: { type: Boolean, default: false },
  disabledExpense: { type: Boolean, default: true },
  disabledLock: { type: Boolean, default: false },
  disabledUnlock: { type: Boolean, default: false },
  showNoRecord: { type: Boolean, default: false },
  noRecord: { type: String, default: '500' },
  headerTitle: { type: String, default: '' },
  headerDesc: { type: String, default: '' }
});
type InitOptions = { skipPermission?: boolean }

const emit = defineEmits([
  'numberOfRowsChanged', 'filterChanged', 'search', 'add', 'restore', 'setDebt', 'setUnDebt', 'register',
  'copyRequest', 'expenseRequest', 'import', 'printer', 'printRequest', 'viewdetail', 'exportPayment',
  'assign', 'delete', 'export', 'exportData', 'updateData', 'changePassword', 'view', 'monthlyCompanyAllocation', 'lock', 'unlock',
  'transfer'
]);

// State
const listParamsFilter = ref<ListHeaderParams | null>(null);
const txtSearch = ref('');
const filterSelected = ref('All');
const noOfRow = ref(props.noRecord);

const arrNumberOfRows = [
  { id: 20, label: '20' },
  { id: 50, label: '50' },
  { id: 100, label: '100' },
  { id: 200, label: '200' },
  { id: 500, label: '500' },
  { id: 1000, label: '1000' },
  { id: 'All', label: 'All' }
];

// Expose init
defineExpose({ initListHeaderParams });

onMounted(() => {
  filterSelected.value = 'All';
  noOfRow.value = props.noRecord;
});

// Xử lý dữ liệu permission để build list button
async function initListHeaderParams(data: ListHeaderParams, options?: InitOptions) {
  if (options?.skipPermission) {
    listParamsFilter.value = data;
    return;
  }
  await store.getMenuAndPermissionAccept();
  const listPermission: Array<FormPermissionDTO> = store.listFormPermissionArray;
  if (!listPermission || listPermission.length === 0) return;
  const userPermission = listPermission.find(x => x.formName == route.name);
  if (!userPermission) return;
  const listButton: Array<BtnFunction> = [];
  data.listBtn.forEach(element => {
    let action: string = element.event;
    if (['restore', 'register', 'assign', 'copyRequest', 'expenseRequest', 'printRequest', 'exportPayment', 'transfer'].includes(element.event)) {
      action = PermissionAction.add;
    }
    if (['changePassword', 'setDebt', 'setUnDebt', 'import', 'exportData', 'updateData', 'monthlyCompanyAllocation'].includes(element.event)) {
      action = PermissionAction.edit;
    }
    // if (element.event === 'viewdetail') {
    //   action = PermissionAction;
    // }
    if (element.event === 'delete') {
      action = PermissionAction.delete;
    }
    if (userPermission.listAction.includes(action)) {
      listButton.push(element);
    }
  });
  listParamsFilter.value = {
    listParams: data.listParams,
    listBtn: listButton
  };
}

// Event handler
function numberOfRowsChanged(val: any) { emit('numberOfRowsChanged', val); }
function filterChanged(val: any) { emit('filterChanged', val); }
function keyEnter() { emit('search', txtSearch.value); }

// Các hàm emit ngắn gọn
const addFunction = () => emit('add');
const restoreFunction = () => emit('restore');
const setDebtFunction = () => emit('setDebt');
const setUnDebtFunction = () => emit('setUnDebt');
const registerFunction = () => emit('register');
const copyRequestFunction = () => emit('copyRequest');
const expenseRequestFunction = () => emit('expenseRequest');
const importFunction = () => emit('import');
const printerFunction = () => emit('printer');
const printRequestFunction = () => emit('printRequest');
const viewDetailFunction = () => emit('viewdetail');
const exportPaymentFunction = () => emit('exportPayment');
const assignFunction = () => emit('assign');
const transferFunction = () => emit('transfer');
const deleteFunction = () => emit('delete');
const exportFunction = () => emit('export');
const exportDataFunction = () => emit('exportData');
const updateDataFunction = () => emit('updateData');
const changePasswordFunction = () => emit('changePassword');
const viewFunction = () => emit('view');
const monthlyCompanyAllocationFunction = () => emit('monthlyCompanyAllocation');

// Xử lý click nút với xác nhận restore
function btnFunction(val: string) {
  switch (val) {
    case 'add': addFunction(); break;
    case 'register': registerFunction(); break;
    case 'setDebt': setDebtFunction(); break;
    case 'changePassword': changePasswordFunction(); break;
    case 'setUnDebt': setUnDebtFunction(); break;
    case 'copyRequest': copyRequestFunction(); break;
    case 'expenseRequest': expenseRequestFunction(); break;
    case 'import': importFunction(); break;
    case 'export': exportFunction(); break;
    case 'exportData': exportDataFunction(); break;
    case 'updateData': updateDataFunction(); break;
    case 'printer': printerFunction(); break;
    case 'printRequest': printRequestFunction(); break;
    case 'exportPayment': exportPaymentFunction(); break;
    case 'viewdetail': viewDetailFunction(); break;
    case 'assign': assignFunction(); break;
    case 'transfer': transferFunction(); break;
    case 'restore':
      ElMessageBox.confirm(
        t('ConfirmRestore').toString(),
        t('Confirm').toString(),
        {
          confirmButtonText: t('OK').toString(),
          cancelButtonText: t('Cancel').toString(),
          type: 'warning'
        }
      ).then(() => {
        restoreFunction();
      }).catch(() => { });
      break;
    case 'delete': deleteFunction();
      break;
    case 'view': viewFunction(); break;
    case 'monthlyCompanyAllocation': monthlyCompanyAllocationFunction(); break;
    case 'lock': emit('lock'); break;
    case 'unlock': emit('unlock'); break;
    default: return;
  }
}

// Map type button -> icon Element Plus
function getElIcon(type: string) {
  switch (type) {
    case 'add': return Plus;
    case 'delete': return Delete;
    case 'assign': return User;
    case 'edit': return Edit;
    case 'copyRequest': return Document;
    case 'printer': return Printer;
    case 'printRequest': return Printer;
    case 'import': return Upload;
    case 'export': return Download;
    case 'exportData': return Download;
    case 'exportPayment': return Download;
    case 'restore': return Refresh;
    case 'setDebt': return Lock;
    case 'setUnDebt': return Lock;
    case 'viewdetail': return View;
    case 'changePassword': return Lock;
    case 'monthlyCompanyAllocation': return DataLine; // Sử dụng icon DataLine cho Monthly Company Allocation
    case 'view': return View;
    case 'lock': return Lock;
    case 'unlock': return Lock;
    default: return null;
  }
}

// Style cho từng loại button (nên gắn CSS class, hạn chế inline)
function calBtnClass(val: string) {
  switch (val) {
    case 'add': return 'btn-add';
    case 'restore': return 'btn-restore';
    case 'search': return 'btn-search';
    case 'register': return 'btn-register';
    case 'copyRequest': return 'btn-copyRequest';
    case 'expenseRequest': return 'btn-printRequest';
    case 'printer': return 'btn-printer';
    case 'import': return 'btn-import';
    case 'export': return 'btn-export';
    case 'exportPayment': return 'btn-export';
    case 'exportData': return 'btn-export-vendor';
    case 'updateData': return 'btn-update-vendor';
    case 'printRequest': return 'btn-printRequest';
    case 'setDebt': return 'btn-setDebt';
    case 'setUnDebt': return 'btn-setUnDebt';
    case 'viewdetail': return 'btn-viewdetail';
    case 'assign': return 'btn-assign';
    case 'delete': return 'btn-delete';
    case 'changePassword': return 'btn-changePassword';
    case 'lock': return 'btn-lock';
    case 'unlock': return 'btn-unlock';
    case 'view': return 'btn-view';
    case 'monthlyCompanyAllocation': return 'btn-monthlyCompanyAllocation';
    default: return 'btn';
  }
}

// Kiểm tra disable nút
function getDisableButton(btnType: string) {
  if ((btnType === 'delete' || btnType === 'restore') && props.disabledDelete) return true;
  if (btnType === 'viewdetail' && props.disabledViewDetail) return true;
  if (btnType === 'assign' && props.disabledAssign) return true;
  if ((btnType === 'setDebt' || btnType === 'setUnDebt') && props.disabledUpdateDebt) return true;
  if (btnType === 'copyRequest' && props.disabledCopy) return true;
  if (btnType === 'expenseRequest' && props.disabledExpense) return true;
  if (btnType === 'printRequest' && props.disabledPrint) return true;
  if (btnType === 'import' && props.disabledImport) return true;
  if (btnType === 'export' && props.disabledExport) return true;
  if (btnType === 'exportPayment' && props.disabledExportPayment) return true;
  if (btnType === 'lock' && props.disabledLock) return true;
  if (btnType === 'unlock' && props.disabledUnlock) return true;
  return false;
}
</script>

<style lang="scss" scoped>
.filter-header-info {
  .filter-title {
    font-size: 22px;
    font-weight: bold;
    color: var(--el-text-color-primary, #212529);
    line-height: 1.2;
    margin-bottom: 8px; // <-- Thêm dòng này cho giãn cách!
  }

  .filter-desc {
    font-size: 0.875rem; // 14px
    color: var(--el-text-color-secondary, #65758b);
  }
}

.event-filter {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-right: 10px;
  padding-bottom: 5px;
  gap: 16px;

  &.so-many {
    flex-wrap: wrap;
  }
}

.textRow {
  font-weight: bold;
  padding-right: 12px;
  font-size: 0.875rem; // 14px
}

.btnfunction_wrap {
  display: flex;
  gap: 8px;
  margin-left: auto;
  flex-wrap: wrap;
}

.btn {
  border-radius: var(--global-radius, 8px) !important;
  padding: 0 18px;
  border: none;
  min-height: 36px;
  box-shadow: none;
  font-weight: 500;
  font-size: 0.875rem; // 14px
  transition: background 0.2s;
  cursor: pointer;
  display: flex;
  align-items: center;

  .el-icon {
    margin-right: 6px;
    font-size: 20px;
  }

  &.btn-add {
    background-color: #22c55e !important;
    color: #fff !important;

    &:hover {
      background-color: #16a34a !important;
    }
  }

  &.btn-delete {
    background-color: #d53441 !important;
    color: #fff !important;

    &:hover {
      background-color: #b91c1c !important;
    }
  }

  &.btn-assign {
    background-color: #409eff !important;
    color: #fff !important;

    &:hover {
      background-color: #2563eb !important;
    }
  }

  &.btn-monthlyCompanyAllocation {
    background-color: #fcad47 !important;
    color: #fff !important;

    &:hover {
      background-color: #fd9d1e !important;
    }
  }

  // Thêm style cho các loại khác nếu cần...
  // Thiện thêm các nút chức năng khác
  &.btn-import {
    background-color: #6b78e7 !important;
    color: #fff !important;

    &:hover {
      background-color: #3745c5 !important;
    }
  }

  &.btn-export {
    background-color: #fa933f !important;
    color: #fff !important;

    &:hover {
      background-color: #c55837 !important;
    }
  }

  &.btn-lock {
    background-color: #f97316 !important;
    color: #fff !important;

    &:hover {
      background-color: #ea580c !important;
    }
  }

  &.btn-unlock {
    background-color: #16a34a !important;
    color: #fff !important;

    &:hover {
      background-color: #15803d !important;
    }
  }
}

// Responsive (Metronic/Bootstrap style)
@media (max-width: 768px) {
  .event-filter {
    flex-direction: column;
    align-items: stretch;

    .btnfunction_wrap {
      justify-content: center;
      margin-left: 0;
      margin-top: 12px;
    }
  }
}
</style>
