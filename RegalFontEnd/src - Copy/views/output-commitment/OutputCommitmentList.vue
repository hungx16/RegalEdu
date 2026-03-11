<template>
  <div>
    <!-- FILTER HEADER -->
    <FilterComponent ref="filterComponentRef" @add="addItem" @delete="onDeleteClicked"
      headerTitle="outputCommitment.headerTitle" headerDesc="outputCommitment.headerDesc"
      :disabledDelete="getDisableDelete" class="mb-6" />

    <!-- 2 THẺ TỔNG QUAN -->
    <div class="row g-4 mb-8">
      <!-- Tổng số cam kết -->
      <div class="col-12 col-md-6">
        <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <span class="fw-semibold fs-5">
              {{ t('outputCommitment.totalCommitments') }}
            </span>
            <i class="bi bi-clipboard-check fs-4 text-body-secondary"></i>
          </div>
          <div class="fs-2 fw-bold mb-1">{{ totalCount }}</div>
          <div class="fs-7 text-body-secondary">
            {{ t('common.records') }}
          </div>
        </div>
      </div>

      <!-- Số cam kết đã hoàn thành -->
      <div class="col-12 col-md-6">
        <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <span class="fw-semibold fs-5">
              {{ t('outputCommitment.finishedCommitments') }}
            </span>
            <i class="bi bi-check2-circle fs-4 text-body-secondary"></i>
          </div>
          <div class="fs-2 fw-bold mb-1">{{ finishedCount }}</div>
          <div class="fs-7 text-body-secondary">
            {{ t('common.records') }}
          </div>
        </div>
      </div>
    </div>

    <!-- BẢNG DỮ LIỆU -->
    <div class="card mb-10 w-100 mt-5">
      <div class="card-header card-header-stretch">
        <div class="card-title d-flex flex-column">
          <h3 class="fw-bold fs-4 mb-1">{{ t('outputCommitment.listTitle') }}</h3>
          <span class="text-body-secondary fw-light fs-8">
            {{ t('outputCommitment.listFunction') }}
          </span>
        </div>
      </div>

      <div class="card-body py-6 px-2 px-md-6">
        <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredItemsAll" :loading="store.loading"
          :showPagination="true" :page="page" :total="filteredItemsAll.length" :pageSize="pageSize" :filter="filter"
          @update:filter="onTableFilter" @update:rows="val => (selectedItems = val)" :showActionsColumn="true"
          :showEdit="true" :showDelete="true" :showView="true" @edit="editItem" @view="viewItem" @delete="handleDelete"
          @update:page="val => (page = val)" @update:pageSize="onPageSizeChange">
          <!-- Mã HS hiển thị badge -->
          <template #cell-studentCode="{ item }">
            <BaseBadge :label="item.studentCode" color="blue" soft :bold="true" bordered />
          </template>

          <!-- Trạng thái -->
          <template #cell-outputCommitmentStatus="{ item }">
            <BaseBadge type="boolean" :label="statusText(item.outputCommitmentStatus)" />
          </template>
        </BaseTable>
      </div>
    </div>

    <!-- DIALOG THÊM / SỬA / XEM -->
    <OutputCommitmentDialog v-model:visible="showDialog" :mode="dialogMode" :data="store.selected"
      :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
  </div>
</template>

<script setup lang="ts">
/** ===== IMPORT ===== */
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import OutputCommitmentDialog from '@/views/output-commitment/OutputCommitmentDialog.vue'
import { useOutputCommitmentStore } from '@/stores/outputCommitmentStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { useStudentStore } from '@/stores/studentStore'
import { formatDate } from '@/utils/format'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import { OutputCommitmentStatus } from '@/types'

/** ===== STORE & I18N ===== */
const { t } = useI18n()
const store = useOutputCommitmentStore()
const notificationStore = useNotificationStore()
const studentStore = useStudentStore()
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

/** ===== REF / STATE ===== */
const showDialog = ref(false)
const dialogMode = ref<'create' | 'edit' | 'view'>('create')
const filterComponentRef = ref()
const page = ref(1)
const pageSize = ref(30)
const filter = ref<Record<string, any>>({})
const selectedItems = ref<any[]>([])

/** ===== HEADER CONFIG (FilterComponent) ===== */
const listHeaderParams = {
  listParams: [],
  listBtn: [
    // { event: 'add', label: 'outputCommitment.add', type: 'add' },
    { event: 'delete', label: 'outputCommitment.delete', type: 'delete' }
  ]
}

/** ===== CẤU HÌNH CỘT BẢNG ===== */
const columns: BaseTableColumn[] = [
  {
    key: 'studentCode',
    labelKey: 'outputCommitment.studentCode',
    filterType: 'text',
    sortable: true,
    sticky: true,
    width: 180,
    isBold: true
  },
  {
    key: 'studentName',
    labelKey: 'outputCommitment.student',
    filterType: 'text',
    sortable: true,
    width: 220
  },
  {
    key: 'beginningLevel',
    labelKey: 'outputCommitment.beginningLevel',
    filterType: 'text',
    sortable: true,
    align: 'center'
  },
  {
    key: 'finalLevel',
    labelKey: 'outputCommitment.finalLevel',
    filterType: 'text',
    sortable: true,
    align: 'center'
  },
  {
    key: 'outputCommitmentStatus',
    labelKey: 'outputCommitment.outputCommitmentStatus',
    filterType: 'select',
    sortable: true,
    width: 200,
    align: 'center',
    filterOptions: [
      { label: 'common.all', value: '' },
      { label: 'outputCommitment.status.notFinished', value: OutputCommitmentStatus.NotFinished },
      { label: 'outputCommitment.status.finished', value: OutputCommitmentStatus.Finished }
    ]
  },
  {
    key: 'createdBy',
    labelKey: 'common.createdBy',
    filterType: 'text',
    sortable: true,
    width: 200
  },
  {
    key: 'createdAt',
    labelKey: 'common.createdAt',
    filterType: 'date',
    sortable: true,
    width: 200,
    formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
  },
  {
    key: 'actions',
    labelKey: 'common.actions',
    width: 220
  }
]

/** ===== COMPUTED ===== */
// Flatten studentName để filter dễ (từ item.student.studentName)
const itemsWithStudentName = computed(() =>
  (store.items || []).map((item: any) => ({
    ...item,
    studentName: resolveStudentName(item)
  }))
)

// Disable nút Xoá khi không chọn dòng nào
const getDisableDelete = computed(() => selectedItems.value.length === 0)

// Tổng số cam kết
const totalCount = computed(() => itemsWithStudentName.value.length)

// Số cam kết đã hoàn thành
const finishedCount = computed(
  () =>
    itemsWithStudentName.value.filter(
      (i: any) => i.outputCommitmentStatus === OutputCommitmentStatus.Finished
    ).length
)

// Lọc theo filter (tương tự divisionList)
const filteredItemsAll = computed(() => {
  let arr = itemsWithStudentName.value

  Object.entries(filter.value).forEach(([key, val]) => {
    if (val == null || val === '') return

    if (key === 'createdAt') {
      arr = arr.filter((item: any) => {
        if (!item[key]) return false
        const dateOnly = String(item[key]).substring(0, 10)
        return dateOnly === val
      })
    } else if (key === 'outputCommitmentStatus') {
      arr = arr.filter(
        (item: any) => String(item.outputCommitmentStatus) === String(val)
      )
    } else {
      arr = arr.filter((item: any) =>
        String(item[key] ?? '')
          .toLowerCase()
          .includes(String(val).toLowerCase())
      )
    }
  })

  return arr
})

/** ===== FUNCTION HANDLER ===== */
function statusText(v: number) {
  if (v === OutputCommitmentStatus.NotFinished) return t('outputCommitment.status.notFinished')
  if (v === OutputCommitmentStatus.Finished) return t('outputCommitment.status.finished')
  return String(v)
}

function resolveStudentName(item: any) {
  if (item.student?.studentName) return item.student.studentName
  const byId = studentStore.students.find(s => s.id === item.studentId)
  if (byId) return byId.fullName || byId.studentCode || ''
  const byCode = studentStore.students.find(s => s.studentCode === item.studentCode)
  return byCode?.fullName || ''
}

function onTableFilter(val: Record<string, any>) {
  filter.value = val
  page.value = 1
}

function onPageSizeChange(newSize: number) {
  pageSize.value = newSize
  page.value = 1
}

function addItem() {
  store.select(null)
  dialogMode.value = 'create'
  showDialog.value = true
}

function editItem(item: any) {
  store.select({ ...item })
  dialogMode.value = 'edit'
  showDialog.value = true
}

function viewItem(item: any) {
  store.select({ ...item })
  dialogMode.value = 'view'
  showDialog.value = true
}

async function onDeleteClicked() {
  if (!selectedItems.value.length) {
    notificationStore.showToast('warning', {
      key: 'toast.noSelected',
      params: { model: t('models.outputCommitment') }
    })
    return
  }
  handleDelete(selectedItems.value)
}

function handleDelete(items: any[] | any) {
  const list = Array.isArray(items) ? items : [items]
  const ids = list
    .filter((i: any) => typeof i.id === 'string' && i.id)
    .map((i: any) => i.id as string)

  if (!ids.length) return

  notificationStore.showConfirm(
    {
      key: 'toast.delete',
      params: { model: t('models.outputCommitment') }
    },
    async () => {
      startLoading()
      try {
        await store.delete(ids)
        await store.fetchAll()
        showDialog.value = false
      } catch (err: any) {
        console.error('Error deleting:', err)
      } finally {
        stopLoading()
      }
    }
  )
}

async function handleSave(data: any) {
  startLoading()
  try {
    await store.save(data)
    if (data.id) {
      notificationStore.showToast('success', {
        key: 'toast.updateSuccess',
        params: { model: t('models.outputCommitment') }
      })
    } else {
      notificationStore.showToast('success', {
        key: 'toast.createSuccess',
        params: { model: t('models.outputCommitment') }
      })
    }
    await store.fetchAll()
    showDialog.value = false
  } catch (err: any) {
    console.error('Error saving:', err?.response?.data?.errors || err)
  } finally {
    stopLoading()
  }
}

/** ===== LIFECYCLE ===== */
onMounted(() => {
  // Khởi tạo header cho FilterComponent giống divisionList
  filterComponentRef.value?.initListHeaderParams(listHeaderParams)
  studentStore.fetchAllStudents?.()
  store.fetchAll()
})
</script>
