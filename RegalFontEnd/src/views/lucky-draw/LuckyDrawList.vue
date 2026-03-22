<template>
  <div>
    <FilterComponent ref="filterComponentRef" @add="onAddClicked" @delete="onDeleteClicked"
      headerTitle="luckyDraw.pageTitle" headerDesc="luckyDraw.pageDesc" :disabledDelete="selectedRowsData.length === 0"
      class="mb-6" />
    <div class="row g-4 mb-6">
      <div class="col-md-4">
        <div class="border rounded p-4 h-100">
          <div class="text-muted fs-7">
            {{ t("luckyDraw.totalPrograms") }}
          </div>
          <div class="fw-bold fs-2 mt-2">{{ store.total }}</div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="border rounded p-4 h-100">
          <div class="text-muted fs-7">
            {{ t("luckyDraw.activePrograms") }}
          </div>
          <div class="fw-bold fs-2 mt-2">{{ activeCount }}</div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="border rounded p-4 h-100">
          <div class="text-muted fs-7">
            {{ t("luckyDraw.completedPrograms") }}
          </div>
          <div class="fw-bold fs-2 mt-2">{{ completedCount }}</div>
        </div>
      </div>
    </div>
    <div class="card mb-10 w-100 mt-5">
      <div class="card-header card-header-stretch">
        <div class="card-title d-flex flex-column">
          <h3 class="fw-bold fs-4 mb-1">{{ t('luckyDraw.listTitle') }}</h3>
          <span class="text-body-secondary fw-light fs-8">{{ t('luckyDraw.listDesc') }}</span>
        </div>
      </div>
      <div class="card-body pt-0">
        <BaseTable :columns="columns" :items="store.luckyDraws" :loading="store.loading" :showPagination="true"
          :page="page" :pageSize="pageSize" :total="store.luckyDraws.length" :showCheckboxColumn="true"
          :showActionsColumn="true" :showEdit="true" :showView="true" :showDelete="false" @update:page="onPageChange"
          @update:pageSize="onPageSizeChange" @update:rows="val => selectedRowsData = val" @edit="editModelLuckyDraw"
          @view="viewModelLuckyDraw">
          <template #cell-reportDate="{ item }">
            {{ formatDate(item.reportDate) }}
          </template>

          <template #cell-status="{ item }">
            <BaseBadge :label="item.status === 1 ? t('common.active') : t('common.inactive')" :rawLabel="true"
              :color="item.status === 1 ? 'success' : 'warning'" />
          </template>
        </BaseTable>
      </div>
    </div>
    <LuckyDrawDialog v-model:visible="showFormModal" :mode="dialogMode" :lucky-draw-data="store.selectedLuckyDraw"
      :loading="false" @submit="handleSave" @delete="handleDelete" />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useLuckyDrawStore } from "@/stores/luckyDrawStore";
import BaseTable, { type BaseTableColumn } from "@/components/table/BaseTable.vue";
import BaseBadge from "@/components/info-badge/BaseBadge.vue";
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import LuckyDrawDialog from './LuckyDrawDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';

const { t } = useI18n();
const store = useLuckyDrawStore();
const notificationStore = useNotificationStore();
const page = ref(1);
const pageSize = ref(30);
const filterComponentRef = ref();
const showFormModal = ref(false);
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const selectedRowsData = ref([]);

const activeCount = computed(() => (store.luckyDraws || []).filter((x) => x.status === 1).length);
const completedCount = computed(() => (store.luckyDraws || []).filter((x) => x.status === 0).length);

const columns: BaseTableColumn[] = [
  { key: "name", labelKey: "luckyDraw.name", filterType: "text", sortable: true, isBold: true },
  { key: "branch", labelKey: "luckyDraw.branch", filterType: "text", sortable: true },
  { key: "region", labelKey: "luckyDraw.region", filterType: "text", sortable: true },
  { key: "reportDate", labelKey: "luckyDraw.reportDate", filterType: "date", sortable: true },
  { key: "reporter", labelKey: "luckyDraw.reporter", filterType: "text", sortable: true },
  {
    key: "status", labelKey: "common.status", filterType: "select", sortable: true,
    filterOptions: [
      { label: "common.all", value: "" },
      { label: "common.active", value: 1 },
      { label: "common.inactive", value: 0 }
    ]
  },
];

function formatDate(value?: string) {
  if (!value) return "-";
  return value.split("T")[0];
}


onMounted(() => {
  filterComponentRef.value?.initListHeaderParams?.({
    listParams: [],
    listBtn: [
      { event: 'add', label: 'luckyDraw.addTitle', type: 'add' },
      { event: 'delete', label: 'luckyDraw.delete', type: 'delete' },
    ],
  });

  store.fetchPagedLuckyDraws();
});

function onPageChange(val: number) {
  page.value = val;
}
function onPageSizeChange(val: number) {
  pageSize.value = val;
  page.value = 1;
}

function onTableFilter(val: Record<string, any>) {
  // placeholder for table filter integration
}

function onAddClicked() {
  dialogMode.value = 'create';
  store.selectLuckyDraw?.(null);
  showFormModal.value = true;
}

function onDeleteClicked() {
  if (selectedRowsData.value.length === 0) {
    notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.luckyDraw') } });
    return;
  }
  handleDelete(selectedRowsData.value);
}

async function handleDelete(items: any) {
  const ids = (Array.isArray(items) ? items : [items]).map((x: any) => x.id);
  notificationStore.showConfirm({ key: 'toast.delete', params: { model: t('models.luckyDraw') } }, async () => {
    try {
      // store.deleteLuckyDraw accepts a single id; delete sequentially
      for (const id of ids) {
        await store.deleteLuckyDraw(String(id));
      }
      await store.fetchPagedLuckyDraws();
      showFormModal.value = false;
      notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.luckyDraw') } });
    } catch (err) {
      notificationStore.showToast('error', { key: 'toast.deleteError', params: { model: t('models.luckyDraw') } });
    }
  });
}

function editModelLuckyDraw(item: any) {
  dialogMode.value = 'edit';
  store.selectLuckyDraw?.({ ...item });
  showFormModal.value = true;
}

function viewModelLuckyDraw(item: any) {
  dialogMode.value = 'view';
  store.selectLuckyDraw?.({ ...item });
  showFormModal.value = true;
}

async function handleSave(data: any) {
  try {
    await store.saveLuckyDraw(data);
    notificationStore.showToast('success', { key: data.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.luckyDraw') } });
    await store.fetchPagedLuckyDraws();
    showFormModal.value = false;
  } catch (err) {
    notificationStore.showToast('error', { key: data.id ? 'toast.updateError' : 'toast.createError', params: { model: t('models.luckyDraw') } });
  }
}
</script>
