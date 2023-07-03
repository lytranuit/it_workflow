
<template>
  <div class="row justify-content-center">
    <div class="col-md-6 col-lg-3">
      <div class="card report-card bg-purple-gradient shadow-purple">
        <div class="card-body">
          <div class="float-right">
            <i class="dripicons-wallet report-main-icon bg-icon-purple"></i>
          </div>
          <span class="badge badge-light text-purple">Tổng số lượt chạy</span>
          <h3 class="my-3">0</h3>
        </div><!--end card-body-->
      </div><!--end card-->
    </div> <!--end col-->
    <div class="col-md-6 col-lg-3">
      <div class="card report-card bg-warning-gradient shadow-warning">
        <div class="card-body">
          <div class="float-right">
            <i class="fas fa-spinner report-main-icon bg-icon-warning"></i>
          </div>
          <span class="badge badge-light text-warning">Đang thực hiện</span>
          <h3 class="my-3">0</h3>
        </div><!--end card-body-->
      </div><!--end card-->
    </div> <!--end col-->
    <div class="col-md-6 col-lg-3">
      <div class="card report-card bg-success-gradient shadow-success">
        <div class="card-body">
          <div class="float-right">
            <i class="dripicons-checkmark report-main-icon bg-icon-success"></i>
          </div>
          <span class="badge badge-light text-success">Đã hoàn thành</span>
          <h3 class="my-3">0</h3>
        </div><!--end card-body-->
      </div><!--end card-->
    </div> <!--end col-->

    <div class="col-md-6 col-lg-3">
      <div class="card report-card bg-danger-gradient shadow-danger">
        <div class="card-body">
          <div class="float-right">
            <i class="fas fa-ban report-main-icon bg-icon-danger"></i>
          </div>
          <span class="badge badge-light text-danger">Đã thất bại</span>
          <h3 class="my-3">0</h3>
        </div><!--end card-body-->
      </div><!--end card-->
    </div> <!--end col-->

    <div class="col-lg-6">
      <div class="card">
        <div class="card-body">
          <h4 class="header-title mt-0 mb-3">Lượt chạy tồn theo bộ phận</h4>
          <div class="">
            <canvas id="bar" class="drop-shadow" height="400"></canvas>
          </div><!--end /div-->
        </div><!--end card-body-->
      </div><!--end card-->
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
                </tr><!--end tr-->
              </thead>
              <tbody>
              </tbody>
            </table> <!--end table-->
          </div><!--end /div-->
        </div><!--end card-body-->
      </div><!--end card-->
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
                </tr><!--end tr-->
              </thead>
              <tbody>
              </tbody>
            </table> <!--end table-->
          </div><!--end /div-->
        </div><!--end card-body-->
      </div><!--end card-->
    </div>
  </div>
</template>
