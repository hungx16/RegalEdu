import { defineStore } from 'pinia'
import { serviceFactory } from '@/services/ServiceFactory';
import type { FormPermissionDTO } from '@/api/accountGroupPermissionApi';

export const userPermissionStore = defineStore('permission', {
  state: () => ({
    listMenuAccept: null as string[] | null,
    listMenuAndPermission: null as FormPermissionDTO[] | null,
    hasLoaded: false

  }),
  getters: {
    getListMenuAccept: (state) => state.listMenuAccept,
    getListMenuAndPermission: (state) => state.listMenuAndPermission,
  },
  actions: {
    async loadResource() {
      if (this.hasLoaded) return;
      if (this.listMenuAccept == null) {
        await this.loadMenuAccept();
      }
      if (this.listMenuAndPermission == null) {
        await this.loadMenuAndPermission();
      }
      this.hasLoaded = true;

    },
    async loadMenuAccept() {
      let listMenu: Array<string> = [];
      const token = localStorage.getItem('accessToken');
      if (!token) return;
      const res = await serviceFactory.accountGroupPermissionService.getMenuAccept();
      if (res && res.succeeded === true) {
        listMenu = res.data;
      }
      this.listMenuAccept = listMenu;
      //console.log('Vao day');
      //window.location.reload(); // Reload the page to apply new permissions
    },
    async loadMenuAndPermission() {
      const token = localStorage.getItem('accessToken');
      if (!token) return;
      let listData: Array<FormPermissionDTO> = [];
      const res = await serviceFactory.accountGroupPermissionService.getMenuAndPermissionAccept();
      if (res && res.succeeded === true) {
        listData = res.data;
      }
      this.listMenuAndPermission = listData;
    },
    reset() {
      this.hasLoaded = false;
      this.listMenuAccept = null;
      this.listMenuAndPermission = null;
    }
  },
});