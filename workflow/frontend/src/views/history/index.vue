<template>
  <div class="row clearfix">
    <div class="col-12">
      <section class="card card-fluid">
        <div class="card-body" style="overflow: auto; position: relative">
          <Toast />
          <Loading :waiting="waiting"></Loading>
          <DataTable
            showGridlines
            :value="datatable"
            :lazy="true"
            ref="dt"
            :paginator="true"
            scrollHeight="70vh"
            class="p-datatable-customers"
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
              <div class="d-flex">
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
                <div class="ml-auto">
                  <Button
                    label="In Excel"
                    icon="pi pi-print"
                    class="p-button-primary p-button-sm mr-2"
                    @click="print"
                  ></Button>
                </div>
              </div>
            </template>

            <template #empty>
              <div class="text-center">Không có dữ liệu.</div>
            </template>
            <Column
              v-for="col of selectedColumns"
              :field="col.data"
              :header="col.label"
              :key="col.data"
            >
              <template #body="slotProps">
                <div v-html="slotProps.data[col.data]"></div>
              </template>

              <template
                #filter="{ filterModel, filterCallback }"
                v-if="col.filter == true"
              >
                <template v-if="col.data == 'datetime'">
                  <Calendar
                    v-model="datetime"
                    dateFormat="mm/dd/yy"
                    placeholder="từ ngày - đến ngày"
                    mask="99/99/9999"
                    selectionMode="range"
                    :hideOnRangeSelection="true"
                  />
                </template>

                <InputText
                  type="text"
                  v-model="filterModel.value"
                  @keydown.enter="filterCallback()"
                  class="p-column-filter"
                  v-else
                />
              </template>
            </Column>
          </DataTable>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, watch, computed } from "vue";
// import the component

import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Toast from "primevue/toast";
import Button from "primevue/button";
import { FilterMatchMode } from "primevue/api";
import Loading from "../../components/Loading.vue";
import InputText from "primevue/inputtext";
import Calendar from "primevue/calendar";
import historyApi from "../../api/historyApi";
const waiting = ref();
const datetime = ref();
const columns = ref([
  {
    id: 0,
    label: "ID",
    data: "id",
    className: "text-center",
  },
  {
    id: 1,
    label: "Người thay đổi",
    data: "user",
    className: "text-center",
    filter: true,
  },
  {
    id: 2,
    label: "Ngày thay đổi",
    data: "datetime",
    className: "text-center",
    filter: true,
  },
  {
    id: 3,
    label: "Loại",
    data: "type",
    className: "text-center",
    filter: true,
  },
  {
    id: 4,
    label: "Mô tả",
    data: "description",
    className: "text-center",
    filter: true,
  },
  {
    id: 5,
    label: "Bảng",
    data: "tableName",
    className: "text-center",
    filter: true,
  },
  {
    id: 6,
    label: "Key",
    data: "primaryKey",
    className: "text-center",
    filter: true,
  },
  {
    id: 7,
    label: "Giá trị cũ",
    data: "oldValues",
    className: "text-center",
    filter: true,
  },
  {
    id: 8,
    label: "Giá trị mới",
    data: "newValues",
    className: "text-center",
    filter: true,
  },
]);

const filters = ref({
  user: { value: null, matchMode: FilterMatchMode.CONTAINS },
  description: { value: null, matchMode: FilterMatchMode.CONTAINS },
  datetime: { value: null, matchMode: FilterMatchMode.CONTAINS },
  type: { value: null, matchMode: FilterMatchMode.CONTAINS },
  tableName: { value: null, matchMode: FilterMatchMode.CONTAINS },
  primaryKey: { value: null, matchMode: FilterMatchMode.CONTAINS },
  oldValues: { value: null, matchMode: FilterMatchMode.CONTAINS },
  newValues: { value: null, matchMode: FilterMatchMode.CONTAINS },
});
const datatable = ref();
const totalRecords = ref(0);
const loading = ref(true);
const showing = ref([]);
const selectedColumns = computed(() => {
  return columns.value.filter((col) => showing.value.includes(col.id));
});
const column_cache = "columns_history";
const first = ref(0);
const rows = ref(10);
const draw = ref(0);
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
    datetime: datetime.value,
  };
});

const onPage = (event) => {
  first.value = event.first;
  rows.value = event.rows;
  draw.value = draw.value + 1;
  loadLazyData();
};
const loadLazyData = () => {
  loading.value = true;
  historyApi.table(lazyParams.value).then((res) => {
    // console.log(res);
    datatable.value = res.data;
    totalRecords.value = res.recordsFiltered;
    loading.value = false;
  });
};

const print = () => {
  waiting.value = true;
  ///
  historyApi.export(lazyParams.value).then((res) => {
    waiting.value = false;
    if (res.success) {
      window.open(res.link, "_blank");
    } else {
      alert(res.message);
    }
  });
};

const dt = ref(null);

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
  loadLazyData();
});
</script>
