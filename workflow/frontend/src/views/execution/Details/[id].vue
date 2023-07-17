<template>
  <Execution
    :process_version_id="route.params.id"
    :execution_id="route.query.execution_id"
    :key="route.query.time"
  ></Execution>
</template>

<script setup>
import { onMounted } from "vue";
import Execution from "../../../components/Execution/index.vue";
import { useRoute } from "vue-router";
import { useProcess } from "../../../stores/process";
import Api from "../../../api/Api";
import { storeToRefs } from "pinia";

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
const route = useRoute();
onMounted(() => {
  /// Lấy department
  Api.department().then((res) => {
    departments.value = res;
  });
  /// Lấy users
  Api.employee().then((res) => {
    users.value = res;
  });
});
</script>
