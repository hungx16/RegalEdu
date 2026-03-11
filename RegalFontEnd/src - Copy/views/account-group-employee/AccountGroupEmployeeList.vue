<template>
  <div class="d-flex flex-wrap flex-stack mb-6">
    <h2>{{ t('accountGroupEmployee.listTitle') }}</h2>
    <button class="btn btn-success mb-3" @click="addEmployee">
      {{ t('accountGroupEmployee.addButton') }}
    </button>

    <BaseTable :columns="columns" :items="store.accountGroupEmployees" :loading="store.loading" @edit="editEmployee">
      <template #actions="{ item }">
        <button class="btn btn-primary btn-sm" @click="editEmployee(item)">
          {{ t('common.edit') }}
        </button>
        <button class="btn btn-danger btn-sm ms-2" @click="handleDelete(item)">
          {{ t('common.delete') }}
        </button>
      </template>
    </BaseTable>

    <AccountGroupEmployeeFormModal v-model:show="showFormModal" :employee="store.selectedEmployee" :groupId="groupId"
      @saved="handleSave" @deleted="handleDelete" :key="store.selectedEmployee?.id || 'new'" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useAccountGroupEmployeeStore } from '../../stores/accountGroupEmployeeStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import AccountGroupEmployeeFormModal from './AccountGroupEmployeeFormModal.vue';

const { t } = useI18n();
const store = useAccountGroupEmployeeStore();
const notificationStore = useNotificationStore();
const showFormModal = ref(false);

// ID nhóm hiện tại - có thể là prop hoặc hardcode test
const groupId = '00000000-0000-0000-0000-000000000000';

const columns: BaseTableColumn[] = [
  { key: 'userCode', labelKey: 'accountGroupEmployee.userCode', width: 200 }
];

onMounted(() => {
  store.fetchEmployees(groupId);
});

function addEmployee() {
  store.selectEmployee(null);
  showFormModal.value = true;
}

function editEmployee(employee: any) {
  store.selectEmployee({ ...employee });
  showFormModal.value = true;
}

async function handleSave(employee: any) {
  try {
    await store.saveEmployee(employee);
    showFormModal.value = false;
    notificationStore.showToast('success', {
      key: employee.id ? 'toast.updateSuccess' : 'toast.createSuccess',
      params: { model: t('models.accountGroupEmployee') }
    });
    store.fetchEmployees(groupId);
  } catch (err: any) {
    console.error('Save error:', err);
  }
}

function handleDelete(employee: any) {
  notificationStore.showConfirm(
    { key: 'toast.delete', params: { model: t('models.accountGroupEmployee') } },
    async () => {
      try {
        await store.deleteEmployee(employee.id);
        notificationStore.showToast('success', {
          key: 'toast.deleteSuccess',
          params: { model: t('models.accountGroupEmployee') }
        });
        store.fetchEmployees(groupId);
      } catch (err: any) {
        console.error('Delete error:', err);
      }
    }
  );
}
</script>
