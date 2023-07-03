<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ i18n["suggest"] }}</div>
    <div class="panelBody">
      <Accordion :activeIndex="0">
        <AccordionTab :header="i18n['detail.general']">
          <DefaultDetail
            :model="model"
            :onChange="onChange"
            :readOnly="readOnly"
          />
        </AccordionTab>
        <AccordionTab :header="i18n['detail.user_performer']">
          <div class="p-3">
            <div class="my-2">
              <b>Người thực hiện:</b>
              <select class="form-control" v-model="model.type_performer">
                <option value="1">Người thực hiện bước trước</option>
                <option value="2">Người thực hiện bước trước tự chọn</option>
                <option value="3">Bộ phận</option>
                <option value="4">Người dùng</option>
                <option value="5">Người khởi tạo</option>
              </select>
            </div>
            <div
              class="my-2"
              v-if="model.type_performer == 1 || model.type_performer == 2"
            >
              <b>Bước:</b>
              <TreeSelect
                :append-to-body="false"
                :options="prev_nodes"
                v-model="model.data_setting.block_id"
              ></TreeSelect>
            </div>
            <div class="my-2" v-if="model.type_performer == 4">
              <b>Người dùng:</b>
              <TreeSelect
                multiple
                v-model="model.data_setting.listuser"
                :options="users"
                :append-to-body="false"
              />
            </div>
            <div class="my-2" v-if="model.type_performer == 3">
              <b>Bộ phận:</b>
              <TreeSelect
                multiple
                v-model="model.data_setting.listdepartment"
                :options="departments"
                :append-to-body="false"
              />
            </div>
          </div>
        </AccordionTab>
        <AccordionTab :header="i18n['detail.time']">
          <div class="p-3">
            <div class="flex-m m-2">
              <div class="">Thời hạn xử lý</div>
              <div class="ml-auto">
                <div class="custom-control custom-switch switch-primary">
                  <input
                    type="checkbox"
                    class="custom-control-input"
                    id="check"
                    v-model="model.has_deadline"
                  />
                  <label class="custom-control-label" for="check"></label>
                </div>
              </div>
            </div>
            <div class="flex-m m-2">
              <div class="">Bước có hạn xử lý sau:</div>
            </div>
            <div class="flex-m">
              <div class="mx-2">
                <b>Ngày</b>
                <input
                  class="form-control"
                  v-model="model.data_setting.days"
                  :disabled="!model.has_deadline"
                />
              </div>
              <div class="mx-2">
                <b>Giờ</b>
                <input
                  class="form-control"
                  v-model="model.data_setting.hours"
                  :disabled="!model.has_deadline"
                />
              </div>
              <div class="mx-2">
                <b>Phút</b>
                <input
                  class="form-control"
                  v-model="model.data_setting.minutes"
                  :disabled="!model.has_deadline"
                />
              </div>
            </div>
          </div>
        </AccordionTab>
        <AccordionTab :header="i18n['esign']">
          <div class="cont-empty my-3 text-center">Chọn mẫu gợi ý</div>
          <div class="row justify-content-center">
            <div class="col-10">
              <TreeSelect
                :options="prev_nodes_print"
                v-model="model.data_setting.blocks_esign_id"
                :append-to-body="false"
              ></TreeSelect>
            </div>
          </div>
        </AccordionTab>
      </Accordion>
    </div>
  </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail.vue";
import { useProcess } from "../../../stores/Process/store";
const store = useProcess();
export default {
  inject: ["i18n"],
  components: {
    DefaultDetail,
  },

  computed: {
    users() {
      return store.users;
    },
    departments() {
      return store.departments;
    },
    nodes() {
      return store.data.nodes;
    },
    prev_nodes() {
      var nodes = this.nodes;
      var model = this.model;
      return nodes.filter(function (item) {
        return item.stt < model.stt;
      });
    },
    prev_nodes_print() {
      var nodes = this.nodes;
      var model = this.model;
      return nodes.filter(function (item) {
        return item.stt < model.stt && item.clazz == "printSystem";
      });
    },
  },
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
    onChange: {
      type: Function,
      default: () => {},
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
