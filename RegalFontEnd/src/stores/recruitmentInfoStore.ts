// src/stores/recruitmentInfoStore.ts
import { defineStore } from 'pinia';
import { RecruitmentInfoService } from '@/services/RecruitmentInfoService';
import type { RecruitmentInfoModel } from '@/api/RecruitmentInfoApi';

const service = new RecruitmentInfoService();

export const useRecruitmentInfoStore = defineStore('recruitmentInfo', {
  state: () => ({
    recruitmentInfoList: [] as RecruitmentInfoModel[],
    selectedRecruitmentInfo: null as RecruitmentInfoModel | null,
    loading: false,
  }),

  actions: {
    async fetchAllRecruitmentInfo() {
      this.loading = true;
      try {
        const res = await service.fetchAll();
        this.recruitmentInfoList = res.data || [];
      } finally {
        this.loading = false;
      }
    },

    selectRecruitmentInfo(item: RecruitmentInfoModel | null) {
      this.selectedRecruitmentInfo = item;
    },

    async saveRecruitmentInfo(data: RecruitmentInfoModel) {
      delete data.department; // Remove department to avoid issues
      if (data.id) await service.update(data);
      else await service.add(data);
    },

    async deleteRecruitmentInfo(ids: string[]) {
      await service.delete(ids);
      await this.fetchAllRecruitmentInfo();
    }
  }
});
