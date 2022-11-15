<template>
	<form id="sidebar-right">
		<div class="header flex-m">
			<div>
				<div class="tilte">{{model.label}}</div>
				<div class="flex-m" v-if="html != ''">
					<div class=" flex-m"> Người thực hiện: <span v-html="html" class="ml-1"></span></div>
				</div>
			</div>
			<div class="ml-auto" v-if="model.blocking">
				<button class="mr-2" :class="{'btn-reverse':item.reverse,'btn-next':!item.reverse}" tabindex="1" type="button" name="button" v-for="item in model.outEdges" :key="item.id" @click="execute_transition(model.id,item.id)">
					{{item.label}}
				</button>
			</div>
		</div>
		<div class="body">
			<FormTask :departments="departments" :users="users" :fields="fields" :readonly="readonly" v-if="model.clazz == 'formTask'"></FormTask>
			<ApproveTask :departments="departments" :users="users" :nodes="nodes" v-if="model.clazz == 'approveTask'" :model="model"></ApproveTask>
		</div>
	</form>
</template>
<script>
	import store from '../../../example/store';
	import FormTask from './FormTask';
	import ApproveTask from './ApproveTask';
	export default {
		inject: ['i18n'],
		components: {
			FormTask,
			ApproveTask
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
		data() {
			return {
				readonly: false,
				html: ""
			}
		},
		watch: {
			model: {
				handler(newData, oldData) {
					if (oldData !== newData) {
						this.initHtml();
						if (this.model.clazz == "formTask")
							this.readonly = !this.model.blocking;
					}
				},
				immediate: true,
				deep: true
			}
		},
		computed: {
			fields() {
				return this.model.fields;
			}
		},
		mounted() {

		},
		methods: {
			execute_transition(from_activity_id, edge_id) {
				var that = this;
				that.$emit('execute_transition', from_activity_id, edge_id);
			},
			initHtml() {
				var html = "";
				var model = this.model;
				var transitions = store.state.data_transition;
				var findTransition = transitions.findIndex(function (item) {
					return item.from_activity_id == model.id;
				});
				var transition = transitions[findTransition];

				var created_at = this.model.created_at;
				var user_created_by = this.model.user_created_by;
				if (user_created_by) {
					html += "<b>" + user_created_by.fullName + "</b>";
				}
				if (transition && transition.label) {
					var reverse = transition.reverse;
					var label = transition.label;
					var Iclass = "ml-1";
					if (reverse)
						Iclass += " text-danger";
					else
						Iclass += " text-success";
					html += "<span class='" + Iclass + "'>đã " + label.toLowerCase() + "</span>";
				}
				if (created_at) {
					html += " lúc <b>" + moment(created_at).format("HH:mm DD/MM/YYYY") + "</b>";
				}
				this.html = html;
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
