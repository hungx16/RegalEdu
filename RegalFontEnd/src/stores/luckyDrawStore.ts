import { defineStore } from "pinia";
import { serviceFactory } from "@/services/ServiceFactory";
import type {
  CustomerRewardModel,
  CustomerRewardQuery,
  LuckyDrawModel,
  LuckyDrawQuery,
  RewardModel,
  RewardQuery,
} from "@/api/LuckyDrawApi";

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useLuckyDrawStore = defineStore("luckyDrawStore", {
  state: () => ({
    luckyDraws: [] as LuckyDrawModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      name: "",
      branch: "",
      region: "",
    } as LuckyDrawQuery,

    customerRewards: [] as CustomerRewardModel[],
    customerRewardTotal: 0,
    customerRewardQuery: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      phoneOrName: "",
      luckyDrawId: undefined,
      companyId: undefined,
      regionId: undefined,
      receiveStatus: undefined,
      acceptanceStatus: undefined,
    } as CustomerRewardQuery,

    rewards: [] as RewardModel[],
    rewardTotal: 0,
    rewardQuery: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      name: "",
      type: "",
    } as RewardQuery,

    selectedLuckyDraw: null as LuckyDrawModel | null,
  }),

  actions: {
    async fetchPagedLuckyDraws() {
      const luckyDrawService = serviceFactory.luckyDrawService;
      this.loading = true;
      try {
        const result = await luckyDrawService.fetchPagedLuckyDraws(this.query);
        console.log("result.succeeded", result?.succeeded);
        console.log("result.data", result?.data);
        console.log("result.data.items", result?.data?.items);
        if (result?.succeeded === true) {
          this.luckyDraws = result.data.data?.items ?? [];
          console.log("after assign luckyDraws", this.luckyDraws);
          this.total = result.data.data?.total ?? 0;
          console.log("after assign total", this.total);
        }
      } finally {
        this.loading = false;
      }
    },

    async fetchAllActiveLuckyDraws() {
      const luckyDrawService = serviceFactory.luckyDrawService;
      this.loading = true;
      try {
        const result = await luckyDrawService.fetchAllActiveLuckyDraws();
        if (result?.succeeded === true) {
          this.luckyDraws = result.data;
          this.total = result.data.length;
        }
      } finally {
        this.loading = false;
      }
    },

    async fetchPagedCustomerRewards() {
      const luckyDrawService = serviceFactory.luckyDrawService;
      this.loading = true;
      try {
        const result = await luckyDrawService.fetchPagedCustomerRewards(
          this.customerRewardQuery,
        );
        if (result?.succeeded === true) {
          this.customerRewards = result.data.items;
          this.customerRewardTotal = result.data.total;
        }
      } finally {
        this.loading = false;
      }
    },

    async fetchPagedRewards() {
      const luckyDrawService = serviceFactory.luckyDrawService;
      this.loading = true;
      try {
        const result = await luckyDrawService.fetchPagedRewards(
          this.rewardQuery,
        );
        if (result?.succeeded === true) {
          this.rewards = result.data.items;
          this.rewardTotal = result.data.total;
        }
      } finally {
        this.loading = false;
      }
    },

    selectLuckyDraw(item: LuckyDrawModel | null) {
      this.selectedLuckyDraw = item;
    },

    async saveLuckyDraw(model: Partial<LuckyDrawModel>) {
      return await serviceFactory.luckyDrawService.saveLuckyDraw(model);
    },

    async deleteLuckyDraw(id: string) {
      return await serviceFactory.luckyDrawService.deleteLuckyDraw(id);
    },

    async confirmReceiveCustomerReward(id: string, note?: string) {
      return await serviceFactory.luckyDrawService.confirmReceiveCustomerReward(
        id,
        note,
      );
    },

    async confirmAcceptanceCustomerReward(id: string, note?: string) {
      return await serviceFactory.luckyDrawService.confirmAcceptanceCustomerReward(
        id,
        note,
      );
    },

    setPage(page: number) {
      this.query.page = page;
    },

    setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
    },

    setFilter(filter: Partial<LuckyDrawQuery>) {
      this.query = { ...this.query, ...filter, page: 1 };
    },
  },
});
