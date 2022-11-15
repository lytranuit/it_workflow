<template>
	<div>
		<div class="item-control flex-m" v-for="(element,index) in fields" :key="element.id">
			<div class="mb-3 ml-3 flex-m w-100">
				<div class="form-input-data-preview mr-3 w-100">
					<div class="mb-2 form-input-control flex-m">
						<div class="custom-title-inline" style="width:200px;">
							<div class="container-label">
								<div class="container-left">
									<div id="idControlName" class="font-weight-bold font-14 pr-2 d-inline-block">
										<div style="">
											{{element.name}}
											<span class="text-danger" style="float: inherit;" v-if="element.is_require"> * </span>
										</div>
									</div>
									<div class="btn-selectTion-guide icon icon-infor-blue cursor-pointer icon-control" style="color:#0c9cdd;float:right;margin-right:5px;" v-show="element.guide && element.guide != ''">
										<el-tooltip :content="element.guide" placement="bottom">
											<i class="fas fa-info-circle" style="cursor:pointer;"></i>
										</el-tooltip>
									</div>
								</div>
							</div>
						</div>
						<div class="form-input-control-left w-100" v-if="element.type != 'table' && !readonly">
							<div v-if="element.type == 'number'">
								<input class="form-control form-control-sm number" type='number' v-model="element.values.value" :required="element.is_require" :name="element.id" />
							</div>
							<div v-if="element.type == 'text'">
								<input class="form-control form-control-sm text" type='text' v-model="element.values.value" :required="element.is_require" :name="element.id" />
							</div>
							<div v-if="element.type == 'email'">
								<input class="form-control form-control-sm email" type='email' v-model="element.values.value" :required="element.is_require" :name="element.id" />
							</div>
							<div v-if="element.type == 'file'">
								<input class="form-control form-control-sm file" type='file' :required="element.is_require" :name="element.id" />
							</div>
							<div v-if="element.type == 'date'">
								<datetime type="datetime" format="yyyy-MM-dd" :flow="['date']" input-class="form-control form-control-sm" v-model="element.values.value" :required="element.is_require" :name="element.id"></datetime>
							</div>
							<div v-if="element.type == 'date_month'">
								<datetime type="datetime" format="yyyy-MM" :flow="['year','month']" input-class="form-control form-control-sm" v-model="element.values.value" :required="element.is_require" :name="element.id"></datetime>
							</div>

							<div v-if="element.type == 'date_time'">
								<datetime type="datetime" format="yyyy-MM-dd HH:mm:ss" input-class="form-control form-control-sm" v-model="element.values.value" :required="element.is_require" :name="element.id"></datetime>
							</div>

							<div v-if="element.type == 'textarea'">
								<textarea class="form-control form-control-sm textarea" v-model="element.values.value" :required="element.is_require" :name="element.id"></textarea>
							</div>

							<div v-if="element.type == 'select'">
								<treeselect :options="get_options(element.data_setting.options)" v-model="element.values.value" :required="element.is_require" name="tran"></treeselect>
							</div>

							<div v-if="element.type == 'select_multiple'">
								<treeselect :options="get_options(element.data_setting.options)" v-model="element.values.value_array" multiple :required="element.is_require" name="tran2"></treeselect>
							</div>
							<div v-if="element.type == 'employee'">
								<treeselect :options="users" v-model="element.values.value" :required="element.is_require" :name="element.id"></treeselect>
							</div>

							<div v-if="element.type == 'employee_multiple'">
								<treeselect :options="users" v-model="element.values.value_array" multiple :required="element.is_require" :name="element.id"></treeselect>
							</div>

							<div v-if="element.type == 'department'">
								<treeselect :options="departments" v-model="element.values.value" :required="element.is_require" :name="element.id"></treeselect>
							</div>

							<div v-if="element.type == 'department_multiple'">
								<treeselect :options="departments" v-model="element.values.value_array" multiple :required="element.is_require" :name="element.id"></treeselect>
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
						<div class="w-100" v-else>
							{{display(element)}}
						</div>
					</div>
					<div v-if="element.type == 'table'">

						<table class="table table-bordered mb-0 bg-white" style="outline: 1px solid #dee2e6 !important">
							<thead class="">
								<tr>
									<th class="border-top-0" v-for="(column,index1) in element.data_setting.columns" :key="column.id">
										{{column.name}}
										<span class="text-danger" v-if="column.is_require">*</span>
									</th>
									<th v-if="!readonly && element.values.list_data.length > 1"></th>
								</tr>
							</thead>
							<tbody v-if="!readonly">
								<tr v-for="(row,index1) in element.values.list_data" :key="index1">
									<td v-for="(column,index2) in element.data_setting.columns" :key="column.id">
										<div v-if="column.type == 'stt'">
											{{row[column.id]}}
										</div>
										<div v-if="column.type == 'number'">
											<input class="form-control form-control-sm number" type='number' v-model="row[column.id]" :name="column.id + '_' + index1" :required="column.is_require" />
										</div>
										<div v-if="column.type == 'text'">
											<input class="form-control form-control-sm text" type='text' v-model="row[column.id]" :name="column.id + '_' + index1" :required="column.is_require" />
										</div>
										<div v-if="column.type == 'email'">
											<input class="form-control form-control-sm email" type='email' v-model="row[column.id]" :name="column.id + '_' + index1" :required="column.is_require" />
										</div>
										<div v-if="column.type == 'date'">
											<input class="form-control form-control-sm date" type='text' v-model="row[column.id]" :name="column.id + '_' + index1" :required="column.is_require" />
										</div>
										<div v-if="column.type == 'date_month'">
											<input class="form-control form-control-sm date_month" type='text' v-model="row[column.id]" :name="column.id + '_' + index1" :required="column.is_require" />
										</div>

										<div v-if="column.type == 'date_time'">
											<input class="form-control form-control-sm date_time" type='text' v-model="row[column.id]" :name="column.id + '_' + index1" :required="column.is_require" />
										</div>
										<div v-if="column.type == 'textarea'">
											<textarea class="form-control form-control-sm textarea" v-model="row[column.id]" :name="column.id + '_' + index1" :required="column.is_require"></textarea>
										</div>
									</td>
									<td v-if="element.values.list_data.length > 1">
										<div class="ml-2 text-danger" style="cursor:pointer;" @click="remove_row(element,index1)"><i class="fas fa-trash-alt"></i></div>
									</td>
								</tr>
								<tr>
									<td :colspan="element.data_setting.columns.length">
										<a href="#" style="color: #1e9dc3;" @click="add_row_table(element)" ref="addRowTable" :data-count="element.values.list_data.length"><i class="fas fa-plus"></i> Thêm dòng</a>
									</td>
								</tr>
							</tbody>
							<tbody v-else>
								<tr v-for="(row,index1) in element.values.list_data" :key="index1">
									<td v-for="(column,index2) in element.data_setting.columns" :key="column.id">
										<div>{{row[column.id]}}</div>
									</td>
								</tr>
							</tbody>
						</table>
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
			fields: {
				type: Array,
				default: () => ([]),
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
			readonly: {
				type: Boolean,
				default: () => (false),
			},
		},
		mounted() {
			var index = 0;
			$(".vue-treeselect__input").each(function () {
				$(this).attr("name", "index_" + index++);
			});
			if (this.$refs.addRowTable) {
				this.$refs.addRowTable.map(function (item) {
					if ($(item).data("count") == 0) {
						item.click();
					}

				})
			}
		},
		methods: {
			get_options(options) {
				return options.map(function (item) {
					item.label = item.name;
					return item;
				});
			},
			add_row_table(field) {
				var columns = field.data_setting.columns;
				var data = {};
				var column_stt = null;
				for (var column of columns) {
					if (column.type == 'stt') {
						column_stt = column.id;
					} else {
						data[column.id] = null;
					}

				}
				field.values.list_data.push(data);
				if (column_stt) {
					field.values.list_data = field.values.list_data.map(function (item, key) {
						item[column_stt] = key + 1;
						return item;
					});
				}
				var name = field.name
				field.name = rand();
				field.name = name;
			},
			remove_row(element, index) {

				element.values.list_data.splice(index, 1);




				var columns = element.data_setting.columns;
				var column_stt = null;
				for (var column of columns) {
					if (column.type == 'stt') {
						column_stt = column.id;
					}
				}
				if (column_stt) {
					element.values.list_data = element.values.list_data.map(function (item, key) {
						item[column_stt] = key + 1;
						return item;
					});
				}
				var name = element.name
				element.name = rand();
				element.name = name;
			},
			display(field) {
				var text = field.values.value;
				var data_setting = field.data_setting;
				if (field.type == 'select') {
					var options = data_setting.options;
					var index = options.findIndex(function (item) {
						return item.id = field.values.value;
					})
					if (index != -1) {
						var option = options[index];
						text = option.name;
					}
				} else if (field.type == 'department') {
					var index = this.departments.findIndex(function (item) {
						return item.id = field.values.value;
					})
					if (index != -1) {
						var option = this.departments[index];
						text = option.label;
					}
				} else if (field.type == 'employee') {
					var index = this.users.findIndex(function (item) {
						return item.id = field.values.value;
					})
					if (index != -1) {
						var option = this.users[index];
						text = option.label;
					}
				} else if (field.type == 'select_multiple') {
					var value_array = field.values.value_array || [];
					var options = data_setting.options;
					var list = options.filter(function (item) {
						return value_array.indexOf(item.id) != -1;
					}).map(function (item) {
						return item.name;
					});
					text = list.join(", ");
				}
				else if (field.type == 'select_department') {
					var value_array = field.values.value_array || [];
					var list = this.departments.filter(function (item) {
						return value_array.indexOf(item.id) != -1;
					}).map(function (item) {
						return item.label;
					});
					text = list.join(", ");
				}
				else if (field.type == 'select_employee') {
					var value_array = field.values.value_array || [];
					var list = this.users.filter(function (item) {
						return value_array.indexOf(item.id) != -1;
					}).map(function (item) {
						return item.label;
					});
					text = list.join(", ");
				} else if (field.type == 'date') {

					text = field.values.value ? moment(field.values.value).format("YYYY-MM-DD") : "";
				} else if (field.type == 'date_month') {
					text = field.values.value ? moment(field.values.value).format("YYYY-MM") : "";
				} else if (field.type == 'date_time') {
					text = field.values.value ? moment(field.values.value).format("YYYY-MM-DD HH:mm") : "";
				}
				return text
			}
		}
	}
</script>
<style lang="scss" scoped>
	#sidebar-right {
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
			height: calc(100vh - 75px);
			overflow: auto;
		}
	}
</style>
