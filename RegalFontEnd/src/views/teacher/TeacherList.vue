<template>
    <div>
        <!-- Header filter -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="teacher.headerTitle" headerDesc="teacher.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <!-- Summary cards -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-3" v-for="card in summaryCards" :key="card.label">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t(card.label) }}</span>
                        <i :class="card.icon + ' fs-4 text-body-secondary'"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ card.value }}</div>
                </div>
            </div>
        </div>

        <!-- Table -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('teacher.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">
                        {{ t('teacher.headerDesc') }}
                    </span>
                </div>
            </div>

            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredTeachersAll"
                    :loading="formLoading" :showPagination="true" :page="page" :total="filteredTeachersAll.length"
                    :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="(val) => (selectedRowsData = val)" @edit="editModelEvent" @view="viewModelEvent"
                    @delete="handleDelete" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" @update:page="(val) => (page = val)" @update:pageSize="onPageSizeChange">
                    <!-- Cột trạng thái -->
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean" :label="item.status === 0 ? t('common.active') : t('common.inactive')
                            " />
                    </template>
                    <template #cell-teacherCode="{ item }">
                        <span>{{ item.applicationUser?.userCode || '-' }}</span>
                    </template>
                    <template #cell-teacherName="{ item }">
                        <span>{{ item.applicationUser?.fullName || '-' }}</span>
                    </template>
                    <!-- Cột chi nhánh -->
                    <template #cell-company="{ item }">
                        <span>{{ getMainCompanyName(item) || t('common.notUpdated') }}</span>
                    </template>

                    <!-- Cột chuyên môn -->
                    <template #cell-specialization="{ item }">
                        <span>{{
                            item.teacherSpecialization || t('common.notUpdated')
                            }}</span>
                    </template>

                    <!-- Cột loại hình -->
                    <template #cell-workType="{ item }">
                        <BaseBadge :type="getWorkTypeBadgeType(item.workType)" :label="getWorkTypeText(item.workType)"
                            soft bordered />
                    </template>

                    <!-- Cột tính năng -->
                    <template #cell-features="{ item }">
                        <div class="d-flex flex-wrap gap-1">
                            <BaseBadge v-if="item.teachingOutside" type="warning" :label="t('teacher.teachingOutside')"
                                size="sm" />
                            <BaseBadge v-if="item.teacherAssistant" type="info" :label="t('teacher.assistant')"
                                size="sm" />
                            <BaseBadge v-if="item.isOnline" type="success" :label="t('teacher.online')" size="sm" />
                        </div>
                    </template>
                </BaseTable>
            </div>
        </div>

        <!-- Dialog form -->
        <TeacherDialog v-model:visible="showFormModal" :mode="dialogMode" :teacher-data="teacherStore.selectedTeacher"
            :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import BaseTable, { type BaseTableColumn } from "@/components/table/BaseTable.vue";
import FilterComponent from "@/components/filter-component/FilterComponent.vue";
import TeacherDialog from "@/views/teacher/TeacherDialog.vue";
import { useTeacherStore } from "@/stores/teacherStore";
import { useNotificationStore } from "@/stores/notificationStore";
import { useLoadingWithTimeout } from "@/composables/useLoadingWithTimeout";
import { useCompanyStore } from "@/stores/companyStore";
import BaseBadge from "@/components/info-badge/BaseBadge.vue";
import { formatDate } from "@/utils/format";
import { useCommonStore } from "@/stores/commonStore";
const commonStore = useCommonStore();
const { t } = useI18n();
const teacherStore = useTeacherStore();
const notificationStore = useNotificationStore();
const companyStore = useCompanyStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } =
    useLoadingWithTimeout(50000);

const filterComponentRef = ref();
const showFormModal = ref(false);
const dialogMode = ref<"create" | "edit" | "view">("create");
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRowsData = ref([]);

// =============== Summary cards ===============
const summaryCards = computed(() => [
    {
        label: "teacher.totalTeachers",
        icon: "bi bi-collection",
        value: teacherStore.teachers.length,
    },
    {
        label: "teacher.activeTeachers",
        icon: "bi bi-check2-circle",
        value: teacherStore.teachers.filter((x) => x.status === 0).length,
    },
    {
        label: "teacher.fulltimeTeachers",
        icon: "bi bi-people",
        value: teacherStore.teachers.filter((x) => x.workType === 0).length,
    },
    {
        label: "teacher.parttimeTeachers",
        icon: "bi bi-bar-chart-line",
        value: teacherStore.teachers.filter((x) => x.workType === 1).length,
    },
]);

// =============== Helper functions ===============
const getWorkTypeText = (workType: number) => {
    switch (workType) {
        case 0:
            return t("teacher.fulltime");
        case 1:
            return t("teacher.parttime");
        case 2:
            return t("teacher.contract");
        default:
            return t("common.notUpdated");
    }
};

const getWorkTypeBadgeType = (workType: number) => {
    switch (workType) {
        case 0:
            return "success";
        case 1:
            return "info";
        case 2:
            return "warning";
        default:
            return "secondary";
    }
};

const getMainCompanyName = (teacher: any) => {
    if (!teacher.companyId) return "";
    const company = companyStore.companies.find(
        (c) => c.id === teacher.companyId
    );
    return company?.companyName || "";
};

// =============== Table columns ===============
const columns = computed<BaseTableColumn[]>(() => [
    {
        key: "teacherCode",
        labelKey: "teacher.code",
        filterType: "text",
        sortable: true,
        width: 150,
        isBold: true,
        sticky: true
    },
    {
        key: "teacherName",
        labelKey: "teacher.name",
        filterType: "text",
        sortable: true,
        isBold: true,
        sticky: true
    },
    {
        key: "company",
        labelKey: "teacher.colCompany",
        filterType: "text",
        sortable: false,
        width: 180,
    },
    {
        key: "specialization",
        labelKey: "teacher.colSpecialization",
        filterType: "text",
        sortable: true,
        width: 200,
    },
    {
        key: "workType",
        labelKey: "teacher.colType",
        filterType: "select",
        sortable: true,
        width: 150,
        align: "center",
        filterOptions: [
            { label: "teacher.fulltime", value: 0 },
            { label: "teacher.parttime", value: 1 },
            { label: "teacher.contract", value: 2 },
        ],
    },
    {
        key: "status",
        labelKey: "common.status",
        filterType: "select",
        sortable: true,
        width: 150,
        align: "center",
        filterOptions: [
            { label: "common.all", value: "" },
            { label: "common.active", value: 0 },
            { label: "common.inactive", value: 1 },
        ],
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 160 },
    { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 160, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY') },
    {
        key: "actions",
        labelKey: "common.actions",
        width: 160,
        align: "center",
    },
]);

// =============== Filters ===============
const filteredTeachersAll = computed(() => {
    let arr = teacherStore.teachers;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== "") {
            if (
                key === "teachingOutside" ||
                key === "teacherAssistant" ||
                key === "isOnline"
            ) {
                arr = arr.filter((item) => item[key] === true);
            } else if (key === "workType" || key === "status") {
                arr = arr.filter((item) => item[key] === Number(val));
            } else {
                arr = arr.filter((item) =>
                    String(item[key] ?? "")
                        .toLowerCase()
                        .includes(String(val).toLowerCase())
                );
            }
        }
    });
    return arr;
});

// =============== Actions ===============
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast("warning", {
            key: "toast.noSelected",
            params: { model: t("models.teacher") },
        });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

async function handleDelete(items: any | any[]) {
    const list = Array.isArray(items) ? items : [items];
    const ids = list.map((item) => item.id).filter(Boolean);
    notificationStore.showConfirm(
        { key: "toast.delete", params: { model: t("models.teacher") } },
        async () => {
            try {
                startLoading();
                await teacherStore.deleteTeacher(ids);
                await teacherStore.fetchAllTeacher();
                // notificationStore.showToast("success", {
                //     key: "toast.deleteSuccess",
                //     params: { model: t("models.teacher") },
                // });
            } catch (err) {
                console.error("Error deleting:", err);
                notificationStore.showToast("error", {
                    key: "toast.deleteError",
                });
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = "create";
    teacherStore.selectTeacher(null);
    await commonStore.generateCode('GV', 'AspNetUsers', 'UserCode', 4);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = "edit";
    teacherStore.selectTeacher({ ...item });
    showFormModal.value = true;
}

function viewModelEvent(item: any) {
    dialogMode.value = "view";
    teacherStore.selectTeacher({ ...item });
    showFormModal.value = true;
}

async function handleSave(teacher: any) {
    startLoading();
    try {
        await teacherStore.saveTeacher(teacher);
        showFormModal.value = false;
        if (teacher.id) {
            notificationStore.showToast("success", {
                key: "toast.updateSuccess",
                params: { model: t("models.teacher") },
            });
        } else {
            notificationStore.showToast("success", {
                key: "toast.createSuccess",
                params: { model: t("models.teacher") },
            });
        }
        await teacherStore.fetchAllTeacher();
    } catch (err) {
        console.error("Error saving teacher:", err);
    } finally {
        stopLoading();
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

// =============== Mounted ===============
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: "add", label: "teacher.add", type: "add" },
            { event: "delete", label: "teacher.delete", type: "delete" },
        ],
    });

    await companyStore.fetchAllCompanies();
    await teacherStore.fetchAllTeacher();
});
</script>

<style scoped>
.summary-card {
    transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

.summary-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1) !important;
}
</style>
