// src/stores/allocationEventStore.ts
import { defineStore } from 'pinia'
import { AllocationEventService } from '@/services/AllocationEventService'
import type { AllocationEventModel } from '@/api/AllocationEventApi'

const service = new AllocationEventService()

export const useAllocationEventStore = defineStore('allocationEvent', {
    state: () => ({
        allocationEvents: [] as AllocationEventModel[],
        selectedEvent: null as AllocationEventModel | null,
        loading: false
    }),

    actions: {
        async fetchAll() {
            this.loading = true
            try {
                const res = await service.fetchAll()
                this.allocationEvents = res.data || []
            } finally {
                this.loading = false
            }
        },
        async fetchAllForRegion() {
            this.loading = true
            try {
                const res = await service.fetchAllForRegion()
                this.allocationEvents = res.data || []
            } finally {
                this.loading = false
            }
        },
        async fetchAllForCompany() {
            this.loading = true
            try {
                const res = await service.fetchAllForCompany()
                this.allocationEvents = res.data || []
            } finally {
                this.loading = false
            }
        },
        selectEvent(item: AllocationEventModel | null) {
            this.selectedEvent = item
        },

        async saveEvent(data: AllocationEventModel) {
            if (data.id) await service.update(data)
            else await service.add(data)
            //await this.fetchAll()
        },

        async deleteEvents(ids: string[]) {
            await service.delete(ids)
            //await this.fetchAll()
        }
    }
})
