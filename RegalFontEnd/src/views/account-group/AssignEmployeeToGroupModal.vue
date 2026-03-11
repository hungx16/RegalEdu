<template>
  <BaseDialogForm :visible="isOpenAssignDialog" @update:visible="(v) => emit('update:isOpenAssignDialog', v)"
    :title="t('accountGroup.assignEmployeeToGroup')" width="950px"
    :description="selectedRowsData?.length ? `${t('accountGroup.userGroup')}: ${selectedRowsData[0].name}` : ''"
    mode="view" :loading="isLoading" :height="dialogHeight" :submitDisabled="false" :hideDefaultSave="true"
    :showEdit="false" :showDelete="false" @submit="onSubmitFromDialog">
    <!-- BODY -->
    <template #form>
      <div style="width: 100%;">
        <!-- Controls bar (centered) -->
        <!-- Controls bar (ONE LINE) -->
        <div class="controls-bar">
          <div class="controls-inline">
            <!-- Trái: Radio -->
            <el-radio-group v-model="rdSelect" @change="rdChanged" class="no-shrink">
              <el-radio :label="RD.Unselected">{{ t('accountGroup.unSelected') }}</el-radio>
              <el-radio :label="RD.Selected">{{ t('accountGroup.selected') }}</el-radio>
            </el-radio-group>

            <!-- Giữa: Select loại -->
            <el-select v-model="userTypeFilter" class="type-select" :placeholder="t('applicationUser.type')"
              @change="onTypeChange">
              <el-option v-for="opt in userTypeOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
            </el-select>

            <!-- Phải: Tìm kiếm -->
            <el-input class="search-input align-right" :placeholder="t('common.search')" :prefix-icon="Search"
              v-model="txtSearch" @update:modelValue="onSearchChange" clearable />
          </div>
        </div>


        <!-- Table centered -->
        <div class="table-wrap">
<BaseTable ref="baseTableRef" :columns="listColumnEmployee" :items="listForGridEmployee" :loading="isLoading"
            :showPagination="true" :page="page" :pageSize="pageSize" :total="listForGridEmployee.length"
            searchMode="client" :showActionsColumn="false" :showCheckboxColumn="true" @update:page="onPageChange"
            @update:pageSize="onPageSizeChange" @update:rows="onRowsUpdate" :row-key="rowKeySelector"
            :reserve-selection="true" :select-on-row-click="false">
            <template #cell-isApprover="{ item }">
              <div data-prevent-row-click>
                <el-switch v-model="item.isApprover" @change="(val: boolean) => onApproverToggle(item, val)" />
              </div>
            </template>
          </BaseTable>
        </div>
      </div>
    </template>

    <!-- FOOTER EXTRA: nút nhãn động Add/Save -->
    <template #footer-extra>
      <el-button type="primary" class="btn btn-primary" @click="onSubmitFromDialog" :disabled="isLoading">
        {{ rdSelect === RD.Unselected ? t('accountGroup.add') : t('accountGroup.save') }}
      </el-button>
    </template>
  </BaseDialogForm>
</template>

<script lang="ts" setup>
import type { AccountGroupModel } from '@/api/AccountGroupApi'
import type { AccountGroupEmployeeRequestModel } from '@/api/accountGroupEmployeeApi'
import type { ApplicationUserModel } from '@/api/ApplicationUserApi'
import type { BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseTable from '@/components/table/BaseTable.vue'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import { useAccountGroupEmployeeStore } from '@/stores/accountGroupEmployeeStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { ref, onMounted, nextTick, watch, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { Search } from '@element-plus/icons-vue'
import { getApplicationUserTypeOptions } from '@/utils/makeList'
const USER_TYPE_ALL = -1
const userTypeFilter = ref<number>(USER_TYPE_ALL)

const userTypeOptions = computed(() => [
  { value: USER_TYPE_ALL, label: t('common.all') },
  ...getApplicationUserTypeOptions(t)   // [{value:0,label:'Teacher'},{value:1,label:'Employee'}] (đã dịch)
])

const { t } = useI18n()
const accountGroupEmployeeStore = useAccountGroupEmployeeStore()
const notificationStore = useNotificationStore()

const emit = defineEmits(['update:selectedRowsEmployee', 'update:isOpenAssignDialog'])
const props = defineProps<{
  isOpenAssignDialog: boolean
  selectedRowsData: Array<AccountGroupModel>
  listEmployeeFull: Array<ApplicationUserModel>
  arrEmpNoGroup: Array<string>
  arrEmpInSelectedGroup: Array<string>
}>()

const RD = {
  Unselected: 'unselected',
  Selected: 'selected',
} as const
type RDType = typeof RD[keyof typeof RD]
type EmployeeRow = ApplicationUserModel & { isApprover?: boolean }

// Theo dõi tab trước đó
const lastTab = ref<RDType>(RD.Selected)

const selectedRowsEmployee = ref<Array<EmployeeRow>>([])
const selectedCodes = ref<Set<string>>(new Set())

const isApproverMap = ref<Map<string, boolean>>(new Map())

let listEmployeeSelected: Array<EmployeeRow> = []
let listEmployeeUnselected: Array<EmployeeRow> = []
const listForGridEmployee = ref<Array<EmployeeRow>>([])

// Columns
const listColumnEmployee: Array<BaseTableColumn> = [
  { key: 'fullName', labelKey: 'applicationUser.fullName', sortable: true },
  // { key: 'userName', labelKey: 'applicationUser.userName', sortable: true, width: 220 },
  { key: 'email', labelKey: 'applicationUser.email', sortable: true, width: 220 },
  { key: 'isApprover', labelKey: 'accountGroupEmployee.isApprover', columnType: 'boolean', width: 140, align: 'center' },
]

// State
const isLoading = ref(false)
const txtSearch = ref('')
const rdSelect = ref<RDType>(RD.Selected)

// Pagination
const page = ref(1)
const pageSize = ref(30)

// Height responsive cho BaseDialogForm
const dialogHeight = computed(() => (window.innerWidth < 768 ? '80vh' : '65vh'))

const baseTableRef = ref()
// thêm flag để tạm thời vô hiệu hoá onRowsUpdate khi đang reapply selection
const suppressOnRowsUpdate = ref(false)
const rowKeySelector = (row: EmployeeRow) => row.userCode ?? row.id ?? row.email ?? row.fullName ?? ''

// Debounce helper
function debounce<T extends (...args: any[]) => void>(fn: T, delay = 200) {
  let t: any
  return (...args: Parameters<T>) => {
    clearTimeout(t)
    t = setTimeout(() => fn(...args), delay)
  }
}

function syncApproverMapFromStore(reset = false) {
  const next = reset ? new Map<string, boolean>() : new Map(isApproverMap.value)
  accountGroupEmployeeStore.accountGroupEmployees.forEach(emp => {
    if (emp?.userCode) {
      next.set(emp.userCode, !!emp.isApprover)
    }
  })
  isApproverMap.value = next
}

function getApproverValue(code?: string | null) {
  if (!code) return false
  const existing = isApproverMap.value.get(code)
  if (existing === undefined) {
    const next = new Map(isApproverMap.value)
    next.set(code, false)
    isApproverMap.value = next
    return false
  }
  return existing
}

function setApproverValue(code: string, value: boolean) {
  if (!code) return
  const next = new Map(isApproverMap.value)
  next.set(code, value)
  isApproverMap.value = next
}

function applyApproverFlag(list: Array<ApplicationUserModel>): Array<EmployeeRow> {
  return list.map(item => {
    const code = item.userCode ?? ''
    const isApprover = getApproverValue(code)
    return { ...item, isApprover }
  })
}

async function onApproverToggle(item: EmployeeRow, value?: boolean) {
  const code = item?.userCode ?? ''
  const nextVal = value ?? !!item?.isApprover
  suppressOnRowsUpdate.value = true
  const selectionSnapshot = new Set(selectedCodes.value)
  setApproverValue(code, nextVal)
  item.isApprover = nextVal
  selectedCodes.value = selectionSnapshot
  await nextTick()
  await reapplySelection()
}

// Đồng bộ selection từ danh sách thành viên thuộc nhóm (arrEmpInSelectedGroup)
function syncSelectedFromGroup(force = false) {
  if (rdSelect.value !== RD.Selected) return
  if (force || selectedCodes.value.size === 0) {
    selectedCodes.value = new Set(props.arrEmpInSelectedGroup.filter(Boolean))
  }
}

// đổi loại nhân viên
function onTypeChange() {
  page.value = 1
  filterList()
}

// Áp lại selection vào bảng theo selectedCodes hiện tại (đợi table render xong)
async function reapplySelection() {
  // avoid reacting to intermediate rows update events
  suppressOnRowsUpdate.value = true

  // đảm bảo DOM / BaseTable đã cập nhật
  await nextTick()
  await nextTick()

  const rowsToCheck = listForGridEmployee.value.filter(u =>
    selectedCodes.value.has(u.userCode ?? '')
  )
  // Nếu BaseTable hỗ trợ API setRowsCheck
  // @ts-ignore
  await baseTableRef.value?.setRowsCheck?.(rowsToCheck)

  // small delay để BaseTable hoàn tất nội bộ trước khi bật lại handler
  await nextTick()
  suppressOnRowsUpdate.value = false
}

// Init khi mount (nếu mở sẵn)
onMounted(() => {
  if (props.isOpenAssignDialog) {
    rdSelect.value = RD.Selected
    lastTab.value = RD.Selected
    syncApproverMapFromStore(true)
    syncSelectedFromGroup(true)   // force init từ nhóm
    rdChanged()
    reapplySelection()
  }
})

// Re-open watcher
watch(
  () => props.isOpenAssignDialog,
  (val) => {
    if (val) {
      rdSelect.value = RD.Selected
      lastTab.value = RD.Selected
      syncApproverMapFromStore(true)
      syncSelectedFromGroup(true) // force init từ nhóm khi mở
      rdChanged()
      reapplySelection()
    } else {
      selectedRowsEmployee.value = []
      // @ts-ignore
      baseTableRef.value?.clearSelection?.()
    }
  }
)

// Props đổi -> refresh
watch(
  () => [props.listEmployeeFull, props.arrEmpNoGroup, props.arrEmpInSelectedGroup],
  () => {
    if (props.isOpenAssignDialog) {
      syncApproverMapFromStore(true)
      // Khi danh sách thành viên nhóm đổi, nếu đang ở Đã chọn thì init lại selection
      if (rdSelect.value === RD.Selected) {
        syncSelectedFromGroup(true)
      }
      rdChanged()
      reapplySelection()
    }
  },
  { deep: true }
)

// Chuyển tab
function rdChanged() {
  page.value = 1

  if (rdSelect.value === RD.Unselected) {
    listEmployeeUnselected = applyApproverFlag(
      props.listEmployeeFull.filter((item) =>
        props.arrEmpNoGroup.includes(item.userCode ?? '')
      )
    )
    listForGridEmployee.value = listEmployeeUnselected
  } else {
    listEmployeeSelected = applyApproverFlag(
      props.listEmployeeFull.filter((item) =>
        props.arrEmpInSelectedGroup.includes(item.userCode ?? '')
      )
    )
    listForGridEmployee.value = listEmployeeSelected

    // Nếu vừa chuyển từ tab khác sang Đã chọn -> init selection từ danh sách nhóm
    syncSelectedFromGroup(lastTab.value !== RD.Selected)
  }

  lastTab.value = rdSelect.value

  filterList()       // áp keyword hiện tại
  reapplySelection() // rồi áp selection vào kết quả
}

// Realtime search
const onSearchChange = debounce(() => {
  filterList()
}, 200)
/** Helper: gom các field có thể search, kể cả trong teacher/employee */
function getSearchHaystack(u: any): string {
  const parts = [
    u?.fullName, u?.email, u?.userName, u?.userCode,
  ]
  return parts.filter(Boolean).join(' | ').toLowerCase()
}

function filterList() {
  page.value = 1
  const source = rdSelect.value === RD.Unselected ? listEmployeeUnselected : listEmployeeSelected

  let filtered = source as Array<ApplicationUserModel>

  // 2) Lọc theo loại nhân viên
  if (userTypeFilter.value !== USER_TYPE_ALL) {
    filtered = filtered.filter((el: any) => {
      if (userTypeFilter.value === 0) return el?.teacher != null     // Teacher
      if (userTypeFilter.value === 1) return el?.employee != null    // Employee
      return true
    })
  }

  // 3) Lọc theo keyword (soi cả nested fields)
  const kw = (txtSearch.value || '').toLowerCase().trim()
  if (kw) {
    filtered = filtered.filter((u: any) => getSearchHaystack(u).includes(kw))
  }

  listForGridEmployee.value = applyApproverFlag(filtered)

  // Sau khi lọc: nếu đang tab Đã chọn, đảm bảo có selection khởi tạo nếu đang trống
  syncSelectedFromGroup(false)
  reapplySelection()
}

// Khi bảng báo rows đang tick -> cập nhật selection bền
function onRowsUpdate(val: Array<EmployeeRow>) {
  // nếu đang reapply selection programmatically thì ignore event
  if (suppressOnRowsUpdate.value) return

  selectedRowsEmployee.value = val

  // Các code đang hiển thị
  const visibleCodes = new Set<string>(listForGridEmployee.value.map(u => u.userCode ?? ''))

  // Gộp selection: bỏ tick của các dòng đang hiển thị rồi cộng lại những dòng đang tick
  const next = new Set(selectedCodes.value)
  visibleCodes.forEach(c => next.delete(c))
    ; (val ?? []).forEach(u => next.add(u.userCode ?? ''))

  selectedCodes.value = next
}

// Submit glue cho BaseDialogForm
async function onSubmitFromDialog() {
  await saveEmployee()
}

function buildIsApproverList(userCodes: string[]): boolean[] {
  return userCodes.map(code => getApproverValue(code))
}

// Save/Add
const saveEmployee = async () => {
  try {
    const arruserCode: Array<string> = Array.from(selectedCodes.value)
    const arrIsApprover: Array<boolean> = buildIsApproverList(arruserCode)
    const dataModel: AccountGroupEmployeeRequestModel = {
      accountGroupId: props.selectedRowsData[0]?.id ?? '',
      listuserCode: arruserCode,
      listIsApprover: arrIsApprover,
    }

    isLoading.value = true

    if (rdSelect.value === RD.Unselected) {
      await accountGroupEmployeeStore.addAccountGroupEmployee(dataModel)
      notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.accountGroupEmployee') } });
    } else {
      await accountGroupEmployeeStore.saveAccountGroupEmployee(dataModel)
      notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.accountGroupEmployee') } });
    }

    emit('update:isOpenAssignDialog', false)
  } catch (e) {
    console.error(e)
  } finally {
    isLoading.value = false
  }
}

// Pagination handlers
function onPageChange(newPage: number) {
  page.value = newPage
}

function onPageSizeChange(newPageSize: number) {
  pageSize.value = newPageSize
  page.value = 1
}
</script>

<style scoped>
/* ONE-LINE controls */
.controls-bar {
  width: 100%;
  margin-bottom: 12px;
}

.controls-inline {
  display: flex;
  align-items: center;
  gap: 12px;
  /* không xuống dòng trên màn hình rộng */
  flex-wrap: nowrap;
}

/* Radio nhóm không co nhỏ quá */
.no-shrink {
  flex: 0 0 auto;
  white-space: nowrap;
}

/* Chiều rộng hợp lý cho select & search để lên cùng 1 hàng */
.type-select {
  width: 220px;
  flex: 0 0 220px;
}

.search-input {
  width: 320px;
  flex: 0 0 320px;
}

/* Mobile: cho phép xuống dòng gọn gàng */
@media (max-width: 768px) {
  .controls-inline {
    flex-wrap: wrap;
    justify-content: center;
  }

  .type-select,
  .search-input {
    flex: 1 1 100%;
    width: 100%;
  }
}
</style>
