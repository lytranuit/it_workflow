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
    <div class="root">
      <ToolbarPanel ref="toolbar_ref" :mode="mode" />
      <div style="display: flex">
        <ItemPanel
          ref="addItemPanel_ref"
          :height="height"
          :model="data.model"
          v-if="mode == 'edit'"
        />
        <div
          ref="canvas"
          class="canvasPanel"
          :style="{
            height: height + 'px',
            width: mode != 'edit' ? '100%' : '65%',
          }"
        ></div>
        <DetailPanel
          ref="detailPanel"
          v-if="mode == 'edit'"
          :height="height"
          :model="selectedModel"
          :readOnly="mode !== 'edit'"
          :onChange="
            (key, val) => {
              onItemCfgChange(key, val);
            }
          "
        />
      </div>
    </div>
  </div>
</template>
<script setup>
import G6 from "@antv/g6/lib";
import Button from "primevue/button";
import { useProcess } from "../../stores/process";
// import Wfd from "./Wfd.vue";
import { rand } from "../../utilities/rand";
import DataFields from "./DataFields.vue";
import { onMounted, ref, watch } from "vue";
import { storeToRefs } from "pinia";
import Api from "../../api/Api";
import { useRouter } from "vue-router";
import ToolbarPanel from "../ToolbarPanel.vue";
import ItemPanel from "./ItemPanel.vue";
import DetailPanel from "./DetailPanel/index.vue";
import registerShape from "../../shape/index";
import registerBehavior from "../../behavior/index";
import { getShapeName } from "../../utilities/clazz";
import Command from "../../plugins/command";
import Toolbar from "../../plugins/toolbar";
import AddItemPanel from "../../plugins/addItemPanel";
import CanvasPanel from "../../plugins/canvasPanel";
registerShape(G6);
registerBehavior(G6);
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
const height = ref(600);
const mode = ref("edit");
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
const canvas = ref();
const toolbar_ref = ref();
const addItemPanel_ref = ref();

const graph = ref();
const selectedModel = ref();
const { departments, users, groups, data } = storeToRefs(store);
// const {} = store;
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
const init = () => {
  let plugins = [];
  var cmdPlugin = new Command();
  const canvasPanel = new CanvasPanel({ container: canvas.value });
  const toolbar = new Toolbar({ container: toolbar_ref.value.$el });
  plugins = [cmdPlugin, toolbar, canvasPanel];

  const addItemPanel = new AddItemPanel({
    container: addItemPanel_ref.value.$el,
  });
  plugins.push(addItemPanel);

  const width = canvas.value.offsetWidth;

  graph.value = new G6.Graph({
    plugins: plugins,
    container: canvas.value,
    height: height.value,
    width: width,
    modes: {
      default: ["drag-canvas", "clickSelected"],
      view: [],
      edit: [
        "drag-canvas",
        "hoverNodeActived",
        "hoverAnchorActived",
        "dragNode",
        "dragEdge",
        "dragPanelItemAddNode",
        "clickSelected",
        "deleteItem",
        "itemAlign",
        "dragPoint",
        "brush-select",
      ],
    },
    defaultEdge: {
      type: "flow-polyline-round",
      // Other properties for all the nodes
    },
  });
  graph.value.setMode(mode.value);
  changeGraph();
  // console.log(this.mode);
  // if (this.data.nodes) {
  //   this.graph.data(this.initShape(this.data));
  //   this.graph.render();
  //   this.graph.fitView();
  //   this.initEvents();
  // }
};
const initShape = (data) => {
  if (data && data.nodes) {
    //var edges = $.extendext(true, 'replace', [], data.edges);
    return {
      nodes: data.nodes.map((node) => {
        return {
          type: getShapeName(node.clazz),
          ...node,
        };
      }),
      edges: data.edges,
    };
  }
  return data;
};
const initEvents = () => {
  // console.log("event");
  graph.value.on("afteritemselected", (items) => {
    // console.log(items);
    if (items && items.length > 0) {
      let item = store.findItembyId(items[0]);
      //if (!item) {
      //     item = this.getNodeInSubProcess(items[0])
      // }
      selectedModel.value = item;
    } else {
      selectedModel.value = {};
    }
  });
  graph.value.on("beforeadditem", (node) => {
    //console.log(node);
    var model = node.model;
    var type = node.type;
    var random = rand();
    model.id = random;
    model.variable = random;
    if (type == "node") {
      model.data_setting = {};
      if (model.clazz == "mailSystem") {
        model.data_setting.mail = {};
      }
      data.value.nodes.push(model);
    } else {
      data.value.edges.push(model);
    }
    selectedModel.value = model;
  });
  graph.value.on("removeShape", (items) => {
    if (items && items.length > 0) {
      removeShape(items[0]);
    }
  });
  graph.value.on("afterupdateitem", (items) => {
    //console.log(items);
    ///UPDATE X,Y
    var cfg = items.cfg;
    if (cfg.x || cfg.y) {
      var item = items.item;
      var id = item._cfg.id;
      var index = store.findIndexNode(id);
      data.value.nodes[index].x = cfg.x;
      data.value.nodes[index].y = cfg.y;
    }
  });

  // const page = this.$refs["canvas"];
  // const graph = this.graph;
  // const height = this.height - 1;
  // this.resizeFunc = () => {
  //   graph.changeSize(page.offsetWidth, height);
  // };
  // window.addEventListener("resize", this.resizeFunc);
};

const removeShape = (id) => {
  let index = data.value.nodes.findIndex(function (item) {
    return item.id == id;
  });

  if (index == -1) {
    index = data.value.edges.findIndex(function (item) {
      return item.id == id;
    });
    if (index != -1) data.value.edges.splice(index, 1);
  } else {
    data.value.nodes.splice(index, 1);
  }
  // console.log(that.data);
};
const changeGraph = () => {
  // console.log(mode.value);
  graph.value.data(initShape(data.value));
  graph.value.render();
  graph.value.fitView();
  initEvents();
};
const onItemCfgChange = (key, value) => {
  const items = graph.value.get("selectedItems");
  if (typeof value == "object") {
    value = $(value.target).val();
  }
  if (items && items.length > 0) {
    let item = graph.value.findById(items[0]);
    // if (!item) {
    //   item = this.getNodeInSubProcess(items[0]);
    // }
    if (graph.value.executeCommand) {
      graph.value.executeCommand("update", {
        itemId: items[0],
        updateModel: { [key]: value },
      });
    } else {
      graph.value.updateItem(item, { [key]: value });
    }
    //this.selectedModel = { ...item.getModel()
  }
};
onMounted(async () => {
  if (props.process_id) {
    await store.init(props.process_id);
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

  init();
});
watch(
  () => data,
  (newValue, oldValue) => {
    if (oldValue != newValue) {
      changeGraph();
    }
  },
  { deep: true }
);
</script>
<style scoped>
#app {
  color: #2c3e50;
  border: 1px solid #e1e1e1;
}
.root {
  width: 100%;
  height: 100%;
  background-color: #fff;
  display: block;
}
.canvasPanel {
  flex: 0 0 auto;
  float: left;
  background-color: #fff;
  border-bottom: 1px solid #e9e9e9;
}
</style>
