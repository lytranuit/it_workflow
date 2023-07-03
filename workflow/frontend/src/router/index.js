import { createRouter, createWebHistory } from "vue-router";
import defaultLayout from "../layouts/Default.vue";
import AdminLayout from "../layouts/Admin.vue";

import routes from "~pages";
import { i18n } from "../service/i18n";

const routes_1 = routes.map((item) => {
  item.meta = {
    layout: AdminLayout,
    transition: "fade",
  };
  return item;
});
routes_1.push({
  path: "/:pathMatch(.*)*",
  component: () => import("../views/404.vue"),
  meta: { layout: defaultLayout },
});
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: routes_1,
});
router.beforeEach((toRoute, fromRoute, next) => {
  i18n.global.locale.value = localStorage.getItem("language") || "vi";
  const title =
    toRoute.meta && toRoute.meta.title ? toRoute.meta.title : "Pymepharco";
  document.title = title;
  next();
});
export default router;
