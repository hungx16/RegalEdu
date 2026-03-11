// src/stores/divisionStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';

import type { GenerateCodeRequest, Province, DocumentType, WebsiteKey, Ward } from '@/api/CommonApi';
import en from '@/core/plugins/i18n/en';
export const useCommonStore = defineStore('common', {
  state: () => ({
    code: '',
    provinces: [] as Province[],
    documentTypes: [] as DocumentType[],
    websiteKeys: [] as WebsiteKey[],
    enWebsiteKeys: [] as WebsiteKey[],
    wards: [] as Ward[],
    generateCodeRequest: {
      prefix: '',
      tableName: '',
      columnName: '',
      length: 4,
    } as GenerateCodeRequest,
  }),
  getters: {
    // dùng cho TagList gợi ý
    websiteKeySuggestions: (state) => state.websiteKeys.map(k => k.key),
    enWebsiteKeySuggestions: (state) => state.enWebsiteKeys.map(k => k.key),
  },

  actions: {
    async generateCode(prefix: string, tableName: string, columnName: string, length: number = 4, format: string = '', year: number = 0, month: number = 0): Promise<string> {
      this.generateCodeRequest.prefix = prefix;
      this.generateCodeRequest.tableName = tableName;
      this.generateCodeRequest.columnName = columnName;
      this.generateCodeRequest.length = length;
      this.generateCodeRequest.format = format;
      this.generateCodeRequest.year = year;
      this.generateCodeRequest.month = month;
      try {
        const result = await serviceFactory.commonService.generateCode(this.generateCodeRequest);
        if (result?.succeeded) {
          this.code = result.data;
          return this.code;
        } else {
          return '';
        }
      } catch (error) {
        return '';
      }
    },
    async fetchProvinces() {
      try {
        const result = await serviceFactory.commonService.fetchProvinces();
        if (result?.succeeded) {
          this.provinces = result.data;
        } else {
          return [];
        }
      } catch (error) {
        return [];
      }
    },
    async fetchWards(provinceCode: string) {
      try {
        const result = await serviceFactory.commonService.fetchWards(provinceCode);
        if (result?.succeeded) {
          this.wards = result.data;
        } else {
          return [];
        }
      } catch (error) {
        return [];
      }
    },
    async fetchDocumentTypes() {
      try {
        const result = await serviceFactory.commonService.fetchDocumentTypes();
        if (result?.succeeded) {
          this.documentTypes = result.data;
        } else {
          return [];
        }
      } catch (error) {
        return [];
      }
    },
    async fetchWebsiteKeys() {
      try {
        const result = await serviceFactory.commonService.fetchWebsiteKeys();
        if (result?.succeeded) {
          this.websiteKeys = result.data;
          console.log('Fetched website keys:', result.data);
        } else {
          return [];
        }
      } catch (error) {
        return [];
      }
    },
    async fetchEnWebsiteKeys() {
      try {
        const result = await serviceFactory.commonService.fetchEnWebsiteKeys();
        if (result?.succeeded) {
          this.enWebsiteKeys = result.data;
          console.log('Fetched EN website keys:', result.data);
        } else {
          return [];
        }
      } catch (error) {
        return [];
      }
    },

    /** đảm bảo key tồn tại trong mảng (không phân biệt hoa/thường) */
    ensureWebsiteKey(key: string) {
      const found = this.websiteKeys.find(k => k.key.toLowerCase() === key.toLowerCase());
      if (!found) this.websiteKeys.push({ key, frequency: 0 });
    },

    /** +delta; nếu chưa có thì tạo mới */
    incrementWebsiteKey(key: string, delta = 1) {
      if (!key) return;
      this.ensureWebsiteKey(key);
      const item = this.websiteKeys.find(k => k.key.toLowerCase() === key.toLowerCase());
      if (item) item.frequency = Math.max(0, (item.frequency ?? 0) + delta);
    },

    /** -delta; nếu về 0 thì xoá khỏi mảng */
    decrementWebsiteKey(key: string, delta = 1) {
      if (!key) return;
      const idx = this.websiteKeys.findIndex(k => k.key.toLowerCase() === key.toLowerCase());
      if (idx === -1) return;
      const item = this.websiteKeys[idx];
      item.frequency = Math.max(0, (item.frequency ?? 0) - Math.abs(delta));
      if (item.frequency <= 0) this.websiteKeys.splice(idx, 1);
    },

    /** Dùng cho TagList */
    addOrIncreaseWebsiteKey(key: string) {
      if (!key) return;
      const f = this.websiteKeys.find(k => k.key.toLowerCase() === key.toLowerCase());
      if (f) f.frequency += 1;
      else this.websiteKeys.push({ key, frequency: 1 });
    },

    /** Dùng cho TagList */
    decreaseOrRemoveWebsiteKey(key: string) {
      if (!key) return;
      const idx = this.websiteKeys.findIndex(k => k.key.toLowerCase() === key.toLowerCase());
      if (idx === -1) return;
      const it = this.websiteKeys[idx];
      if ((it.frequency ?? 0) > 1) it.frequency -= 1;
      else this.websiteKeys.splice(idx, 1);
    },

    /** Tách chuỗi raw '#$#' thành mảng key */
    splitRawKeys(raw: string | string[] | null | undefined): string[] {
      if (Array.isArray(raw)) return raw.map(s => String(s).trim()).filter(Boolean);
      return (raw ?? '')
        .toString()
        .split('#$#')
        .map(s => s.trim())
        .filter(Boolean);
    },

    /** Chuẩn hoá key để đếm tần số ổn định */
    normalizeKey(k: string) {
      return (k ?? '').replace(/\s+/g, ' ').trim();
    },

    /**
     * Dự phóng websiteKeys mới sau khi áp dụng delta (lowercase key -> change).
     * Ví dụ delta: { "ai": -2, "cloud": -1 }
     */
    projectWebsiteKeysWithDelta(
      current: { key: string; frequency: number }[],
      deltaIndex: Record<string, number>
    ): { key: string; frequency: number }[] {
      const map = new Map<string, { key: string; frequency: number }>();
      current.forEach(w => map.set(w.key.toLowerCase(), { key: w.key, frequency: w.frequency ?? 0 }));

      for (const [lower, change] of Object.entries(deltaIndex)) {
        if (!change) continue;
        const cur = map.get(lower);
        if (cur) {
          cur.frequency = (cur.frequency ?? 0) + change;
          if (cur.frequency <= 0) map.delete(lower);
        } else if (change > 0) {
          map.set(lower, { key: lower, frequency: change }); // sẽ được "làm đẹp" khi rebuild
        }
      }
      return Array.from(map.values());
    },

    /** Rebuild toàn bộ websiteKeys từ danh sách documents (đếm tần số) */
    rebuildWebsiteKeysFromDocuments(docs: Array<{ websiteKeys: string | string[] | null | undefined }>) {
      const freq: Record<string, number> = {};
      const originalCase: Record<string, string> = {};

      for (const d of docs || []) {
        const keys = this.splitRawKeys(d?.websiteKeys);
        for (const raw of keys) {
          const clean = this.normalizeKey(raw);
          if (!clean) continue;
          const lower = clean.toLowerCase();
          freq[lower] = (freq[lower] ?? 0) + 1;
          if (!originalCase[lower]) originalCase[lower] = clean;
        }
      }

      this.websiteKeys = Object.entries(freq).map(([lower, f]) => ({
        key: originalCase[lower],
        frequency: f
      }));
    },
    /** đảm bảo EN key tồn tại trong mảng (không phân biệt hoa/thường) */
    ensureEnWebsiteKey(key: string) {
      const found = this.enWebsiteKeys.find(k => k.key.toLowerCase() === key.toLowerCase());
      if (!found) this.enWebsiteKeys.push({ key, frequency: 0 });
    },

    /** EN +delta; nếu chưa có thì tạo mới */
    incrementEnWebsiteKey(key: string, delta = 1) {
      if (!key) return;
      this.ensureEnWebsiteKey(key);
      const item = this.enWebsiteKeys.find(k => k.key.toLowerCase() === key.toLowerCase());
      if (item) item.frequency = Math.max(0, (item.frequency ?? 0) + delta);
    },

    /** EN -delta; nếu về 0 thì xoá khỏi mảng */
    decrementEnWebsiteKey(key: string, delta = 1) {
      if (!key) return;
      const idx = this.enWebsiteKeys.findIndex(k => k.key.toLowerCase() === key.toLowerCase());
      if (idx === -1) return;
      const item = this.enWebsiteKeys[idx];
      item.frequency = Math.max(0, (item.frequency ?? 0) - Math.abs(delta));
      if (item.frequency <= 0) this.enWebsiteKeys.splice(idx, 1);
    },

    /** EN – Dùng cho TagList */
    addOrIncreaseEnWebsiteKey(key: string) {
      if (!key) return;
      const f = this.enWebsiteKeys.find(k => k.key.toLowerCase() === key.toLowerCase());
      if (f) f.frequency += 1;
      else this.enWebsiteKeys.push({ key, frequency: 1 });
    },

    /** EN – Dùng cho TagList */
    decreaseOrRemoveEnWebsiteKey(key: string) {
      if (!key) return;
      const idx = this.enWebsiteKeys.findIndex(k => k.key.toLowerCase() === key.toLowerCase());
      if (idx === -1) return;
      const it = this.enWebsiteKeys[idx];
      if ((it.frequency ?? 0) > 1) it.frequency -= 1;
      else this.enWebsiteKeys.splice(idx, 1);
    },

    /**
     * EN – Dự phóng enWebsiteKeys mới sau khi áp dụng delta (lowercase key -> change).
     * Ví dụ delta: { "ai": -2, "cloud": -1 }
     */
    projectEnWebsiteKeysWithDelta(
      current: { key: string; frequency: number }[],
      deltaIndex: Record<string, number>
    ): { key: string; frequency: number }[] {
      const map = new Map<string, { key: string; frequency: number }>();
      current.forEach(w => map.set(w.key.toLowerCase(), { key: w.key, frequency: w.frequency ?? 0 }));

      for (const [lower, change] of Object.entries(deltaIndex)) {
        if (!change) continue;
        const cur = map.get(lower);
        if (cur) {
          cur.frequency = (cur.frequency ?? 0) + change;
          if (cur.frequency <= 0) map.delete(lower);
        } else if (change > 0) {
          map.set(lower, { key: lower, frequency: change }); // sẽ "làm đẹp" khi rebuild
        }
      }
      return Array.from(map.values());
    },

    /** EN – Rebuild toàn bộ enWebsiteKeys từ danh sách documents (đếm tần số) */
    rebuildEnWebsiteKeysFromDocuments(docs: Array<{ enWebsiteKeys: string | string[] | null | undefined }>) {
      const freq: Record<string, number> = {};
      const originalCase: Record<string, string> = {};

      for (const d of docs || []) {
        const keys = this.splitRawKeys(d?.enWebsiteKeys);
        for (const raw of keys) {
          const clean = this.normalizeKey(raw);
          if (!clean) continue;
          const lower = clean.toLowerCase();
          freq[lower] = (freq[lower] ?? 0) + 1;
          if (!originalCase[lower]) originalCase[lower] = clean;
        }
      }

      this.enWebsiteKeys = Object.entries(freq).map(([lower, f]) => ({
        key: originalCase[lower],
        frequency: f
      }));
    },

  }
});
