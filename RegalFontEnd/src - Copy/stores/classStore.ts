// src/stores/classStore.ts
import { defineStore } from 'pinia'
import type { ClassModel } from '@/api/ClassApi'
import type { ClassScheduleModel } from '@/api/ClassScheduleApi'
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useClassStore = defineStore('class', {
    state: () => ({
        classes: [] as ClassModel[],
        selectedClass: null as ClassModel | null,
        loading: false,
        homeTeacherSchedules: [] as ClassScheduleModel[],
        homeTeacherScheduleLoading: false
    }),

    actions: {
        async fetchAllClasses() {
            this.loading = true
            try {
                const res = await serviceFactory.classService.fetchAll()
                this.classes = res.data || []
            } finally {
                this.loading = false
            }
        },
        async fetchHomeTeacherSchedules() {
            this.homeTeacherScheduleLoading = true
            try {
                const res = await serviceFactory.classService.getSchedulesByHomeTeacher()
                this.homeTeacherSchedules = res.data || []
                return res
            } catch (error) {
                console.error('Error fetching home teacher schedules:', error)
                this.homeTeacherSchedules = []
            } finally {
                this.homeTeacherScheduleLoading = false
            }
        },

        selectClass(item: ClassModel | null) {
            this.selectedClass = item
        },

        async saveClass(data: ClassModel) {
            if (data.id) await serviceFactory.classService.update(data)
            else await serviceFactory.classService.add(data)
            await this.fetchAllClasses()
        },

        async deleteClasses(ids: string[]) {
            await serviceFactory.classService.delete(ids)
            await this.fetchAllClasses()
        },

        async fetchAllClassesByTeacherId(teacherId: string) {
            this.loading = true
            try {
                const res = await serviceFactory.classService.getByTeacherId(teacherId)
                this.classes = res.data || []
            } finally {
                this.loading = false
            }
        },
    }
})
