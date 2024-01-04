<template>
  <div class="left-sidenav">
    <ul class="metismenu left-sidenav-menu">

      <PopupExecution></PopupExecution>

      <li v-if="is_admin">
        <router-link class="nav-link" to="/">
          <i class="fas fa-chart-bar"></i>
          <span>Tổng quan</span>
        </router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/execution/wait">
          <i class="fas fa-spinner"></i>
          <span>Cần thực hiện</span>
        </router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/execution/userdone">
          <i class="far fa-check-circle"></i>
          <span>Đã thực hiện</span>
        </router-link>
      </li>

      <li v-if="is_admin || is_manager">
        <router-link class="nav-link" to="/execution">
          <i class="fas fa-list"></i>
          <span>Tất cả lượt chạy</span>
        </router-link>
      </li>
      <li v-if="is_admin || is_manager">
        <router-link class="nav-link" to="/history">
          <i class="fas fa-history"></i>
          <span>Audittrails</span>
        </router-link>
      </li>

      <li v-if="is_admin || is_manager">
        <a href="javascript: void(0);">
          <i class="ti-briefcase"></i>
          <span>{{ $t("system") }}</span>
          <span class="menu-arrow"><i class="mdi mdi-chevron-right"></i></span>
        </a>

        <ul class="nav-second-level" aria-expanded="false">
          <li class="nav-item" v-if="is_admin">
            <router-link class="nav-link" to="/user"><i class="ti-control-record"></i>{{ $t("user") }}</router-link>
          </li>
          <li class="nav-item" v-if="is_admin">
            <router-link class="nav-link" to="/department"><i class="ti-control-record"></i>Bộ phận</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" to="/processgroup"><i class="ti-control-record"></i>Nhóm quy trình</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" to="/process"><i class="ti-control-record"></i>Quy trình</router-link>
          </li>
        </ul>
      </li>
    </ul>
  </div>
</template>
<script setup>
import { useAuth } from "../stores/auth";
import { onMounted, ref } from "vue";
import { useLayout } from "../layouts/composables/layout";
import { storeToRefs } from "pinia";
import PopupExecution from "./PopupExecution.vue";

const { initMetisMenu, initActiveMenu } = useLayout();
const store = useAuth();
const { is_admin, is_manager } = storeToRefs(store);

onMounted(() => {
  initMetisMenu();
  initActiveMenu();
});
</script>
