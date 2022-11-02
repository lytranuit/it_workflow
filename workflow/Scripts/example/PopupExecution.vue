<template>
	<div id="popup-execution">
		<div id="myModal2" class="modal" tabindex="-1" role="dialog" data-backdrop="static">
			<div class="modal-dialog modal-lg" role="document">
				<div class="modal-content">
					<div class="modal-body">
						<div class="row">
							<div class="col-12 mb-3">
								<h5 class="modal-title">
									Chọn quy trình
									<button type="button" class="close" data-dismiss="modal" aria-label="Close">
										<span aria-hidden="true">&times;</span>
									</button>
								</h5>
							</div>
							<div class="col-9">
								<div class="input-group">
									<span class="input-group-prepend">
										<button type="button" class="btn btn-gradient-secondary"><i class="fas fa-search"></i></button>
									</span>
									<input type="text" id="example-input1-group2" name="example-input1-group2" class="form-control" placeholder="Tìm kiếm" v-model="search" v-on:keydown="change">
								</div>
							</div>
							<div class="col-3">
								<select class="form-control form-control-sm" v-model="selected" v-on:change="change">
									<option value="-1">Tất cả các nhóm</option>
									<option v-for="option in groups" :key="option.id" :value="option.id">{{option.name}}</option>
								</select>
							</div>
							<div class="col-12 mt-3">
								<div class="accordion" id="accordionselect">
									<div class="process-group" v-for="group in groups" :key="group.id" v-if="group.list_process_version.length > 0 && !group.hidden">
										<div class="h-acc" data-toggle="collapse" :data-target="'#group_' + group.id" aria-expanded="true">
											{{group.name}}
										</div>
										<div :id="'group_'+group.id" class="collapse show" style="">
											<a v-for="process_version in group.list_process_version" :key="process_version.id" class="process d-block" :href="'/admin/execution/create/' + process_version.id" v-if="!process_version.hidden">
												{{process_version.process.name}}
											</a>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
	import store from './store'
	export default {
		name: 'popup-add',
		data: {
			selected: "-1",
			search: "",
		},
		computed: {
			groups() {
				return store.state.groups;
			}
		},
		mounted() {
			var that = this;
			$.ajax({
				url: path + "/admin/api/ProcessGroupWithProcess",
				success: function (data) {
					//console.log(data);
					store.commit("SET_GROUP", data);
					that.selected = "-1";
				}
			})
		},
		methods: {
			change() {
				var val = this.selected;
				var search = this.search ? this.search.toLowerCase() : '';
				this.groups = this.groups.map(function (item) {

					item.list_process_version = item.list_process_version.map(function (item1) {
						if (search == "" || item1.process.name.toLowerCase().indexOf(search) != -1) {
							item1.hidden = false
						} else {
							item1.hidden = true;
						}
						return item1;
					});
					if (val == -1 || val == item.id) {
						item.hidden = false
					} else {
						item.hidden = true;
					}
					if (!item.list_process_version.length)
						item.hidden = true;
					return item;
				})
			}
		},
	}
</script>
