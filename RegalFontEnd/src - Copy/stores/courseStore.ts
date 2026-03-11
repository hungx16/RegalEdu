// src/stores/courseStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { CourseModel, CourseQuery } from '@/api/CourseApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useCourseStore = defineStore('course', {
  state: () => ({
    courses: [] as CourseModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {},
    } as CourseQuery,
    selectedCourse: null as CourseModel | null,
  }),
  actions: {
    async fetchCourses() {
      const service = serviceFactory.courseService;
      this.loading = true;
      try {
        const result = await service.fetchPagedCourses(this.query);
        if (result?.succeeded) {
          this.courses = result.data.items;
          this.total = result.data.total;
        }
      } finally {
        this.loading = false;
      }
    },
    async fetchAllCourses() {
      const service = serviceFactory.courseService;
      this.loading = true;
      try {
        const result = await service.fetchAllCourses();
        if (result?.succeeded) this.courses = result.data;
      } finally {
        this.loading = false;
      }
    },
    async getCourseById(id: string) {
      const service = serviceFactory.courseService;
      this.loading = true;
      try {
        const course = await service.getCourseById(id);
        return course;
      } finally {
        this.loading = false;
      }
    },
    selectCourse(model: CourseModel | null) {
      this.selectedCourse = model;
    },
    async saveCourse(model: Partial<CourseModel>) {
      await serviceFactory.courseService.saveCourse(model);
      // await this.fetchCourses();
    },
    async deleteCourses(ids: string[]) {
      await serviceFactory.courseService.deleteCourses(ids);
      //await this.fetchCourses();
    },
    async restoreCourses(ids: string[]) {
      await serviceFactory.courseService.restoreCourses(ids);
      // await this.fetchCourses();
    },

    async setPage(page: number) {
      this.query.page = page;
      await this.fetchCourses();
    },
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
      await this.fetchCourses();
    },
    async setFilter(filter: Record<string, any>) {
      this.query.filters = { ...this.query.filters, ...filter };
      this.query.page = 1;
      await this.fetchCourses();
    },
  },
});
