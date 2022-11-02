<template>
	<div class="sidebar-right">
		<div class="header flex-m">
			<span class="tilte">{{model.label}}</span>

			<div class="ml-auto" v-if="model.blocking">
				<button class="mr-2" :class="{'btn-reverse':item.reverse,'btn-next':!item.reverse}" tabindex="1" type="button" name="button" v-for="item in model.outEdges" :key="item.id" @click="execute_transition(model.id,item.id)">
					{{item.label}}
				</button>
			</div>
		</div>
		<div class="body">
			<div class="item-control flex-m" v-for="(element,index) in model.fields" :key="element.id">
				<div class="mb-3 ml-3 flex-m w-100">
					<div class="form-input-data-preview mr-3 w-100">
						<div class="form-input-control flex-m">
							<div class="mb-2 custom-title-inline pr-2" style="width:200px;">
								<div class="container-label">
									<div class="container-left">
										<div id="idControlName" class="font-weight-bold font-14 pr-2 d-inline-block">
											<div style="">
												{{element.name}}
												<span class="text-danger" style="float: inherit;" v-if="element.is_require"> * </span>
											</div>
										</div>
										<div class="btn-selectTion-guide icon icon-infor-blue cursor-pointer icon-control" style="color:#0c9cdd;float:right;" v-show="element.guide && element.guide != ''">
											<el-tooltip :content="element.guide" placement="bottom">
												<i class="fas fa-info-circle" style="cursor:pointer;"></i>
											</el-tooltip>
										</div>
									</div>
								</div>
							</div>
							<div class="form-input-control-left w-100" v-if="element.type != 'table'">
								<div v-if="element.type == 'number'">
									<input class="form-control form-control-sm number" type='number' v-model="element.value" />
								</div>
								<div v-if="element.type == 'text'">
									<input class="form-control form-control-sm text" type='text' v-model="element.value" />
								</div>
								<div v-if="element.type == 'email'">
									<input class="form-control form-control-sm email" type='email' v-model="element.value" />
								</div>
								<div v-if="element.type == 'file'">
									<input class="form-control form-control-sm file" type='file' />
								</div>
								<div v-if="element.type == 'date'">
									<datetime type="datetime" format="yyyy-MM-dd" :flow="['date']" input-class="form-control form-control-sm"></datetime>
								</div>
								<div v-if="element.type == 'date_month'">
									<datetime type="datetime" format="yyyy-MM" :flow="['year','month']" input-class="form-control form-control-sm"></datetime>
								</div>

								<div v-if="element.type == 'date_time'">
									<datetime type="datetime" format="yyyy-MM-dd HH:mm:ss" input-class="form-control form-control-sm"></datetime>
								</div>

								<div v-if="element.type == 'select'">
									<treeselect :options="get_options(element.data_setting.options)" :value="element.has_default ? element.data_setting.default_value : ''"> </treeselect>
								</div>

								<div v-if="element.type == 'select_multiple'">
									<treeselect :options="get_options(element.data_setting.options)" :value="element.has_default ? element.data_setting.default_value_array : []" multiple></treeselect>
								</div>
								<div v-if="element.type == 'textarea'">
									<textarea class="form-control form-control-sm textarea" :value="element.has_default ? element.data_setting.default_value : ''"></textarea>
								</div>

								<div v-if="element.type == 'employee'">
									<treeselect :options="users" :value="element.has_default ? element.data_setting.default_value : ''"></treeselect>
								</div>

								<div v-if="element.type == 'employee_multiple'">
									<treeselect :options="users" :value="element.has_default ? element.data_setting.default_value_array : []" multiple></treeselect>
								</div>

								<div v-if="element.type == 'department'">
									<treeselect :options="departments" :value="element.has_default ? element.data_setting.default_value : ''"></treeselect>
								</div>

								<div v-if="element.type == 'department_multiple'">
									<treeselect :options="departments" :value="element.has_default ? element.data_setting.default_value_array : []" multiple></treeselect>
								</div>

								<div v-if="element.type == 'task'">
									<div class="checkbox checkbox-success checkbox-circle" v-for="element in element.data_setting.options" :key="element.id">
										<input :id="'task-' + element.id" type="checkbox">
										<label :for="'task-' + element.id">
											{{element.name}}
										</label>
									</div>
								</div>
							</div>
						</div>
						<div v-if="element.type == 'table'">

							<table class="table table-bordered mb-0 table-centered">
								<thead class="thead-light">
									<tr>
										<th class="border-top-0" v-for="(column,index1) in element.data_setting.columns" :key="column.id">
											{{column.name}}
											<span class="text-danger" v-if="column.is_require">*</span>
										</th>
									</tr>
								</thead>
								<tr>
									<td v-for="(column,index1) in element.data_setting.columns" :key="column.id">
										<div v-if="column.type == 'stt'">
											1
										</div>
										<div v-if="column.type == 'number'">
											<input class="form-control form-control-sm number" type='number' />
										</div>
										<div v-if="column.type == 'text'">
											<input class="form-control form-control-sm text" type='text' />
										</div>
										<div v-if="column.type == 'email'">
											<input class="form-control form-control-sm email" type='email' />
										</div>
										<div v-if="column.type == 'date'">
											<input class="form-control form-control-sm date" type='text' />
										</div>
										<div v-if="column.type == 'date_month'">
											<input class="form-control form-control-sm date_month" type='text' />
										</div>

										<div v-if="column.type == 'date_time'">
											<input class="form-control form-control-sm date_time" type='text' />
										</div>
										<div v-if="column.type == 'textarea'">
											<textarea class="form-control form-control-sm textarea"></textarea>
										</div>
									</td>
								</tr>
								<tbody>
								</tbody>
							</table>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>
<script>

	export default {
		inject: ['i18n'],
		components: {
		},
		props: {
			model: {
				type: Object,
				default: () => ({}),
			},
			users: {
				type: Array,
				default: () => ([]),
			},
			nodes: {
				type: Array,
				default: () => ([]),
			},
			departments: {
				type: Array,
				default: () => ([]),
			},
		},
		methods: {
			execute_transition(from_activity_id, edge_id) {
				var that = this;
				that.$emit('execute_transition', from_activity_id, edge_id);
			}
		}
	}
</script>
<style lang="scss" scoped>
	.sidebar-right {
		position: fixed;
		top: 0;
		right: 0;
		width: 500px;
		border: 1px solid #d5d5d5;
		height: 100vh;
		background: #f0f2f5;
		opacity: 1;
		z-index: 1999;

		.header {
			padding: 20px 10px;
			border-bottom: 1px solid #d5d5d5;

			.tilte {
				font-weight: bold;
				font-size: 16px;
			}

			.btn-reverse {
				border-radius: 4px;
				padding: 6px 16px;
				background-color: rgb(255, 255, 255);
				color: rgb(239, 41, 47);
				border: 1px solid rgb(239, 41, 47);
				cursor: pointer;
				font-weight: bold;
			}

			.btn-next {
				border-radius: 4px;
				padding: 6px 16px;
				background-color: #0c9cdd;
				color: white;
				border: 1px solid #0c9cdd;
				cursor: pointer;
				font-weight: bold;
			}
		}

		.body {
			padding: 20px 10px;
		}
	}
</style>
