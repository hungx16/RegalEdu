import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { ClassScoreSummaryModel, UpdateClassScoreBoardModel, ClassScoreBoardModel } from '@/api/ClassScoreBoardApi';

export const useClassScoreBoardStore = defineStore('classScoreBoard', {
  state: () => ({
    scores: [] as ClassScoreSummaryModel[],
    scoreDetails: [] as ClassScoreBoardModel[],
    loading: false,
    saving: false
  }),
  actions: {
    async fetchByClassId(classId: string) {
      this.loading = true;
      try {
        const res = await serviceFactory.classScoreBoardService.getByClassId(classId);
        this.scores = res.data || [];
        return res;
      } catch (error) {
        console.error('Error fetching class score board:', error);
        this.scores = [];
      } finally {
        this.loading = false;
      }
    },
    async fetchDetailsByClassId(classId: string) {
      this.loading = true;
      try {
        const res = await serviceFactory.classScoreBoardService.getScoresByClassId(classId);
        this.scoreDetails = res.data || [];
        return res;
      } catch (error) {
        console.error('Error fetching class score details:', error);
        this.scoreDetails = [];
      } finally {
        this.loading = false;
      }
    },
    async updateScoreBoard(model: UpdateClassScoreBoardModel) {
      this.saving = true;
      try {
        const res = await serviceFactory.classScoreBoardService.updateScoreBoard(model);
        return res;
      } catch (error) {
        console.error('Error updating class score board:', error);
        throw error;
      } finally {
        this.saving = false;
      }
    }
  }
});
