<template>
  <div class="row clearfix">
    <div class="col-12">
      <form method="POST" id="form">
        <AlertError :message="messageError" v-if="messageError" />
        <AlertSuccess :message="messageSuccess" v-if="messageSuccess" />
        <section class="card card-fluid">
          <div class="d-inline-block w-100">
            <div class="card-header">
              <a
                href="#"
                class="btn btn-sm btn-success"
                data-toggle="modal"
                data-animation="bounce"
                data-target=".modal-pass"
                >Đổi mật khẩu</a
              >

              <button
                type="submit"
                class="btn btn-sm btn-primary float-right"
                @click.prevent="submit()"
              >
                Save
              </button>
            </div>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col-md-8">
                <input type="hidden" name="id" v-model="data.id" required />
                <div class="form-group row">
                  <b class="col-12 col-lg-2 col-form-label"
                    >Email:<i class="text-danger">*</i></b
                  >
                  <div class="col-12 col-lg-4 pt-1">
                    <input
                      class="form-control form-control-sm"
                      type="text"
                      name="email"
                      placeholder="Email"
                      v-model="data.email"
                      required
                    />
                  </div>
                </div>
                <div class="form-group row">
                  <b class="col-12 col-lg-2 col-form-label"
                    >Họ và tên:<i class="text-danger">*</i></b
                  >
                  <div class="col-12 col-lg-4 pt-1">
                    <input
                      class="form-control form-control-sm"
                      type="text"
                      name="fullName"
                      required=""
                      placeholder="FullName"
                      v-model="data.fullName"
                    />
                  </div>
                  <b class="col-12 col-lg-2 col-form-label">Nhóm:</b>
                  <div class="col-lg-4 pt-1">
                    <RoleTreeSelect
                      v-model="data.roles"
                      multiple
                    ></RoleTreeSelect>
                    <select
                      name="roles[]"
                      v-model="data.roles"
                      multiple
                      class="d-none"
                    >
                      <option
                        v-for="option in data.roles"
                        :key="option"
                        :value="option"
                      ></option>
                    </select>
                  </div>
                </div>
                <div class="form-group row">
                  <b class="col-12 col-lg-2 col-form-label">Bộ phận:</b>
                  <div class="col-lg-10 pt-1">
                    <DepartmentTreeSelect
                      v-model="data.departments"
                      valueConsistsOf="ALL_WITH_INDETERMINATE"
                      :flat="false"
                      multiple
                    ></DepartmentTreeSelect>
                    <select
                      name="departments[]"
                      v-model="data.departments"
                      multiple
                      class="d-none"
                    >
                      <option
                        v-for="option in data.departments"
                        :key="option"
                        :value="option"
                      ></option>
                    </select>
                  </div>
                </div>
              </div>
              <div class="col-md-4">
                <div class="form-group row">
                  <div class="col-12">
                    <div class="card no-shadow border">
                      <div class="card-header">Hình đại diện</div>
                      <div class="card-body text-center">
                        <input
                          type="hidden"
                          name="image_url"
                          v-model="data.image_url"
                        />
                        <ImageManager @choose="choose" :image="data.image_url">
                        </ImageManager>
                      </div>
                    </div>
                  </div>
                  <div class="col-12">
                    <div class="card no-shadow border">
                      <div class="card-header">Chữ ký</div>
                      <div class="card-body text-center">
                        <input
                          type="hidden"
                          name="image_sign"
                          v-model="data.image_sign"
                        />
                        <ImageManager
                          @choose="chooseSign"
                          :image="data.image_sign"
                        >
                        </ImageManager>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>
      </form>
    </div>

    <div class="modal fade modal-pass" tabindex="-1" role="dialog">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <form method="POST" id="form-pass">
            <div class="modal-header">
              <h5 class="modal-title mt-0" id="myLargeModalLabel">
                Đổi mật khẩu
              </h5>
              <button
                type="button"
                class="close"
                data-dismiss="modal"
                aria-hidden="true"
              >
                ×
              </button>
            </div>
            <div class="modal-body">
              <input type="hidden" name="id" :value="data.id" />
              <div class="row justify-content-md-center">
                <div class="col-md-4">
                  <div class="form-floating mt-2">
                    <input
                      type="password"
                      id="password"
                      class="form-control"
                      name="NewPassword"
                      minlength="6"
                      required=""
                      placeholder="Mật khẩu mới"
                      autocomplete="off"
                    />
                  </div>
                  <div class="form-floating mt-2">
                    <input
                      type="password"
                      class="form-control"
                      name="ConfirmPassword"
                      minlength="6"
                      data-rule-equalTo="#password"
                      required=""
                      placeholder="Nhập lại mật khẩu mới"
                      autocomplete="off"
                    />
                  </div>
                </div>
              </div>
            </div>
            <div class="model-fotter">
              <div class="row justify-content-md-center m-2">
                <div class="col-12 text-center">
                  <button
                    type="submit"
                    class="btn btn-sm btn-primary"
                    style="width: 200px"
                    @click.prevent="change_pass()"
                  >
                    Cập nhật mật khẩu
                  </button>
                </div>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { onMounted, computed } from "vue";
import { useAuth } from "../../../stores/auth";

import AlertError from "../../../components/AlertError.vue";
import AlertSuccess from "../../../components/AlertSuccess.vue";
import { storeToRefs } from "pinia";
import { useRoute } from "vue-router";
import userApi from "../../../api/userApi";
import ImageManager from "../../../components/ImageManager.vue";
import RoleTreeSelect from "../../../components/TreeSelect/RoleTreeSelect.vue";
import DepartmentTreeSelect from "../../../components/TreeSelect/DepartmentTreeSelect.vue";
const route = useRoute();
const messageError = ref();
const messageSuccess = ref();
const store = useAuth();
const { roles, data } = storeToRefs(store);
// const dataRoles = ref([]);
onMounted(() => {
  store.fetchRoles();
  // store.fetchDepartment();
  store.fetchData(route.params.id);
});

const choose = (path) => {
  data.value.image_url = "/private/upload" + path;
};
const chooseSign = (path) => {
  data.value.image_sign = "/private/upload" + path;
};
const submit = () => {
  var vaild = $("#form").valid();
  if (!vaild) {
    return;
  }
  let formData = $("#form").serialize();
  userApi.edit(formData).then((response) => {
    messageError.value = "";
    messageSuccess.value = "";
    if (response.success) {
      messageSuccess.value = response.message;
    } else {
      messageError.value = response.message;
    }
    // console.log(response)
  });
};

const change_pass = () => {
  var vaild = $("#form-pass").valid();
  if (!vaild) {
    return;
  }
  let formData = $("#form-pass").serialize();
  userApi.changepassword(formData).then((response) => {
    messageError.value = "";
    messageSuccess.value = "";
    if (response.success) {
      messageSuccess.value = response.message;
    } else {
      messageError.value = response.message;
    }
    $(".modal-pass").modal("hide");
    // console.log(response)
  });
};
</script>
