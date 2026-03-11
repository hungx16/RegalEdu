import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { ClassAttendantModel } from '@/api/ClassAttendantApi';

export const useClassAttendantStore = defineStore('classAttendant', {
  state: () => ({
    attendantsMap: {} as Record<string, ClassAttendantModel[]>,
    loading: false,
    saving: false
  }),
  actions: {
    async fetchByScheduleId(scheduleId: string) {
      this.loading = true;
      try {
        const res = await serviceFactory.classAttendantService.getByScheduleId(scheduleId);
        const existing = this.attendantsMap[scheduleId] || [];
        const incoming = res.data || [];
        this.attendantsMap[scheduleId] = incoming.map((item) => {
          const current = existing.find((c) => c.studentId === item.studentId);
          return {
            ...item,
            student: item.student ?? current?.student
          };
        });
        return res;
      } catch (error) {
        console.error('Error fetching class attendants:', error);
        this.attendantsMap[scheduleId] = [];
      } finally {
        this.loading = false;
      }
    },
    async updateAttendants(scheduleId: string, attendants: ClassAttendantModel[]) {
      this.saving = true;
      try {
        const res = await serviceFactory.classAttendantService.update({
          scheduleId,
          attendants
        });
        const current = this.attendantsMap[scheduleId] || [];
        const source = res.data && res.data.length ? res.data : attendants;
        const merged = source.map((a) => {
          const existing = current.find((c) => c.studentId === a.studentId);
          return {
            ...existing,
            ...a,
            student: a.student ?? existing?.student
          };
        });
        this.attendantsMap[scheduleId] = merged.length ? merged : current;
        return res;
      } catch (error) {
        console.error('Error updating class attendants:', error);
        throw error;
      } finally {
        this.saving = false;
      }
    }
  }
});
