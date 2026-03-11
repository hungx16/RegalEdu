// src/stores/companyEventReportStore.ts
import { defineStore } from 'pinia'
import { CompanyEventService } from '@/services/CompanyEventService'
import type { CompanyEventReportModel } from '@/api/AllocationEventApi'

const service = new CompanyEventService()

export const useCompanyEventReportStore = defineStore('companyEventReport', {
  state: () => ({
    reports: [] as CompanyEventReportModel[],
    selectedReport: null as CompanyEventReportModel | null,
    loading: false
  }),

  actions: {
    async fetchByCompanyEventId(companyEventId: string) {
      if (!companyEventId) {
        this.reports = []
        return
      }
      this.loading = true
      try {
        const res = await service.fetchReportsByCompanyEventId(companyEventId)
        this.reports = res.data || []
      } finally {
        this.loading = false
      }
    },
    selectReport(report: CompanyEventReportModel | null) {
      this.selectedReport = report
    },
    clear() {
      this.reports = []
      this.selectedReport = null
    }
  }
})
