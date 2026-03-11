<template>
  <div class="row" v-loading="loading">
    <div class="col-12">
      <div class="table-responsive" style="width: 100%;">
        <el-table :data="sortedItems" :border="true" style="width: 100%;" :fit="true" ref="tableRef"
          :row-class-name="tableRowClassName" @row-dblclick="onRowDblClick" @sort-change="onSortChange"
          @selection-change="onSelectionChange" @row-click="onRowClick" :height="props.height || 600"
          class="custom-header-bg" :row-key="props.rowKey" :reserve-selection="props.reserveSelection"
          :select-on-row-click="props.selectOnRowClick">
          <el-table-column v-if="showCheckboxColumn" type="selection" width="40" align="center" fixed="left" />

          <el-table-column v-if="showIndex" :label="t('common.index')" width="60" align="center" header-align="center"
            fixed="left">
            <template #default="scope">
              {{ (props.page - 1) * props.pageSize + scope.$index + 1 }}
            </template>
          </el-table-column>

          <el-table-column v-for="col in processedColumns" :key="col.key" :prop="col.key"
            :label="col.isLocale === false ? col.labelKey : t(col.labelKey)"
            :width="col.key === '__filler__' ? undefined : col.width" :min-width="col.minWidth"
            :fixed="isMobile ? false : (typeof col.fixed !== 'undefined' ? col.fixed : (col.sticky ? 'left' : false))"
            :align="col.align || 'left'" :header-align="col.headerAlign || 'center'"
            :show-overflow-tooltip="col.key !== '__filler__'" header-cell-class-name="custom-header-cell">
            <template #header>
              <div v-if="col.key !== '__filler__'" class="header-wrap">
                <div class="header-label-wrap">
                  <span class="header-label">{{ col.isLocale === false ? col.labelKey : t(col.labelKey) }}</span>
                  <span v-if="col.sortable !== false && col.sortable !== undefined" class="sort-icons tight compact">
                    <el-icon class="sort-icon">
                      <CaretTop :class="{ active: sortByLocal === col.key && sortDirectionLocal === 'asc' }"
                        @click.stop="toggleSort(col.key, 'asc')" />
                    </el-icon>
                    <el-icon class="sort-icon">
                      <CaretBottom :class="{ active: sortByLocal === col.key && sortDirectionLocal === 'desc' }"
                        @click.stop="toggleSort(col.key, 'desc')" />
                    </el-icon>
                  </span>
                </div>
                <div class="header-filter-row">
                  <el-input v-if="col.filterType === 'text'" v-model="filterLocal[col.key]"
                    @input="onFilterChange(col.key, filterLocal[col.key])"
                    :placeholder="t('common.filter') + ' ' + (col.isLocale === false ? col.labelKey : t(col.labelKey))"
                    clearable />
                  <el-select v-else-if="col.filterType === 'select'" v-model="filterLocal[col.key]"
                    @change="onFilterChange(col.key, filterLocal[col.key])" clearable filterable style="width: 100%;">
                    <el-option v-for="opt in col.filterOptions" :key="String(opt.value)"
                      :label="opt.isLocale === false ? opt.label : t(opt.label)" :value="opt.value" />
                  </el-select>
                  <el-date-picker v-else-if="col.filterType === 'date'" v-model="filterLocal[col.key]" type="date"
                    clearable value-format="YYYY-MM-DD" format="DD/MM/YYYY"
                    :placeholder="t('common.filter') + ' ' + (col.isLocale === false ? col.labelKey : t(col.labelKey))"
                    @change="onFilterChange(col.key, filterLocal[col.key])" style="width: 100%;" />
                </div>
              </div>
            </template>
            <template #default="scope">
              <template v-if="col.key === '__filler__'">
                <!-- Empty filler column to occupy space -->
              </template>
              <template v-else>
                <template v-if="$slots['cell-' + col.key]">
                  <slot :name="'cell-' + col.key" :item="scope.row" />
                </template>
                <template v-else-if="col.columnType === 'boolean'">
                  <el-checkbox v-model="scope.row[col.key]" size="large" />
                </template>
                <template v-else-if="col.formatter">
                  {{ col.formatter(scope.row[col.key], scope.row) }}
                </template>
                <template v-else>
                  <span :class="{ 'cell-bold': col.isBold }">{{ scope.row[col.key] }}</span>
                </template>
              </template>
            </template>
          </el-table-column>

          <el-table-column v-if="props.showActionsColumn" prop="actions" :label="t('common.actions')"
            :width="props.actionsColumnWidth ?? 180" align="center" :fixed="isMobile ? false : 'right'">
            <template #default="scope">
              <div class="action-buttons">
                <!-- View -->
                <el-tooltip v-if="props.showView" :content="t('common.viewDetail')" placement="top" :teleported="true"
                  :popper-options="{ strategy: 'fixed' }" popper-class="table-tooltip-popper">
                  <el-button circle size="small" @click="$emit('view', scope.row)">
                    <el-icon>
                      <View />
                    </el-icon>
                  </el-button>
                </el-tooltip>

                <!-- Edit -->
                <el-tooltip v-if="props.showEdit" :content="t('common.edit')" placement="top" :teleported="true"
                  :popper-options="{ strategy: 'fixed' }" popper-class="table-tooltip-popper">
                  <el-button circle size="small" @click="$emit('edit', scope.row)">
                    <el-icon>
                      <Edit />
                    </el-icon>
                  </el-button>
                </el-tooltip>
                <!-- Các action custom khác -->

                <slot name="actions" :item="scope.row" />
                <!-- Delete -->
                <el-tooltip v-if="props.showDelete" :content="t('common.delete')" placement="top" :teleported="true"
                  :popper-options="{ strategy: 'fixed' }" popper-class="table-tooltip-popper">
                  <el-button circle size="small" @click="$emit('delete', scope.row)">
                    <el-icon>
                      <Delete />
                    </el-icon>
                  </el-button>
                </el-tooltip>



              </div>
            </template>
          </el-table-column>
        </el-table>
        <div v-if="props.showPagination" class="table-pagination-wrap center">
          <el-pagination background :layout="isMobile ? 'prev, pager, next' : 'total, sizes, prev, pager, next, jumper'"
            :total="searchMode === 'client' ? filteredItems.length : total" :page-size="props.pageSize"
            :current-page="props.page" @size-change="onPageSizeChange" @current-change="onPageChange"
            :page-sizes="[1, 10, 20, 30, 50, 100]" style="margin-top: 12px; justify-content: flex-end;" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { ref, watch, computed, type PropType, onMounted, onBeforeUnmount, nextTick } from 'vue';
import { CaretTop, CaretBottom } from '@element-plus/icons-vue';
import { View, Edit, Delete, } from '@element-plus/icons-vue';

const { t } = useI18n();
const tableRef = ref();
export interface BaseTableColumn {
  key: string
  labelKey: string
  isLocale?: boolean
  width?: number | string
  minWidth?: number | string
  sortable?: boolean
  filterType?: 'text' | 'select' | 'date' | 'number' | 'boolean'
  filterOptions?: Array<{ label: string; value: any, isLocale?: boolean }>
  sticky?: boolean
  fixed?: 'left' | 'right' | boolean
  columnType?: 'text' | 'boolean' | 'date' | 'number' | 'index'
  formatter?: (value: any, row?: any) => string
  align?: 'left' | 'center' | 'right'
  headerAlign?: 'left' | 'center' | 'right'
  isBold?: boolean
};

const props = defineProps({
  columns: Array as PropType<BaseTableColumn[]>,
  items: Array as PropType<any[]>,
  loading: Boolean,
  filter: Object as PropType<Record<string, any>>,
  showCheckboxColumn: Boolean,
  searchMode: {
    type: String as PropType<'server' | 'client'>,
    default: 'client',
  },
  showActionsColumn: {
    type: Boolean,
    default: true,
  },
  showPagination: {
    type: Boolean,
    default: false,
  },
  page: {
    type: Number,
    default: 1,
  },
  pageSize: {
    type: Number,
    default: 30,
  },
  total: {
    type: Number,
    default: 0,
  },
  height: {
    type: [String, Number],
    default: 600,
  },
  showView: { type: Boolean, default: false },
  showEdit: { type: Boolean, default: false },
  showDelete: { type: Boolean, default: false },
  showIndex: {
    type: Boolean,
    default: false,
  },
  actionsColumnWidth: { type: [String, Number], default: 120 },
  disableRowDblClick: { type: Boolean, default: false },
  rowKey: {
    type: [String, Function] as PropType<string | ((row: any) => string | number)>,
    default: undefined
  },
  reserveSelection: {
    type: Boolean,
    default: true
  },
  selectOnRowClick: {
    type: Boolean,
    default: true
  }

});
defineExpose({
  setRowsCheck
});
const emit = defineEmits(['update:filter', 'view', 'edit', 'delete', 'update:rows', 'update:page', 'update:pageSize']);

const filterLocal = ref<Record<string, any>>({ ...props.filter });
const ignoreNextSelectionChange = ref(false);
const isRestoringSelection = ref(false);
const lastSelectionSnapshot = ref<any[]>([]);
let clearIgnoreTimer: any = null;

watch(() => props.filter, (val) => {
  filterLocal.value = { ...val };
});

function onSelectionChange(selection: any[]) {
  if (isRestoringSelection.value) return;
  if (ignoreNextSelectionChange.value) {
    ignoreNextSelectionChange.value = false;
    restoreSelection(lastSelectionSnapshot.value);
    return;
  }
  lastSelectionSnapshot.value = selection;
  emit('update:rows', selection);
}

function onRowClick(row: any, column: any, event: Event) {
  // Nếu click từ cột isApprover (custom slot) -> prevent selection change
  const target = event.target as HTMLElement
  const cellElement = target.closest('[data-prevent-row-click]')
  if (cellElement) {
    ignoreNextSelectionChange.value = true;
    if (clearIgnoreTimer) clearTimeout(clearIgnoreTimer);
    clearIgnoreTimer = setTimeout(() => {
      ignoreNextSelectionChange.value = false;
    }, 0);
    event.stopPropagation()
  }
}

async function restoreSelection(selection: any[]) {
  const tableref = tableRef.value;
  if (!tableref) return;
  isRestoringSelection.value = true;
  tableref.clearSelection?.();
  (selection ?? []).forEach(row => {
    tableref.toggleRowSelection(row, true);
  });
  await nextTick();
  isRestoringSelection.value = false;
}

function onFilterChange(key: string, value: any) {
  const newFilter = { ...filterLocal.value, [key]: value };
  filterLocal.value = newFilter;
  emit('update:filter', newFilter);
}

const sortByLocal = ref<string>('');
const sortDirectionLocal = ref<'asc' | 'desc'>('asc');

function toggleSort(key: string, direction?: 'asc' | 'desc') {
  if (sortByLocal.value === key) {
    sortDirectionLocal.value = direction || (sortDirectionLocal.value === 'asc' ? 'desc' : 'asc');
  } else {
    sortByLocal.value = key;
    sortDirectionLocal.value = direction || 'asc';
  }
}

function onSortChange({ prop, order }: any) {
  sortByLocal.value = prop || '';
  sortDirectionLocal.value = order === 'ascending' ? 'asc' : 'desc';
}

const sortedItems = computed(() => {
  let items = [...(props.items ?? [])];
  // Filter client
  if (props.searchMode === 'client') {
    Object.entries(filterLocal.value).forEach(([key, val]) => {
      if (val != null && val !== '') {
        items = items.filter(item => String(item[key]).toLowerCase().includes(String(val).toLowerCase()));
      }
    });
  }
  // Sort client
  if (sortByLocal.value) {
    items = items.sort((a, b) => {
      const valA = a[sortByLocal.value];
      const valB = b[sortByLocal.value];
      if (valA == null) return 1;
      if (valB == null) return -1;
      return sortDirectionLocal.value === 'asc' ? (valA < valB ? -1 : 1) : (valA > valB ? -1 : 1);
    });
  }
  // Pagination client
  if (props.searchMode === 'client' && props.showPagination) {
    const start = (props.page - 1) * props.pageSize;
    const end = start + props.pageSize;
    return items.slice(start, end);
  }
  return items;
});

const filteredItems = computed(() => {
  let items = [...(props.items ?? [])];
  // Filter
  if (props.searchMode === 'client') {
    Object.entries(filterLocal.value).forEach(([key, val]) => {
      if (val != null && val !== '') {
        items = items.filter(item => String(item[key]).toLowerCase().includes(String(val).toLowerCase()));
      }
    });
  }
  return items;
});
const isMobile = ref(window.innerWidth <= 767);
function handleResize() {
  isMobile.value = window.innerWidth <= 767;
}

onMounted(() => {
  window.addEventListener('resize', handleResize);
  handleResize();
});
onBeforeUnmount(() => {
  window.removeEventListener('resize', handleResize);
});

const processedColumns = computed(() => {
  return (props.columns ?? [])
    .filter(col => col.key !== 'actions')
    .map(col => (!col.width ? { ...col, minWidth: 200 } : col));
});

function onRowDblClick(item: any) {
  if (!props.disableRowDblClick) {
    emit('edit', item);
  }
}

function tableRowClassName({ row }: any) {
  return row.isParentRow ? 'parent-row-label-row' : '';
}

function setRowsCheck(arrRow: Array<any>) {
  const tableref = tableRef.value;
  arrRow.forEach(row => {
    tableref.toggleRowSelection(row, true);
  });
}

function onPageChange(page: number) {
  emit('update:page', page);
}
function onPageSizeChange(size: number) {
  emit('update:pageSize', size);
}
</script>

<style lang="scss" scoped>
.header-wrap {
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 2px 6px;
  box-sizing: border-box;
}

.table-responsive {
  width: 100%;
  overflow-x: auto;
  font-size: 0.75rem !important;
  /* 14px */
}

.header-label-wrap {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  gap: 1px;
}

.header-label {
  font-weight: bold;
  font-size: 13px;
  color: var(--el-text-color-primary);
  white-space: nowrap;
}

.sort-icons {
  display: flex;
  flex-direction: column;
  align-items: center;
  font-size: 11px;
  color: #f8b08d;
  cursor: pointer;
  line-height: 1;
  gap: 0px;
}

.sort-icons.tight {
  gap: 0px;
  margin-left: 2px;
  line-height: 1;
}

.sort-icons.compact .sort-icon+.sort-icon {
  margin-top: -3px;
}

.sort-icons .active {
  color: #409EFF !important;
}

.header-filter-row {
  display: flex;
  align-items: center;
  width: 100%;
  gap: 4px;
}

.el-table .caret-wrapper {
  flex-direction: column !important;
  align-items: center !important;
  justify-content: center !important;
  height: auto !important;
  width: auto !important;
  position: static !important;
}

:deep(.custom-header-bg .el-table__header-wrapper th) {
  background-color: var(--el-bg-color-page) !important;
  transition: background-color 0.3s ease;
}

.table-pagination-wrap {
  display: flex;
  align-items: center;
}



.table-pagination-wrap.center {
  justify-content: center;
}

:deep(.el-table) {
  border-radius: 8px;
  overflow: hidden;
  /* giúp nội dung bo theo góc */
  border: 1px solid var(--el-border-color);
}

:deep(.el-table__header-wrapper),
:deep(.el-table__body-wrapper) {
  border-radius: 0;
}

.cell-bold {
  font-weight: bold !important;
}

/* Cấu hình cho các nút action, bỏ màu nền và viền */
.el-button.is-circle {
  background: transparent !important;
  border: none !important;
  box-shadow: none !important;
  border-radius: 12px !important;
  min-width: 20px;
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

/* Cấu hình chung cho các nút action */
.el-button.is-circle {
  background: transparent !important;
  border: none !important;
  box-shadow: none !important;
  border-radius: 12px !important;
  min-width: 30px;
  height: 30px;
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
  border-radius: 12px;
  z-index: 0;
  opacity: 0;
  transition: opacity 0.2s;
  pointer-events: none;
  display: block;
}

/* Khi hover/focus vào nút, hiện nền vàng bo góc */
.el-button.is-circle:hover::after,
.el-button.is-circle:focus::after {
  opacity: 1;
}

/* Icon sẽ nằm trên nền vàng */
.el-button.is-circle .el-icon,
.el-button.is-circle i {
  position: relative;
  z-index: 1;
  color: var(--kt-menu-text-color) !important;
  font-size: 14px;
}

/* Icon Delete màu đỏ riêng biệt */
.el-button.is-circle:last-child .el-icon,
.el-button.is-circle:last-child i {
  color: #f44336 !important;
}

/* Căn chỉnh các nút hành động theo một hàng ngang */
.action-buttons {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 2px;
}

:deep(.el-table .el-table__cell) {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

:deep(.el-table) {
  border-radius: 8px;
  /* overflow: hidden;  // ❌ bỏ dòng này nếu đang có */
  border: 1px solid var(--el-border-color);
}

/* Cho phép phần tử “bay” (teleported) / fixed popper hiển thị đúng */
:deep(.el-table__inner-wrapper) {
  overflow: visible !important;
}

/* Tăng z-index cho popper của tooltip (phòng trường hợp page có overlay cao) */
:deep(.table-tooltip-popper) {
  z-index: 9999 !important;
}
</style>
