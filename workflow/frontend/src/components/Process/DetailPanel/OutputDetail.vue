<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ $t("output") }}</div>
    <div class="panelBody">
      <Accordion :activeIndex="0">
        <AccordionTab :header="$t('detail.general')">
          <DefaultDetail :model="model" :onChange="onChange" />
        </AccordionTab>
        <AccordionTab :header="$t('detail.output')">
          <div class="form-group">
            <div class="col-12 mt-2">
              <b class="col-form-label">Loại</b>
              <div class="pt-1 flex-m">
                <select class="form-control form-control-sm" v-model="model.data_setting.type_output">
                  <option value="esign">File trình ký</option>
                </select>
              </div>
            </div>
            <div class="col-12 mt-2" v-if="model.data_setting.type_output == 'esign'">
              <b class="col-form-label">Loại trình ký</b>
              <div class="pt-1 flex-m">
                <EsignTypeTreeSelect v-model="model.data_setting.esign_type_id"></EsignTypeTreeSelect>
              </div>
            </div>
            <div class="col-12 mt-2">
              <b class="col-form-label">Từ trường</b>
              <div class="pt-1 flex-m">
                <FieldFileTreeSelect v-model="model.data_setting.field_output"></FieldFileTreeSelect>
              </div>
            </div>
          </div>
        </AccordionTab>
      </Accordion>
    </div>
  </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail.vue";
import FieldFileTreeSelect from '../../TreeSelect/FieldFileTreeSelect.vue';
import {
  DxHtmlEditor,
  DxToolbar,
  DxItem,
  DxVariables,
} from "devextreme-vue/html-editor";
import { useProcess } from "../../../stores/process";
import EsignTypeTreeSelect from "../../TreeSelect/EsignTypeTreeSelect.vue";
const store = useProcess();
export default {
  components: {
    DefaultDetail,
    DxHtmlEditor,
    DxItem,
    DxToolbar,
    DxVariables,
    FieldFileTreeSelect,
    EsignTypeTreeSelect
  },
  data() {
    return {
      activeName: "1",
    };
  },
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
    onChange: {
      type: Function,
      default: () => { },
    },
  },
  computed: {
    file_template() {
      return this.model.data_setting.file_template;
    },
    nodes() {
      return store.data.nodes;
    },
  },
  mounted() { },
  methods: {
    open_select_file() {
      $(this.$refs["file"]).trigger("click");
    },
    async upload_file() {
      var files = $(this.$refs["file"])[0].files;
      var formData = new FormData();
      formData.append("files", files[0]);
      var resp = await $.ajax({
        type: "POST",
        url: "/v1/api/uploadFileTemplate",
        data: formData,
        dataType: "json",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
      });
      this.model.data_setting.file_template = resp.item;

      var label = this.model.label;
      this.model.label = rand();
      this.model.label = label;
    },
  },
};
</script>
<style></style>
