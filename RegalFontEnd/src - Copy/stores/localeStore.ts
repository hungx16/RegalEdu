import { defineStore } from 'pinia'
import i18n from '@/core/plugins/i18n'
import { elementLocales } from '@/core/plugins/element-locale'

export const useLocaleStore = defineStore('locale', {
  state: () => ({
    currentLocale: 'vi' as 'vi' | 'en', // Chỉ hỗ trợ vi/en
  }),
  getters: {
    // Getter này trả về object locale tương ứng của Element Plus
    elementLocale: (state) => elementLocales[state.currentLocale],
  },
  actions: {
    setLocale(locale: 'vi' | 'en') {
      console.log('Setting locale to:', locale);

      this.currentLocale = locale
      localStorage.setItem('locale', locale)

      // Cập nhật ngôn ngữ cho vue-i18n
      i18n.global.locale.value = locale
    },
    loadLocale() {
      const saved = localStorage.getItem('locale') as 'vi' | 'en' | null
      if (saved && (saved === 'vi' || saved === 'en')) {
        this.setLocale(saved)
      } else {
        this.setLocale(this.currentLocale) // fallback default
      }
    },
  },
})
