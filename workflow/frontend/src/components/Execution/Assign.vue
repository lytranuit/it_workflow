<template>
  <div id="popup-assign">
    <div
      id="myModal-assign"
      class="modal"
      tabindex="-1"
      role="dialog"
      :data-backdrop="checkModal()"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Phân công</h5>
            <button
              type="button"
              class="close"
              data-dismiss="modal"
              aria-label="Close"
              v-if="!required"
            >
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <div class="row">
              <form id="form-assign" class="col-12">
                <Accordion :activeIndex="activeName" multiple>
                  <AccordionTab
                    :header="element.block.label"
                    :key="index"
                    v-for="(element, index) in data_custom_block"
                  >
                    <div class="row bg-white">
                      <div class="col-12">Người thực hiện:</div>
                      <div class="col-3">
                        <select
                          class="form-control"
                          :name="'sel_' + index"
                          v-model="element.data_setting.type_performer"
                          required
                        >
                          <option value="3">Bộ phận</option>
                          <option value="4">Người dùng</option>
                        </select>
                      </div>
                      <div class="col-9">
                        <div v-if="element.data_setting.type_performer == 4">
                          <UserTreeSelect
                            multiple
                            required
                            v-model="element.data_setting.listuser"
                            :name="'user_' + index"
                          ></UserTreeSelect>
                        </div>
                        <div v-if="element.data_setting.type_performer == 3">
                          <DepartmentTreeSelect
                            multiple
                            required
                            v-model="element.data_setting.listdepartment"
                            :name="'dep_' + index"
                          ></DepartmentTreeSelect>
                        </div>
                      </div>
                    </div>
                  </AccordionTab>
                </Accordion>
              </form>
            </div>
          </div>
          <div class="modal-footer justify-content-center">
            <button type="button" class="btn btn-success" @click="save()">
              Lưu lại
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import { useProcess } from "../../stores/process";
import DepartmentTreeSelect from "../TreeSelect/DepartmentTreeSelect.vue";
import UserTreeSelect from "../TreeSelect/UserTreeSelect.vue";
var store = useProcess();
export default {
  components: { UserTreeSelect, DepartmentTreeSelect },
  props: {
    data_custom_block: {
      type: Array,
      default: () => [],
    },
    required: {
      type: Boolean,
      default: () => true,
    },
  },
  data() {
    return {
      //activeName: [0, 1, 2],
    };
  },
  computed: {
    activeName() {
      return this.data_custom_block.map(function (item, key) {
        return key;
      });
    },
    departments() {
      return store.departments;
    },
    users() {
      return store.users;
    },
  },
  mounted() {
    var that = this;
    $("#myModal-assign").modal("show");
    $("#myModal-assign").on("hidden.bs.modal", function (e) {
      that.$emit("close");
    });
  },
  methods: {
    save() {
      var vaild = $("#form-assign").valid();
      if (!vaild) {
        return;
      }
      ////VAILD TIEU DE
      if (!$(".tieu_de").val()) {
        //alert("Bạn chưa nhập tiêu đề!");
        $(".tieu_de").focus();
        return;
      }
      this.$emit("save_data");
    },
    checkModal() {
      var text = "static";
      if (!this.required) text = false;
      return text;
    },
  },
};
</script>

<style lang="scss"></style>
