<template>
	<div class="root">
		<ToolbarPanel ref="toolbar" :mode="mode" />
		<div ref="canvas" class="canvasPanel" :style="{'height':height+'px','width':'100%'}"></div>
		<Transition name="slide">
			<sidebar :model="selectedModel" ref="sidebar" v-if="selectedModel != null" @execute_transition="execute_transition"></sidebar>
		</Transition>
	</div>
</template>
<script>
	import G6 from '@antv/g6/lib';
	import { getShapeName } from '../util/clazz'
	import Command from '../plugins/command'
	import Toolbar from '../plugins/toolbar'
	import CanvasPanel from '../plugins/canvasPanel'
	import ToolbarPanel from '../components/ToolbarPanel'
	import Sidebar from '../components/Sidebar'
	import i18n from '../locales'
	import registerShape from '../shape'
	import registerBehavior from '../behavior'

	import store from '../../example/store';

	registerShape(G6);
	registerBehavior(G6);
	export default {
		name: "wfd-vue-execution",
		components: {
			ToolbarPanel,
			Sidebar
		},
		provide() {
			return {
				i18n: i18n[this.lang]
			}
		},
		props: {
			mode: {
				type: String,
				default: "edit"
			},
			height: {
				type: Number,
				default: 800,
			},
			lang: {
				type: String,
				default: "en"
			},
			data: {
				type: Object,
				default: () => ({ nodes: [], edges: [], model: {} })
			},
			data_transition: {
				type: Array,
				default: () => ([])
			},
			data_activity: {
				type: Array,
				default: () => ([])
			},
			users: {
				type: Array,
				default: () => ([])
			},

			departments: {
				type: Array,
				default: () => ([])
			},

			groups: {
				type: Array,
				default: () => ([])
			},
		},
		data() {
			return {
				resizeFunc: () => { },
				selectedModel: null,
				graph: null,
				cmdPlugin: null,
			};
		},
		watch: {
			data: {
				handler(newData, oldData) {
					if (oldData !== newData) {
						var data2 = this.initShape();
						this.graph.read(data2);
					}
				},
				deep: true
			}
		},
		methods: {
			initShape() {
				var that = this;
				var data = this.data;
				var nodes = data.nodes || [];
				var edges = data.edges || [];
				///reset
				nodes = nodes.map(function (item) {
					item.active = false;
					return item;
				});
				edges = edges.map(function (item) {
					item.active = false;
					return item;
				});
				var data_activity = this.data_activity;
				var data_transition = this.data_transition;
				//console.log(data_transition)
				for (var transition of data_transition) {
					var edge_id = transition.link_id;
					var indexEdge = this.findIndexEdge(edge_id);
					var edge = data.edges[indexEdge];
					edge.active = true;
					if (!transition.to_activity_id) {
						var target = that.graph.find("node", (node) => {
							return node.get("model").id == transition.to_block_id;
						});
						var indexNode = this.findIndexNode(transition.to_block_id);
						var node = data.nodes[indexNode];
						node.active = true;

						var outEdges = target.getOutEdges().map(function (i) {
							return i.get("model");
						});
						var activity = {
							execution_id: null,
							label: target.get("model").label,
							block_id: target.get("model").id,
							stt: data_activity.length + 1,
							performer_id: null,////
							clazz: target.get("model").clazz,
							is_new: true,
							executed: false,
							failed: false,
							blocking: true,
							outEdges: outEdges,
							fields: node.fields,
							in_transition_id: transition.id,
							id: rand()
						}
						data_activity.push(activity);

					}
				}
				if (data && data.nodes) {
					//var edges = $.extendext(true, 'replace', [], data.edges);
					return {
						nodes: data.nodes.map(node => {
							return {
								shape: getShapeName(node.clazz),
								...node,
							}
						}),
						edges: data.edges
					}
				}
				return data;
			},
			initEvents() {
				var that = this;
				this.graph.on('node:click', (e) => {
					var item = e.item;
					var id = item.get("model").id;
					var findBlocking = that.data_activity.findIndex(function (item) {
						return item.block_id == id;
					});
					if (findBlocking != -1) {
						var model = that.data_activity[findBlocking];
						that.selectedModel = model;
						return;
					}
					that.selectedModel = null;
				});
				this.graph.on('canvas:click', (e) => {
					that.selectedModel = null;
				});
				//const page = this.$refs['canvas'];
				//const graph = this.graph;
				//const height = this.height - 1;
				//this.resizeFunc = () => {
				//	graph.changeSize(page.offsetWidth, height);
				//};
				//window.addEventListener("resize", this.resizeFunc);
			},
			findIndexNode(id) {
				let index = this.data.nodes.findIndex(function (item) {
					return item.id == id;
				});
				return index;
			},
			findIndexEdge(id) {
				let index = this.data.edges.findIndex(function (item) {
					return item.id == id;
				});
				return index;
			},
			findItembyId(id) {
				var that = this;
				let index = that.data.nodes.findIndex(function (item) {
					return item.id == id;
				});

				let item;
				if (index == -1) {
					index = that.data.edges.findIndex(function (item) {
						return item.id == id;
					});
					item = that.data.edges[index];
				} else {
					item = that.data.nodes[index];
				}
				return item;
			},
			init() {
				if (this.graph) {
					$(this.$refs['canvas']).empty();
				}
				let plugins = [];
				this.cmdPlugin = new Command();
				const canvasPanel = new CanvasPanel({ container: this.$refs['canvas'] });
				const toolbar = new Toolbar({ container: this.$refs['toolbar'].$el });
				plugins = [this.cmdPlugin, toolbar, canvasPanel];
				const width = this.$refs['canvas'].offsetWidth;
				this.graph = new G6.Graph({
					plugins: plugins,
					container: this.$refs['canvas'],
					height: this.height,
					width: width,
					modes: {
						default: [
							'drag-canvas', 'clickSelected',
							{
								type: 'activate-relations',
								resetSelected: true,
							}
						],
						view: [],
						edit: ['drag-canvas', 'hoverNodeActived', 'hoverAnchorActived', 'dragNode', 'dragEdge',
							'dragPanelItemAddNode', 'clickSelected', 'deleteItem', 'itemAlign', 'dragPoint', 'brush-select'],

					},
					animate: true,
					animateCfg: {
						duration: 500, // Number, the duration of one animation
						easing: 'linearEasing', // String, the easing function
					},
					defaultEdge: {
						shape: 'flow-polyline-round',
					},
				});
				this.graph.setMode(this.mode);
				this.graph.data(this.initShape());
				this.graph.render();
				this.graph.fitView()
				this.initEvents();
			},
			onItemCfgChange(key, value) {
				const items = this.graph.get('selectedItems');
				if (typeof value == 'object') {
					value = $(value.target).val();
				}

				if (items && items.length > 0) {
					let item = this.graph.findById(items[0]);
					if (!item) {
						item = this.getNodeInSubProcess(items[0])
					}
					if (this.graph.executeCommand) {
						this.graph.executeCommand('update', {
							itemId: items[0],
							updateModel: { [key]: value }
						});
					} else {
						this.graph.updateItem(item, { [key]: value });
					}
					//this.selectedModel = { ...item.getModel()
				}
			},
			getNodeInSubProcess(itemId) {
				const subProcess = this.graph.find('node', (node) => {
					if (node.get('model')) {
						const clazz = node.get('model').clazz;
						if (clazz === 'subProcess') {
							const containerGroup = node.getContainer();
							const subGroup = containerGroup.subGroup;
							const item = subGroup.findById(itemId);
							return subGroup.contain(item);
						} else {
							return false;
						}
					} else {
						return false;
					}
				});
				if (subProcess) {
					const group = subProcess.getContainer();
					return group.getItem(subProcess, itemId);
				}
				return null;
			},
			execute_transition(from_activity_id, edge_id) {
				var that = this;
				var data_activity = this.data_activity;
				var data_transition = this.data_transition;
				//var nodes = state.data.nodes;
				var findIndexActivity = data_activity.findIndex(function (item) {
					return item.id == from_activity_id
				});
				var activity = data_activity[findIndexActivity];
				var in_transition_id = activity.in_transition_id;
				var findIndexTransition = data_transition.findIndex(function (item) {
					return item.id == in_transition_id
				});
				var transition = data_transition[findIndexTransition];
				var edge = this.graph.find('edge', (node) => {
					return node.get('model').id === edge_id;
				});
				//////UPDATE Current
				activity.blocking = false;
				activity.executed = true;
				activity.failed = true;
				if (!edge.reverse) {
					activity.failed = false;
				}
				transition.to_activity_id = activity.id;
				//
				var source = edge.getSource();
				var target = edge.getTarget();

				/////ADD TRANSITION
				var transition_new = {
					is_new: true,
					label: edge.get("model").label,
					reverse: false,
					link_id: edge.get("model").id,
					execution_id: null,
					from_block_id: source.get("model").id,
					to_block_id: target.get("model").id,
					from_activity_id: activity.id,
					//to_activity_id: activity_to.id,
					stt: data_transition.length + 1,
					id: rand(),
				}
				data_transition.push(transition_new);

				store.commit('SET_DATA_ACTIVITY', data_activity);
				store.commit('SET_DATA_TRANSITION', data_transition);

				setTimeout(function () {
					var data2 = that.initShape();
					that.graph.read(data2);
					that.graph.fitView();
					that.selectedModel = null;
					that.save_data();
				}, 100)

			},
			save_data() {
				this.$emit("save_data");
			}
		},
		destroyed() {
			window.removeEventListener("resize", this.resizeFunc);
			this.graph.getNodes().forEach(node => {
				node.getKeyShape().stopAnimate();
			});
		},
		mounted() {
			this.init()
		}
	};
</script>
<style lang="scss" scoped>
	.root {
		width: 100%;
		height: 100%;
		background-color: #fff;
		display: block;
	}

	.canvasPanel {
		flex: 0 0 auto;
		float: left;
		background-color: #fff;
		border-bottom: 1px solid #E9E9E9;
	}

	/*
	  Enter and leave animations can use different
	  durations and timing functions.
	*/
	.slide-enter-active,
	.slide-leave-active {
		transition: all 0.3s ease-in-out;
	}

	.slide-enter-from {
		opacity: 1;
		transform: translateX(-30px);
	}

	.slide-leave-to {
		opacity: 0;
		transform: translateX(30px);
	}
</style>
