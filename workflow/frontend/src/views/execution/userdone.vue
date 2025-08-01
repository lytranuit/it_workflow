<template>
  <div class="row clearfix">
    <div class="col-12">
      <section class="card card-fluid">
        <div class="card-body" style="overflow: auto; position: relative">
          <DataTable class="p-datatable-customers" showGridlines :value="datatable" :lazy="true" ref="dt"
            scrollHeight="70vh" v-model:selection="selectedProducts" :paginator="true"
            :rowsPerPageOptions="[10, 50, 100]" :rows="rows" :totalRecords="totalRecords" @page="onPage($event)"
            :rowHover="true" :loading="loading" responsiveLayout="scroll" :resizableColumns="true"
            columnResizeMode="expand" v-model:filters="filters" filterDisplay="menu">
            <template #header>
              <div style="width: 200px">
                <TreeSelect :options="columns" v-model="showing" multiple :limit="0"
                  :limitText="(count) => 'Hiển thị: ' + count + ' cột'">
                </TreeSelect>
              </div>
            </template>

            <template #empty> Không có dữ liệu. </template>
            <Column v-for="col of selectedColumns" :field="col.data" :header="col.label" :key="col.data"
              :showFilterMatchModes="false" :class="col.className">
              <template #body="slotProps">
                <template v-if="col.data == 'id'">
                  <RouterLink :to="'/execution/details/' +
                    slotProps.data['process_version_id'] +
                    '?execution_id=' +
                    slotProps.data['id']
                    ">
                    <i class="fas fa-pencil-alt mr-2"></i>
                    {{ slotProps.data[col.data] }}
                  </RouterLink>
                </template>
                <template v-else-if="col.data == 'status_id'">
                  <Button :label="slotProps.data['status']" :class="'p-button-sm status status_' + slotProps.data['status_id']
                    "></Button>
                </template>
                <template v-else-if="col.data == 'activities'">
                  <div style="position: relative; margin: 0 auto" :id="'conta_' + slotProps.data[col.data]">
                    <span v-for="activity of slotProps.data.activities" v-tooltip.top="activity.label" :class="{
                      e_activity: true,
                      e_activity_success:
                        !activity.blocking && !activity.failed,
                      e_activity_blocking: activity.blocking,
                      e_activity_fail: activity.failed,
                    }"></span>
                  </div>
                </template>
                <div v-else v-html="slotProps.data[col.data]"></div>
              </template>
              <template #filter="{ filterModel, filterCallback }" v-if="col.filter == true">
                <template v-if="col.data == 'status_id'">
                  <select class="form-control" v-model="filterModel.value" @change="filterCallback()">
                    <option value="2">Đang thực hiện</option>
                    <option value="3">Hoàn thành</option>
                    <option value="4">Thất bại</option>
                    <option value="5">Hủy</option>
                  </select>
                </template>
                <template v-else-if="col.data == 'process'">
                  <ProcessRunTreeSelect v-model="filterModel.value" @update:modelValue="filterCallback()"
                    style="width: 300px;">
                  </ProcessRunTreeSelect>
                </template>
                <InputText type="text" v-model="filterModel.value" @keydown.enter="filterCallback()"
                  class="p-column-filter" v-else />
              </template>
            </Column>
            <Column style="width: 1rem">
              <template #body="slotProps">
                <Button icon="pi pi-trash" class="p-button p-button-danger p-button-sm"
                  @click="confirmDelete(slotProps.data['id'])"></Button>
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
import executionApi from "../../api/executionApi";
import Button from "primevue/button";
import DataTable from "primevue/datatable";
import { FilterMatchMode } from "primevue/api";
import Column from "primevue/column"; ////Datatable
import InputText from "primevue/inputtext";
import ConfirmDialog from "primevue/confirmdialog";
import { useConfirm } from "primevue/useconfirm";
import Loading from "../../components/Loading.vue";
import ProcessRunTreeSelect from "../../components/TreeSelect/ProcessRunTreeSelect.vue";

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
    label: "Tiêu đề",
    data: "title",
    filter: true,
  },
  {
    id: 2,
    label: "Quy trình",
    data: "process",
    className: "text-center",
    filter: true,
  },
  {
    id: 3,
    label: "Người tạo",
    data: "user_create",
    className: "text-center",
  },
  {
    id: 4,
    label: "Ngày tạo",
    data: "date_create",
    className: "text-center",
  },
  {
    id: 5,
    label: "Trạng thái",
    data: "status_id",
    className: "text-center",
    filter: true
  },
  {
    id: 6,
    label: "Tiến trình",
    data: "activities",
    className: "text-center",
  },
]);

const filters = ref({
  id: { value: null, matchMode: FilterMatchMode.CONTAINS },
  title: { value: null, matchMode: FilterMatchMode.CONTAINS },
  process: { value: null, matchMode: FilterMatchMode.CONTAINS },
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
    type: "user_done",
  };
});
const dt = ref(null);

////Data table
const loadLazyData = () => {
  loading.value = true;
  executionApi.table(lazyParams.value).then((res) => {
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
    message: "Bạn có muốn xóa Mục này không?",
    header: "Xác nhận",
    icon: "pi pi-exclamation-triangle",
    accept: () => {
      executionApi.delete(id).then((res) => {
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
watch(filters, async (newa, old) => {
  loadLazyData();
});
</script>
<style scoped>
.status {
  border: 0px;
}

.status_2 {
  background-color: #d8f4ff;
  color: #0c9cdd;
}

.status_3 {
  background-color: #1ecab8;
  color: white;
}

.status_4 {
  background-color: #f1646c;
  color: white;
}

.status_5 {
  background-color: #f1646c;
  color: white;
}

.e_activity {
  width: 15px;
  height: 7px;
  background: #d1d1d1;
  display: inline-block;
  margin: 3px;
  cursor: pointer;
}

.e_activity_success {
  background: green;
}

.e_activity_fail {
  background: red;
}

.e_activity_blocking {
  background: #d8f4ff;
  border: 1px double #dbdbdb;
}
</style>
