<template>
  <div>
    <!-- FilterComponent -->
    <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
      headerTitle="partnerType.headerTitle" headerDesc="partnerType.headerDesc" :disabledDelete="getDisableDelete"
      class="mb-6" />

    <!-- BẢNG DỮ LIỆU -->
    <div class="card mb-10 w-100 mt-5">
      <div class="card-header card-header-stretch">
        <div class="card-title d-flex flex-column">
          <h3 class="fw-bold fs-4 mb-1">{{ t('partnerType.listTitle') }}</h3>
          <span class="text-body-secondary fw-light fs-8">{{ t('partnerType.listFunction') }}</span>
        </div>
      </div>
      <div class="card-body py-6 px-2 px-md-6">
        <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredPartnerTypesAll"
          :loading="partnerTypeStore.loading" :showPagination="true" :page="page"
          :total="filteredPartnerTypesAll.length" :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
          @update:rows="val => selectedRowsData = val" @edit="editModelEvent" @view="viewModelEvent"
          :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true" @delete="handleDelete"
          @update:page="val => page = val" @update:pageSize="onPageSizeChange">
          <template #cell-status="{ item }">
            <BaseBadge type="boolean" :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
          </template>
          <template #cell-partnerTypeCode="{ item }">
            <BaseBadge :label="item.partnerTypeCode" color="blue" soft :bold="true" bordered />
          </template>
        </BaseTable>
      </div>
    </div>

    <PartnerTypeDialog v-model:visible="showFormModal" :mode="dialogMode"
      :partner-type-data="partnerTypeStore.selectedPartnerType" :loading="formLoading" @submit="handleSave"
      @delete="handleDelete" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { usePartnerTypeStore } from '@/stores/partnerTypeStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import type { PartnerTypeModel } from '@/api/PartnerTypeApi';
import PartnerTypeDialog from '@/views/partner-type/PartnerTypeDialog.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { formatDate } from '@/utils/format';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';

const { t } = useI18n();
const partnerTypeStore = usePartnerTypeStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(30);
const filter = ref({});
const selectedRowsData = ref<Array<PartnerTypeModel>>([]);

const listHeaderParams = {
  listParams: [],
  listBtn: [
    { event: 'add', label: 'partnerType.add', type: 'add' },
    { event: 'delete', label: 'partnerType.delete', type: 'delete' },
  ],
};

const columns: BaseTableColumn[] = [
  { key: 'partnerTypeName', labelKey: 'partnerType.name', filterType: 'text', sortable: true, sticky: true, width: 200, isBold: true },
  { key: 'partnerTypeCode', labelKey: 'partnerType.code', filterType: 'text', sortable: true, sticky: true, width: 160, isBold: true, align: 'center' },
  { key: 'description', labelKey: 'partnerType.description', filterType: 'text', sortable: false },
  {
    key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
    filterOptions: [
      { label: 'common.all', value: '' },
      { label: 'common.active', value: 0 },
      { label: 'common.inactive', value: 1 },
    ]
  },
  { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 250 },
  {
    key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 250,
    formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
  },
  { key: 'actions', labelKey: 'common.actions', width: 220 },
];

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

const filteredPartnerTypesAll = computed(() => {
  let arr = partnerTypeStore.partnerTypes;
  Object.entries(filter.value).forEach(([key, val]) => {
    if (val != null && val !== '') {
      if (key === 'createdAt') {
        arr = arr.filter(item => {
          if (!item[key]) return false;
          const dateOnly = String(item[key]).substring(0, 10);
          return dateOnly === val;
        });
      } else {
        arr = arr.filter(item =>
          String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
        );
      }
    }
  });
  return arr;
});

function onDeleteClicked() {
  if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
    notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.partnerType') } });
    return;
  }
  handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
  filter.value = val;
  page.value = 1;
}

function handleDelete(items: PartnerTypeModel | PartnerTypeModel[]) {
  const list = Array.isArray(items) ? items : [items];
  const ids = list
    .filter(item => typeof item.id === 'string' && item.id)
    .map(item => item.id as string);

  notificationStore.showConfirm(
    {
      key: 'toast.delete',
      params: { model: t('models.partnerType') },
    },
    async () => {
      startLoading()
      try {
        await partnerTypeStore.deletePartnerTypes(ids);
        await partnerTypeStore.fetchAllPartnerTypes();
        stopLoading()
        showFormModal.value = false;
      } catch (err: any) {
        console.error('Error deleting:', err);
        stopLoading()
      }
    }
  );
}

async function addModelEvent() {
  dialogMode.value = 'create';
  await commonStore.generateCode('PT', 'PartnerType', 'PartnerTypeCode', 4);
  partnerTypeStore.selectPartnerType(null);
  showFormModal.value = true;
}

function editModelEvent(model: any) {
  dialogMode.value = 'edit';
  partnerTypeStore.selectPartnerType({ ...model });
  showFormModal.value = true;
}

function viewModelEvent(model: any) {
  dialogMode.value = 'view';
  partnerTypeStore.selectPartnerType({ ...model });
  showFormModal.value = true;
}

async function handleSave(model: any) {
  startLoading()
  try {
    await partnerTypeStore.savePartnerType(model);
    if (model.id) {
      notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.partnerType') } });
    } else {
      notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.partnerType') } });
    }
    await partnerTypeStore.fetchAllPartnerTypes();
    stopLoading()
    showFormModal.value = false;
  } catch (err: any) {
    console.error('Error saving:', err?.response?.data?.errors || err);
    stopLoading()
  }
}

function onPageSizeChange(newSize: number) {
  pageSize.value = newSize;
  page.value = 1;
}

onMounted(() => {
  filterComponentRef.value?.initListHeaderParams(listHeaderParams);
  partnerTypeStore.fetchAllPartnerTypes();
});
</script>
