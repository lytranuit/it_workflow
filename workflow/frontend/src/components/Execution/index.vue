<template>
  <div id="execution">
    <div class="card-header flex-m mb-2">
      <div>
        <div class="flex-m">
          <span class="title">
            <span v-show="!editTitle" ref="spanTitle">{{ model.title }}</span>
            <input
              v-show="editTitle"
              class="form-control form-control-sm tieu_de"
              v-model="model.title"
              ref="inputTitle"
              placeholder="Tiêu đề"
            />
          </span>

          <span
            class="edit-title ml-2"
            @click="toggle_edit()"
            :class="{ 'btn-success btn btn-sm': editTitle }"
          >
            <i class="fas fa-pen" v-if="!edit"></i>
            <i class="fas fa-check" v-else></i>
          </span>
          <div class="status ml-2" :class="'status_' + model.status_id">
            {{ model.status }}
          </div>
        </div>
        <div class="flex-m">
          <span class="">
            <span class="">ID</span>:
            <span class="font-weight-bold"> {{ model.code }} </span>
          </span>
          <span class="mx-2">|</span>
          <div class="">
            <span class="">Người tạo</span>:
            <span class="font-weight-bold" v-if="model.user">
              {{ model.user.fullName }}
            </span>
          </div>
          <span class="mx-2">|</span>
          <div class="">
            <span class=""> Ngày tạo: </span
            ><span class="font-weight-bold"> {{ model.created_at }} </span>
          </div>
        </div>
      </div>
      <!--<div class="" style="margin-left: auto">
                <div class="dropdown d-inline-block" style="font-size:15px">
                    <a class="nav-link dropdown-toggle" id="drop2" data-toggle="dropdown" href="#" role="button" aria-haspopup="false" aria-expanded="false">
                        <i class="fas fa-ellipsis-v text-muted"></i>
                    </a>
                    <div class="dropdown-menu" aria-labelledby="drop2">
                        <a class="dropdown-item" href="#">Phân công lại</a>
                        <a class="dropdown-item" href="#">Hủy</a>

                    </div>
                </div>
            </div>-->
    </div>
    <div class="root">
      <ToolbarPanel ref="toolbar_ref" :mode="mode" />
      <div
        ref="canvas"
        class="canvasPanel"
        :style="{ height: height + 'px', width: '100%' }"
      ></div>
      <Sidebar
        v-if="selectedModel != null"
        @save_data="save_data"
        @execute_transition="execute_transition"
        @assign_again="assign_again"
        @require_sign="require_sign"
        @close="close"
        @saveDraft="saveDraft"
      >
      </Sidebar>
      <Assign
        v-if="custom_block.length > 0"
        :data_custom_block="custom_block"
        :required="required"
        @save_data="save_data"
        @close="close"
      ></Assign>
      <RequireSign
        v-if="is_require_sign"
        :activity="activity_require"
        @save_data="save_data"
        @close="close"
      >
      </RequireSign>
    </div>
    <div class="row mt-2" v-if="model.id > 0">
      <div class="col-md-9">
        <comment :model="model"></comment>
      </div>
      <div class="col-md-3">
        <event :model="model"></event>
      </div>
    </div>
    <Loading :waiting="waiting"></Loading>
  </div>
</template>
<script setup>
import G6 from "@antv/g6/lib";
import Button from "primevue/button";
import Sidebar from "./Sidebar/index.vue";
import moment from "moment/moment";
import { useProcess } from "../../stores/process";
import { useAuth } from "../../stores/auth";
// import Wfd from "./Wfd.vue";
import { rand } from "../../utilities/rand";
import { onMounted, ref, watch } from "vue";
import { storeToRefs } from "pinia";
import Api from "../../api/Api";
import { useRouter } from "vue-router";
import Assign from "./Assign.vue";
import RequireSign from "./RequireSign.vue";
import Comment from "./Comment.vue";
import Event from "./Event.vue";
import Loading from "../Loading.vue";
import ToolbarPanel from "../ToolbarPanel.vue";
import registerShape from "../../shape/index";
import registerBehavior from "../../behavior/index";
import { getShapeName } from "../../utilities/clazz";
import Command from "../../plugins/command";
import Toolbar from "../../plugins/toolbar";
import CanvasPanel from "../../plugins/canvasPanel";

registerShape(G6);
registerBehavior(G6);
const router = useRouter();
const store = useProcess();
const store_auth = useAuth();

const custom_block = ref([]);
const errors = ref([]);
const edit = ref(false);
const height = ref(600);
const mode = ref("executing");
const required = ref(true);
const is_require_sign = ref(false);
const activity_require = ref();

const canvas = ref();
const toolbar_ref = ref();
const spanTitle = ref();
const inputTitle = ref();
const waiting = ref();
// const graph = ref();
const {
  departments,
  users,
  groups,
  editTitle,
  data,
  model,
  graph,
  data_custom_block,
  data_transition,
  data_activity,
  selectedModel,
} = storeToRefs(store);
const { user } = storeToRefs(store_auth);

const props = defineProps({
  process_version_id: String,
  execution_id: String,
});
const toggle_edit = () => {
  editTitle.value = !editTitle.value;
  if (editTitle.value) {
    setTimeout(function () {
      const spanTitle = spanTitle.value;
      const inputTitle = inputTitle.value;
      var width = $(spanTitle).width();
      width = width > 200 ? width : 200;
      $(inputTitle).css("width", width).focus();
    }, 10);
  } else {
    if (model.value.id > 0) {
      Api.updateexecution({ id: model.value.id, title: model.value.title });
    }
  }
};
const save_data = async () => {
  // var data_custom_block = this.data_custom_block;
  // var data_transition = this.data_transition;
  // var data_activity = this.data_activity;
  //console.log(data_transition);
  //console.log(data_activity)
  //return;
  // console.log(1);
  custom_block.value = [];
  var tasks = [];
  waiting.value = true;
  if (model.value.id > 0) {
  } else {
    ///create execute
    var resp = await Api.createexecution(model.value);
    if (resp.success) {
      model.value = resp.data;
    }
  }
  for (var item of data_custom_block.value) {
    item.execution_id = model.value.id;
    if (item.is_new) {
      await Api.createcustomblock(item);
    } else if (item.is_update) {
      await Api.updatecustomblock(item);
    }
  }
  for (var item of data_transition.value) {
    item.execution_id = model.value.id;
    if (item.is_new) {
      var task = { api: "createtransition", data: item };
      tasks.push(task);
      //var resp = await $.ajax({ url: path + "/admin/api/createtransition", data: item, type: "POST", dataType: "JSON" });
    }
  }
  for (var item of data_activity.value) {
    item.execution_id = model.value.id;
    if ((item.is_new || item.is_update) && !item.failed) {
      if (item.fields) {
        for (var field of item.fields) {
          field.activity_id = item.id;
          field.execution_id = model.value.id;
          field.process_field_id = field.id;
          field.id = rand();
          if (
            (field.type == "file" || field.type == "file_multiple") &&
            field.files &&
            field.files.length
          ) {
            var formData = new FormData();
            for (var i = 0; i < field.files.length; i++) {
              formData.append("files", field.files[i]);
            }
            formData.append("execution_id", model.value.id);
            var resp = await Api.uploadFile(formData);
            field.values.files = resp.list;
            field.files = null;
          }
        }
      }
      if (item.sign) {
        var sign = item.sign;
        var resp = await Api.SaveSign(sign);
        delete item.sign;
        if (resp.success == 1) {
          var resp_sign = resp.sign;
          var data_setting = item.data_setting || {};
          var listusersign = data_setting.listusersign || [];
          if (listusersign.length > 0) {
            var findindex = listusersign.findLastIndex(function (item) {
              return item.user_sign == resp_sign.user_sign;
            });
            listusersign[findindex] = resp_sign;
            item.data_setting.listusersign = listusersign;
          }
        }
      }
    }
    if (item.is_new) {
      var task = { api: "createactivity", data: item };
      tasks.push(task);
      //var resp = await $.ajax({ url: path + "/admin/api/createactivity", data: item, type: "POST", dataType: "JSON" });
    } else if (item.is_update) {
      var task = { api: "updateactivity", data: item };
      tasks.push(task);
      //var resp = await $.ajax({ url: path + "/admin/api/updateactivity", data: item, type: "POST", dataType: "JSON" });
    }
  }
  tasks.sort(function (a, b) {
    if (a.data.created_at == b.data.created_at) return 0;
    if (a.data.created_at > b.data.created_at) return 1;
    if (a.data.created_at < b.data.created_at) return -1;
  });
  for (var item of tasks) {
    await Api[item.api](item.data);
  }
  router.push(
    "/execution/details/" +
      props.process_version_id +
      "?execution_id=" +
      model.value.id +
      "&time=" +
      moment().valueOf()
  );
};
const init = () => {
  let plugins = [];
  var cmdPlugin = new Command();
  const canvasPanel = new CanvasPanel({ container: canvas.value });
  const toolbar = new Toolbar({ container: toolbar_ref.value.$el });
  plugins = [cmdPlugin, toolbar, canvasPanel];
  const width = canvas.value.offsetWidth;
  graph.value = new G6.Graph({
    plugins: plugins,
    container: canvas.value,
    height: height.value,
    width: width,
    modes: {
      default: [
        "drag-canvas",
        "clickSelected",
        {
          type: "activate-relations",
          resetSelected: true,
        },
      ],
    },
    animate: true,
    animateCfg: {
      duration: 500, // Number, the duration of one animation
      easing: "linearEasing", // String, the easing function
    },
    defaultEdge: {
      type: "flow-polyline-round",
      // Other properties for all the nodes
    },
  });
  graph.value.setMode(mode.value);

  var data2 = initShape();
  graph.value.read(data2);
  // graph.value.fitView();
  initEvents();
};
const initEvents = () => {
  graph.value.on("node:touchend", (e) => {
    var item = e.item;
    var id = item.get("model").id;
    var findBlocking = data_activity.value.findLastIndex(function (item) {
      return item.block_id == id;
    });
    if (findBlocking != -1) {
      var model = data_activity.value[findBlocking];
      selectedModel.value = model;
      // console.log(selectedModel.value);
      return;
    }
    selectedModel.value = null;
  });
  graph.value.on("node:click", (e) => {
    var item = e.item;
    var id = item.get("model").id;
    var findBlocking = data_activity.value.findLastIndex(function (item) {
      return item.block_id == id;
    });
    if (findBlocking != -1) {
      var model = data_activity.value[findBlocking];
      selectedModel.value = model;
      // console.log(selectedModel.value);
      return;
    }
    selectedModel.value = null;
  });
  graph.value.on("delete-shape:click", async (e) => {
    var text = "Bạn có muốn rút lại bước này!";
    if (confirm(text)) {
      var edge = e.item;
      var id = edge.get("model").id;
      var findTransition = data_transition.value.findLastIndex(function (item) {
        return item.link_id == id;
      });
      if (findTransition != -1) {
        var transition = data_transition.value[findTransition];
        var resp = await $.ajax({
          url: path + "/admin/api/withdraw",
          data: { transition_id: transition.id },
          type: "POST",
          dataType: "JSON",
        });
        location.reload();
      }
    }
  });

  //this.graph.on('delete-shape:mouseenter', async (e) => {
  //    console.log(e.item)
  //    this.graph.setItemState(e.item, 'hover', true);
  //});
  //this.graph.on('delete-shape:mouseleave', async (e) => {
  //    console.log(e.item)
  //    this.graph.setItemState(e.item, 'hover', false);
  //});
  //this.graph.on('canvas:click', (e) => {
  //    that.selectedModel = null;
  //});
};
const initShape = () => {
  var nodes = data.value.nodes || [];
  var edges = data.value.edges || [];
  var status_id = model.value.status_id;
  //console.log(data_transition)
  for (var transition of data_transition.value) {
    var edge_id = transition.link_id;
    var indexEdge = store.findIndexEdge(edge_id);
    var edge = edges[indexEdge];
    if (!transition.reverse) {
      edge.active = true;
    } else {
      edge.reverse = true;
    }
    //if (i == data_transition.length - 1 && that.current_user.id == transition.created_by && status_id == 2) {
    //    edge.allowDelete = true;
    //}
  }
  for (var activity of data_activity.value) {
    if (activity.blocking) {
      var block_id = activity.block_id;
      var indexNode = store.findIndexNode(block_id);
      var block = nodes[indexNode];
      block.active = true;
      var node = graph.value.find("node", (n) => {
        return n.get("model").id == activity.block_id;
      });
      // console.log(node);
      if (node == null) continue;
      var outEdges = node.getOutEdges().map(function (i) {
        return i.get("model");
      });
      var findCustomBlock = data_custom_block.value.findIndex(function (item) {
        return item.block_id == block.id;
      });
      if (findCustomBlock != -1) {
        // var node_data_setting =
        activity.data_setting = $.extendext(
          true,
          "replace",
          node.get("model").data_setting,
          activity.data_setting,
          data_custom_block.value[findCustomBlock].data_setting
        );
      }
      console.log(activity.fields);
      if (activity.is_new || !activity.fields.length) {
        var fields = block.fields || [];
        fields = fields.map(function (i) {
          i.data_setting = i.data_setting || {};
          i.values = {};
          switch (i.type) {
            case "number":
            case "text":
            case "email":
            case "date":
            case "date_month":
            case "date_time":
            case "select":
            case "department":
            case "textarea":
            case "employee":
            case "currency":
            case "radio":
              var value = i.has_default ? i.data_setting.default_value : null;
              i.values = { value: value };
              break;
            case "select_multiple":
            case "employee_multiple":
            case "department_multiple":
            case "checkbox":
              var value_array =
                i.has_default && i.data_setting.default_value_array
                  ? i.data_setting.default_value_array
                  : [];
              i.values = { value_array: value_array };
              break;
            case "table":
              i.values = { list_data: [] };
              break;
          }
          return i;
        });
        activity.fields = fields;
        // console.log(fields);
      }

      activity.outEdges = outEdges;
    }
  }
  if (data.value && data.value.nodes) {
    //var edges = $.extendext(true, 'replace', [], data.edges);
    return {
      nodes: nodes.map((node) => {
        return {
          type: getShapeName(node.clazz),
          ...node,
        };
      }),
      edges: edges,
    };
  }
  return data.value;
};
const execute_transition = (from_activity_id, edge_id) => {
  //var nodes = state.data.nodes;
  var findIndexActivity = data_activity.value.findLastIndex(function (item) {
    return item.id == from_activity_id;
  });
  var activity = data_activity.value[findIndexActivity];

  var edge = graph.value.find("edge", (node) => {
    return node.get("model").id === edge_id;
  });
  ////VAILD TIEU DE
  if (!$(".tieu_de").val()) {
    Swal.fire({
      title: "Nhập tiêu đề",
      input: "text",
      inputAttributes: {
        autocapitalize: "off",
      },
      showCancelButton: false,
      confirmButtonText: "Gửi đi",
      showLoaderOnConfirm: true,
      preConfirm: (data) => {
        if (data) {
          model.value.title = data;
          return true;
        } else {
          return false;
        }
      },
      allowOutsideClick: () => !Swal.isLoading(),
    }).then((result) => {
      // console.log(result);
      if (result.value) {
        execute_transition(from_activity_id, edge_id);
      }
    });
    //alert("Bạn chưa nhập tiêu đề!");
    $(".tieu_de").focus();
    return;
  }
  //////UPDATE Current
  if (!edge.get("model").reverse) {
    var vaild = $("#sidebar-right").valid();
    if (!vaild) {
      return;
    }

    activity.failed = false;
    $("#sidebar-right input[type='file']").each(function (item) {
      var id = $(this).attr("name");
      var findfield = activity.fields.findIndex(function (i) {
        return i.id == id;
      });
      var field = activity.fields[findfield];
      field.files = $(this)[0].files;
    });
    //////
    if ($("#approve-sign").length) {
      $("#approve-sign").addClass("active");
      let sign = $(".signature");
      var sign_x = sign[0].offsetLeft;
      var sign_y = sign[0].offsetTop;
      var parent = sign.closest(".box-canvas");
      if (!parent.length) {
        alert("Kéo chữ ký vào văn bản để ký!");
        return;
      }
      var reason_b = $(".reason", sign).text();
      var explode = reason_b.split("Ý kiến:");
      var reason;
      if (explode.length > 1) {
        reason = explode[1];
      }
      var page = parent.index() + 1;
      var height_page = $(".pdf-page-canvas", parent).height();
      var sign_image = $(".sign_image", sign);
      var sign_info = $(".sign_info", sign);
      //var sign_info_x = sign_info[0].offsetLeft;
      //var sign_info_y = sign_info[0].offsetTop;
      var sign_image_x = sign_image[0].offsetLeft;
      var sign_image_y = sign_image[0].offsetTop;
      var image_size_width = sign_image.width();
      var image_size_height = sign_image.height();
      var position_image_x = sign_image_x + sign_x;
      var position_image_y = height_page - (image_size_height + sign_y);
      var position_x = sign_x;
      var position_y = height_page - (image_size_height + sign_y + 40);
      if (reason) {
        position_y -= 30;
      }
      if (!sign_info.length) {
        position_y = position_image_y;
      }
      var url = $("#pdf-viewer").data("url");
      var activity_esign_id = $("#pdf-viewer").data("activity_esign");
      var user_esign = sign.data("id");
      activity.sign = {
        block_id: activity.block_id,
        page: page,
        position_x: position_x,
        position_y: position_y,
        position_image_x: position_image_x,
        position_image_y: position_image_y,
        image_size_width: image_size_width,
        image_size_height: image_size_height,
        url: url,
        reason: reason,
        user_sign: user.value.id,
        user_esign: user_esign,
        activity_esign_id: activity_esign_id,
        activity_id: activity.id,
      };
    }
    if ($("#suggest").length) {
      var list = {};
      if ($("#pdf-viewer .signature").length != $("#sign .signature").length) {
        alert("Kéo chữ ký vào văn bản để gợi ý!");
        return;
      }
      $("#pdf-viewer .signature").each(function () {
        let sign = $(this);
        var sign_x = sign[0].offsetLeft;
        var sign_y = sign[0].offsetTop;
        var parent = sign.closest(".box-canvas");
        var page = parent.index() + 1;
        var height_page = $(".pdf-page-canvas", parent).height();
        var sign_image = $(".sign_image", sign);
        var sign_info = $(".sign_info", sign);
        //var sign_info_x = sign_info[0].offsetLeft;
        //var sign_info_y = sign_info[0].offsetTop;
        var sign_image_x = sign_image[0].offsetLeft;
        var sign_image_y = sign_image[0].offsetTop;
        var image_size_width = sign_image.width();
        var image_size_height = sign_image.height();
        var position_image_x = sign_image_x + sign_x;
        var position_image_y = height_page - (image_size_height + sign_y);
        var position_x = sign_x;
        var position_y = height_page - (image_size_height + sign_y + 40);
        if (!sign_info.length) {
          position_y = position_image_y;
        }
        var block_id = sign.data("id");

        var item = {
          page: page,
          position_x: position_x,
          position_y: position_y,
          position_image_x: position_image_x,
          position_image_y: position_image_y,
          image_size_width: image_size_width,
          image_size_height: image_size_height,
          block_id: block_id,
        };
        list[block_id] = item;
      });

      activity.data_setting.suggests = list;
    }
  } else if (!activity.note) {
    Swal.fire({
      title: "Nhập lý do",
      input: "textarea",
      inputAttributes: {
        autocapitalize: "off",
      },
      showCancelButton: true,
      confirmButtonText: "Xác nhận",
      showLoaderOnConfirm: true,
      preConfirm: (login) => {
        if (login) {
          activity.note = login;
          return true;
        } else {
          Swal.showValidationMessage("Vui lòng nhập lý do!");
          return false;
        }
      },
      allowOutsideClick: () => !Swal.isLoading(),
    }).then((result) => {
      if (result.value) {
        execute_transition(from_activity_id, edge_id);
      }
    });
    return;
  } else {
    activity.failed = true;
    delete activity.fields;
  }
  activity.blocking = false;
  activity.executed = true;
  activity.is_update = true;
  activity.created_by = user.value.id;
  //
  var source = edge.getSource();
  var target = edge.getTarget();

  /////ADD TRANSITION
  var transition_new = {
    is_new: true,
    label: edge.get("model").label,
    reverse: edge.get("model").reverse,
    link_id: edge.get("model").id,
    execution_id: null,
    from_block_id: source.get("model").id,
    to_block_id: target.get("model").id,
    from_activity_id: activity.id,
    //to_activity_id: activity_to.id,371
    stt: data_transition.value[data_transition.value.length - 1].stt + 1,
    created_at: moment().valueOf(),
    id: rand(),
  };
  data_transition.value.push(transition_new);
  //TARGET
  var create_new = true;
  if (target.get("model").clazz == "inclusiveGateway") {
    var find_activity = data_activity.value.findLastIndex(function (item) {
      return item.block_id == target.get("model").id;
    });
    if (find_activity != -1) {
      create_new = false;
      activity_new = data_activity.value[find_activity];
    }
  }
  if (create_new) {
    var blocking = false;
    if (
      target.get("model").clazz == "formTask" ||
      target.get("model").clazz == "approveTask" ||
      target.get("model").clazz == "suggestTask" ||
      target.get("model").clazz == "mailSystem" ||
      target.get("model").clazz == "printSystem" ||
      target.get("model").clazz == "outputSystem"
    ) {
      blocking = true;
    }

    var data_setting = target.get("model").data_setting || {};
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
      created_at: moment().valueOf(),
      id: rand(),
    };
    data_activity.value.push(activity_new);
  }
  transition_new.to_activity_id = activity_new.id;
  ////Custom Block
  if (blocking) {
    var findCustomBlock = data_custom_block.value.findIndex(function (item) {
      return item.block_id == target.get("model").id;
    });
    if (findCustomBlock == -1) {
      var data_setting_block = target.get("model").data_setting || {};
      var type_performer = target.get("model").type_performer;
      var data_setting = {};
      if (
        type_performer == null ||
        (type_performer == 1 && data_setting_block.block_id == null)
      ) {
        data_setting.type_performer = 4;
        data_setting.listuser = [user.value.id];
      } else if (type_performer == 1 && data_setting_block.block_id != null) {
        data_setting.type_performer = 4;
        var findIndexActivity = data_activity.value.findLastIndex(function (
          item
        ) {
          return item.block_id == data_setting_block.block_id;
        });
        var findActivity = data_activity.value[findIndexActivity];
        data_setting.listuser = [findActivity.created_by];
      } else if (type_performer == 3 || type_performer == 4) {
        data_setting = data_setting_block;
        data_setting.type_performer = type_performer;
      } else if (type_performer == 5) {
        data_setting.type_performer = 4;
        data_setting.listuser = [model.value.user_id];
      }
      if (type_performer != 2) {
        var custom_block = {
          data_setting: data_setting,
          block_id: target.get("model").id,
          is_new: true,
        };
        data_custom_block.value.push(custom_block);
      }
    }
  }
  //// Check previous
  // graph.get
  var neighbors = graph.value.getNeighbors(source, "source");
  // console.log(source);
  // console.log(neighbors);
  if (neighbors.length) {
    for (var node_source of neighbors) {
      var clazz = node_source.get("model").clazz;
      if (clazz == "exclusiveGateway") {
        var targets = graph.value.getNeighbors(node_source, "target");
        for (var t of targets) {
          var find_activity_target = data_activity.value.findLastIndex(
            function (item) {
              return item.block_id == t.get("model").id;
            }
          );
          if (find_activity_target == -1) continue;
          var activity_target = data_activity.value[find_activity_target];
          if (activity_target.blocking) {
            data_activity.value.splice(find_activity_target, 1); /// XÓA activity
            var find_transition = data_transition.value.findLastIndex(function (
              item
            ) {
              return item.to_activity_id == activity_target.id;
            });
            if (find_transition == -1) continue;
            data_transition.value.splice(find_transition, 1); /// XÓA transaction
          }
        }
      }
    }
  }
  // console.log(data_activity.value);

  // console.log(data_transition.value);
  // return;
  store.create_next(activity_new);
  setTimeout(function () {
    //var data2 = that.initShape();
    //that.graph.read(data2);
    //that.graph.fitView();

    selectedModel.value = null;
    if (!activity.failed) {
      check_assign(activity.block_id);
    } else {
      save_data();
    }
    //
  }, 100);
};
const saveDraft = (from_activity_id) => {
  var findIndexActivity = data_activity.value.findLastIndex(function (item) {
    return item.id == from_activity_id;
  });
  var activity = data_activity.value[findIndexActivity];

  ////VAILD TIEU DE
  if (!$(".tieu_de").val()) {
    Swal.fire({
      title: "Nhập tiêu đề",
      input: "text",
      inputAttributes: {
        autocapitalize: "off",
      },
      showCancelButton: false,
      confirmButtonText: "Lưu lại",
      showLoaderOnConfirm: true,
      preConfirm: (data) => {
        if (data) {
          model.value.title = data;
          return true;
        } else {
          return false;
        }
      },
      allowOutsideClick: () => !Swal.isLoading(),
    }).then((result) => {
      // console.log(result);
      if (result.value) {
        saveDraft(from_activity_id);
      }
    });
    //alert("Bạn chưa nhập tiêu đề!");
    $(".tieu_de").focus();
    return;
  }
  if (!activity.is_new) {
    activity.is_update = true;
  }
  save_data();
  // console.log(data_activity.value);
};
const check_assign = (block_id) => {
  var nodes = data.value.nodes;
  var findNodes = nodes.filter(function (item) {
    return item.type_performer == 2 && item.data_setting.block_id == block_id;
  });
  if (findNodes.length) {
    for (var node of findNodes) {
      var block_id = node.id;
      var custom = {
        data_setting: {},
        block_id: block_id,
        block: node,
      };
      custom_block.value.push(custom);

      var findCustom = data_custom_block.value.findIndex(function (item) {
        return item.block_id == block_id;
      });
      if (findCustom != -1) {
        if (data_custom_block.value[findCustom].id) {
          custom.is_update = true;
          custom.id = data_custom_block.value[findCustom].id;
        } else {
          custom.is_new = true;
        }
        data_custom_block.value[findCustom] = custom;
      } else {
        custom.is_new = true;
        data_custom_block.value.push(custom);
      }
    }
    required.value = true;
  } else {
    save_data();
  }
  ////
};
const assign_again = (block_id) => {
  var nodes = data.value.nodes;
  var findIndex = nodes.findIndex(function (item) {
    return item.id == block_id;
  });
  var node = nodes[findIndex];
  var custom = {
    data_setting: {},
    block_id: block_id,
    block: node,
  };
  custom_block.value.push(custom);
  var findCustom = data_custom_block.value.findIndex(function (item) {
    return item.block_id == block_id;
  });
  if (findCustom != -1) {
    if (data_custom_block.value[findCustom].id) {
      custom.is_update = true;
      custom.event_type = "reassignment";
      custom.id = data_custom_block.value[findCustom].id;
    } else {
      custom.is_new = true;
    }
    data_custom_block.value[findCustom] = custom;
  } else {
    custom.is_new = true;
    data_custom_block.value.push(custom);
  }

  selectedModel.value = null;
  required.value = false;
};
const require_sign = (activity) => {
  custom_block.value = [];
  selectedModel.value = null;
  is_require_sign.value = true;
  activity_require.value = activity;
};
const close = () => {
  custom_block.value = [];
  selectedModel.value = null;
  is_require_sign.value = false;
  activity_require.value = null;
};

const initModel = (execution) => {
  execution.created_at = moment(execution.created_at).format("DD/MM/YYYY");
  return execution;
};

const get_execution = async (execution_id) => {
  waiting.value = true;
  var execution = await Api.execution(execution_id);
  model.value = initModel(execution);
  data_transition.value = await Api.TransitionByExecution(execution_id);
  data_activity.value = await Api.ActivityByExecution(execution_id);
  data_custom_block.value = await Api.CustomBlockByExecution(execution_id);
  // setTimeout(function () {
  var data2 = initShape();
  graph.value.read(data2);
  graph.value.fitView();
  graph.value.executeCommand("currentFlow");
  //// active block lên
  // store.active_activity();
  /////Bỏ loadding
  waiting.value = false;
  // }, 100);
};
onMounted(async () => {
  store.reset();
  waiting.value = true;
  /// Lấy template
  var ress = await Api.ProcessVersion(props.process_version_id);
  //console.log(data);
  let res = ress.process;
  let data_tmp = {};
  data_tmp.edges = res.links.map(function (item) {
    var default_setting = {};
    item = $.extendext(true, "replace", default_setting, item);
    return item;
  });
  data_tmp.nodes = res.blocks.map(function (item) {
    var default_setting = {
      data_setting: {},
      executed: false,
      failed: false,
      blocking: false,
    };
    delete item.process;
    item = $.extendext(true, "replace", default_setting, item);
    if (item.clazz == "start") {
      item.executed = true;
    }
    return item;
  });
  delete res.blocks;
  delete res.links;
  delete res.fields;

  data.value = data_tmp;

  init();
  waiting.value = false;
  // console.log(props.execution_id);
  if (props.execution_id) {
    await get_execution(props.execution_id);
  } else {
    //   /// INIT
    //   setTimeout(function () {
    //     //// activity START

    model.value = {
      title: "",
      status: "Đang thực hiện",
      status_id: 2,
      process_version_id: props.process_version_id,
      id: "---",
      user_id: user.value.id,
      user: user.value,
      created_at: moment().format("DD/MM/YYYY"),
    };
    editTitle.value = true;
    var start = graph.value.find("node", (node) => {
      return node.get("model").clazz === "start";
    });
    // console.log(start);
    var data_activity_tmp = [];
    var activity_start = {
      execution_id: null,
      label: start.get("model").label,
      block_id: start.get("model").id,
      stt: 1,
      clazz: start.get("model").clazz,
      variable: start.get("model").variable,
      is_new: true,
      executed: true,
      failed: false,
      blocking: false,
      created_by: user.id,
      created_at: moment().valueOf(),
      id: rand(),
    };
    data_activity_tmp.push(activity_start);
    data_activity.value = data_activity_tmp;

    // setTimeout(function () {
    store.create_next(activity_start);
    var data2 = initShape();
    graph.value.read(data2);
    graph.value.fitView();
    graph.value.executeCommand("currentFlow");
    //// active block lên
    // store.active_activity();

    /////Bỏ loadding
    waiting.value = false;

    //console.log(node_block);
    // }, 100);
    //   }, 100);
  }
});
watch(
  () => data,
  (newValue, oldValue) => {
    if (oldValue != newValue) {
      var data2 = initShape();
      graph.value.read(data2);
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
  display: inline-block;
}

.canvasPanel {
  flex: 0 0 auto;
  float: left;
  background-color: #fff;
  border-bottom: 1px solid #e9e9e9;
}

.status_2 {
  background-color: #d8f4ff;
  color: #0c9cdd;
}

.status_3 {
  background-color: #1ecab8;
  color: white;
}

.status_4 {
  background-color: #f1646c;
  color: white;
}

.status {
  border-radius: 4px;
  padding: 0 16px;
  height: 28px;
  min-width: auto;
  display: inline-block;
  overflow: hidden;
  font-size: 12px;
  font-weight: 500;
  line-height: 28px;
}
</style>
