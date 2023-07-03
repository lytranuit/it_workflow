<template>
    <form method="POST" id="form">
        <section class="card card-fluid">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-8">
                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Tài khoản:</b>
                            <div class="col-12 col-lg-10 pt-1">

                                <input name="userName" placeholder="Username" class="form-control" readonly
                                    :value="user.email" />
                            </div>
                        </div>
                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Email:</b>
                            <div class="col-12 col-lg-10 pt-1">
                                <input name="email" placeholder="Email" class="form-control" readonly
                                    :value="user.email" />
                            </div>
                        </div>

                        <div class="form-group row">
                            <b class="col-12 col-lg-2 col-form-label">Họ và tên:</b>
                            <div class="col-12 col-lg-10 pt-1">
                                <input class="form-control form-control-sm" type="text" name="fullName"
                                    placeholder="FullName" readonly :value="user.fullName" />
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
                                        <img style="max-height:100px;max-width: 100%;cursor:pointer;"
                                            :src="user.image_url" class="image_feature" />
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
<script setup lang="ts">
import { ref } from '@vue/reactivity';
import { useAuth } from '../../stores/auth'
const store = useAuth();
const user = store.user;
// console.log(store.user);
// console.log(user);
</script>