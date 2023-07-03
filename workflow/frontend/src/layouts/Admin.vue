<script setup>
import { computed, watch, ref, onMounted } from "vue";
import LeftSide from "../components/LeftSide.vue";
import Topbar from "../components/Topbar.vue";
import DynamicDialog from "primevue/dynamicdialog";
import { useLayout } from "@/layouts/composables/layout";

import { useAuth } from "../stores/auth";
const store = useAuth();
const response = await store.getUser();
if (response.success == false) {
  store.logout();
}
const { layoutConfig, layoutState, isSidebarActive } = useLayout();

const outsideClickListener = ref(null);

watch(isSidebarActive, (newVal) => {
  if (newVal) {
    bindOutsideClickListener();
  } else {
    unbindOutsideClickListener();
  }
});

const containerClass = computed(() => {
  return {
    "layout-theme-light": layoutConfig.darkTheme.value === "light",
    "layout-theme-dark": layoutConfig.darkTheme.value === "dark",
    "layout-overlay": layoutConfig.menuMode.value === "overlay",
    "layout-static": layoutConfig.menuMode.value === "static",
    "layout-static-inactive":
      layoutState.staticMenuDesktopInactive.value &&
      layoutConfig.menuMode.value === "static",
    "layout-overlay-active": layoutState.overlayMenuActive.value,
    "layout-mobile-active": layoutState.staticMenuMobileActive.value,
    "p-input-filled": layoutConfig.inputStyle.value === "filled",
    "p-ripple-disabled": !layoutConfig.ripple.value,
  };
});
const bindOutsideClickListener = () => {
  if (!outsideClickListener.value) {
    outsideClickListener.value = (event) => {
      if (isOutsideClicked(event)) {
        layoutState.overlayMenuActive.value = false;
        layoutState.staticMenuMobileActive.value = false;
        layoutState.menuHoverActive.value = false;
      }
    };
    document.addEventListener("click", outsideClickListener.value);
  }
};
const unbindOutsideClickListener = () => {
  if (outsideClickListener.value) {
    document.removeEventListener("click", outsideClickListener);
    outsideClickListener.value = null;
  }
};
const isOutsideClicked = (event) => {
  const sidebarEl = document.querySelector(".layout-sidebar");
  const topbarEl = document.querySelector(".layout-menu-button");

  return !(
    sidebarEl.isSameNode(event.target) ||
    sidebarEl.contains(event.target) ||
    topbarEl.isSameNode(event.target) ||
    topbarEl.contains(event.target)
  );
};
const timeoutInMS = 5 * 60 * 1000; // 3 minutes -> 3 * 60 * 1000
let timeoutId;

function handleInactive() {
  // Here you want to logout a user and/or ping your token
  // store.logout();
}

function startTimer() {
  // setTimeout returns an ID (can be used to start or clear a timer)
  // console.log("1 starrt");
  timeoutId = setTimeout(handleInactive, timeoutInMS);
}

function resetTimer() {
  clearTimeout(timeoutId);
  startTimer();
}

function setupTimers() {
  document.addEventListener("keypress", resetTimer, false);
  document.addEventListener("mousemove", resetTimer, false);
  document.addEventListener("mousedown", resetTimer, false);
  document.addEventListener("touchmove", resetTimer, false);

  startTimer();
}
onMounted(() => {
  // setupTimers();
});
</script>

<template>
  <div class="layout-wrapper" :class="containerClass">
    <Topbar></Topbar>
    <div class="layout-sidebar">
      <LeftSide></LeftSide>
    </div>
    <div class="layout-main-container">
      <div class="layout-main">
        <router-view></router-view>
      </div>
    </div>
    <div class="layout-mask"></div>
    <DynamicDialog> </DynamicDialog>
  </div>
</template>

<style lang="scss" scoped></style>
