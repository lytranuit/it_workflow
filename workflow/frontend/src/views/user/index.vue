<template>
  <div class="row clearfix">
    <div class="col-12">
      <h5 class="card-header drag-handle">
        <RouterLink to="user/add">
          <Button
            label="Tạo mới"
            icon="pi pi-plus"
            class="p-button-success p-button-sm mr-2"
          ></Button>
        </RouterLink>
        <Button
          label="Xuất Excel"
          icon="pi pi-excel"
          class="p-button-primary p-button-sm"
          @click="excel"
        ></Button>
      </h5>
      <section class="card card-fluid">
        <div class="card-body" style="overflow: auto; position: relative">
          <ConfirmDialog></ConfirmDialog>
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
            >
              <template #body="slotProps">
                <template v-if="col.data == 'id'">
                  <RouterLink :to="'/user/edit/' + slotProps.data[col.data]">
                    <i class="fas fa-pencil-alt mr-2"></i>
                    {{ slotProps.data[col.data] }}
                  </RouterLink>
                </template>
                <template v-else-if="col.data == 'image'">
                  <div
                    class="text-center"
                    v-html="slotProps.data[col.data]"
                  ></div>
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
                <a
                  class="p-link text-danger font-16"
                  @click="confirmDelete(slotProps.data['id'])"
                  ><i class="pi pi-trash"></i
                ></a>
              </template>
            </Column>
          </DataTable>
        </div>
      </section>
    </div>

    <Loading :waiting="waiting"></Loading>
  </div>
</template>

<script setup>
import { onMounted, ref, computed, watch } from "vue";
import userApi from "../../api/userApi";
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
    className: "text-center",
    filter: true,
  },

  {
    id: 1,
    label: "Email",
    data: "email",
    className: "text-center",
    filter: true,
  },
  {
    id: 2,
    label: "Họ và tên",
    data: "fullName",
    className: "text-center",
    filter: true,
  },
  {
    id: 3,
    label: "Hình đại diện",
    data: "image",
    className: "text-center",
  },
]);
const filters = ref({
  id: { value: null, matchMode: FilterMatchMode.CONTAINS },
  fullName: { value: null, matchMode: FilterMatchMode.CONTAINS },
  email: { value: null, matchMode: FilterMatchMode.CONTAINS },
});
const totalRecords = ref(0);
const loading = ref(true);
const showing = ref([]);
const column_cache = "columns_user"; ////
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

////Data table
const loadLazyData = () => {
  loading.value = true;
  userApi.table(lazyParams.value).then((res) => {
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
    message: "Bạn có muốn xóa user này?",
    header: "Xác nhận",
    icon: "pi pi-exclamation-triangle",
    accept: () => {
      userApi.delete(id).then((res) => {
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
const excel = () => {
  waiting.value = true;
  userApi.excel().then((res) => {
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
