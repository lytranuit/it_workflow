<template>
  <Sidebar v-model:visible="visible" :position="position" style="width: 50rem">
    <template #header>
      <div class="control ml-auto flex-m">
        <a class="nav-link cursor-pointer flex-m items-center font-13" href="#" @click="saveDraft(selectedModel.id)" v-if="selectedModel.blocking &&
          (hasPermission() || model.user_id == current_user.id) && selectedModel.clazz == 'formTask'
          ">
          <i class="far fa-clipboard font-16"></i>
          <div class="ml-2">Lưu nháp</div>
        </a>
        <a class="nav-link cursor-pointer flex-m items-center font-13" href="#"
          @click="assign_again(selectedModel.block_id)" v-if="selectedModel.blocking &&
            (hasPermission() || model.user_id == current_user.id)
            ">
          <i class="fas fa-share font-16"></i>
          <div class="ml-2">Phân công lại</div>
        </a>
        <a class="nav-link cursor-pointer flex-m items-center font-13" href="#" @click="require_sign(selectedModel)" v-if="selectedModel.blocking &&
          hasPermission() &&
          selectedModel.clazz == 'approveTask'
          ">
          <i class="fas fa-signature"></i>
          <div class="ml-2">Yêu cầu phê duyệt</div>
        </a>
        <a class="nav-link cursor-pointer flex-m items-center font-13" href="#" v-if="isPopup == true"
          @click="setIsPopup(false)">
          <i class="fas fa-expand font-16"></i>
          <div class="ml-2">Phóng to</div>
        </a>
        <a class="nav-link cursor-pointer flex-m items-center font-13" href="#" v-if="isPopup == false"
          @click="setIsPopup(true)">
          <i class="fas fa-location-arrow font-16"></i>
          <div class="ml-2">Thu lại</div>
        </a>
      </div>
    </template>
    <form id="sidebar-right">
      <div class="header">
        <div class="flex-m">
          <div>
            <div class="tilte">{{ selectedModel.label }}</div>
            <div class="flex-m" v-if="html != ''">
              <div class="flex-m">
                Người thực hiện: <span v-html="html" class="ml-1"></span>
              </div>
            </div>
          </div>
        </div>
        <div class="box_transition" v-if="selectedModel.blocking && hasPermission()">
          <button class="mr-2" :class="{
            'btn-reverse': item.reverse,
            'btn-next': !item.reverse,
          }" tabindex="1" type="button" name="button" v-for="item in selectedModel.outEdges" :key="item.id"
            @click="execute_transition(selectedModel.id, item.id)">
            {{ item.label }}
          </button>
        </div>
        <div class="box_transition" v-if="selectedModel.blocking && !hasPermission() && !hasRequireSign()">
          <span class="text-danger">Bạn không có quyền thực hiện bước này.</span>
        </div>
        <div class="box_transition" v-if="selectedModel.blocking && !hasPermission() && hasRequireSign()">
          <button class="mr-2 btn-reverse" type="button" name="button" @click="disagree()">
            Không đồng ý
          </button>
          <button class="mr-2 btn-next" type="button" name="button" @click="agree()">
            Đồng ý
          </button>
        </div>
      </div>
      <div class="body" v-if="(selectedModel.blocking && hasPermission()) ||
        selectedModel.executed ||
        hasRequireSign()
        ">
        <FormTask :departments="departments" :users="users" :fields="fields" :readonly="readonly"
          v-if="selectedModel.clazz == 'formTask'"></FormTask>
        <ApproveTask :departments="departments" :users="users" :nodes="nodes" :readonly="readonly"
          v-if="selectedModel.clazz == 'approveTask'" :model="selectedModel" @require_sign="require_sign"></ApproveTask>
        <SuggestTask :nodes="nodes" :readonly="readonly" v-if="selectedModel.clazz == 'suggestTask'"
          :model="selectedModel"></SuggestTask>
        <PrintSystem v-if="selectedModel.clazz == 'printSystem'" :nodes="nodes" :model="selectedModel"></PrintSystem>
        <OutputSystem v-if="selectedModel.clazz == 'outputSystem'" :nodes="nodes" :model="selectedModel"></OutputSystem>
      </div>
    </form>
  </Sidebar>
</template>
<script>
import moment from "moment/moment";
import FormTask from "./FormTask.vue";
import ApproveTask from "./ApproveTask.vue";
import SuggestTask from "./SuggestTask.vue";
import PrintSystem from "./PrintSystem.vue";
import OutputSystem from "./OutputSystem.vue";
import Sidebar from "primevue/sidebar";
import { useProcess } from "../../../stores/process";
import { useAuth } from "../../../stores/auth";
var store = useProcess();
var store_auth = useAuth();
export default {
  components: {
    Sidebar,
    FormTask,
    ApproveTask,
    SuggestTask,
    PrintSystem,
    OutputSystem,
  },
  data() {
    return {
      readonly: false,
      isPopup: false,
      position: "right",
      html: "",
      visible: false,
    };
  },
  watch: {
    selectedModel: {
      handler(newData, oldData) {
        this.visible = this.selectedModel != null;
        // console.log(this.model);
        if (oldData !== newData) {
          this.initHtml();
          this.readonly = !this.selectedModel.blocking;
        }
      },
      immediate: true,
      deep: true,
    },

    visible: {
      handler(newData, oldData) {
        if (newData == false) {
          this.close();
        }
      },
    },
    isPopup: {
      handler(newData, oldData) {
        if (this.isPopup) {
          this.position = "right";
        } else {
          this.position = "full";
        }
      },
    },
  },
  computed: {
    model() {
      return store.model;
    },
    selectedModel() {
      return store.selectedModel;
    },
    users() {
      return store.users;
    },
    nodes() {
      return store.data.nodes;
    },
    departments() {
      return store.departments;
    },
    fields() {
      return this.selectedModel.fields;
    },
    current_user() {
      return store_auth.user;
    },
    data_transition() {
      return store.data_transition;
    },
  },
  mounted() {
    this.isPopup = JSON.parse(localStorage.getItem("isPopup")) || false;
    if (this.isPopup) {
      this.position = "right";
    } else {
      this.position = "full";
    }
    // console.log(this.model);
  },
  methods: {
    setIsPopup(isPopup) {
      if (typeof localStorage !== "undefined") {
        this.isPopup = isPopup;
        localStorage.setItem("isPopup", isPopup);
      }
    },
    execute_transition(from_activity_id, edge_id) {
      var that = this;
      that.$emit("execute_transition", from_activity_id, edge_id);
    },
    async agree() {
      var that = this;
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
      var sign_data = {
        block_id: that.selectedModel.block_id,
        page: page,
        position_x: position_x,
        position_y: position_y,
        position_image_x: position_image_x,
        position_image_y: position_image_y,
        image_size_width: image_size_width,
        image_size_height: image_size_height,
        url: url,
        reason: reason,
        user_sign: that.current_user.id,
        user_esign: user_esign,
        activity_esign_id: activity_esign_id,
        activity_id: that.selectedModel.id,
      };
      $(".preloader").fadeIn();
      var resp = await $.ajax({
        url: path + "/admin/api/SaveSign",
        data: sign_data,
        type: "POST",
        dataType: "JSON",
      });
      if (resp.success == 1) {
        var resp_sign = resp.sign;
        var data_setting = this.selectedModel.data_setting || {};
        var listusersign = data_setting.listusersign || [];
        var findindex = listusersign.findLastIndex(function (item) {
          return item.user_sign == that.current_user.id;
        });

        listusersign[findindex] = resp_sign;
        this.selectedModel.data_setting.listusersign = listusersign;
        this.selectedModel.is_update = true;
        //console.log(this.model);
        this.$emit("save_data");
      }
    },
    disagree() {
      var that = this;
      var data_setting = this.selectedModel.data_setting || {};
      var listusersign = data_setting.listusersign || [];
      var findindex = listusersign.findLastIndex(function (item) {
        return item.user_sign == that.current_user.id;
      });

      listusersign[findindex].status = 3;
      this.selectedModel.data_setting.listusersign = listusersign;
      this.selectedModel.is_update = true;
      //console.log(this.model);
      this.$emit("save_data");
    },
    close() {
      this.$emit("close");
    },
    assign_again(block_id) {
      var that = this;
      that.$emit("assign_again", block_id);
    },
    require_sign(activity) {
      this.$emit("require_sign", activity);
    },
    saveDraft(from_activity_id) {
      var that = this;
      that.$emit("saveDraft", from_activity_id);
    },
    getAll(departments) {
      var that = this;
      var list = [];
      departments.map(function (item) {
        list.push(item);
        var children = item.children || [];
        children = that.getAll(children);
        list = [...list, ...children];
      });
      return list;
    },
    initHtml() {
      var that = this;
      var html = "";
      var model = this.selectedModel;
      if (model.blocking) {
        var data_setting = model.data_setting;
        var type_performer = data_setting.type_performer;
        if (type_performer == 4) {
          var listuser = data_setting.listuser;
          //console.log(data_setting);
          //console.log(that.users)
          listuser = listuser.map(function (item) {
            var findUser =
              that.users != null
                ? that.users.findLastIndex(function (user) {
                  return user.id == item;
                })
                : -1;
            if (findUser == -1) return null;
            var user = that.users[findUser];
            return "<b>" + user.name + "</b>";
          });
          listuser = listuser.filter(function (item) {
            return item != null;
          });
          if (listuser.length) {
            html += listuser.join(",");
          }
        } else if (type_performer == 3) {
          var listdepartment = data_setting.listdepartment;
          var departments = this.getAll(that.departments);
          listdepartment = listdepartment.map(function (item) {
            var findDepartment =
              departments != null
                ? departments.findLastIndex(function (user) {
                  return user.id == item;
                })
                : -1;
            if (findDepartment == -1) return null;
            var department = departments[findDepartment];
            return "<b>" + department.label + "</b>";
          });
          listdepartment = listdepartment.filter(function (item) {
            return item != null;
          });
          if (listdepartment.length) {
            html += listdepartment.join(",");
          }
        }
      } else {
        var transitions = this.data_transition;
        var findTransition = transitions.findLastIndex(function (item) {
          return item.from_activity_id == model.id;
        });
        var transition = transitions[findTransition];

        var created_at = this.selectedModel.created_at;
        var user_created_by = this.selectedModel.user_created_by;
        if (user_created_by) {
          html += "<b>" + user_created_by.fullName + "</b>";
        }
        if (transition && transition.label) {
          var reverse = transition.reverse;
          var label = transition.label;
          var Iclass = "ml-1";
          if (reverse) Iclass += " text-danger";
          else Iclass += " text-success";
          html +=
            "<span class='" +
            Iclass +
            "'>đã " +
            label.toLowerCase() +
            "</span>";
        }
        if (created_at) {
          html +=
            " lúc <b>" + moment(created_at).format("HH:mm DD/MM/YYYY") + "</b>";
        }
      }

      this.html = html;
    },
    hasPermission() {
      var data_setting = this.selectedModel.data_setting || {};
      var type_performer = data_setting.type_performer;
      var current_user = this.current_user;
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
    },
    hasRequireSign() {
      var that = this;
      var data_setting = this.selectedModel.data_setting || {};
      var listusersign = data_setting.listusersign || [];
      var findIndex = listusersign.findLastIndex(function (item) {
        return item.user_sign == that.current_user.id && item.status == 1;
      });
      if (findIndex != -1) {
        return true;
      }
      return false;
    },
  },
};
</script>
<style lang="scss" scoped>
#sidebar-right {
  height: 100%;
}

.header {
  padding: 0px 0px 10px;
  border-bottom: 1px solid #d5d5d5;

  .tilte {
    font-weight: bold;
    font-size: 16px;
  }

  .dropdown-menu {
    z-index: 1999;
  }

  .box_transition {
    margin-top: 10px;
    border-top: 1px solid #dddddd;
    padding-top: 10px;
    text-align: center;
  }

  .btn-reverse {
    border-radius: 4px;
    padding: 6px 16px;
    background-color: rgb(255, 255, 255);
    color: rgb(239, 41, 47);
    border: 1px solid rgb(239, 41, 47);
    cursor: pointer;
    font-weight: bold;
  }

  .btn-next {
    border-radius: 4px;
    padding: 6px 16px;
    background-color: #0c9cdd;
    color: white;
    border: 1px solid #0c9cdd;
    cursor: pointer;
    font-weight: bold;
  }
}

.body {
  padding: 20px 10px;
  height: calc(100% - 49px);
  overflow: auto;
}
</style>
