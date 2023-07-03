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
          label="Xóa"
          icon="pi pi-trash"
          class="p-button-danger p-button-sm"
          @click="confirmDeleteSelected"
          :disabled="!selectedProducts || !selectedProducts.length"
        ></Button>
      </h5>
      <section class="card card-fluid">
        <div class="card-body" style="overflow: auto; position: relative">
          <DataTable
            class="p-datatable-customers"
            showGridlines
            :value="datatable"
            :lazy="true"
            ref="dt"
            scrollHeight="70vh"
            v-model:selection="selectedProducts"
            :paginator="true"
            :rowsPerPageOptions="[10, 50, 100]"
            :rows="rows"
            :totalRecords="totalRecords"
            @page="onPage($event)"
            :rowHover="true"
            :loading="loading"
            responsiveLayout="scroll"
            :resizableColumns="true"
            columnResizeMode="expand"
            v-model:filters="filters"
            filterDisplay="menu"
          >
            <template #header>
              <div style="width: 200px">
                <TreeSelect
                  :options="columns"
                  v-model="showing"
                  multiple
                  :limit="0"
                  :limitText="(count) => 'Hiển thị: ' + count + ' cột'"
                >
                </TreeSelect>
              </div>
            </template>

            <template #empty> Không có dữ liệu. </template>
            <Column
              selectionMode="multiple"
              style="width: 3rem"
              :exportable="false"
            ></Column>
            <Column
              v-for="col of selectedColumns"
              :field="col.data"
              :header="col.label"
              :key="col.data"
              :showFilterMatchModes="false"
            >
              <template #body="slotProps">
                <div
                  :style="
                    'background:' +
                    slotProps.data[col.data] +
                    ';display: inline-block; width: 50px; height: 20px'
                  "
                  v-if="col.data == 'color'"
                ></div>
                <div v-else v-html="slotProps.data[col.data]"></div>
              </template>
              <template
                #filter="{ filterModel, filterCallback }"
                v-if="col.filter == true"
              >
                <InputText
                  type="text"
                  v-model="filterModel.value"
                  @keydown.enter="filterCallback()"
                  class="p-column-filter"
                />
              </template>
            </Column>
            <Column>
              <template #body="slotProps">
                <a
                  class="p-link text-warning mr-2 font-16"
                  @click="edit(slotProps.data)"
                  ><i class="pi pi-pencil"></i
                ></a>
                <a
                  class="p-link text-danger font-16"
                  @click="confirmDelete(slotProps.data)"
                  ><i class="pi pi-trash"></i
                ></a>
              </template>
            </Column>
          </DataTable>
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
// import the component

import DataTable from "primevue/datatable";
import { FilterMatchMode } from "primevue/api";
import Column from "primevue/column";
import Button from "primevue/button";
import Dialog from "primevue/dialog";
import InputText from "primevue/inputtext";

import Toast from "primevue/toast";
import ConfirmDialog from "primevue/confirmdialog";
import { useToast } from "primevue/usetoast";
import processGroupApi from "../../api/processGroupApi";
import { useConfirm } from "primevue/useconfirm";
const toast = useToast();
const confirm = useConfirm();

////Datatable
const datatable = ref();
const columns = ref([
  {
    id: 0,
    label: "ID",
    data: "id",
    className: "text-center",
    filter: true,
  },
  {
    id: 1,
    label: "Nhóm quy trình",
    data: "name",
    className: "text-center",
    filter: true,
  },
  {
    id: 2,
    label: "Màu sắc",
    data: "color",
    className: "text-center",
  },
]);
const filters = ref({
  id: { value: null, matchMode: FilterMatchMode.CONTAINS },
  name: { value: null, matchMode: FilterMatchMode.CONTAINS },
});
const totalRecords = ref(0);
const loading = ref(true);
const showing = ref([]);
const column_cache = "columns_processGroup"; ////
const first = ref(0);
const rows = ref(10);
const draw = ref(0);
const selectedProducts = ref();
const selectedColumns = computed(() => {
  return columns.value.filter((col) => showing.value.includes(col.id));
});
const lazyParams = computed(() => {
  let data_filters = {};
  for (let key in filters.value) {
    data_filters[key] = filters.value[key].value;
  }
  return {
    draw: draw.value,
    start: first.value,
    length: rows.value,
    filters: data_filters,
  };
});
const dt = ref(null);

////Form
const model = ref();
const submitted = ref();
const headerForm = ref("");
///Control
const productDialog = ref();
const waiting = ref(false);

////Data table
const loadLazyData = () => {
  loading.value = true;
  processGroupApi.table(lazyParams.value).then((res) => {
    // console.log(res);
    datatable.value = res.data;
    totalRecords.value = res.recordsFiltered;
    loading.value = false;
  });
};
const onPage = (event) => {
  first.value = event.first;
  rows.value = event.rows;
  draw.value = draw.value + 1;
  loadLazyData();
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
  processGroupApi.save(model.value).then((res) => {
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
const confirmDeleteSelected = () => {
  confirm.require({
    message: "Bạn có muốn xóa các mục đã chọn?",
    header: "Xác nhận",
    icon: "pi pi-exclamation-triangle",
    accept: () => {
      let items = selectedProducts.value.map((item) => {
        return item.id;
      });
      waiting.value = true;
      processGroupApi.remove({ item: items }).then((res) => {
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
        selectedProducts.value = null;
        loadLazyData();
      });
    },
  });
};
const confirmDelete = (m) => {
  confirm.require({
    message: "Bạn có muốn xóa mục này?",
    header: "Xác nhận",
    icon: "pi pi-exclamation-triangle",
    accept: () => {
      waiting.value = true;
      processGroupApi.remove({ item: [m.id] }).then((res) => {
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
////Core
onMounted(() => {
  let cache = localStorage.getItem(column_cache);
  if (!cache) {
    showing.value = columns.value.map((item) => {
      return item.id;
    });
  } else {
    showing.value = JSON.parse(cache);
  }
  loadLazyData();
});
watch(showing, async (newa, old) => {
  localStorage.setItem(column_cache, JSON.stringify(newa));
});
watch(filters, async (newa, old) => {
  first.value = 0;
  loadLazyData();
});
</script>
