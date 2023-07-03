<template>
  <div class="row clearfix">
    <Toast />
    <ConfirmDialog></ConfirmDialog>
    <div class="col-12">
      <h5 class="card-header drag-handle">
        <Button
          label="Tạo mới"
          icon="pi pi-plus"
          class="p-button-success p-button-sm mr-2"
          @click="openNew"
        ></Button>
        <Button
          label="Lưu lại"
          icon="pi pi-save"
          class="p-button-primary p-button-sm float-right"
          @click="saveorder"
        ></Button>
      </h5>
      <section class="card card-fluid">
        <div class="card-body" style="overflow: auto; position: relative">
          <div class="dd" id="nestable_list_1">
            <ol class="dd-list ui-sortable" id="nestable">
              <TreeDepartment
                v-for="node in datatable"
                :key="node.id"
                :node="node"
                @edit="edit"
                @remove="confirmDelete"
              />
            </ol>
          </div>
        </div>
      </section>
    </div>

    <Dialog
      v-model:visible="productDialog"
      :header="headerForm"
      :modal="true"
      class="p-fluid"
    >
      <div class="row mb-2">
        <div class="field col">
          <label for="name">Tên <span class="text-danger">*</span></label>
          <InputText
            id="name"
            class="p-inputtext-sm"
            v-model.trim="model.name"
            required="true"
            :class="{ 'p-invalid': submitted && !model.name }"
          />
          <small class="p-error" v-if="submitted && !model.name"
            >Required.</small
          >
        </div>
        <div class="field col">
          <label for="name">Màu sắc</label>
          <div>
            <input
              type="color"
              class="form-control form-control-sm"
              v-model="model.color"
            />
          </div>
        </div>
      </div>

      <template #footer>
        <Button
          label="Cancel"
          icon="pi pi-times"
          class="p-button-text"
          @click="hideDialog"
        ></Button>
        <Button
          label="Save"
          icon="pi pi-check"
          class="p-button-text"
          @click="saveProduct"
        ></Button>
      </template>
    </Dialog>
    <Loading :waiting="waiting"></Loading>
  </div>
</template>

<script setup>
import { onMounted, ref, watch, computed } from "vue";
import Loading from "../../components/Loading.vue";
import TreeDepartment from "../../components/TreeDepartment.vue";
// import the styles

import Button from "primevue/button";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";

import Toast from "primevue/toast";
import ConfirmDialog from "primevue/confirmdialog";
import { useToast } from "primevue/usetoast";
import departmentApi from "../../api/departmentApi";
import { useConfirm } from "primevue/useconfirm";
const toast = useToast();
const confirm = useConfirm();

const loading = ref(true);
////Form
const model = ref();
const submitted = ref();
const headerForm = ref("");
///Control
const productDialog = ref();
const waiting = ref(false);
const datatable = ref([]);
////
const loadLazyData = () => {
  loading.value = true;
  departmentApi.get().then((res) => {
    datatable.value = res;
    loading.value = false;
    setTimeout(() => {
      $("#nestable").nestedSortable({
        forcePlaceholderSize: true,
        items: "li",
        opacity: 0.6,
        placeholder: "dd-placeholder",
      });
    }, 250);
  });
};
///Form
const valid = () => {
  if (!model.value.name.trim()) return false;
  return true;
};
const saveProduct = () => {
  submitted.value = true;
  if (!valid()) return;
  waiting.value = true;
  departmentApi.save(model.value).then((res) => {
    waiting.value = false;
    if (res.success) {
      if (old_key.value != null) {
        //edit
        toast.add({
          severity: "success",
          summary: "Thành công",
          detail: "Cập nhật " + model.value.name + " thành công",
          life: 3000,
        });
      } else {
        toast.add({
          severity: "success",
          summary: "Thành công",
          detail: "Tạo mới thành công",
          life: 3000,
        });
      }
    } else {
      toast.add({
        severity: "error",
        summary: "Lỗi",
        detail: res.message,
        life: 3000,
      });
    }
    loadLazyData();
    model.value = {};
    productDialog.value = false;
  });
};
const old_key = ref();
const edit = (m) => {
  console.log(m.id);
  old_key.value = m.id;
  headerForm.value = m.id;
  model.value = { ...m };
  productDialog.value = true;
};
const openNew = () => {
  model.value = {};
  old_key.value = null;
  headerForm.value = "Tạo mới";
  submitted.value = false;
  productDialog.value = true;
};
const confirmDelete = (id) => {
  confirm.require({
    message: "Bạn có muốn xóa mục này?",
    header: "Xác nhận",
    icon: "pi pi-exclamation-triangle",
    accept: () => {
      waiting.value = true;
      departmentApi.remove({ item: [id] }).then((res) => {
        waiting.value = false;
        if (res.success) {
          toast.add({
            severity: "success",
            summary: "Thành công",
            detail: "Đã xóa thành công!",
            life: 3000,
          });
        } else {
          toast.add({
            severity: "error",
            summary: "Lỗi",
            detail: res.message,
            life: 3000,
          });
        }
        loadLazyData();
      });
    },
  });
};
const hideDialog = () => {
  productDialog.value = false;
  submitted.value = false;
};
const saveorder = () => {
  var arraied = $("#nestable").nestedSortable("toArray", {
    excludeRoot: true,
  });
  waiting.value = true;
  departmentApi.saveorder(arraied).then((res) => {
    waiting.value = false;
    if (res.success) {
      toast.add({
        severity: "success",
        summary: "Thành công",
        detail: "Cập nhật thành công",
        life: 3000,
      });
    } else {
      toast.add({
        severity: "error",
        summary: "Lỗi",
        detail: res.message,
        life: 3000,
      });
    }
    loadLazyData();
    model.value = {};
    productDialog.value = false;
  });
};
////Core
onMounted(() => {
  loadLazyData();
});
</script>
