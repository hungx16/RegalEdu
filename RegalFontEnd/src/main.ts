import { createApp, watch } from "vue";
import { createPinia } from "pinia";
import { Tooltip } from "bootstrap";
import App from "./App.vue";

/*
TIP: To get started with clean router change path to @/router/clean.ts.
 */
import router from "./router";
import ElementPlus from "element-plus";
import i18n from "@/core/plugins/i18n";

//imports for app initialization
// import ApiService from "@/core/services/ApiService";
import { initApexCharts } from "@/core/plugins/apexcharts";
import { initInlineSvg } from "@/core/plugins/inline-svg";
import { initVeeValidate } from "@/core/plugins/vee-validate";
import { initKtIcon } from "@/core/plugins/keenthemes";
import 'leaflet/dist/leaflet.css'

import "@/core/plugins/prismjs";
import { useLocaleStore } from "./stores/localeStore";

const app = createApp(App);

app.use(createPinia());
const localeStore = useLocaleStore();
localeStore.loadLocale();
app.use(i18n);
app.use(router);
app.use(ElementPlus, {
  locale: localeStore.elementLocale,
});
watch(
  () => i18n.global.locale.value,
  (newLocale) => {
    localeStore.setLocale(newLocale === 'vi' ? 'vi' : 'en');
  }
)
//ApiService.init(app);
initApexCharts(app);
initInlineSvg(app);
initKtIcon(app);
initVeeValidate();

app.use(i18n);

app.directive("tooltip", (el) => {
  new Tooltip(el);
});

app.mount("#app");
