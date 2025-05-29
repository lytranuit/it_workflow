<template>
  <div class="row clearfix">
    <div class="col-12">
      <h5 class="card-header drag-handle">
        <RouterLink to="/process/add">
          <Button
            label="Tạo mới"
            icon="pi pi-plus"
            class="p-button-success p-button-sm mr-2"
          ></Button>
        </RouterLink>
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
              v-for="col of selectedColumns"
              :field="col.data"
              :header="col.label"
              :key="col.data"
              :showFilterMatchModes="false"
              :class="col.className"
            >
              <template #body="slotProps">
                <template v-if="col.data == 'id'">
                  <RouterLink :to="'/process/edit/' + slotProps.data[col.data]">
                    <i class="fas fa-pencil-alt mr-2"></i>
                    {{ slotProps.data[col.data] }}
                  </RouterLink>
                </template>
                <template v-else-if="col.data == 'group'">
                  <Button
                    :label="slotProps.data[col.data].name"
                    class="p-button-sm mr-2"
                    :style="
                      'background-color: ' +
                      slotProps.data[col.data].color +
                      ';border-color: ' +
                      slotProps.data[col.data].color +
                      ';'
                    "
                  ></Button>
                </template>
                <template v-else-if="col.data == 'status_id'">
                  <Button
                    label="Draft"
                    class="p-button-secondary p-button-sm mr-2"
                    v-if="slotProps.data[col.data] == 1"
                  ></Button>
                  <Button
                    label="Release"
                    class="p-button-primary p-button-sm mr-2"
                    v-else-if="slotProps.data[col.data] == 2"
                  ></Button>
                </template>
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
            <Column style="width: 1rem">
              <template #body="slotProps">
                <span class="p-buttonset">
                  <Button
                    icon="fas fa-chart-pie"
                    class="p-button-success p-button-sm"
                    title="Tổng quan"
                    @click="routertoDashboard(slotProps.data['id'])"
                  ></Button>
                  <Button
                    v-if="slotProps.data['status_id'] == 1"
                    icon="fas fa-arrow-up"
                    class="p-button-success p-button-sm"
                    @click="confirmRelease(slotProps.data['id'])"
                  ></Button>
                  <Button
                    icon="pi pi-download"
                    class="p-button-primary p-button-sm"
                    @click="excel(slotProps.data['id'])"
                  ></Button>
                  <Button
                    icon="pi pi-trash"
                    class="p-button-danger p-button-sm"
                    @click="confirmDelete(slotProps.data['id'])"
                  ></Button>
                </span>
              </template>
            </Column>
          </DataTable>
        </div>
      </section>
    </div>
    <ConfirmDialog></ConfirmDialog>
    <Loading :waiting="waiting"></Loading>
  </div>
</template>

<script setup>
import { onMounted, ref, computed, watch } from "vue";
import processApi from "../../api/processApi";
import Button from "primevue/button";
import DataTable from "primevue/datatable";
import { FilterMatchMode } from "primevue/api";
import Column from "primevue/column"; ////Datatable
import InputText from "primevue/inputtext";
import ConfirmDialog from "primevue/confirmdialog";
import { useConfirm } from "primevue/useconfirm";
import Loading from "../../components/Loading.vue";
const confirm = useConfirm();
const datatable = ref();
const columns = ref([
  {
    id: 0,
    label: "ID",
    data: "id",
    filter: true,
  },

  {
    id: 1,
    label: "Quy trình",
    data: "name",
    filter: true,
  },
  {
    id: 2,
    label: "Nhóm Quy trình",
    data: "group",
    className: "text-center",
    filter: true,
  },
  {
    id: 3,
    label: "Trạng thái",
    data: "status_id",
    className: "text-center",
  },
]);
const filters = ref({
  id: { value: null, matchMode: FilterMatchMode.CONTAINS },
  name: { value: null, matchMode: FilterMatchMode.CONTAINS },
  group_id: { value: null, matchMode: FilterMatchMode.CONTAINS },
  status_id: { value: null, matchMode: FilterMatchMode.CONTAINS },
});
const totalRecords = ref(0);
const loading = ref(true);
const showing = ref([]);
const column_cache = "columns_process"; ////
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
const routertoDashboard = (id) => {
  window.open("/process/dashboard/" + id, "_blank");
};
const dt = ref(null);

////Data table
const loadLazyData = () => {
  loading.value = true;
  processApi.table(lazyParams.value).then((res) => {
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

const confirmDelete = (id) => {
  confirm.require({
    message: "Bạn có muốn xóa process này?",
    header: "Xác nhận",
    icon: "pi pi-exclamation-triangle",
    accept: () => {
      processApi.remove({ item: [id] }).then((res) => {
        loadLazyData();
      });
    },
  });
};
const dashboard = (id) => {};
const confirmRelease = (id) => {
  confirm.require({
    message: "Bạn có muốn phát hành process này?",
    header: "Xác nhận",
    icon: "pi pi-exclamation-triangle",
    accept: () => {
      processApi.release(id).then((res) => {
        loadLazyData();
      });
    },
  });
};
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
const waiting = ref();
const excel = (id) => {
  waiting.value = true;
  processApi.export(id).then((res) => {
    console.log(res);
    waiting.value = false;
    if (res.success) {
      window.open(res.link, "_blank");
    } else {
      alert(res.message);
    }
  });
};
watch(filters, async (newa, old) => {
  loadLazyData();
});
</script>
