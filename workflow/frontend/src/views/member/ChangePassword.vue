<template>
    <form id="change-password-form" method="post" autocomplete="on">
        <AlertSuccess :message="messageSuccess" v-if="messageSuccess" />
        <AlertError :message="messageError" v-if="messageError" />
        <section class="card card-fluid">
            <div class="card-body">
                <div class="row justify-content-md-center">
                    <div class="col-md-4">

                        <div class="input-group mt-2">
                            <input type="password" name="OldPassword" class="form-control" minlength="6" required
                                placeholder="Mật khẩu hiện tại" autocomplete="off" v-model="password" />
                            <a class="input-group-append toggle_pass" href="#" @click="toggle_pass($event)">
                                <span class="input-group-text "><i class="far fa-eye"></i></span>
                            </a>
                        </div>
                        <div class="input-group mt-2">
                            <input type="password" id="password" class="form-control" name="NewPassword" minlength="6"
                                required placeholder="Mật khẩu mới" autocomplete="off" v-model="new_password" />
                            <a class="input-group-append toggle_pass" href="#" @click="toggle_pass($event)">
                                <span class="input-group-text"><i class="far fa-eye"></i></span>
                            </a>
                        </div>
                        <div class="input-group mt-2">
                            <input type="password" class="form-control" name="ConfirmPassword" minlength="6"
                                data-rule-equalTo="#password" required placeholder="Nhập lại mật khẩu mới"
                                autocomplete="off" v-model="confirm">
                            <a class="input-group-append toggle_pass" href="#" @click="toggle_pass($event)">
                                <span class="input-group-text"><i class="far fa-eye"></i></span>
                            </a>
                        </div>

                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="d-inline-block w-100 text-center">
                    <button type="submit" class="btn btn-sm btn-primary" style="width:200px" @click.prevent="submit()">Cập
                        nhật mật khẩu</button>
                </div>
            </div>
        </section>
    </form>
</template>
<script setup>
import { onMounted } from 'vue';
import { ref } from '@vue/reactivity';
import AlertSuccess from '../../components/AlertSuccess.vue';
import AlertError from '../../components/AlertError.vue';
import userApi from '../../api/userApi';
const messageSuccess = ref();
const messageError = ref();
const confirm = ref('');
const password = ref('');
const new_password = ref('');
onMounted(() => {
    $("#change-password-form").validate({
        errorPlacement: function (error, element) {
            error.appendTo(element.parents(".input-group"));
        }
    });
})
const toggle_pass = (e) => {
    var parent = $(e.target).closest(".input-group");
    var input = $("input", parent);
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
}

const submit = () => {
    var vaild = $("#change-password-form").valid();
    if (!vaild) {
        return;
    }
    let formData = new FormData()
    formData.append('oldpassword', password.value)
    formData.append('newpassword', new_password.value)
    formData.append('confirm', confirm.value)
    userApi.changepassword(formData).then((response) => {
        messageSuccess.value = '';
        messageError.value = '';
        if (response.success) {
            messageSuccess.value = response.message;
        } else {
            messageError.value = response.message;
        }
    })
}
</script>