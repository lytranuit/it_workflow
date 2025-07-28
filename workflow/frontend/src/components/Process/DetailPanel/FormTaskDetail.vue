<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ $t("formTask") }}</div>
    <div class="panelBody">
      <Accordion :activeIndex="0">
        <AccordionTab :header="$t('detail.general')">
          <DefaultDetail :model="model" :onChange="onChange" :readOnly="readOnly" />
        </AccordionTab>
        <AccordionTab :header="$t('detail.user_performer')">
          <div class="p-3">
            <div class="my-2">
              <b>Người thực hiện:</b>
              <select class="form-control" v-model="model.type_performer">
                <option value="1">Người thực hiện bước trước</option>
                <option value="2">Người thực hiện bước trước tự chọn</option>
                <option value="3">Bộ phận</option>
                <option value="4">Người dùng</option>
                <option value="5">Người khởi tạo</option>
                <option value="6">Trưởng bộ phận</option>
              </select>
            </div>
            <div class="my-2" v-if="model.type_performer == 1 || model.type_performer == 2">
              <b>Bước:</b>
              <NodePreviousTreeSelect v-model="model.data_setting.block_id"></NodePreviousTreeSelect>
            </div>
            <div class="my-2" v-if="model.type_performer == 4">
              <b>Người dùng:</b>
              <UserTreeSelect multiple v-model="model.data_setting.listuser"></UserTreeSelect>
            </div>
            <div class="my-2" v-if="model.type_performer == 3">
              <b>Bộ phận:</b>
              <DepartmentTreeSelect multiple v-model="model.data_setting.listdepartment"></DepartmentTreeSelect>
            </div>
          </div>
        </AccordionTab>
        <AccordionTab :header="$t('detail.time')">
          <div class="p-3">
            <div class="flex-m m-2">
              <div class="">Thời hạn xử lý</div>
              <div class="ml-auto">
                <div class="custom-control custom-switch switch-primary">
                  <input type="checkbox" class="custom-control-input" id="check" v-model="model.has_deadline" />
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
                <input class="form-control" v-model="model.data_setting.days" :disabled="!model.has_deadline" />
              </div>
              <div class="mx-2">
                <b>Giờ</b>
                <input class="form-control" v-model="model.data_setting.hours" :disabled="!model.has_deadline" />
              </div>
              <div class="mx-2">
                <b>Phút</b>
                <input class="form-control" v-model="model.data_setting.minutes" :disabled="!model.has_deadline" />
              </div>
            </div>
          </div>
        </AccordionTab>
        <AccordionTab :header="$t('detail.fields')">
          <setting-field :model="model"></setting-field>
        </AccordionTab>
        <AccordionTab :header="$t('notification')">
          <div class="flex-m m-2">
            <div class="">Thông báo</div>
            <div class="ml-auto">
              <div class="custom-control custom-switch switch-primary">
                <input type="checkbox" class="custom-control-input" id="checknoti"
                  v-model="model.data_setting.has_notification" />
                <label class="custom-control-label" for="checknoti"></label>
              </div>
            </div>
          </div>
          <setting-mail :model="model"></setting-mail>
        </AccordionTab>
      </Accordion>
    </div>
  </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail.vue";
import SettingField from "./Fields/SettingField.vue";
import SettingMail from "./SettingMail.vue";
import { useProcess } from "../../../stores/process";
import DepartmentTreeSelect from "../../TreeSelect/DepartmentTreeSelect.vue";
import UserTreeSelect from "../../TreeSelect/UserTreeSelect.vue";
import NodeTreeSelect from "../../TreeSelect/NodeTreeSelect.vue";
import NodePreviousTreeSelect from "../../TreeSelect/NodePreviousTreeSelect.vue";
const store = useProcess();
export default {
  components: {
    DefaultDetail,
    SettingField,
    SettingMail,
    UserTreeSelect,
    NodeTreeSelect,
    DepartmentTreeSelect,
    NodePreviousTreeSelect
  },
  data() {
    return {
      activeName: "1",
    };
  },
  methods: {},
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
    onChange: {
      type: Function,
      default: () => { },
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    users() {
      return store.users;
    },
    departments() {
      return store.departments;
    },
    nodes() {
      return store.nodes;
    },
  },
};
</script>
