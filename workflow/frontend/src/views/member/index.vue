<template>
    <form method="POST" id="form">

        <AlertError :message="messageError" v-if="messageError" />
        <AlertSuccess :message="messageSuccess" v-if="messageSuccess" />
        <section class="card card-fluid">
            <div class="card-header flex-m">
                <div class="font-16 font-weight-bold">Thông tin tài khoản</div>
                <div class="ml-auto">
                    <Button label="Lưu lại" icon="pi pi-save" class="p-button-primary p-button-sm mr-2"
                        @click.prevent="submit()">
                    </Button>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-8">
                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Tài khoản:<span class="text-danger">*</span></b>
                            <div class="col-12 col-lg-10 pt-1">

                                <input name="userName" placeholder="Username" class="form-control" readonly
                                    :value="data.email" />
                            </div>
                        </div>
                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Email:<span class="text-danger">*</span></b>
                            <div class="col-12 col-lg-10 pt-1">
                                <input name="email" placeholder="Email" class="form-control" readonly :value="data.email" />
                            </div>
                        </div>

                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Họ và tên:<span class="text-danger">*</span></b>
                            <div class="col-12 col-lg-10 pt-1">
                                <input class="form-control form-control-sm" type="text" name="fullName" placeholder=""
                                    readonly :value="data.fullName" required />
                            </div>
                        </div>
                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Mã số nhân viên:<span class="text-danger">*</span></b>
                            <div class="col-12 col-lg-10 pt-1">
                                <input class="form-control form-control-sm" type="text" name="fullName" placeholder=""
                                    v-model="data.msnv" required/>
                            </div>
                        </div>
                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Số điện thoại:<span class="text-danger">*</span></b>
                            <div class="col-12 col-lg-10 pt-1">
                                <input class="form-control form-control-sm" type="text" name="fullName" placeholder=""
                                    v-model="data.phoneNumber" required/>
                            </div>
                        </div>
                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Phòng/Bộ phận:<span class="text-danger">*</span></b>
                            <div class="col-12 col-lg-10 pt-1">
                                <input class="form-control form-control-sm" type="text" name="fullName" placeholder=""
                                    v-model="data.department_text" required/>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="form-group row">
                            <div class="col-12">
                                <div class="card no-shadow border">
                                    <div class="card-header">
                                        Hình đại diện
                                    </div>
                                    <div class="card-body text-center">
                                        <input type="hidden" name="image_url" v-model="data.image_url" />
                                        <ImageManager @choose="choose" :image="data.image_url">
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
</template>
<script setup>
import { ref } from '@vue/reactivity';
import { useAuth } from '../../stores/auth'
import { onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import ImageManager from '../../components/ImageManager.vue';
import Button from 'primevue/button';
import authApi from '../../api/authApi';
import AlertError from '../../components/AlertError.vue';
import AlertSuccess from '../../components/AlertSuccess.vue';
const store = useAuth();
const user_id = store.user.id;
const { data } = storeToRefs(store);
const messageError = ref();
const messageSuccess = ref();
// console.log(store.user);
// console.log(user);
const choose = (path) => {
    data.value.image_url = "/private/upload" + path;
};
const submit = () => {
    var vaild = $("#form").valid();
    if (!vaild) {
        return;
    }
    authApi.edit(data.value).then((response) => {
        messageError.value = "";
        messageSuccess.value = "";
        if (response.success) {
            messageSuccess.value = response.message;
            localStorage.setItem("is_first", 1);
        } else {
            messageError.value = response.message;
        }
        // console.log(response)
    });
};
onMounted(() => {
    store.fetchDataAuth();
})
</script>