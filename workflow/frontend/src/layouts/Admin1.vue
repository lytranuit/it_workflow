
<script setup>

import { ref } from '@vue/reactivity';
import LeftSide from '../components/LeftSide.vue'
import { useAuth } from '../stores/auth'
const is_hide = ref(false);
const store = useAuth();
const response = await store.getUser();
if (response.success == false) {
	store.logout();
}
const user = store.user;
// console.log(store.user);
// console.log(user);
</script>

<template>
	<div :class="{ 'enlarge-menu': is_hide == true }">
		<!-- Top Bar Start -->
		<div class="topbar">

			<!-- LOGO -->
			<div class="topbar-left">
				<router-link to="/" class="logo">
					<span>
						<img src="../assets/images/favicon.png" alt="logo-small" class="logo-sm">
					</span>
					<span>
						<img src="../assets/images/clientlogo_astahealthcare.com_f1800.png" alt="logo-large" class="logo-lg logo-light">
					</span>
				</router-link>
			</div>
			<!--end logo-->
			<!-- Navbar -->
			<nav class="navbar-custom">
				<ul class="list-unstyled topbar-nav float-right mb-0">
					<li class="dropdown">
						<a class="nav-link dropdown-toggle waves-effect waves-light nav-user" data-toggle="dropdown"
							href="#" role="button" aria-haspopup="false" aria-expanded="false">
							<img :src="user.image_url" alt="profile-user" class="rounded-circle" />
							<span class="ml-1 nav-user-name hidden-sm">{{ user.fullName }} <i
									class="mdi mdi-chevron-down"></i> </span>
						</a>
						<div class="dropdown-menu dropdown-menu-right">
							<router-link to="/member" class="dropdown-item">
								<i class="dripicons-user text-muted mr-2"></i>Thông tin tài khoản
							</router-link>
							<router-link to="/member/changepassword" class="dropdown-item" href="">
								<i class="dripicons-anchor text-muted mr-2"></i> Đổi mật khẩu
							</router-link>

							<div class="dropdown-divider"></div>
							<form id="logoutForm" class="form-inline" action="/V1/Auth/Logout" method="post">
								<a class="dropdown-item" style="cursor:pointer;" @click="store.logout()"><i
										class="dripicons-exit text-muted mr-2"></i> Đăng xuất</a>
							</form>
						</div>
					</li>
				</ul><!--end topbar-nav-->

				<ul class="list-unstyled topbar-nav mb-0">
					<li>
						<button class="button-menu-mobile nav-link waves-effect waves-light" @click="is_hide = !is_hide">
							<i class="dripicons-menu nav-icon"></i>
						</button>
					</li>
				</ul>
			</nav>
			<!-- end navbar-->
		</div>
		<!-- Top Bar End -->

		<div class="page-wrapper">
			<!-- Left Sidenav -->
			<LeftSide />
			<!-- end left-sidenav-->
			<!-- Page Content-->
			<div class="page-content">

				<div class="container-fluid">
					<router-view />
				</div><!-- container -->

			</div>
			<!-- end page content -->
		</div>
		<!-- end page-wrapper -->
</div>
</template>