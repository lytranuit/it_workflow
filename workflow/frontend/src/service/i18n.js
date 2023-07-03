import messages from "./lang";
import { createI18n } from "vue-i18n";
export const i18n = createI18n({
  legacy: false,
  locale: "vi",
  fallbackLocale: "vi",
  messages,
});
