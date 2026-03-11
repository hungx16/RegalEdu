// src/stores/outputCommitmentStore.ts
import { defineStore } from 'pinia'
import type { OutputCommitmentModel } from '@/api/OutputCommitmentApi'
import { OutputCommitmentService } from '@/services/OutputCommitmentService'

const service = new OutputCommitmentService()

export const useOutputCommitmentStore = defineStore('outputCommitment', {
  state: () => ({
    items: [] as OutputCommitmentModel[],
    selected: null as OutputCommitmentModel | null,
    loading: false,
  }),
  actions: {
    async fetchAll() {
      this.loading = true
      try {
        const res = await service.fetchAll()
        this.items = res.data || []
      } finally {
        this.loading = false
      }
    },
    async save(model: OutputCommitmentModel) {
      await service.save(model)
      await this.fetchAll()
    },
    async delete(ids: string[]) {
      await service.delete(ids)
      await this.fetchAll()
    },
    async downloadPdf(model: Partial<OutputCommitmentModel>) {
      return await service.downloadPdf(model)
    },
    select(item: OutputCommitmentModel | null) {
      this.selected = item
    }
  }
})
