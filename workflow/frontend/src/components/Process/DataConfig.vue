<template>
  <div>
    <Button label="Cấu hình" class="p-button-warning p-button-sm" style="float: right; margin-top: 4px; margin-right: 4px"
      data-toggle="modal" data-target="#modal-data-config"></Button>
    <div id="modal-data-config" class="modal" tabindex="-1" role="dialog" data-backdrop="static">
      <div class="modal-dialog modal-xl" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <b class="modal-title font-16"> Cấu hình </b>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <div style="overflow: auto;max-height: 700px;">
              <JsonEditorVue v-model="data_temp" :onChange="(updatedContent) => {
                data_temp = updatedContent.json
              }"></JsonEditorVue>
            </div>
          </div>
          <div class="modal-footer">
            <Button label="Lưu lại" class="p-button-primary p-button-sm"
              style="float: right; margin-top: 4px; margin-right: 4px" @click="save_config"></Button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { storeToRefs } from "pinia";
import Button from "primevue/button";
import { useProcess } from "../../stores/process";
import { computed, onMounted } from "vue";
import JsonEditorVue from 'json-editor-vue'

const store = useProcess();
const { data_temp } = storeToRefs(store);
const emit = defineEmits(["save"]);
const save_config = () => {
  emit("save");
  $("#modal-data-config").modal("hide");
}
onMounted(() => {
  // data_temp.value = JSON.stringify(data_temp.value);
})
</script>
