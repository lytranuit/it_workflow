<template>
  <div class="row justify-content-center">
    <div class="col-md-6 col-lg-3">
      <div class="card report-card bg-purple-gradient shadow-purple">
        <div class="card-body">
          <div class="float-right">
            <i class="dripicons-wallet report-main-icon bg-icon-purple"></i>
          </div>
          <span class="badge badge-light text-purple">Tổng số lượt chạy</span>
          <h3 class="my-3">{{ execution_amount }}</h3>
        </div>
        <!--end card-body-->
      </div>
      <!--end card-->
    </div>
    <!--end col-->
    <div class="col-md-6 col-lg-3">
      <div class="card report-card bg-warning-gradient shadow-warning">
        <div class="card-body">
          <div class="float-right">
            <i class="fas fa-spinner report-main-icon bg-icon-warning"></i>
          </div>
          <span class="badge badge-light text-warning">Đang thực hiện</span>
          <h3 class="my-3">{{ execution_wait }}</h3>
        </div>
        <!--end card-body-->
      </div>
      <!--end card-->
    </div>
    <!--end col-->
    <div class="col-md-6 col-lg-3">
      <div class="card report-card bg-success-gradient shadow-success">
        <div class="card-body">
          <div class="float-right">
            <i class="dripicons-checkmark report-main-icon bg-icon-success"></i>
          </div>
          <span class="badge badge-light text-success">Đã hoàn thành</span>
          <h3 class="my-3">{{ execution_success }}</h3>
        </div>
        <!--end card-body-->
      </div>
      <!--end card-->
    </div>
    <!--end col-->

    <div class="col-md-6 col-lg-3">
      <div class="card report-card bg-danger-gradient shadow-danger">
        <div class="card-body">
          <div class="float-right">
            <i class="fas fa-ban report-main-icon bg-icon-danger"></i>
          </div>
          <span class="badge badge-light text-danger">Đã thất bại</span>
          <h3 class="my-3">{{ execution_fail }}</h3>
        </div>
        <!--end card-body-->
      </div>
      <!--end card-->
    </div>
    <!--end col-->

    <div class="col-lg-6">
      <div class="card">
        <div class="card-body">
          <h4 class="header-title mt-0 mb-3">Lượt chạy tồn theo bộ phận</h4>
          <div class="">
            <Chart type="bar" :data="chartData" :options="chartOptions" />
          </div>
          <!--end /div-->
        </div>
        <!--end card-body-->
      </div>
      <!--end card-->
    </div>

    <div class="col-lg-6">
      <div class="card">
        <div class="card-body">
          <h4 class="header-title mt-0 mb-3">Lượt chạy tồn theo người</h4>
          <div class="">
            <table class="table mb-0" id="table_user">
              <thead class="thead-light">
                <tr>
                  <th class="border-top-0">Họ và tên</th>
                  <th class="border-top-0">Email</th>
                  <th class="border-top-0">Số lượt</th>
                </tr>
                <!--end tr-->
              </thead>
              <tbody>
                <tr v-for="tr of datatableUser">
                  <td>
                    <RouterLink :to="'/execution/wait?user_id=' + tr.user.id">
                      {{ tr.user.fullName }}
                    </RouterLink>
                  </td>
                  <td v-html="tr.user.email"></td>
                  <td v-html="tr.count"></td>
                </tr>
              </tbody>
            </table>
            <!--end table-->
          </div>
          <!--end /div-->
        </div>
        <!--end card-body-->
      </div>
      <!--end card-->
    </div>
    <div class="col-lg-6">
      <div class="card">
        <div class="card-body">
          <h4 class="header-title mt-0 mb-3">Lượt chạy theo quy trình</h4>
          <div class="">
            <table class="table mb-0" id="table_process">
              <thead class="thead-light">
                <tr>
                  <th class="border-top-0">Tên Quy trình</th>
                  <th class="border-top-0">Version</th>
                  <th class="border-top-0">Số lượt</th>
                  <th class="border-top-0">Export</th>
                </tr>
                <!--end tr-->
              </thead>
              <tbody>
                <tr v-for="tr of datatableProcess">
                  <td v-html="tr.name"></td>
                  <td v-html="tr.version"></td>
                  <td v-html="tr.count"></td>
                  <td>
                    <a href="#" @click="exportVersion(tr.id)"><i class="fas fa-download"></i></a>
                  </td>
                </tr>
              </tbody>
            </table>
            <!--end table-->
          </div>
          <!--end /div-->
        </div>
        <!--end card-body-->
      </div>
      <!--end card-->
    </div>
    <div class="col-lg-6">
      <div class="card">
        <div class="card-body">
          <h4 class="header-title mt-0 mb-3">Lượt chạy</h4>
          <div class="">
            <table class="table mb-0" id="table_execution">
              <thead class="thead-light">
                <tr>
                  <th class="border-top-0">ID</th>
                  <th class="border-top-0">Tiêu đề</th>
                  <th class="border-top-0">Người tạo</th>
                  <th class="border-top-0">Trạng thái</th>
                </tr>
                <!--end tr-->
              </thead>
              <tbody>
                <tr v-for="tr of datatableExecution">
                  <td><a :href="'/execution/details/' + tr.process_version_id + '?execution_id=' + tr.id">{{ tr.id }}</a>
                  </td>
                  <td v-html="tr.title"></td>
                  <td v-html="tr.user.fullName"></td>
                  <td>
                    <Button :label="tr.status" :class="'p-button-sm status status_' + tr.status_id"></Button>
                  </td>
                </tr>
              </tbody>
            </table>
            <!--end table-->
          </div>
          <!--end /div-->
        </div>
        <!--end card-body-->
      </div>
    </div>
  </div>
  <Loading :waiting="waiting"></Loading>
</template>
<script setup>
import { onMounted, ref } from "vue";
import Chart from "primevue/chart";
import Button from "primevue/button";
import Api from "../../../api/Api"
import processApi from "../../../api/processApi";
import Loading from "../../../components/Loading.vue";
import { useRoute } from "vue-router";
const execution_success = ref(0);
const execution_fail = ref(0);
const execution_amount = ref(0);
const execution_wait = ref(0);
const waiting = ref(false);
const chartOptions = ref({
  responsive: true,
  plugins: {
    legend: {
      display: false,
    },
  },
});
const chartData = ref({
  labels: [],
  datasets: [],
});
const datatableUser = ref([]);
const datatableProcess = ref([]);
const datatableExecution = ref([]);
const route = useRoute();
const exportVersion = (id) => {
  waiting.value = true;
  processApi.exportVersion(id).then((res) => {
    console.log(res);
    waiting.value = false;
    if (res.success) {
      window.open(res.link, "_blank");
    } else {
      alert(res.message);
    }
  });
};

onMounted(() => {
  var process_id = route.params.id;
  Api.HomeBadge(process_id).then((res) => {
    execution_amount.value = res.execution_amount;
    execution_wait.value = res.execution_wait;
    execution_success.value = res.execution_success;
    execution_fail.value = res.execution_fail;
  });
  Api.datachartDepartment(process_id).then((res) => {
    chartData.value.labels = res.labels;
    chartData.value.datasets = res.datasets;
  });
  Api.tableUser(process_id).then((res) => {
    datatableUser.value = res.data;
  });
  Api.tableProcess(process_id).then((res) => {
    datatableProcess.value = res.data;
  });
  Api.tableExecution(process_id).then((res) => {
    datatableExecution.value = res.data;
  });
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
</style>