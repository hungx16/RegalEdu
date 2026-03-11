import { defineStore } from 'pinia';
import type { TeacherModel, TeacherQuery } from '@/api/TeacherApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useTeacherStore = defineStore('teacher', {
    state: () => ({
        teachers: [] as TeacherModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                TeacherCode: '',
                TeacherName: '',
            },
        } as TeacherQuery,
        selectedTeacher: null as TeacherModel | null,
        isCurrentUserTeacher: false,
    }),
    actions: {
        async fetchAllTeacher() {
            this.loading = true;
            try {
                const res = await serviceFactory.teacherService.fetchAllTeacher();
                if (res?.succeeded) {
                    // Đảm bảo mỗi teacher có trường companies
                    this.teachers = res.data.map(teacher => ({
                        ...teacher,
                        companies: teacher.companies || [] // Đảm bảo có trường companies
                    }));
                    this.total = res.data.length;
                }
            } catch (error) {
                console.error('Error fetching teachers:', error);
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedTeacher() {
            this.loading = true;
            try {
                const res = await serviceFactory.teacherService.fetchPagedTeacher(this.query);
                if (res?.succeeded) {
                    // Đảm bảo mỗi teacher có trường companies
                    this.teachers = res.data.items.map(teacher => ({
                        ...teacher,
                        companies: teacher.companies || []
                    }));
                    this.total = res.data.total;
                }
            } catch (error) {
                console.error('Error fetching paged teachers:', error);
            } finally {
                this.loading = false;
            }
        },

        async saveTeacher(model: Partial<TeacherModel>) {
            try {
                const result = await serviceFactory.teacherService.saveTeacher(model);
                return result; // Trả về kết quả để có thể lấy ID nếu là tạo mới
            } catch (error) {
                console.error('Error saving teacher:', error);
                throw error;
            }
        },

        async deleteTeacher(ids: string[]) {
            try {
                await serviceFactory.teacherService.deleteTeacher(ids);
            } catch (error) {
                console.error('Error deleting teachers:', error);
                throw error;
            }
        },

        async restoreTeacher(ids: string[]) {
            try {
                await serviceFactory.teacherService.restoreTeacher(ids);
            } catch (error) {
                console.error('Error restoring teachers:', error);
                throw error;
            }
        },

        selectTeacher(model: TeacherModel | null) {
            this.selectedTeacher = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedTeacher();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedTeacher();
        },

        async setFilter(filter: Partial<TeacherQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedTeacher();
        },

        async fetchDeletedTeacher() {
            this.loading = true;
            try {
                const result = await serviceFactory.teacherService.fetchDeletedTeacher();
                if (result?.succeeded === true) {
                    // Đảm bảo mỗi teacher có trường companies
                    this.teachers = result.data.map(teacher => ({
                        ...teacher,
                        companies: teacher.companies || []
                    }));
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted teachers:', error);
            } finally {
                this.loading = false;
            }
        },

        async checkIsCurrentUserTeacher() {
            try {
                const result = await serviceFactory.teacherService.isCurrentUserTeacher();
                this.isCurrentUserTeacher = !!(result?.succeeded && result.data);
                return this.isCurrentUserTeacher;
            } catch (error) {
                console.error('Error checking current user teacher status:', error);
                this.isCurrentUserTeacher = false;
                return false;
            }
        },

        // Hàm mới: Load chi nhánh cho một giáo viên cụ thể
        async loadTeacherCompanies(teacherId: string) {
            try {
                const result = await serviceFactory.companyTeacherService.getTeacherCompanies(teacherId);
                if (result.succeeded) {

                    return result.data;
                }
                return [];
            } catch (error) {
                console.error('Error loading teacher companies:', error);
                return [];
            }
        },

        // Hàm mới: Cập nhật chi nhánh cho giáo viên
        async updateTeacherCompanies(teacherId: string, companies: any[]) {
            try {
                await serviceFactory.companyTeacherService.updateTeacherCompanies(teacherId, companies);

                // Cập nhật lại danh sách teachers để reflect changes
                await this.fetchAllTeacher();
                return true;
            } catch (error) {
                console.error('Error updating teacher companies:', error);
                throw error;
            }
        }
    },

    getters: {
        // Getter để lấy teacher theo ID
        getTeacherById: (state) => (id: string) => {
            return state.teachers.find(teacher => teacher.id === id);
        },

        // Getter để lấy chi nhánh chính của teacher
        getMainCompany: (state) => (teacherId: string) => {
            const teacher = state.teachers.find(t => t.id === teacherId);

            return null;
        },

        // Getter để lấy chi nhánh phụ của teacher
        getSubCompanies: (state) => (teacherId: string) => {
            const teacher = state.teachers.find(t => t.id === teacherId);

            return [];
        }
    },
});
