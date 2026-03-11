<template>
  <div>
    <!-- ===== FILTER BAR ===== -->
    <FilterComponent ref="filterComponentRef" headerTitle="accountGroupPermission.headerTitle"
      headerDesc="accountGroupPermission.headerDesc" class="mb-6" @add="onFilterAdd" @delete="onFilterDelete"
      @assign="onFilterAssign" :disabledDelete="false" :disabledAssign="false" />

    <!-- ========= SUMMARY CARDS: 2 thẻ thống kê ========= -->
    <div class="row g-4 mb-8">
      <div class="col-12 col-md-6">
        <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <span class="fw-semibold fs-5">{{ t('accountGroupPermission.totalForms') }}</span>
            <i class="bi bi-ui-checks-grid fs-4 text-body-secondary"></i>
          </div>
          <div class="fs-2 fw-bold mb-1">{{ totalLeafNodes }}</div>
          <div class="fs-7 text-body-secondary">{{ t('accountGroupPermission.forms') }}</div>
        </div>
      </div>

      <div class="col-12 col-md-6">
        <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <span class="fw-semibold fs-5">{{ t('accountGroupPermission.totalAllowed') }}</span>
            <i class="bi bi-check2-circle fs-4 text-body-secondary"></i>
          </div>
          <div class="fs-2 fw-bold mb-1">{{ totalAllowedCount }}</div>
          <div class="fs-7 text-body-secondary">{{ t('accountGroupPermission.allowedActions') }}</div>
        </div>
      </div>
    </div>

    <!-- ========= CARD (TABLE) ========= -->
    <div class="card mb-10 w-100 mt-5">
      <!-- Card Header -->
      <div class="card-header card-header-stretch flex-wrap gap-4">
        <div class="card-title d-flex flex-column">
          <h3 class="fw-bold fs-4 mb-1">{{ t('accountGroupPermission.listTitle') }}</h3>
          <span class="text-body-secondary fw-light fs-8">
            {{ t('accountGroupPermission.listFunction') }}
          </span>
        </div>

        <!-- Chọn nhóm quyền ở bên phải -->
        <div class="ms-auto d-flex align-items-center gap-3">
          <label class="fw-semibold fs-8 text-nowrap m-0">
            {{ t('accountGroupPermission.accountGroup') }}
          </label>
          <el-select class="input-border-bottom" v-model="accountGroupSelected" clearable
            :placeholder="t('accountGroupPermission.chooseAccountGroup')" filterable :no-data-text="t('NoData')"
            @change="accountGroupSelectedChanged" style="min-width: 280px">
            <el-option v-for="item in accountGroupStore.groups" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </div>
      </div>

      <!-- Card Body / Table -->
      <div class="card-body py-6 px-2 px-md-6">
        <el-table :data="listDataForTree" row-key="id" :tree-props="{ children: 'arrChildren' }" border :fit="true"
          :header-cell-style="headerCellStyle" default-expand-all style="width: 100%;">
          <el-table-column :label="t('accountGroupPermission.menuList')" minWidth="300">
            <template #default="{ row }">
              <span :class="{ 'parent-label': row.arrChildren && row.arrChildren.length > 0 }">
                {{ row.label }}
              </span>
            </template>
          </el-table-column>

          <!-- Cột All -->
          <el-table-column prop="all" :label="t('accountGroupPermission.all')" width="90" align="center">
            <template #default="{ row }">
              <template v-if="!row.arrChildren || row.arrChildren.length === 0">
                <el-checkbox v-model="row.all" @change="checkAllChangedRow(row)" />
              </template>
              <template v-else>
                <span>-</span>
              </template>
            </template>
          </el-table-column>

          <!-- Các action (giữ nguyên permissionActions từ code cũ) -->
          <el-table-column v-for="action in permissionActions.filter(a => a !== 'all')" :key="action" :prop="action"
            :label="getActionLabel(action)" width="120" align="center">
            <template #default="{ row }">
              <template v-if="!row.arrChildren || row.arrChildren.length === 0">
                <el-checkbox v-model="row[action]" :disabled="getDisableCheckbox(row, action)"
                  @change="checkedChanged(row.id, action)" />
              </template>
              <template v-else>
                <span>-</span>
              </template>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- Card Footer -->
      <div class="card-footer d-flex flex-wrap gap-3 justify-content-end">
        <el-button @click="onFilterDelete" round>{{ t('accountGroupPermission.reload') }}</el-button>
        <el-button type="primary" @click="onFilterAdd" round>{{ t('accountGroupPermission.save') }}</el-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
/**
 * GIỮ NGUYÊN LOGIC CŨ — CHỈ THÊM NHẸ PHẦN GIAO DIỆN
 */

import { ref, onMounted, onBeforeMount, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useAccountGroupPermissionStore } from '../../stores/accountGroupPermissionStore';
import { useAccountGroupStore } from '@/stores/accountGroupStore';
import MainMenuConfig from '@/layouts/default-layout/config/MainMenuConfig';
import { PermissionAction, type AccountGroupPermissionModel, type AccountGroupPermissionRequestModel } from '@/api/accountGroupPermissionApi';
import { useNotificationStore } from '@/stores/notificationStore';

/* ====== (MỚI) FilterComponent để reuse layout ====== */
import FilterComponent from '@/components/filter-component/FilterComponent.vue';

interface TreeData {
  id: string;
  label: string;
  all: boolean;
  view: boolean;
  add: boolean;
  edit: boolean;
  delete: boolean;
  // approval: boolean;
  // book: boolean;
  // buy: boolean;
  // accept: boolean;
  arrChildren: Array<TreeData>;
  isChildItem: boolean;
}
const headerCellStyle = { fontWeight: '900' };

const { t } = useI18n();
const store = useAccountGroupPermissionStore();
const accountGroupStore = useAccountGroupStore();
const accountGroupSelected = ref<string | null>(null);
const listDataForTree = ref<TreeData[]>([]);
const permissionActions = ['all', ...Object.values(PermissionAction)];
const notificationStore = useNotificationStore();
const actionI18nKeyMap: Record<string, string> = {
  view: 'permission.view',
  add: 'permission.add',
  edit: 'permission.edit',
  delete: 'permission.delete',
};

// Lấy nhãn theo i18n; fallback: viết hoa chữ cái đầu
function getActionLabel(action: string) {
  const key = actionI18nKeyMap[action];
  return key ? t(key) : action.charAt(0).toUpperCase() + action.slice(1);
}
/* ====== GIỮ NGUYÊN: build tree trước khi mount ====== */
onBeforeMount(() => {
  listDataForTree.value = convertMenuToTreeData(MainMenuConfig)
});

/* ====== GIỮ NGUYÊN ====== */
onMounted(() => {
  // Khởi tạo FilterComponent nếu có cấu hình button riêng
  filterComponentRef.value?.initListHeaderParams({
    listParams: [],
    listBtn: [

    ],
  });
  loadAllAccountGroup();
});

async function loadAllAccountGroup() {
  await accountGroupStore.fetchAllAccountGroups();
}

/* ====== GIỮ NGUYÊN ====== */
function checkAllChangedRow(item: TreeData) {
  const actions = permissionActions.filter(a => a !== 'all');
  actions.forEach(action => {
    // @ts-ignore
    item[action] = item.all;
  });
}

/* ====== GIỮ NGUYÊN ====== */
function getDisableCheckbox(_data: TreeData, _action: string) {
  let disable = false;
  return disable;
}

/* ====== GIỮ NGUYÊN ====== */
async function save() {
  if (!accountGroupSelected.value) {
    notificationStore.showToast('warning', { key: 'accountGroupPermission.selectAccountGroup' });
    return;
  }

  const arrGroupPermission: Array<AccountGroupPermissionModel> = [];

  // ✅ Gom quyền cho TẤT CẢ LÁ (mọi cấp)
  traverseLeaves(listDataForTree.value, (leaf) => {
    Object.values(PermissionAction).forEach((action) => {
      // @ts-ignore
      arrGroupPermission.push(createAccountGroupPermissionModel(leaf.id, action, leaf[action]));
    });
  });

  const requestModel: AccountGroupPermissionRequestModel = {
    accountGroupId: accountGroupSelected.value,
    listGroupPermission: arrGroupPermission,
  };
  await store.saveAccountGroupPermission(requestModel);
  notificationStore.showToast('success', { key: 'toast.saveSuccess', params: { model: t('models.accountGroupPermission') } });
  location.reload();
}


/* ====== GIỮ NGUYÊN ====== */
function checkedChanged(id: string, _checkboxType: string) {
  let data: TreeData | undefined = findTreeNodeById(listDataForTree.value, id);
  if (data) {
    changeCheckboxAll(data);
  }
}

/* ====== GIỮ NGUYÊN ====== */
function changeCheckboxAll(data: TreeData) {
  const arr = permissionActions.filter(a => a !== 'all');
  // @ts-ignore
  data.all = arr.every(action => data[action]);
}

/* ====== GIỮ NGUYÊN (load quyền theo nhóm) ====== */
async function accountGroupSelectedChanged(val: any) {
  if (val == null || val === "") return;

  await store.getAccountGroupPermissionByAccountGroupId(val);
  const arrGroupPermission: Array<AccountGroupPermissionModel> = store.permissions || [];

  // Tạo map: formName -> { action(lower): allow }
  const map = new Map<string, Record<string, boolean>>();
  for (const p of arrGroupPermission) {
    const actionKey = (p.action as string).toLowerCase();
    if (!map.has(p.formName)) map.set(p.formName, {});
    map.get(p.formName)![actionKey] = !!p.allowAction;
  }

  // ✅ Áp quyền cho TẤT CẢ LÁ (mọi cấp)
  traverseLeaves(listDataForTree.value, (leaf) => {
    const byForm = map.get(leaf.id);
    leaf.all = false;
    Object.values(PermissionAction).forEach((action) => {
      const key = (action as string).toLowerCase();
      // @ts-ignore
      leaf[action] = byForm ? !!byForm[key] : false;
    });
    // Đồng bộ lại “All”
    changeCheckboxAll(leaf);
  });
}

/* ====== GIỮ NGUYÊN ====== */
function getPermissionByAction(arrGroupPermissionByForm: AccountGroupPermissionModel[], action: string): boolean {
  let result = false;
  const permissionData = arrGroupPermissionByForm.find(x => x.action.toLowerCase() == action);
  if (permissionData != undefined && permissionData.allowAction == true) {
    result = true;
  }
  return result;
}

/* ====== GIỮ NGUYÊN ====== */
function findTreeNodeById(tree: TreeData[], id: string): TreeData | undefined {
  for (const node of tree) {
    if (node.id === id) return node;
    if (node.arrChildren && node.arrChildren.length > 0) {
      const found = findTreeNodeById(node.arrChildren, id);
      if (found) return found;
    }
  }
  return undefined;
}

/* ====== GIỮ NGUYÊN ====== */
function convertMenuToTreeData(menuConfig: any[], isChildItem = false): TreeData[] {
  const result: TreeData[] = [];

  for (const item of menuConfig) {
    const hasWrapperOnly = !item.heading && !item.sectionTitle && Array.isArray(item.pages);
    if (hasWrapperOnly) {
      // Wrapper không có tiêu đề -> dàn phẳng children
      result.push(...convertMenuToTreeData(item.pages, false));
      continue;
    }

    // Tạo node nếu có heading/sectionTitle/route
    if (item.heading || item.sectionTitle || item.route || item.label || item.labelKey) {
      const node = createTreeData(item, isChildItem);

      // Gom children từ cả pages và sub (nếu có)
      const children: any[] = [];
      if (Array.isArray(item.pages) && item.pages.length > 0) children.push(...item.pages);
      if (Array.isArray(item.sub) && item.sub.length > 0) children.push(...item.sub);

      if (children.length > 0) {
        node.arrChildren = convertMenuToTreeData(children, true);
      }

      result.push(node);
    }
  }

  return result;
}


/* ====== GIỮ NGUYÊN ====== */
function createTreeData(item: any, isChildItem = false): TreeData {
  // Ưu tiên id từ route; fallback heading/sectionTitle để ổn định
  const id: string = item.route || item.heading || item.sectionTitle || Math.random().toString();

  // Ưu tiên labelKey -> heading -> sectionTitle -> label -> route
  const rawLabel = item.labelKey || item.heading || item.sectionTitle || item.label || item.route || '';
  const label = t(rawLabel);

  return {
    id,
    label,
    all: false,
    view: false,
    add: false,
    edit: false,
    delete: false,
    arrChildren: [],
    isChildItem,
  };
}

/* ====== GIỮ NGUYÊN ====== */
function createAccountGroupPermissionModel(formName: string, action: string, allow: boolean) {
  const data: AccountGroupPermissionModel = {
    formName: formName,
    action: action,
    allowAction: allow
  };
  return data;
}

/* ========================================================= */
/* ============ (MỚI) CHỈ CHO UI / KHÔNG ĐỔI LOGIC ========= */
/* ========================================================= */

const filterComponentRef = ref();

/** Map sự kiện FilterComponent / Footer về hàm cũ */
function onFilterAdd() {
  // Nút Save
  save();
}
function onFilterDelete() {
  // Nút Reload lại quyền của nhóm đang chọn (nếu có)
  if (accountGroupSelected.value) {
    accountGroupSelectedChanged(accountGroupSelected.value);
  }
}
function onFilterAssign() {
  // Dùng như reload để khớp layout 3 nút
  onFilterDelete();
}

/** Summary: tổng số form (lá) & tổng số action bật (từ state hiện tại) */
const totalLeafNodes = computed(() => {
  const countLeaves = (arr: any[]): number =>
    arr.reduce((acc, n) => acc + ((n.arrChildren && n.arrChildren.length) ? countLeaves(n.arrChildren) : 1), 0)
  return countLeaves(listDataForTree.value || [])
});

const totalAllowedCount = computed(() => {
  const actions = permissionActions.filter((a: string) => a !== 'all')
  const walk = (arr: any[]): number => arr.reduce((acc, n) => {
    if (n.arrChildren && n.arrChildren.length) return acc + walk(n.arrChildren)
    const c = actions.reduce((t: number, a: string) => t + (n[a] ? 1 : 0), 0)
    return acc + c
  }, 0)
  return walk(listDataForTree.value || [])
});

/** Duyệt toàn bộ LÁ (node không có arrChildren hoặc arrChildren.length === 0) */
function traverseLeaves(nodes: TreeData[], fn: (leaf: TreeData) => void) {
  for (const n of nodes) {
    if (n.arrChildren && n.arrChildren.length > 0) {
      traverseLeaves(n.arrChildren, fn);
    } else {
      fn(n);
    }
  }
}

</script>

<style scoped>
/* Style cũ (giữ nguyên) */
.checkbox-title__wrap {
  width: 100%;
  display: flex;
  font-size: 13px;
  justify-content: space-around;
}

.checkbox-title__wrap>span {
  flex: 1;
  text-align: center;
}

.button-container {
  display: flex;
  justify-content: flex-end;
}

/* Style mới cho layout Metronic 8 */
.summary-card {
  border: 1px solid rgba(0, 0, 0, 0.06);
}

.input-border-bottom :deep(.el-input__wrapper) {
  box-shadow: none !important;
  border: 0;
  border-bottom: 1px solid var(--bs-border-color, #e5e7eb);
  border-radius: 0;
}

.card-header.card-header-stretch {
  gap: 1rem;
}

.parent-label {
  font-weight: 700;
  /* hoặc 600 tùy ý */
}
</style>
