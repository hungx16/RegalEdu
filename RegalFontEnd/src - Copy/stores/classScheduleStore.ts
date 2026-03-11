import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { CancelClassScheduleByShiftingCommand, CancelClassScheduleWithSubstitutionCommand, ClassScheduleModel } from '@/api/ClassScheduleApi';

export const useClassScheduleStore = defineStore('classSchedule', {
  state: () => ({
    schedules: [] as ClassScheduleModel[],
    selectedSchedule: null as ClassScheduleModel | null,
    loading: false,
    detailLoading: false
  }),
  actions: {
    async fetchByClassId(classId: string) {
      this.loading = true;
      try {
        const res = await serviceFactory.classScheduleService.getSchedulesByClassId(classId);
        this.schedules = (res.data || []).map((s, idx) => ({
          ...s,
          sessionIndex: s.sessionIndex ?? idx + 1
        }));
        return res;
      } catch (error) {
        console.error('Error fetching class schedules:', error);
        this.schedules = [];
      } finally {
        this.loading = false;
      }
    },
    async fetchById(id: string) {
      this.detailLoading = true;
      try {
        const res = await serviceFactory.classScheduleService.getScheduleById(id);
        this.selectedSchedule = res.data || null;
        return res;
      } catch (error) {
        console.error('Error fetching schedule detail:', error);
        this.selectedSchedule = null;
      } finally {
        this.detailLoading = false;
      }
    },
    async updateSchedule(payload: Partial<ClassScheduleModel>) {
      try {
        const res = await serviceFactory.classScheduleService.updateSchedule(payload);
        const updated = res.data || payload;
        if (updated.id) {
          const idx = this.schedules.findIndex((s) => s.id === updated.id);
          if (idx >= 0) {
            this.schedules.splice(idx, 1, { ...this.schedules[idx], ...updated });
          }
        }
        this.selectedSchedule = (updated as ClassScheduleModel) || this.selectedSchedule;
        return res;
      } catch (error) {
        console.error('Error updating schedule:', error);
        throw error;
      }
    },
    resetSelected() {
      this.selectedSchedule = null;
    },
    async cancelAndShift(data: CancelClassScheduleByShiftingCommand) {
      await serviceFactory.classScheduleService.cancelAndShift(data);
    },
    async cancelAndSubstitute(data: CancelClassScheduleWithSubstitutionCommand) {
      await serviceFactory.classScheduleService.cancelAndSubstitute(data);
    }
  }
});