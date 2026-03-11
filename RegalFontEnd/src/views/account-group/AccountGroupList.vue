<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="onAddClicked" @delete="onDeleteClicked"
            headerTitle="accountGroup.headerTitle" headerDesc="accountGroup.headerDesc" @assign="assignGroup"
            :disabledDelete="getDisableDelete" class="mb-6" :disabledAssign="getDisableDelete" />

        <!-- ========= SUMMARY CARDS: 2 thẻ thống kê ========= -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('common.totalGroups') }}</span>
                        <i class="bi bi-people fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ store.total }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.group') }}</div>
                </div>
            </div>

            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('accountGroup.group') }}</span>
                        <i class="bi bi-check2-circle fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalEnabled }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
        </div>

        <!-- ========= LIST CARD ========= -->
        <div class="card mb-10 w-100 mt-5">
            <!-- Card Header -->
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('accountGroup.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">
                        {{ t('accountGroup.listFunction') }}
                    </span>
                </div>
            </div>
            <!-- Card Body / Table -->
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="store.groups" :loading="store.loading"
                    searchMode="server" :filter="filter" :showActionsColumn="true" :showEdit="true" :showDelete="false"
                    :showPagination="true" :page="page" :total="store.total" :pageSize="pageSize"
                    @update:rows="val => (selectedRowsData = val)" @update:filter="onTableFilter" @edit="editGroup"
                    @delete="onDeleteClicked" @update:page="val => setPage(val)" @update:pageSize="onPageSizeChange">
                    <template #cell-enable="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.enable === false ? t('common.inactive') : t('common.active')" />
                    </template>

                    <template #cell-useDefault="{ item }">
                        <BaseBadge type="boolean" :label="item.useDefault ? t('common.yes') : t('common.no')" />
                    </template>

                    <template #cell-name="{ item }">
                        <span class="fw-semibold">{{ item.name }}</span>
                    </template>
                </BaseTable>
            </div>
        </div>
        <!-- <button class="btn btn-success mb-3" @click="assignGroup">
            {{ t('accountGroup.assignButton') }}
        </button> -->
        <!-- ========= MODALS ========= -->
        <AccountGroupFormModal v-model:show="showFormModal" :group="store.selectedGroup" @saved="handleSave"
            @deleted="handleDeleteSingle" :key="store.selectedGroup?.id || 'new'" />

        <AssignEmployeeToGroupModal :isOpenAssignDialog="showAssignModal" :selectedRowsData="selectedRowsData"
            :arrEmpNoGroup="accountGroupEmployeeStore.listEmpNoGroup" :arrEmpInSelectedGroup="arrEmpInSelectedGroup"
            :listEmployeeFull="userStore.users" :group="store.selectedGroup" @saved="handleSave"
            @deleted="handleDeleteSingle" :key="store.selectedGroup?.id || 'new'"
            @update:isOpenAssignDialog="val => (showAssignModal = val)" />
    </div>
</template>

<script setup lang="ts">


import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';

import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';

import AccountGroupFormModal from '@/views/account-group/AccountGroupFormModal.vue';
import AssignEmployeeToGroupModal from './AssignEmployeeToGroupModal.vue';

import { formatDate } from '@/utils/format';

import { useAccountGroupStore } from '@/stores/accountGroupStore';
import { useApplicationUserStore } from '@/stores/applicationUserStore';
import { useAccountGroupEmployeeStore } from '@/stores/accountGroupEmployeeStore';
import { useNotificationStore } from '@/stores/notificationStore';

import type { AccountGroupModel } from '@/api/AccountGroupApi';

const { t } = useI18n();

const store = useAccountGroupStore();
const userStore = useApplicationUserStore();
const accountGroupEmployeeStore = useAccountGroupEmployeeStore();
const notificationStore = useNotificationStore();

/** ===== UI State ===== */
const showFormModal = ref(false);
const showAssignModal = ref(false);
const filterComponentRef = ref();

/** ===== Bảng / phân trang (đồng bộ dạng server) ===== */
const page = ref<number>(1);
const pageSize = ref<number>(30);

/** ===== Data phụ ===== */
const arrEmpInSelectedGroup = ref<Array<string>>([]);
const selectedRowsData = ref<Array<AccountGroupModel>>([]);

/** ===== Filter server ===== */
const filter = computed(() => store.query);

/** ===== Columns (định dạng theo style Division) ===== */
const columns: BaseTableColumn[] = [
    { key: 'name', labelKey: 'accountGroup.name', filterType: 'text', sortable: true, sticky: true, width: 240, isBold: true },
    {
        key: 'enable', labelKey: 'accountGroup.enable', filterType: 'select', width: 200, columnType: 'boolean', align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: true },
            { label: 'common.inactive', value: false },
        ]
    },
    {
        key: 'useDefault', labelKey: 'accountGroup.useDefault', filterType: 'select', width: 220, columnType: 'boolean', align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.yes', value: true },
            { label: 'common.no', value: false },
        ]
    },
    {
        key: 'createdAt',
        labelKey: 'common.createdAt',
        filterType: 'date',
        sortable: true,
        width: 220,
        align: 'center',
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY'),
    },
    { key: 'note', labelKey: 'accountGroup.note', filterType: 'text', sortable: true },
];

/** ===== Summary card: total enabled ===== */
const totalEnabled = computed(() => (store.groups || []).filter(g => g.enable === true).length);

/** ===== Chặn nút xóa ở FilterComponent ===== */
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

/** ===== Life cycle ===== */
onMounted(async () => {
    // Khởi tạo FilterComponent nếu có cấu hình button riêng
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'accountGroup.addButton', type: 'add' },
            { event: 'delete', label: 'accountGroup.delete', type: 'delete' },
            { event: 'assign', label: 'accountGroup.assignGroup', type: 'assign' },
        ],
    });

    await Promise.all([
        store.fetchAccountGroups(),
        userStore.fetchAllApplicationUsers(),
    ]);

    // Map page & size từ store.query (nếu đã có)
    page.value = store.query.page ?? 1;
    pageSize.value = store.query.pageSize ?? 30;
});

/** ===== Filter/Paging (server-mode) ===== */
function onTableFilter(newFilter: Record<string, any>) {
    store.setFilter(newFilter);
    store.setPage(1);
    page.value = 1;
    store.fetchAccountGroups();
}

function setPage(p: number) {
    page.value = p;
    store.setPage(p);
    store.fetchAccountGroups();
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    store.setPageSize(newSize);
    store.setPage(1);
    page.value = 1;
    store.fetchAccountGroups();
}

/** ===== Toolbar actions ===== */
function onAddClicked() {
    addGroup();
}

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', {
            key: 'toast.noSelected',
            params: { model: t('models.accountGroup') },
        });
        return;
    }
    const ids = selectedRowsData.value
        .filter(g => g.id)
        .map(g => g.id as string);

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.accountGroup') } },
        async () => {
            try {
                // Nếu store có delete nhiều: store.deleteAccountGroups(ids)
                // Nếu chỉ xóa từng id: lặp
                for (const id of ids) {
                    await store.deleteAccountGroup(id);
                }
                notificationStore.showToast('success', {
                    key: 'toast.deleteSuccess',
                    params: { model: t('models.accountGroup') },
                });
                await store.fetchAccountGroups();
                selectedRowsData.value = [];
                showFormModal.value = false
            } catch (err: any) {
                console.error('Error deleting:', err);
            }
        }
    );
}

/** ===== Các hàm CRUD cũ (giữ nguyên) ===== */
function addGroup() {
    store.selectGroup(null);
    showFormModal.value = true;
}

async function assignGroup() {
    showAssignModal.value = false;
    if (selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.selectGroup' });
        return;
    }
    await accountGroupEmployeeStore.fetchAccountGroupEmployeeByGroupId(selectedRowsData.value[0].id);
    await accountGroupEmployeeStore.fetchEmployeeNoGroup();
    arrEmpInSelectedGroup.value = accountGroupEmployeeStore.accountGroupEmployees.map(emp => emp.userCode);
    showAssignModal.value = true;
}

function editGroup(group: any) {
    store.selectGroup({ ...group });
    showFormModal.value = true;
}

async function handleSave(group: any) {
    try {
        await store.saveAccountGroup(group);
        showFormModal.value = false;
        notificationStore.showToast('success', {
            key: group.id ? 'toast.updateSuccess' : 'toast.createSuccess',
            params: { model: t('models.accountGroup') },
        });
        await store.fetchAccountGroups();
    } catch (err: any) {
        console.error('Error saving:', err);
    }
}

function handleDeleteSingle(groupId: any) {
    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.accountGroup') } },
        async () => {
            try {
                await store.deleteAccountGroup(groupId);
                notificationStore.showToast('success', {
                    key: 'toast.deleteSuccess',
                    params: { model: t('models.accountGroup') },
                });
                await store.fetchAccountGroups();
            } catch (err: any) {
                console.error('Error deleting:', err);
            }
        }
    );
}
</script>

<style scoped>
/* Nhẹ nhàng theo Metronic 8 */
.bg-dark-1 {
    background: #1e1e2d;
}

.summary-card {
    border: 1px solid rgba(0, 0, 0, 0.06);
}
</style>
