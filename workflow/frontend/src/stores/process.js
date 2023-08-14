import { ref, computed } from "vue";
import { defineStore, storeToRefs } from "pinia";
import Api from "../api/Api";
import moment from "moment";
import { rand } from "../utilities/rand";
import { useAuth } from "../stores/auth";
const store_auth = useAuth();
const { user } = storeToRefs(store_auth);
export const useProcess = defineStore("process", () => {
  const data_transition = ref([]);
  const data_activity = ref([]);
  const data = ref({});
  const data_custom_block = ref([]);
  const groups = ref([]);
  const roles = ref([]);
  const departments = ref([]);
  const users = ref([]);
  const model = ref({});
  const selectedModel = ref({});
  const editTitle = ref(false);
  const graph = ref();
  const nodes = computed(() => {
    return data.value.nodes;
  });
  const edges = computed(() => {
    return data.value.edges;
  });
  const prev_nodes = computed(() => {
    return nodes.value.filter(function (item) {
      return item.stt < selectedModel.value.stt;
    });
  });
  const prev_nodes_print = computed(() => {
    return nodes.value.filter(function (item) {
      return item.stt < selectedModel.value.stt && item.clazz == "printSystem";
    });
  });
  const prev_nodes_form = computed(() => {
    return nodes.value.filter(function (item) {
      return item.stt < selectedModel.value.stt && item.clazz == "formTask";
    });
  });
  const init = (process_id) => {
    return Api.process(process_id).then((res) => {
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

  const findIndexNode = (id) => {
    let index = data.value.nodes.findIndex(function (item) {
      return item.id == id;
    });
    return index;
  };
  const findIndexEdge = (id) => {
    let index = data.value.edges.findIndex(function (item) {
      return item.id == id;
    });
    return index;
  };
  const findItembyId = (id) => {
    let index = data.value.nodes.findIndex(function (item) {
      return item.id == id;
    });

    let item;
    if (index == -1) {
      index = data.value.edges.findIndex(function (item) {
        return item.id == id;
      });
      item = data.value.edges[index];
    } else {
      item = data.value.nodes[index];
    }
    return item;
  };
  const create_next = (activity) => {
    var node = graph.value.find("node", (node) => {
      return node.get("model").id == activity.block_id;
    });
    if (activity.clazz == "inclusiveGateway") {
      var ins = node.getInEdges();
      var transitions = data_transition.value.filter(function (item) {
        return item.to_activity_id == activity.id;
      });
      if (transitions.length < ins.length) {
        activity.blocking = true;
      } else {
        activity.blocking = false;
      }
      ////
      if (!activity.is_new) activity.is_update = true;
    }
    if (activity.blocking) return;
    // var data_custom_block = this.data_custom_block;
    // var data_activity = this.data_activity;
    // var data_transition = this.data_transition;

    var outs = node.getOutEdges();
    if (outs.length) {
      for (var out of outs) {
        var source = out.getSource();
        var target = out.getTarget();

        var transition = {
          is_new: true,
          label: out.get("model").label,
          reverse: out.get("model").reverse,
          link_id: out.get("model").id,
          execution_id: null,
          from_block_id: source.get("model").id,
          to_block_id: target.get("model").id,
          from_activity_id: activity.id,
          //to_activity_id: activity.id,
          stt: data_transition.value.length + 1,
          created_at: moment().valueOf(),
          id: rand(),
        };
        data_transition.value.push(transition);

        ///CREATE TARGET ACTIVITY

        var create_new = true;
        if (target.get("model").clazz == "inclusiveGateway") {
          var find_activity = data_activity.value.findLastIndex(function (
            item
          ) {
            return item.block_id == target.get("model").id;
          });
          if (find_activity != -1) {
            create_new = false;
            activity_new = data_activity.value[find_activity];
          }
        }
        if (create_new) {
          var data_setting = target.get("model").data_setting || {};
          var blocking = false;
          if (
            target.get("model").clazz == "formTask" ||
            target.get("model").clazz == "approveTask" ||
            target.get("model").clazz == "suggestTask" ||
            target.get("model").clazz == "mailSystem" ||
            target.get("model").clazz == "printSystem"
          ) {
            blocking = true;
          }
          var activity_new = {
            execution_id: null,
            label: target.get("model").label,
            block_id: target.get("model").id,
            variable: target.get("model").variable,
            stt: data_activity.value[data_activity.value.length - 1].stt + 1,
            clazz: target.get("model").clazz,
            is_new: true,
            executed: !blocking,
            failed: false,
            blocking: blocking,
            data_setting: data_setting,
            in_transition_id: transition.id,
            created_by: user.value.id,
            created_at: moment().valueOf(),
            id: rand(),
          };
          data_activity.value.push(activity_new);
        }

        transition.to_activity_id = activity_new.id;
        ////Custom Block
        if (blocking) {
          var findCustomBlock = data_custom_block.value.findIndex(function (
            item
          ) {
            return item.block_id == target.get("model").id;
          });
          if (findCustomBlock == -1) {
            var data_setting_block = target.get("model").data_setting || {};
            var type_performer = target.get("model").type_performer;
            var data_setting = {};
            if (type_performer == 1 && data_setting_block.block_id == null) {
              data_setting.type_performer = 4;
              data_setting.listuser = [user.value.id];
            } else if (
              type_performer == 1 &&
              data_setting_block.block_id != null
            ) {
              data_setting.type_performer = 4;
              var findIndexActivity = data_activity.value.findLastIndex(
                function (item) {
                  return item.block_id == data_setting_block.block_id;
                }
              );
              var findActivity = data_activity.value[findIndexActivity];
              data_setting.listuser = [findActivity.created_by];
            } else if (type_performer == 3 || type_performer == 4) {
              data_setting = data_setting_block;
              data_setting.type_performer = type_performer;
            } else if (type_performer == 5) {
              data_setting.type_performer = 4;
              data_setting.listuser = [model.value.user_id];
            }
            var custom_block = {
              data_setting: data_setting,
              block_id: target.get("model").id,
              is_new: true,
            };
            data_custom_block.value.push(custom_block);
          }
        }
        create_next(activity_new);
      }
    }
  };
  const active_activity = () => {
    var node_blocks = graph.value.findAll("node", (node) => {
      return node.get("model").active == true;
    });
    for (var node of node_blocks) {
      var index_activity_block = data_activity.value.findLastIndex(function (
        i
      ) {
        return i.block_id == node.get("model").id;
      });
      if (index_activity_block == -1) {
        continue;
      }
      var activity = data_activity.value[index_activity_block];
      if (hasPermission(activity)) {
        graph.value.emit("node:click", { item: node });
      }
    }
  };

  const hasPermission = (activity) => {
    var data_setting = activity.data_setting || {};
    var type_performer = data_setting.type_performer;
    var current_user = user.value;
    var user_id = current_user.id;
    var user_department = current_user.departments.map(function (item) {
      return item.department_id;
    });
    if (type_performer == 4) {
      var listuser = data_setting.listuser || [];
      var result = listuser.filter(function (n) {
        return n == user_id;
      });
      if (result.length > 0) return true;
    } else if (type_performer == 3) {
      var listdepartment = data_setting.listdepartment || [];
      var result = listdepartment.filter(function (n) {
        return user_department.indexOf(n) !== -1;
      });
      if (result.length > 0) return true;
    }
    return false;
  };

  const reset = () => {
    data_transition.value = [];
    data_activity.value = [];
    data_custom_block.value = [];
    selectedModel.value = null;
    model.value = {};
    editTitle.value = false;
    data.value = {};
  };
  const fetchUsers = () => {
    if (users.value.length) return;
    Api.employee().then((res) => {
      users.value = res;
    });
  };

  const fetchDepartments = () => {
    if (departments.value.length) return;
    Api.department().then((res) => {
      departments.value = res;
    });
  };

  const fetchGroups = () => {
    if (groups.value.length) return;
    Api.processgroup().then((res) => {
      groups.value = res;
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
    model,
    editTitle,
    graph,
    nodes,
    edges,
    prev_nodes,
    prev_nodes_print,
    prev_nodes_form,
    selectedModel,
    init,
    findIndexNode,
    findIndexEdge,
    findItembyId,
    create_next,
    active_activity,
    hasPermission,
    reset,
    fetchUsers,
    fetchGroups,
    fetchDepartments,
  };
});
