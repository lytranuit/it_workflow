<template>
	<section class="login-block">
		<div class="container">
			<div class="row align-items-start" style="height: 100vh;">
				<div class="col-12 align-self-center">
					<div class="auth-page">
						<div class="card auth-card shadow-lg">
							<div class="card-body">
								<div class="px-3">
									<div class="auth-logo-box" style="top:-70px;">
										<a href="#" class=""><img src="../assets/images/clientlogo_astahealthcare.com_f1800.png"
												alt="logo" class=""></a>
									</div><!--end auth-logo-box-->
									<img src="../assets/images/500.jpg" alt="" class="d-block mx-auto mt-4"
										height="250">
									<div class="text-center auth-logo-text mb-4">
										<h4 class="mt-0 mb-3 mt-5">Không có quyền truy cập</h4>
										<form id="logoutForm" class="text-center" action="/V1/Auth/Logout"
											method="post">
											<a class="btn btn-sm btn-gradient-primary text-white"
												style="cursor:pointer;" @click="store.logout()">Đăng xuất</a>
										</form>
									</div> <!--end auth-logo-text-->
								</div><!--end /div-->
							</div><!--end card-body-->
						</div><!--end card-->
					</div><!--end auth-page-->
				</div><!--end col-->
			</div>
		</div>
	</section>
</template>
<style>
.login-block {
	background-image: url(../assets/images/bg.jpg);
	background-size: cover;
	background-position: center center;
	width: 100%;
	height: 100vh;
}
</style>
<script setup>
import { useAuth } from '../stores/auth'
const store = useAuth();
</script>