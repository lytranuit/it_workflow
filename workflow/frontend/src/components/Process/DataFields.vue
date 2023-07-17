<template>
  <div>
    <Button
      label="Trường dữ liệu"
      class="p-button-primary p-button-sm"
      style="float: right; margin-top: 4px; margin-right: 4px"
      data-toggle="modal"
      data-target="#modal-data-field"
    ></Button>
    <div
      id="modal-data-field"
      class="modal"
      tabindex="-1"
      role="dialog"
      data-backdrop="static"
    >
      <div class="modal-dialog modal-xl" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <b class="modal-title font-16"> Trường dữ liệu </b>
            <button
              type="button"
              class="close"
              data-dismiss="modal"
              aria-label="Close"
            >
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <table class="table text-center table-bordered mb-0 bg-white">
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Tên</th>
                  <th>Kiểu dữ liệu</th>
                  <th>Block</th>
                  <th>Biến</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(field, index) in fields" :key="index">
                  <td>{{ index + 1 }}</td>
                  <td>{{ field.name }}</td>
                  <td>{{ field.type }}</td>
                  <td>{{ field.block }}</td>
                  <td>
                    <input
                      class="form-control font-control-sm"
                      v-model="field.variable"
                    />
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-primary"
              @click="save_variable()"
            >
              Lưu lại
            </button>
            <button
              type="button"
              class="btn btn-secondary"
              data-dismiss="modal"
            >
              Hủy bỏ
            </button>
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
import { computed } from "vue";
const store = useProcess();
const { data } = storeToRefs(store);
const fields = computed(() => {
  var fields = [];
  var nodes = data.value.nodes || [];
  for (var node of nodes) {
    var fields_n = node.fields || [];
    for (var field of fields_n) {
      fields.push({
        id: field.id,
        name: field.name,
        type: field.type,
        variable: field.variable,
        block: node.label,
      });
    }
  }
  return fields;
});
const save_variable = () => {
  for (var field of fields.value) {
    var id = field.id;
    for (var node of data.value.nodes) {
      var fields_n = node.fields || [];
      for (var field1 of fields_n) {
        if (field1.id == id) {
          field1.variable = field.variable;
        }
      }
    }
  }
  $("#modal-data-field").modal("hide");
};
</script>
