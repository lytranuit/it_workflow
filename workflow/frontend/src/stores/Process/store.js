// import { createStore } from 'vuex'

// export default createStore({
//     state: {
//         data: {},
//         groups: [],
//         data_transition: [],
//         data_activity: [],
//         data_custom_block: [],
//         roles: null,
//         departments: null,
//         users: null,
//         current_user: {},
//     },

//     mutations: {
//         SET_DATA(state, data) {
//             state.data = data;
//         },
//         SET_DATA_TRANSITION(state, data) {
//             state.data_transition = data;
//         },
//         SET_DATA_ACTIVITY(state, data) {
//             state.data_activity = data;
//         },
//         SET_CUSTOM_BLOCK(state, data) {
//             state.data_custom_block = data;
//         },
//         SET_GROUP(state, data) {
//             state.groups = data;
//         },
//         SET_ROLES(state, data) {
//             state.roles = data;
//         },
//         SET_DEPARTMENTS(state, data) {
//             state.departments = data;
//         },
//         SET_USERS(state, data) {
//             state.users = data;
//         },
//         SET_CURRENT_USER(state, data) {
//             state.current_user = data;
//         }
//     }, actions: {
//         init({ commit }) {
//             $.ajax({
//                 url: path + "/admin/api/process",
//                 data: { id: process_id },
//                 success: function (res) {
//                     //console.log(data);
//                     let data = {};
//                     data.edges = res.links;
//                     var blocks = res.blocks.map(function (item) {
//                         var default_setting = { data_setting: {} };
//                         delete item.process;
//                         item = $.extendext(true, 'replace', default_setting, item);
//                         return item;
//                     });
//                     data.nodes = blocks;
//                     delete res.blocks;
//                     delete res.links;
//                     delete res.fields;
//                     data.model = res;
//                     commit('SET_DATA', data);
//                 }
//             })
//         },
//         async fetchRoles({ commit, state }) {
//             if (state.roles)
//                 return;
//             let data = await $.ajax({ url: path + "/admin/api/roles" });
//             commit('SET_ROLES', data);
//         },
//         async fetchDepartment({ commit, state }) {
//             if (state.departments)
//                 return;
//             let data = await $.ajax({ url: path + "/admin/api/department" });
//             commit('SET_DEPARTMENTS', data);
//         },
//         async fetchUsers({ commit, state }) {
//             if (state.users)
//                 return;
//             let data = await $.ajax({ url: path + "/admin/api/employee" });
//             commit('SET_USERS', data);
//         }
//     },
// });

import { ref, computed } from "vue";
import { defineStore } from "pinia";
import Api from "../../api/Api";
export const useProcess = defineStore("process", () => {
  const data_transition = ref([]);
  const data_activity = ref([]);
  const data = ref({});
  const data_custom_block = ref([]);
  const groups = ref([]);
  const roles = ref();
  const departments = ref();
  const users = ref();
  const current_user = ref({});
  const init = (process_id) => {
    Api.process(process_id).then((res) => {
      let data_tmp = {};
      data_tmp.edges = res.links;
      var blocks = res.blocks.map(function (item) {
        var default_setting = { data_setting: {} };
        delete item.process;
        item = $.extendext(true, "replace", default_setting, item);
        return item;
      });
      data_tmp.nodes = blocks;
      delete res.blocks;
      delete res.links;
      delete res.fields;
      data_tmp.model = res;
      data.value = data_tmp;
    });
  };
  return {
    data,
    data_transition,
    data_activity,
    data_custom_block,
    roles,
    departments,
    users,
    groups,
    current_user,
    init,
  };
});
