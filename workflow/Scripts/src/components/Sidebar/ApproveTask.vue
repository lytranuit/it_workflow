<template>
	<div>
		<el-collapse v-model="activeName">
			<el-collapse-item :title="element.label" :name="element.id" v-for="(element,index) in blocks_approve">
				<div class="bg-white">
					<FormTask :departments="departments" :users="users" :fields="element.fields" readonly></FormTask>
				</div>
			</el-collapse-item>
		</el-collapse>
	</div>
</template>
<script>
	import store from '../../../example/store';
	import FormTask from './FormTask';
	export default {
		inject: ['i18n'],
		components: {
			FormTask
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
				blocks_approve: [],
				activeName: '1',
			}
		},
		computed: {
			data_activity() {
				return store.state.data_activity;
			},
		},
		mounted() {
			var model = this.model;
			var that = this;
			var indexBlock = this.nodes.findIndex(function (item) {
				return item.id == model.block_id;
			});
			var block = this.nodes[indexBlock];
			var blocks_approve_id = block.data_setting.blocks_approve_id || [];
			var blocks_approve = blocks_approve_id.map(function (block_id) {
				let indexActivity = that.data_activity.findIndex(function (i) {
					return i.block_id == block_id;
				});
				let activity = that.data_activity[indexActivity];
				return activity;
			});
			this.blocks_approve = blocks_approve;
			if (blocks_approve.length)
				this.activeName = blocks_approve[0].id;
			//console.log(blocks_approve);
		},
		methods: {
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
