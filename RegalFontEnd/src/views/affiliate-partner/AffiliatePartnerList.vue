<template>
  <div>
    <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
      headerTitle="affiliatePartner.headerTitle" headerDesc="affiliatePartner.headerDesc"
      :disabledDelete="getDisableDelete" class="mb-6" />

    <div class="card mb-10 w-100 mt-5">
      <div class="card-header card-header-stretch">
        <div class="card-title d-flex flex-column">
          <h3 class="fw-bold fs-4 mb-1">{{ t('affiliatePartner.listTitle') }}</h3>
          <span class="text-body-secondary fw-light fs-8">{{ t('affiliatePartner.listFunction') }}</span>
        </div>
      </div>

      <div class="card-body py-6 px-2 px-md-6">
        <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredAffiliatePartnersAll"
          :loading="formLoading" :showPagination="true" :page="page" :total="filteredAffiliatePartnersAll.length"
          :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
          @update:rows="val => selectedRowsData = val" @edit="editModelEvent" @view="viewModelEvent"
          :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true" @delete="handleDelete"
          @update:page="val => page = val" @update:pageSize="onPageSizeChange" />
      </div>
    </div>

    <AffiliatePartnerDialog v-model:visible="showFormModal" :mode="dialogMode"
      :affiliate-partner-data="affiliatePartnerStore.selectedAffiliatePartner" :loading="formLoading"
      @submit="handleSave" @delete="handleDelete" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useAffiliatePartnerStore } from '@/stores/affiliatePartnerStore';
import { usePartnerTypeStore } from '@/stores/partnerTypeStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import type { AffiliatePartnerModel } from '@/api/AffiliatePartnerApi';
import AffiliatePartnerDialog from '@/views/affiliate-partner/AffiliatePartnerDialog.vue';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { useCommonStore } from '@/stores/commonStore';
import { formatDate } from '@/utils/format';
const commonStore = useCommonStore();
const { t } = useI18n();
const affiliatePartnerStore = useAffiliatePartnerStore();
const partnerTypeStore = usePartnerTypeStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(30);
const filter = ref<Record<string, any>>({});
const selectedRowsData = ref<Array<AffiliatePartnerModel>>([]);
const provinceFilterOptions = ref<{ label: string; value: any }[]>([]);
// Lấy danh sách tỉnh/thành từ commonStore để làm filter
watch(
  () => commonStore.provinces,
  (provinces) => {
    provinceFilterOptions.value = [
      { label: ('common.all'), value: '' },
      ...provinces.map(province => ({
        label: province.provinceName,
        value: province.provinceName,
        isLocale: false,
      }))
      ,
      {
        label: t('common.none'),
        value: t('common.none'),
        isLocale: false,
      }
    ];
  },
  { immediate: true }
);
const partnerTypeOptions = computed(() => [
  { label: 'common.all', value: '' },
  ...partnerTypeStore.partnerTypes.map(pt => ({
    label: pt.partnerTypeName,
    value: pt.partnerTypeName,
    isLocale: false,
  }))
]);

const columns = computed<BaseTableColumn[]>(() => [
  { key: 'partnerName', labelKey: 'affiliatePartner.name', filterType: 'text', sortable: true },
  { key: 'partnerCode', labelKey: 'affiliatePartner.code', filterType: 'text', sortable: true },
  // Hiển thị tên loại đối tác từ object partnerType (nếu đã include từ API)
  {
    key: 'partnerTypeName',
    labelKey: 'affiliatePartner.partnerType',
    width: 160, sortable: true, align: 'center', filterType: 'select', filterOptions: partnerTypeOptions.value
  },
  { key: 'contactPerson', labelKey: 'affiliatePartner.contactPerson', filterType: 'text', sortable: true },
  { key: 'phone', labelKey: 'affiliatePartner.phone', filterType: 'text' },
  { key: 'provinceName', labelKey: 'company.provinceCode', width: 160, sortable: true, align: 'center', filterType: 'select', filterOptions: provinceFilterOptions.value },
  { key: 'position', labelKey: 'affiliatePartner.position', filterType: 'text' },
  {
    key: 'publish',
    labelKey: 'affiliatePartner.isPublish',
    filterType: 'select',
    width: 140,
    filterOptions: [
      { label: 'common.all', value: '' },
      { label: 'common.yes', value: t('common.yes') },
      { label: 'common.no', value: t('common.no') }
    ]

  },
  { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text' },
  {
    key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 200,
    formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
  },
  { key: 'actions', labelKey: 'common.actions', width: 220 },
]);

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);
// Tính số phòng ban cho từng division
const affiliatePartnersWithCount = computed(() =>
  affiliatePartnerStore.affiliatePartners.map(item => ({
    ...item,
    partnerTypeName: item.partnerType?.partnerTypeName || '',
    provinceName: commonStore.provinces.find(p => p.provinceCode === item.province)?.provinceName ?? t('common.none'),
    publish: item.isPublish ? t('common.yes') : t('common.no'),
  }))
);

// Lọc data theo filter
const filteredAffiliatePartnersAll = computed(() => {
  let arr = affiliatePartnersWithCount.value;
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
    notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.affiliatePartner') } });
    return;
  }
  handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
  filter.value = val;
  page.value = 1;
}

function handleDelete(items: AffiliatePartnerModel | AffiliatePartnerModel[]) {
  const list = Array.isArray(items) ? items : [items];
  const ids = list.map(item => item.id).filter(Boolean);
  notificationStore.showConfirm(
    { key: 'toast.delete', params: { model: t('models.affiliatePartner') } },
    async () => {
      startLoading();
      try {
        await affiliatePartnerStore.deleteAffiliatePartners(ids as string[]);
        await affiliatePartnerStore.fetchAllAffiliatePartners();
        stopLoading();
        showFormModal.value = false;
      } catch (err: any) {
        console.error('Error deleting:', err);
        stopLoading();
      }
    }
  );
}

async function addModelEvent() {
  dialogMode.value = 'create';
  await commonStore.generateCode('AP', 'AffiliatePartner', 'PartnerCode', 4);
  affiliatePartnerStore.selectAffiliatePartner(null);
  showFormModal.value = true;
}

function editModelEvent(model: any) {
  dialogMode.value = 'edit';
  affiliatePartnerStore.selectAffiliatePartner({ ...model });
  showFormModal.value = true;
}

function viewModelEvent(model: any) {
  dialogMode.value = 'view';
  affiliatePartnerStore.selectAffiliatePartner({ ...model });
  showFormModal.value = true;
}

async function handleSave(model: any) {
  startLoading();
  try {
    await affiliatePartnerStore.saveAffiliatePartner(model);
    await affiliatePartnerStore.fetchAllAffiliatePartners();
    notificationStore.showToast('success', {
      key: model.id ? 'toast.updateSuccess' : 'toast.createSuccess',
      params: { model: t('models.affiliatePartner') }
    });
    stopLoading();
    showFormModal.value = false;
  } catch (err: any) {
    console.error('Error saving:', err?.response?.data?.errors || err);
    stopLoading();
  }
}

function onPageSizeChange(newSize: number) {
  pageSize.value = newSize;
  page.value = 1;
}

onMounted(async () => {
  filterComponentRef.value?.initListHeaderParams({
    listParams: [],
    listBtn: [
      { event: 'add', label: 'affiliatePartner.add', type: 'add' },
      { event: 'delete', label: 'affiliatePartner.delete', type: 'delete' }
    ]
  });

  // tải dữ liệu
  await commonStore.fetchProvinces();
  await partnerTypeStore.fetchAllPartnerTypes();
  await affiliatePartnerStore.fetchAllAffiliatePartners();
});
</script>
