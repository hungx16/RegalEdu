<template>
    <div class="card">
        <div class="card-header border-0 pt-6">
            <div>
                <h3 class="fw-bold mb-1">{{ t('luckyDraw.pageTitle') }}</h3>
                <div class="text-muted">{{ t('luckyDraw.pageDesc') }}</div>
            </div>
        </div>

        <div class="card-body pt-0">
            <div class="row g-4 mb-6">
                <div class="col-md-4">
                    <div class="border rounded p-4 h-100">
                        <div class="text-muted fs-7">{{ t('luckyDraw.totalPrograms') }}</div>
                        <div class="fw-bold fs-2 mt-2">{{ store.total }}</div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="border rounded p-4 h-100">
                        <div class="text-muted fs-7">{{ t('luckyDraw.activePrograms') }}</div>
                        <div class="fw-bold fs-2 mt-2">{{ activeCount }}</div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="border rounded p-4 h-100">
                        <div class="text-muted fs-7">{{ t('luckyDraw.completedPrograms') }}</div>
                        <div class="fw-bold fs-2 mt-2">{{ completedCount }}</div>
                    </div>
                </div>
            </div>

            <div class="table-responsive">
                <table class="table align-middle gs-0 gy-3">
                    <thead>
                        <tr class="fw-semibold text-muted">
                            <th>#</th>
                            <th>{{ t('luckyDraw.name') }}</th>
                            <th>{{ t('luckyDraw.branch') }}</th>
                            <th>{{ t('luckyDraw.region') }}</th>
                            <th>{{ t('luckyDraw.reportDate') }}</th>
                            <th>{{ t('luckyDraw.reporter') }}</th>
                            <th>{{ t('common.status') }}</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(item, index) in store.luckyDraws" :key="item.id || index">
                            <td>{{ index + 1 }}</td>
                            <td class="fw-semibold">{{ item.name }}</td>
                            <td>{{ item.branch || '-' }}</td>
                            <td>{{ item.region || '-' }}</td>
                            <td>{{ formatDate(item.reportDate) }}</td>
                            <td>{{ item.reporter || '-' }}</td>
                            <td>
                                <span class="badge"
                                    :class="item.status === 1 ? 'badge-light-success' : 'badge-light-warning'">
                                    {{ item.status === 1 ? t('common.active') : t('common.inactive') }}
                                </span>
                            </td>
                        </tr>

                        <tr v-if="!store.loading && store.luckyDraws.length === 0">
                            <td colspan="7" class="text-center text-muted py-10">{{ t('common.noData') }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useLuckyDrawStore } from '@/stores/luckyDrawStore';

const { t } = useI18n();
const store = useLuckyDrawStore();

const activeCount = computed(() => store.luckyDraws.filter((x) => x.status === 1).length);
const completedCount = computed(() => store.luckyDraws.filter((x) => x.status === 2).length);

function formatDate(value?: string) {
    if (!value) return '-';
    return value.split('T')[0];
}

onMounted(async () => {
    await store.fetchPagedLuckyDraws();
});
</script>