<template>
  <div class="d-flex flex-wrap flex-stack mb-6">
    <h2>{{ t('applicationUser.listTitle') }}</h2>
    <FilterComponent ref="filterComponentRef" @add="addUser" @delete="onDeleteClicked"
      :disabledDelete="getDisableDelete" />
    <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredUsers" :loading="store.loading"
      :showPagination="true" :filter="filter" @update:filter="onTableFilter"
      @update:rows="val => selectedRowsData = val" @edit="editUser" :page="page" :total="filteredUsers.length"
      @update:page="onPageChange" :pageSize="pageSize" @update:pageSize="onPageSizeChange" :showActionsColumn="true"
      :showEdit="true" :showDelete="true" @delete="handleDelete">
      <!-- <template #cell-email="{ item }">
        <InfoBadge type="email" :value="item.email" :showFullValue="true" />
      </template>
<template #cell-isDeleted="{ item }">
        <InfoBadge type="boolean" :value="String(item.isDeleted)" />
      </template> -->
    </BaseTable>

    <ApplicationUserDialog v-model:visible="showFormModal" :is-edit="!!store.selectedUser?.id" :loading="store.loading"
      :user-data="store.selectedUser" @submit="handleSave" @delete="handleDelete" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useApplicationUserStore } from '@/stores/applicationUserStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import { formatDate } from '@/utils/format';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import type { ApplicationUserModel } from '@/api/ApplicationUserApi';
import ApplicationUserDialog from './ApplicationUserDialog.vue';
const { t } = useI18n();
const store = useApplicationUserStore();
const notificationStore = useNotificationStore();
const showFormModal = ref(false);
const filterComponentRef = ref();

const listHeaderParams = {
  listParams: [/* ... */],
  listBtn: [
    { event: 'add', label: 'Thêm', type: 'add' },
    { event: 'delete', label: 'Xóa', type: 'delete' },
  ]
};

const columns: BaseTableColumn[] = [
  { key: 'fullName', labelKey: 'applicationUser.fullName', filterType: 'text', sortable: true, sticky: true, width: 360 },
  { key: 'userName', labelKey: 'applicationUser.userName', filterType: 'text', sortable: true, sticky: true, width: 220 },
  { key: 'email', labelKey: 'applicationUser.email', filterType: 'text', width: 240, sortable: true },
  { key: 'phoneNumber', labelKey: 'applicationUser.phoneNumber', filterType: 'text', width: 200, sortable: true },
  {
    key: 'isDeleted',
    labelKey: 'applicationUser.isDeleted',
    filterType: 'select',
    filterOptions: [
      { label: 'common.all', value: '' },
      { label: 'common.deleted', value: true },
      { label: 'common.notDeleted', value: false }
    ],
    align: 'center',
  },
  { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY') },
  { key: 'actions', labelKey: 'common.actions', width: 320 }
];
const selectedRowsData = ref<Array<ApplicationUserModel>>([]);

const page = ref(1);
const pageSize = ref(30);
const filter = ref({});

const filteredUsers = computed(() => {
  let arr = store.users;
  Object.entries(filter.value).forEach(([key, val]) => {
    if (val != null && val !== '') {
      if (key === 'createdAt') {
        arr = arr.filter(item => {
          if (!item[key]) return false;
          const dateOnly = String(item[key]).substring(0, 10);
          return dateOnly === val;
        });
      } else if (key === 'isDeleted') {
        if (val === true || val === false) arr = arr.filter(item => item.isDeleted === val);
      } else {
        arr = arr.filter(item => String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase()));
      }
    }
  });
  return arr;
});

const pagedUsers = computed(() => {
  const start = (page.value - 1) * pageSize.value;
  return filteredUsers.value.slice(start, start + pageSize.value);
});

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

onMounted(() => {
  filterComponentRef.value?.initListHeaderParams(listHeaderParams);
  store.fetchAllApplicationUsers();
});

function onDeleteClicked() {
  if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
    notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.user') } });
    return;
  }
  handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
  filter.value = val;
  page.value = 1;
}

function handleDelete(users: ApplicationUserModel | ApplicationUserModel[]) {
  const userList = Array.isArray(users) ? users : [users];
  const ids = userList
    .filter(user => typeof user.id === 'string' && user.id)
    .map(user => user.id as string);

  notificationStore.showConfirm(
    {
      key: 'toast.delete',
      params: { model: t('models.user') }
    },
    async () => {
      try {
        await store.deleteApplicationUser(ids);
        notificationStore.showToast('success', {
          key: 'toast.deleteSuccess',
          params: { model: t('models.user') }
        });
        store.fetchAllApplicationUsers();
      } catch (err: any) {
        console.error('Error deleting user(s):', err);
      }
    }
  );
}

function addUser() {
  store.selectUser(null);
  showFormModal.value = true;
}

function editUser(user: any) {
  store.selectUser({ ...user });
  showFormModal.value = true;
}

async function handleSave(user: any) {
  try {
    await store.saveApplicationUser(user);
    showFormModal.value = false;
    if (user.id) {
      notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.user') } });
    } else {
      notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.user') } });
    }
    store.fetchAllApplicationUsers();
  } catch (err: any) {
    console.error('Error saving:', err?.response?.data?.errors || err);
  }
}
// Pagination handlers
function onPageChange(newPage: number) {
  page.value = newPage;
}

function onPageSizeChange(newSize: number) {
  pageSize.value = newSize;
  page.value = 1; // reset về trang 1
}
</script>

<style>
.badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 12px;
  color: #fff;
}

.badge-gmail {
  background-color: #db4437;
}

.badge-example {
  background-color: #f4b400;
}

.badge-regal {
  background-color: #0f9d58;
}

.badge-default {
  background-color: #ccc;
}
</style>