<template>
  <div id="app">
    <div class="alert alert-danger icon-custom-alert" v-if="errors.length > 0">
      <i class="mdi mdi-close alert-icon"></i>
      <div v-for="error in errors" v-html="error"></div>
    </div>
    <Button
      label="Lưu lại"
      icon="pi pi-save"
      class="p-button-success p-button-sm"
      style="float: right; margin-top: 4px; margin-right: 4px"
      @click="save_data"
    ></Button>
    <DataFields></DataFields>
    <Wfd ref="wfd" :height="600" :lang="lang" />
  </div>
</template>
<script setup>
import Button from "primevue/button";
import { useProcess } from "../../stores/Process/store";
import Wfd from "./Wfd.vue";
import { rand } from "../../utilities/rand";
import DataFields from "./DataFields.vue";
import { onMounted, ref } from "vue";
import { storeToRefs } from "pinia";
import Api from "../../api/Api";
import { useRouter } from "vue-router";
const router = useRouter();
const store = useProcess();
var new_process_id = rand();
var startNode1 = rand();
var taskNode1 = rand();
var taskNode2 = rand();
var successNode = rand();
var failNode = rand();
const errors = ref([]);
const modalVisible = ref(false);
const lang = ref("vi");
const demoData = ref({
  nodes: [
    {
      id: startNode1,
      process_id: new_process_id,
      x: 50,
      y: 200,
      label: "Bắt đầu",
      stt: 0,
      clazz: "start",
      variable: rand(),
      type_performer: 1,
    },
    {
      id: taskNode1,
      process_id: new_process_id,
      x: 300,
      y: 200,
      label: "Biểu mẫu",
      stt: 1,
      clazz: "formTask",
      type_performer: 1,
      data_setting: {},
      variable: rand(),
      fields: [],
    },
    {
      id: taskNode2,
      process_id: new_process_id,
      x: 500,
      y: 200,
      label: "Phê duyệt",
      stt: 2,
      clazz: "approveTask",
      variable: rand(),
      type_performer: 1,
      data_setting: {},
    },
    {
      id: successNode,
      process_id: new_process_id,
      x: 800,
      y: 200,
      label: "Hoàn thành",
      stt: 3,
      clazz: "success",
      variable: rand(),
      type_performer: 1,
    },
    {
      id: failNode,
      process_id: new_process_id,
      x: 800,
      y: 320,
      label: "Thất bại",
      stt: 3,
      clazz: "fail",
      variable: rand(),
      type_performer: 1,
    },
  ],
  edges: [
    {
      id: rand(),
      process_id: new_process_id,
      source: startNode1,
      target: taskNode1,
      sourceAnchor: 1,
      targetAnchor: 3,
      variable: rand(),
      clazz: "flow",
    },
    {
      id: rand(),
      process_id: new_process_id,
      source: taskNode1,
      target: taskNode2,
      sourceAnchor: 1,
      targetAnchor: 3,
      clazz: "flow",
      label: "Gửi đi",
      variable: rand(),
    },
    {
      id: rand(),
      process_id: new_process_id,
      source: taskNode2,
      target: successNode,
      sourceAnchor: 1,
      targetAnchor: 3,
      variable: rand(),
      clazz: "flow",
      label: "Đồng ý",
    },
    {
      id: rand(),
      process_id: new_process_id,
      source: taskNode2,
      target: failNode,
      sourceAnchor: 2,
      variable: rand(),
      targetAnchor: 2,
      clazz: "flow",
      label: "Không đồng ý",
      reverse: true,
    },
  ],
  model: {
    id: new_process_id,
    group_id: 1,
    name: "",
  },
});
const { departments, users, groups, data } = storeToRefs(store);
const props = defineProps({
  process_id: String,
});
const save_data = () => {
  if (vaild()) {
    var item = data.value.model;
    var nodes = data.value.nodes;
    var edges = data.value.edges;
    nodes = nodes.map(function (d) {
      delete d.type;
      delete d.shape;
      delete d.size;
      delete d.style;
      return d;
    });
    edges = edges.map(function (d) {
      delete d.sourceNode;
      delete d.targetNode;
      delete d.endPoint;
      delete d.startPoint;
      delete d.style;
      return d;
    });
    item = $.extendext(true, "replace", {}, data.value.model, {
      blocks: nodes,
      links: edges,
    });
    // console.log(item);s
    // $(".preloader").fadeIn();

    Api.saveprocess(item).then((res) => {
      router.push("/process");
    });
  }
};
const vaild = () => {
  var model = data.value.model;
  errors.value = [];
  if (!model.name || model.name == "") {
    errors.value.push("- <b>Tên quy trình</b> chưa nhập!");
  }
  if (!model.group_id || model.group_id == "") {
    errors.value.push("- <b>Nhóm quy trình</b> chưa chọn!");
  }
  if (errors.value.length) {
    return false;
  }
  errors.value = [];
  return true;
};
onMounted(() => {
  if (props.process_id) {
    store.init(props.process_id);
  } else {
    data.value = demoData.value;
  }

  /// Lấy department
  Api.department().then((res) => {
    departments.value = res;
  });
  /// Lấy users
  Api.employee().then((res) => {
    users.value = res;
  });
  /// Lấy processgroup
  Api.processgroup().then((res) => {
    groups.value = res;
  });
});
</script>
<style scoped>
#app {
  color: #2c3e50;
  border: 1px solid #e1e1e1;
}
</style>
