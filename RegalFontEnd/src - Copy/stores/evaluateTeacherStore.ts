import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type {
  EvaluateTeacherModel,
  EvaluateTeacherQuery,
  EvaluateTeacherSummary,
  EvaluateTeacherSummaryQuery,
  RespondEvaluateTeacherCommand,
} from '@/api/EvaluateTeacherApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useEvaluateTeacherStore = defineStore('evaluateTeacher', {
  state: () => ({
    evaluations: [] as EvaluateTeacherModel[],
    total: 0,
    loading: false,
    summary: {} as EvaluateTeacherSummary,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
    } as EvaluateTeacherQuery,
  }),
  actions: {
    async fetchPagedEvaluateTeachers(overrides?: Partial<EvaluateTeacherQuery>) {
      this.loading = true;
      try {
        this.query = { ...this.query, ...overrides };
        const res =
          await serviceFactory.evaluateTeacherService.getPagedEvaluateTeachers(
            this.query
          );
        if (res?.data) {
          this.evaluations = res.data.items || [];
          this.total = res.data.total || 0;
        } else {
          this.evaluations = [];
          this.total = 0;
        }
        return res;
      } catch (error) {
        console.error('Error fetching paged evaluate teachers:', error);
        this.evaluations = [];
        this.total = 0;
      } finally {
        this.loading = false;
      }
    },

    async fetchAllEvaluateTeachers() {
      this.loading = true;
      try {
        const res = await serviceFactory.evaluateTeacherService.getAllEvaluateTeachers();
        this.evaluations = res.data || [];
        this.total = this.evaluations.length;
        return res;
      } catch (error) {
        console.error('Error fetching all evaluate teachers:', error);
        this.evaluations = [];
        this.total = 0;
      } finally {
        this.loading = false;
      }
    },

    async fetchTeacherEvaluations(teacherId: string) {
      this.loading = true;
      try {
        const res =
          await serviceFactory.evaluateTeacherService.getTeacherEvaluations(teacherId);
        this.evaluations = res.data || [];
        this.total = this.evaluations.length;
        return res;
      } catch (error) {
        console.error('Error fetching teacher evaluations:', error);
        this.evaluations = [];
        this.total = 0;
      } finally {
        this.loading = false;
      }
    },

    async fetchEvaluateTeacherSummary(filters?: EvaluateTeacherSummaryQuery) {
      try {
        const res =
          await serviceFactory.evaluateTeacherService.getEvaluateTeacherSummary(
            filters || {}
          );
        this.summary = res.data || {};
        return res;
      } catch (error) {
        console.error('Error fetching evaluate teacher summary:', error);
        this.summary = {};
      }
    },

    async saveEvaluateTeacher(model: EvaluateTeacherModel) {
      try {
        const res = await serviceFactory.evaluateTeacherService.saveEvaluateTeacher(
          model
        );
        const updated = res?.data as EvaluateTeacherModel | undefined;
        if (updated?.id) {
          const idx = this.evaluations.findIndex((e) => e.id === updated.id);
          if (idx >= 0) {
            this.evaluations.splice(idx, 1, updated);
          } else {
            this.evaluations.unshift(updated);
            this.total += 1;
          }
        }
        return res;
      } catch (error) {
        console.error('Error saving evaluate teacher:', error);
      }
    },

    async deleteEvaluateTeacher(id: string) {
      try {
        const res = await serviceFactory.evaluateTeacherService.deleteEvaluateTeacher(
          id
        );
        this.evaluations = this.evaluations.filter((e) => e.id !== id);
        this.total = Math.max(0, this.total - 1);
        return res;
      } catch (error) {
        console.error('Error deleting evaluate teacher:', error);
      }
    },

    async respondEvaluateTeacher(command: RespondEvaluateTeacherCommand) {
      try {
        return await serviceFactory.evaluateTeacherService.respondEvaluateTeacher(
          command
        );
      } catch (error) {
        console.error('Error responding evaluate teacher:', error);
      }
    },

    setPage(page: number) {
      this.query.page = page;
    },

    setPageSize(size: number) {
      this.query.pageSize = size;
      this.query.page = 1;
    },

    setFilters(filters: Partial<EvaluateTeacherQuery>) {
      this.query = { ...this.query, ...filters, page: 1 };
    },

    reset() {
      this.evaluations = [];
      this.summary = {};
      this.total = 0;
      this.query = { page: 1, pageSize: DEFAULT_PAGE_SIZE } as EvaluateTeacherQuery;
      this.loading = false;
    },
  },
});
