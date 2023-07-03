import { toRefs, reactive, computed } from "vue";

const contextPath = import.meta.env.BASE_URL;

const layoutConfig = reactive({
  ripple: false,
  darkTheme: false,
  inputStyle: "outlined",
  menuMode: "static",
  theme: "lara-light-indigo",
  scale: 14,
  activeMenuItem: null,
});

const layoutState = reactive({
  staticMenuDesktopInactive: false,
  overlayMenuActive: false,
  profileSidebarVisible: false,
  configSidebarVisible: false,
  staticMenuMobileActive: false,
  menuHoverActive: false,
});

export function useLayout() {
  const changeThemeSettings = (theme, darkTheme) => {
    layoutConfig.darkTheme = darkTheme;
    layoutConfig.theme = theme;
  };

  const setScale = (scale) => {
    layoutConfig.scale = scale;
  };

  const setActiveMenuItem = (item) => {
    layoutConfig.activeMenuItem = item.value || item;
  };

  const onMenuToggle = () => {
    if (layoutConfig.menuMode === "overlay") {
      layoutState.overlayMenuActive = !layoutState.overlayMenuActive;
    }

    if (window.innerWidth > 991) {
      layoutState.staticMenuDesktopInactive =
        !layoutState.staticMenuDesktopInactive;
    } else {
      layoutState.staticMenuMobileActive = !layoutState.staticMenuMobileActive;
    }
  };

  const initMetisMenu = () => {
    //metis menu
    $(".metismenu").metisMenu();
  };

  const initActiveMenu = () => {
    // === following js will activate the menu in left side bar based on url ====
    $(".left-sidenav a").each(function () {
      var pageUrl = window.location.href.split(/[?#]/)[0];
      if (this.href == pageUrl) {
        $(this).addClass("active");
        $(this).parent().addClass("active"); // add active to li of the current link
        $(this).parent().addClass("mm-active");
        $(this).parent().parent().addClass("in");
        $(this).parent().parent().addClass("mm-show");
        $(this).parent().parent().parent().addClass("mm-active");
        $(this).parent().parent().prev().addClass("active"); // add active class to an anchor
        $(this).parent().parent().parent().addClass("active");
        $(this).parent().parent().parent().parent().addClass("mm-show"); // add active to li of the current link
        $(this)
          .parent()
          .parent()
          .parent()
          .parent()
          .parent()
          .addClass("mm-active");
      }
    });
  };

  const isSidebarActive = computed(
    () => layoutState.overlayMenuActive || layoutState.staticMenuMobileActive
  );

  const isDarkTheme = computed(() => layoutConfig.darkTheme);
  const curentLang = computed(() => localStorage.getItem("language") || "vi");
  return {
    contextPath,
    layoutConfig: toRefs(layoutConfig),
    layoutState: toRefs(layoutState),
    curentLang,
    changeThemeSettings,
    setScale,
    onMenuToggle,
    initMetisMenu,
    initActiveMenu,
    isSidebarActive,
    isDarkTheme,
    setActiveMenuItem,
  };
}
