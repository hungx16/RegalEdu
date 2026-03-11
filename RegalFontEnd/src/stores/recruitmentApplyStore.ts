// src/stores/recruitmentApplyStore.ts
import { defineStore } from 'pinia'
import { RecruitmentApplyService } from '@/services/RecruitmentApplyService'
import type { RecruitmentApplyModel } from '@/api/RecruitmentApplyApi'

const service = new RecruitmentApplyService()

export const useRecruitmentApplyStore = defineStore('recruitmentApply', {
  state: () => ({
    recruitmentApplies: [] as RecruitmentApplyModel[],
    selectedRecruitmentApply: null as RecruitmentApplyModel | null,
    loading: false
  }),

  actions: {
    async fetchAllRecruitmentApplies() {
      this.loading = true
      try {
        const res = await service.fetchAll()
        this.recruitmentApplies = res.data || []
      } finally {
        this.loading = false
      }
    },

    selectRecruitmentApply(item: RecruitmentApplyModel | null) {
      this.selectedRecruitmentApply = item
    },

    async saveRecruitmentApply(data: RecruitmentApplyModel) {
      if (data.id) await service.update(data)
      else await service.add(data)
      await this.fetchAllRecruitmentApplies()
    },

    async deleteRecruitmentApply(ids: string[]) {
      await service.delete(ids)
      await this.fetchAllRecruitmentApplies()
    }
  }
})
