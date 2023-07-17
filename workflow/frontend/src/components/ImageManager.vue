<template>
  <div class="control-section">
    <div id="container" class="fileupload">
      <img
        style="max-height: 100px; max-width: 100%; cursor: pointer"
        :src="image"
        class="image_feature"
        @click="formDialog = true"
      />
    </div>
    <Dialog
      v-model:visible="formDialog"
      header="Hình ảnh"
      :modal="true"
      class="p-fluid"
      style="width: 90vw"
    >
      <FileManagerComponent
        ref="filemanagerObj"
        id="filemanager"
        :ajaxSettings="ajaxSettings"
        v-bind:allowMultiSelection="false"
        :fileOpen="onFileOpen"
        :view="view"
        :uploadSettings="uploadSettings"
      >
      </FileManagerComponent>
    </Dialog>
  </div>
</template>
<script setup>
import { ref, provide } from "vue";
import {
  NavigationPane,
  Toolbar,
  DetailsView,
  FileManagerComponent,
} from "@syncfusion/ej2-vue-filemanager";
import Dialog from "primevue/dialog";

const emit = defineEmits(["choose"]);
provide("filemanager", [DetailsView, NavigationPane, Toolbar]);
const props = defineProps({
  image: String,
});
const formDialog = ref(false);
const view = ref("Details");
const ajaxSettings = ref({
  url: "/v1/FileManager/FileOperations",
  getImageUrl: "/v1/FileManager/GetImage",
  uploadUrl: "/v1/FileManager/Upload",
  downloadUrl: "/v1/FileManager/Download",
});
const uploadSettings = ref({
  autoUpload: true,
  minFileSize: 0,
  maxFileSize: 30000000,
  allowedExtensions: ".jpeg,.jpg,.png",
  autoClose: false,
  directoryUpload: false,
});
const onFileOpen = (args) => {
  console.log(args);
  var fileDetails = args.fileDetails;
  var isFile = fileDetails.isFile;
  var type = fileDetails.type;
  if (isFile && ".jpeg,.png,.jpg".indexOf(type) != -1) {
    var path = fileDetails.filterPath + fileDetails.name;
    path = path.replaceAll("\\", "/");
    emit("choose", path);
    formDialog.value = false;
  }
  return false;
};
</script>
<style>
@import "../../node_modules/@syncfusion/ej2-base/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-icons/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-inputs/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-popups/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-buttons/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-splitbuttons/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-navigations/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-layouts/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-grids/styles/material.css";
@import "../../node_modules/@syncfusion/ej2-vue-filemanager/styles/material.css";
</style>
