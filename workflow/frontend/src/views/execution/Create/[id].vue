<template>
  <Execution
    :process_version_id="route.params.id"
    ref="execution_ref"
    :key="route.query.time"
  ></Execution>
</template>

<script setup>
import { onMounted, ref } from "vue";
import Execution from "../../../components/Execution/index.vue";
import { useRoute } from "vue-router";
import { storeToRefs } from "pinia";
import { useProcess } from "../../../stores/process";
import { useAuth } from "../../../stores/auth";
import moment from "moment";
import { rand } from "../../../utilities/rand";
import Api from "../../../api/Api";
const route = useRoute();
const store_auth = useAuth();
const { user } = storeToRefs(store_auth);
const execution_ref = ref();
const store = useProcess();
const {
  departments,
  users,
  groups,
  data,
  model,
  graph,
  editTitle,
  data_custom_block,
  data_transition,
  data_activity,
} = storeToRefs(store);
onMounted(() => {
  /// Lấy department
  Api.department().then((res) => {
    departments.value = res;
  });
  /// Lấy users
  Api.employee().then((res) => {
    users.value = res;
  });
  // setTimeout(function () {
  //   //// activity START
  //   console.log(graph.value);
  //   var start = graph.value.find("node", (node) => {
  //     return node.get("model").clazz === "start";
  //   });
  //   var data_activity_tmp = [];
  //   var activity_start = {
  //     execution_id: null,
  //     label: start.get("model").label,
  //     block_id: start.get("model").id,
  //     stt: 1,
  //     clazz: start.get("model").clazz,
  //     is_new: true,
  //     executed: true,
  //     failed: false,
  //     blocking: false,
  //     created_by: user.id,
  //     created_at: moment().valueOf(),
  //     id: rand(),
  //   };
  //   data_activity_tmp.push(activity_start);
  //   data_activity.value = data_activity_tmp;

  //   setTimeout(function () {
  //     // console.log(execution_ref.value);
  //     store.create_next(activity_start);
  //     var data2 = initShape();
  //     graph.value.read(data2);
  //     graph.value.fitView();
  //     graph.value.executeCommand("currentFlow");
  //     //// active block lên
  //     var node_blocks = graph.value.findAll("node", (node) => {
  //       return node.get("model").active == true;
  //     });
  //     for (var node of node_blocks) {
  //       var index_activity_block = data_activity.value.findLastIndex(function (
  //         i
  //       ) {
  //         return i.block_id == node.get("model").id;
  //       });
  //       if (index_activity_block == -1) {
  //         continue;
  //       }
  //       var activity = data_activity.value[index_activity_block];
  //       if (hasPermission(activity)) {
  //         graph.value.emit("node:click", { item: node });
  //       }
  //     }

  //     /////Bỏ loadding
  //     waiting.value = false;

  //     //console.log(node_block);
  //   }, 100);
  // }, 100);
});
</script>
