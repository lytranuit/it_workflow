<template>
	<div id="user">
		<div class="row clearfix">
			<div class="col-12">
				<form method="POST" id="form">
					<section class="card card-fluid">
						<div class="card-header">
							<div class="d-inline-block w-100">
								<button type="submit" class="btn btn-sm btn-primary float-right">Save</button>
							</div>
						</div>
						<div class="card-body">
							<div class="row">
								<div class="col-md-8">
									<div class="form-group row">
										<b class="col-12 col-lg-2 col-form-label">Email:<i class="text-danger">*</i></b>
										<div class="col-12 col-lg-4 pt-1">
											<input class="form-control form-control-sm" type="text" name="email" placeholder="Email" v-model="data.email" />
										</div>
									</div>

									<div class="form-group row">
										<b class="col-12 col-lg-2 col-form-label">FullName:<i class="text-danger">*</i></b>
										<div class="col-12 col-lg-4 pt-1">
											<input class="form-control form-control-sm" type="text" name="fullName" required="" placeholder="FullName" v-model="data.fullName" />
										</div>
										<b class="col-12 col-lg-2 col-form-label">Bộ phận:</b>
										<div class="col-lg-4 pt-1">
											<treeselect :options="departments" multiple value-consists-of="ALL_WITH_INDETERMINATE" v-model="data.departments"></treeselect>
											<select name="departments[]" v-model="data.departments" multiple class="d-none">
												<option v-for="option in data.departments" :key="option" :value="option"></option>
											</select>
										</div>
									</div>
									<div class="form-group row">
										<b class="col-12 col-lg-2 col-form-label">Nhóm:</b>
										<div class="col-lg-4 pt-1">
											<treeselect :options="roles" multiple v-model="data.groups"></treeselect>
											<select name="groups[]" v-model="data.groups" multiple class="d-none">
												<option v-for="option in data.groups" :key="option" :value="option"></option>
											</select>
										</div>
									</div>
								</div>
								<div class="col-md-4">
									<div class="form-group row">
										<div class="col-12">
											<div class="card no-shadow border">
												<div class="card-header">
													Image
												</div>
												<div class="card-body text-center">
													<div id="image_url" class="image_ft"></div>
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
						<div class="card-footer">
							<div class="d-inline-block w-100">
								<button type="submit" class="btn btn-sm btn-primary float-right">Save</button>
							</div>
						</div>
					</section>
				</form>
			</div>
		</div>
	</div>
</template>

<script>
	import store from './store'
	export default {
		name: 'user',
		data() {
			return {
				data: data
			}
		},
		computed: {
			roles() {
				return store.state.roles;
			},
			departments() {
				return store.state.departments;
			}
		},
		mounted() {
			var that = this;
			$("#image_url").imageFeature({
				'image': "/private/images/user.webp",
			});
			if (this.data.image_url) {
				$("#image_url").imageFeature("set_image", this.data.image_url);
			}
			store.dispatch("fetchRoles");
			store.dispatch("fetchDepartment");
		},
		methods: {

		},
	}
</script>
