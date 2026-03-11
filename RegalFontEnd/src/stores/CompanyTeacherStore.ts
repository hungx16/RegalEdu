// src/stores/companyTeacherStore.ts
import { defineStore } from 'pinia';
import type { CompanyTeacherModel } from '@/api/CompanyTeacherApi';
import { serviceFactory } from '@/services/ServiceFactory';

export const useCompanyTeacherStore = defineStore('companyTeacher', {
    state: () => ({
        teacherCompanies: [] as CompanyTeacherModel[],
        loading: false,
    }),

    actions: {
        // SỬA: Đảm bảo gọi đúng service method
        async fetchTeacherCompanies(teacherId: string): Promise<CompanyTeacherModel[]> {
            this.loading = true;
            try {
                // SỬA: Gọi getTeacherCompanies thay vì getCompanyTeachers
                const result = await serviceFactory.companyTeacherService.getTeacherCompanies(teacherId);
                if (result.succeeded) {
                    this.teacherCompanies = result.data || [];
                    return this.teacherCompanies;
                }
                return [];
            } catch (error) {
                console.error('Error fetching teacher companies:', error);
                return [];
            } finally {
                this.loading = false;
            }
        },

        // Cập nhật chi nhánh cho giáo viên
        async updateTeacherCompanies(teacherId: string, companies: Partial<CompanyTeacherModel>[]): Promise<boolean> {
            this.loading = true;
            try {
                await serviceFactory.companyTeacherService.updateTeacherCompanies(teacherId, companies);
                // Refresh danh sách chi nhánh sau khi cập nhật
                await this.fetchTeacherCompanies(teacherId);
                return true;
            } catch (error) {
                console.error('Error updating teacher companies:', error);
                throw error;
            } finally {
                this.loading = false;
            }
        },

        // Thêm chi nhánh cho giáo viên
        async addCompanyToTeacher(data: Partial<CompanyTeacherModel>): Promise<boolean> {
            try {
                await serviceFactory.companyTeacherService.addCompanyToTeacher(data);
                return true;
            } catch (error) {
                console.error('Error adding company to teacher:', error);
                throw error;
            }
        },

        // Xóa chi nhánh khỏi giáo viên
        async removeCompanyFromTeacher(companyTeacherId: string): Promise<boolean> {
            try {
                await serviceFactory.companyTeacherService.removeCompanyFromTeacher(companyTeacherId);
                return true;
            } catch (error) {
                console.error('Error removing company from teacher:', error);
                throw error;
            }
        },

        // Clear store data
        clearData() {
            this.teacherCompanies = [];
        }
    },

    getters: {
        // Lấy chi nhánh chính
        mainCompany: (state) => {
            return state.teacherCompanies.find(company => company.companyCenter === 0);
        },

        // Lấy chi nhánh phụ
        subCompanies: (state) => {
            return state.teacherCompanies.filter(company => company.companyCenter === 1);
        },

        // Lấy tất cả companyIds
        companyIds: (state) => {
            return state.teacherCompanies.map(company => company.companyId);
        }
    }
});