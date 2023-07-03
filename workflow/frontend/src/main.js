import { createApp } from "vue";
import { createPinia } from "pinia";

import App from "./App.vue";
import router from "./router";
import PrimeVue from "primevue/config";
import ToastService from "primevue/toastservice";
import ConfirmationService from "primevue/confirmationservice";
import { i18n } from "./service/i18n";
import Tooltip from "primevue/tooltip";
import DialogService from "primevue/dialogservice";
import { FileManagerPlugin } from "@syncfusion/ej2-vue-filemanager";
import Accordion from "primevue/accordion";
import AccordionTab from "primevue/accordiontab";
import Calendar from "primevue/calendar";
import TreeSelect from "vue3-acies-treeselect";
// import the styles
import "vue3-acies-treeselect/dist/vue3-treeselect.css";
// import { registerLicense } from '@syncfusion/ej2-base';

// import "primevue/resources/themes/bootstrap4-light-blue/theme.css"; //theme
import "@/assets/styles.scss";
import "@/assets/lib/sortable/sortable.css";

// registerLicense('ORg4AjUWIQA/Gnt2VFhiQlhPcUBFQmFJfFBmRGNTfF96dVFWACFaRnZdQV1lSXlRdEZnXHhZeXVd');
// registerLicense('tran');
const app = createApp(App);
app
  .component("Accordion", Accordion)
  .component("AccordionTab", AccordionTab)
  .component("Calendar", Calendar)
  .component("TreeSelect", TreeSelect);

app.use(FileManagerPlugin);
app.use(ConfirmationService);
app.use(DialogService);
app.use(ToastService);
app.use(PrimeVue);
app.use(i18n);
app.use(createPinia());
app.use(router);
app.directive("tooltip", Tooltip);
app.mount("#app");

// app.config.errorHandler = () => null;
// app.config.warnHandler = () => null;

import "../src/assets/js/jquery.min.js";
import "../src/assets/js/jquery-ui.min.js";
import "../src/assets/js/bootstrap.bundle.min.js";
import "../src/assets/js/metisMenu.min.js";

import "../src/assets/lib/deepmerge/jquery-extendext.js";
import "../src/assets/lib/sortable/jquery.mjs.nestedSortable.js";
import "../src/assets/lib/jquery-validation/dist/jquery.validate.js";
import "../src/assets/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js";
