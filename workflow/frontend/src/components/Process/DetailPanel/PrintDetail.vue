<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ $t("print") }}</div>
    <div class="panelBody">
      <Accordion :activeIndex="0">
        <AccordionTab :header="$t('detail.general')">
          <DefaultDetail :model="model" :onChange="onChange" />
        </AccordionTab>
        <AccordionTab :header="$t('detail.template')">
          <div class="form-group">
            <div class="col-12 mt-2">
              <b class="col-form-label">Loại</b>
              <div class="pt-1 flex-m">
                <select class="form-control form-control-sm" v-model="model.data_setting.type_template">
                  <option value="html">HTML</option>
                  <option value="word">Word</option>
                </select>
              </div>
            </div>
          </div>
          <div class="form-group" v-if="model.data_setting.type_template == 'html'">
            <div class="col-12 mt-2">
              <b class="col-form-label">Mẫu</b>
              <div class="pt-1 flex-m">
                <select class="form-control form-control-sm" v-model="model.data_setting.type_template_html">
                  <option value="nghiphep">Nghĩ phép</option>
                </select>
              </div>
            </div>
          </div>
          <div class="form-group" v-if="model.data_setting.type_template == 'word'">
            <div class="col-12 mt-2">
              <b class="col-form-label">Mẫu：</b>
              <div class="pt-1 flex-m" v-if="file_template">
                <div class="file-icon" data-type="docx"></div>
                <a :href="file_template.url" :download="file_template.name" style="margin-left: 5px">
                  {{ file_template.name }}
                </a>
              </div>
            </div>
            <div class="text-center mt-3">
              <button class="btn btn-success btn-sm" @click="open_select_file()">
                <i class="fas fa-plus mr-1"></i>
                Upload
              </button>
              <input type="file" class="d-none" ref="file" accept=".docx" @change="upload_file()" />
            </div>
          </div>
        </AccordionTab>
      </Accordion>
    </div>
  </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail.vue";
import {
  DxHtmlEditor,
  DxToolbar,
  DxItem,
  DxVariables,
} from "devextreme-vue/html-editor";
import { useProcess } from "../../../stores/process";
const store = useProcess();
export default {
  components: {
    DefaultDetail,
    DxHtmlEditor,
    DxItem,
    DxToolbar,
    DxVariables,
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
