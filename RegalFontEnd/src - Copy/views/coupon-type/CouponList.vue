<template>
    <div>
        <h3 class="fw-bold fs-4 mb-4">{{ t('coupon.listCouponTitle') }}</h3>

        <!-- <div class="card shadow-sm p-4 mb-5">
            <el-input v-model="filter.searchTerm" :placeholder="t('coupon.searchPlaceholder')" class="w-50" />
        </div> -->

        <div class="card w-100 p-4">
            <BaseTable :columns="columns" :items="filterCoupons" :loading="issueStore.loading" :showIndex="true"
                :show-checkbox-column="true" :page="issueStore.query.page" :total="issueStore.total" :show-delete="true"
                @delete="onDeleteClicked" :pageSize="issueStore.query.pageSize" :filter="filter"
                @update:rows="val => selectedRowsData = val" :showView="true"
                @update:page="val => couponTypeStore.setPage(val)" :showPagination="true"
                @update:pageSize="val => couponTypeStore.setPageSize(val)">
                @update:pageSize="issueStore.setPageSize" @update:filter="onTableFilter"
                >

                <template #cell-code="{ item }">
                    <span class="fw-bold text-primary">{{ item.code }}</span>
                </template>

                <template #cell-couponTypeName="{ item }">
                    <span>{{ item.couponIssue.couponType.name }}</span>
                </template>
                <template #cell-studentCode="{ item }">
                    <BaseBadge :label="getStudentCodeLabel(item.studentId)"
                        :color="getStatusColor(item.couponStatus)" />
                </template>
                <template #cell-couponStatus="{ item }">
                    <BaseBadge :label="getStatusLabel(item.couponStatus)" :color="getStatusColor(item.couponStatus)" />
                </template>

            </BaseTable>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { defineStore } from 'pinia';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
//nhập đợt phát hành issue store
import { useCouponIssueStore } from '@/stores/couponIssueStore';
const issueStore = useCouponIssueStore();
//nhập coupon type store
import { useCouponTypeStore } from '@/stores/couponTypeStore';
import { CouponStatus } from '@/types';
import type { CouponModel } from '@/api/CouponIssueApi';
const couponTypeStore = useCouponTypeStore();
// notification store (used to show toasts)
import { useNotificationStore } from '@/stores/notificationStore';
const notificationStore = useNotificationStore();
//khai báo sử dụng student store
import { useStudentStore } from '@/stores/studentStore';
const studentStore = useStudentStore();
// --- GIẢ LẬP CouponStore cho mục đích hiển thị LIST ---
// Giả định CouponStore có các field cần thiết

// --------------------------------------------------------

const { t } = useI18n();
const filter = ref<{ searchTerm?: string }>({});
const selectedRowsData = ref<Array<CouponModel>>([]);

// computed list used by the template
const filterCoupons = computed(() => {
    //issueStore.fetchAllCoupons();
    const items = issueStore.allCoupons || [];
    console.log("Lọc danh sách coupon từ đợt phát hành", issueStore.allCoupons);

    const term = (filter.value?.searchTerm || '').toString().trim().toLowerCase();
    if (!term) return items;
    return items.filter((it: any) => {
        return (it.code || '').toString().toLowerCase().includes(term)
            || (it.couponTypeName || '').toString().toLowerCase().includes(term)
            || (it.createdBy || '').toString().toLowerCase().includes(term)
            || (it.studentId || '').toString().toLowerCase().includes(term);
    });
});

const onTableFilter = (newFilter: Record<string, any>) => {
    filter.value = newFilter || {};
    // reset to first page when filter changes (if setPage exists)
    issueStore.setPage?.(1);
    issueStore.fetchAllCoupons();
};

const getStatusLabel = (status: number) => {
    if (status === CouponStatus.NotUsed) return t('coupon.status.notUsed');
    if (status === CouponStatus.Used) return t('coupon.status.used');
    if (status === CouponStatus.Expired) return t('coupon.status.expired');
    return t('coupon.status.canceled');
};

const getStatusColor = (status: number) => {
    if (status === CouponStatus.NotUsed) return 'success'; // Đã phát hành
    if (status === CouponStatus.Used) return 'primary';  // Đã sử dụng
    if (status === CouponStatus.Expired) return 'danger';   // Hết hạn
    return 'warning';
};

const getStudentCodeLabel = (studentId?: number | string) => {
    // Return empty string when there is no student id, otherwise stringified id
    if (studentId === null || studentId === undefined || studentId === '') return '';
    // Find student by id from student store
    const student = studentStore.students.find(s => s.id === studentId || String(s.id) === String(studentId));
    if (student && student.studentCode) return student.studentCode;
    return String(studentId);
};

const getCouponTypeName = (id: number | string) => {
    //console.log("Lấy tên loại coupon cho couponIssueId:", id);
    console.log("Danh sách loại coupon:", issueStore.couponIssues);
    if (!id) return '';
    if (!issueStore.couponIssues || issueStore.couponIssues.length === 0) return '';
    const issue = issueStore.couponIssues.find((i: any) => i.id === id);
    return issue?.couponTypeId || '';
};

const columns: BaseTableColumn[] = [
    { key: 'code', labelKey: 'coupon.code', width: 160, filterType: 'text', sortable: true, sticky: true },
    { key: 'couponTypeName', labelKey: 'coupon.couponType', width: 250, filterType: 'text', sortable: true },
    { key: 'createdBy', labelKey: 'coupon.issuer', width: 150, filterType: 'text', sortable: true },
    { key: 'studentCode', labelKey: 'student.studentCode', width: 120, filterType: 'text', sortable: true },
    { key: 'createdDate', labelKey: 'coupon.createdDate', width: 120, formatter: (val: string) => val ? new Date(val).toLocaleDateString() : '', sortable: true, filterType: 'date' },
    { key: 'expiredDate', labelKey: 'coupon.expiredDate', width: 100, formatter: (val: string) => val ? new Date(val).toLocaleDateString() : '', sortable: true, filterType: 'date' },
    { key: 'couponStatus', labelKey: 'common.status', width: 120, filterType: 'select', sortable: true },
    // { key: 'actions', labelKey: 'common.actions', width: 100 },
];
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

// local UI state for loading/modal so startLoading/stopLoading and showFormModal exist
const showFormModal = ref(false);
const isDeleting = ref(false);
const startLoading = () => { isDeleting.value = true; };
const stopLoading = () => { isDeleting.value = false; };

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.couponType') } });
        return;
    }
    console.log("coupons chọn xóa là: ", selectedRowsData.value);
    handleDelete(selectedRowsData.value);
}
function handleDelete(coupons: CouponModel | CouponModel[]) {
    const list = Array.isArray(coupons) ? coupons : [coupons];


    const ids = list.filter(item => typeof item.id === 'string' && item.id).map(item => item.id as string);
    // If any selected coupon type is active (status === 1), prevent deletion
    if (list.some(item => item.status === 1)) {
        notificationStore.showToast('warning', { key: 'toast.deleteActiveError', params: { model: t('models.coupon') } });
        return;
    }
    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.coupon') } },
        async () => {
            try {
                startLoading();
                await couponTypeStore.deleteCouponTypes(ids);
                await couponTypeStore.fetchAll();

            } catch (err: any) {
                console.error('Error deleting:', err);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}
watch(

    () => [issueStore.query.page, couponTypeStore.query.pageSize, filter],
    () => {
        //  issueStore.fetchAll();
        issueStore.fetchAllCoupons();
        couponTypeStore.fetchAll();
        issueStore.fetchAll();
        console.log("Lấy dữ liệu đợt phát hành coupon và loại coupon", issueStore.allCoupons);

    },
    {
        immediate: true

    },

);

onMounted(() => {
    // Mock Data
    issueStore.fetchAllCoupons()
    issueStore.fetchAll();
    couponTypeStore.fetchAll();
    studentStore.fetchAllStudents();
    // console.log("Lấy dữ liệu đợt phát hành coupon và loại coupon", issueStore.allCoupons);
    // couponStore.fetchPagedCoupons();
});
</script>