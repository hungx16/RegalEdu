import { defineStore } from 'pinia';
import type { SkillModel, SkillQuery } from '@/api/SkillApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useSkillStore = defineStore('Skill', {
    state: () => ({
        skills: [] as SkillModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                categoryCode: '',
                categoryName: '',
            },
        } as SkillQuery,
        selectedSkill: null as SkillModel | null,
    }),
    actions: {
        async fetchAllSkills() {
            this.loading = true;
            try {
                const res = await serviceFactory.skillService.fetchAllSkills();
                if (res?.succeeded) {
                    this.skills = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedSkills() {
            this.loading = true;
            try {
                const res = await serviceFactory.skillService.fetchPagedSkills(this.query);
                if (res?.succeeded) {
                    this.skills = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },

        async saveSkill(model: Partial<SkillModel>) {
            await serviceFactory.skillService.saveSkill(model);
        },

        async deleteSkills(ids: string[]) {
            await serviceFactory.skillService.deleteSkills(ids);
        },

        async restoreSkills(ids: string[]) {
            await serviceFactory.skillService.restoreSkills(ids);
        },

        selectSkill(model: SkillModel | null) {
            this.selectedSkill = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedSkills();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedSkills();
        },

        async setFilter(filter: Partial<SkillQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedSkills();
        },
        async fetchDeletedSkills() {
            this.loading = true;
            try {
                const result = await serviceFactory.skillService.fetchDeletedSkills();
                if (result?.succeeded === true) {
                    this.skills = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted age groups:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
