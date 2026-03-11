import { fileURLToPath, URL } from "node:url";

//import { defineConfig } from "vite";
import { defineConfig } from 'vitest/config';

import vue from "@vitejs/plugin-vue";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      "vue-i18n": "vue-i18n/dist/vue-i18n.cjs.js",
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  base: "/",
  build: {
    chunkSizeWarningLimit: 3000,
  },
  server: {
    port: 3000, // 👉 cổng cố định bạn muốn
    host: '0.0.0.0', // (optional) nếu muốn truy cập từ LAN (các máy khác trong mạng)
    strictPort: true, // 👉 Nếu true → nếu cổng bị chiếm → báo lỗi, không tự chọn cổng khác
  },
  test: {
    globals: true,
    environment: 'jsdom', // nếu test component
    setupFiles: [],       // nếu có setup test
  },
});
