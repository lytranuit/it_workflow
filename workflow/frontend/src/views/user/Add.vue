<template>
  <div class="row clearfix">
    <div class="col-12">
      <form method="POST" id="form">
        <AlertError :message="messageError" v-if="messageError" />
        <section class="card card-fluid">
          <div class="card-header">
            <div class="d-inline-block w-100">
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
                    >Mât khẩu:<i class="text-danger">*</i></b
                  >
                  <div class="col-12 col-lg-4 pt-1">
                    <input
                      type="password"
                      id="password"
                      class="form-control"
                      name="password"
                      minlength="6"
                      required=""
                      autocomplete="off"
                    />
                  </div>
                  <b class="col-12 col-lg-2 col-form-label"
                    >Xác nhận mật khẩu:<i class="text-danger">*</i></b
                  >
                  <div class="col-12 col-lg-4 pt-1">
                    <input
                      type="password"
                      class="form-control"
                      name="confirmpassword"
                      minlength="6"
                      data-rule-equalTo="#password"
                      required=""
                      autocomplete="off"
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
  </div>
</template>

<script setup>
import { ref } from "vue";
import { onMounted, computed } from "vue";
import { useAuth } from "../../stores/auth";

import AlertError from "../../components/AlertError.vue";
import { useRouter } from "vue-router";
import userApi from "../../api/userApi";
import ImageManager from "../../components/ImageManager.vue";
import RoleTreeSelect from "../../components/TreeSelect/RoleTreeSelect.vue";
import DepartmentTreeSelect from "../../components/TreeSelect/DepartmentTreeSelect.vue";
const router = useRouter();
const messageError = ref();
const store = useAuth();
const data = ref({});
onMounted(() => {
  store.fetchRoles();
  data.value.departments = [];
  data.value.roles = [];
  data.value.image_url = "/private/images/user.webp";
  data.value.image_sign = "/private/images/tick.png";
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
  userApi.create(formData).then((response) => {
    messageError.value = "";
    if (response.success) {
      router.push("/user");
    } else {
      messageError.value = response.message;
    }
    // console.log(response)
  });
};
</script>
