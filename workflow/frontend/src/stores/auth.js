import { ref, computed } from "vue";
import { defineStore } from "pinia";
// import VueJwtDecode from "vue-jwt-decode";

import { useCookies } from "vue3-cookies";
import authApi from "../api/authApi";
import userApi from "../api/userApi";
export const useAuth = defineStore("auth", () => {
  const data = ref({});
  const roles = ref([]);
  const departments = ref([]);
  const isAuth = computed(() => {
    const { cookies } = useCookies();
    const Token = cookies.get("Auth-Token");
    return Token ? true : false;
  });
  const user = computed(() => {
    const cacheUser = localStorage.getItem("me") || "{}";
    return JSON.parse(cacheUser);
  });
  const is_admin = computed(() => {
    return in_groups(["Administrator"]);
  });

  async function getUser() {
    const { cookies } = useCookies();
    const Token = cookies.get("Auth-Token");
    const cacheUser = localStorage.getItem("me");
    if (!cacheUser || JSON.parse(cacheUser).token != Token) {
      return authApi.TokenInfo(Token).then((response) => {
        // console.log(localStorage.getItem("me"));
        if (response.success) {
          localStorage.setItem("me", JSON.stringify(response));
        }
        return response;
      });
    }
    return JSON.parse(cacheUser);
  }
  async function logout() {
    localStorage.removeItem("me");
    // document.getElementById("logoutForm").submit();
    authApi.logout().then((res) => {
      location.href = "/Identity/Account/Login";
    });
  }
  async function fetchRoles() {
    if (roles.value.length) return;
    return userApi.roles().then((response) => {
      roles.value = response;
      return response;
    });
  }
  async function fetchDepartment() {
    if (departments.value.length) return;
    return userApi.departments().then((response) => {
      departments.value = response;
      return response;
    });
  }
  async function fetchData(id) {
    return userApi.get(id).then((response) => {
      data.value = response;
      return response;
    });
  }
  function in_groups(groups) {
    let re = false;
    let user_roles = user.value.roles;
    if (user_roles) {
      for (let d of user_roles) {
        if (groups.indexOf(d) != -1) {
          re = true;
          break;
        }
      }
    }
    return re;
  }
  return {
    data,
    roles,
    departments,
    isAuth,
    user,
    is_admin,
    getUser,
    logout,
    fetchRoles,
    fetchDepartment,
    fetchData,
  };
});
