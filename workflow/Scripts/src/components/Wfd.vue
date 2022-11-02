<template>
    
    <div class="root">
            <ToolbarPanel ref="toolbar" :mode="mode" />
            <div style="display: flex">
                <ItemPanel ref="addItemPanel" :height="height" :model="data.model" :groups="groups" v-if="mode == 'edit'"/>
                <div ref="canvas" class="canvasPanel" :style="{'height':height+'px','width':mode != 'edit' ?'100%':'65%','border-bottom':isView?0:null}"></div>
                <DetailPanel ref="detailPanel"
                             v-if="mode == 'edit'"
                             :height="height"
                             :model="selectedModel"
                             :readOnly="mode !== 'edit'"
                             :users="users"
                             :nodes="data.nodes"
                             :departments="departments"
                             :onChange="(key, val) => { onItemCfgChange(key, val) }" />
            </div>
    </div>
</template>
<script>
	import G6 from '@antv/g6/lib';
	import { getShapeName } from '../util/clazz'
	import Command from '../plugins/command'
	import Toolbar from '../plugins/toolbar'
	import AddItemPanel from '../plugins/addItemPanel'
	import CanvasPanel from '../plugins/canvasPanel'
	import ToolbarPanel from '../components/ToolbarPanel'
	import ItemPanel from '../components/ItemPanel'
	import DetailPanel from '../components/DetailPanel'
	import i18n from '../locales'
	import { exportXML, exportImg } from "../util/bpmn"
	import registerShape from '../shape'
	import registerBehavior from '../behavior'

	registerShape(G6);
	registerBehavior(G6);
	export default {
		name: "wfd-vue",
		components: {
			ToolbarPanel,
			ItemPanel,
			DetailPanel
		},
		provide() {
			return {
				i18n: i18n[this.lang]
			}
		},
		props: {
			isView: {
				type: Boolean,
				default: false,
			},
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
				selectedModel: {},
				graph: null,
				cmdPlugin: null,
			};
		},
		watch: {
			data: {
				handler(newData, oldData) {
					if (oldData !== newData) {

						this.init();
					}
				},
				deep: true
			}
		},
		methods: {
			initShape(data) {
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
				this.graph.on('afteritemselected', (items) => {
					if (items && items.length > 0) {

						let item = that.findItembyId(items[0]);
						//if (!item) {
						//     item = this.getNodeInSubProcess(items[0])
						// }
						that.selectedModel = item;
					} else {
						that.selectedModel = {};
					}
				});
				this.graph.on('beforeadditem', (node) => {
					var model = node.model;
					var type = node.type;
					model.id = rand();
					if (type == "node") {
						model.data_setting = {};
						that.data.nodes.push(model);
					} else {
						that.data.edges.push(model);
					}
					that.selectedModel = model;
				});
				this.graph.on('removeShape', (items) => {
					if (items && items.length > 0) {
						that.removeShape(items[0]);
					}

				});
				this.graph.on('afterupdateitem', (items) => {

					//console.log(items);
					///UPDATE X,Y
					var cfg = items.cfg;
					if (cfg.x || cfg.y) {
						var item = items.item;
						var id = item._cfg.id;
						var index = that.findIndexNode(id);
						that.data.nodes[index].x = cfg.x
						that.data.nodes[index].y = cfg.y;
					}
				});


				const page = this.$refs['canvas'];
				const graph = this.graph;
				const height = this.height - 1;
				this.resizeFunc = () => {
					graph.changeSize(page.offsetWidth, height);
				};
				window.addEventListener("resize", this.resizeFunc);
			},
			findIndexNode(id) {
				let index = this.data.nodes.findIndex(function (item) {
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
			removeShape(id) {
				var that = this;
				let index = that.data.nodes.findIndex(function (item) {
					return item.id == id;
				});

				let item;
				if (index == -1) {
					index = that.data.edges.findIndex(function (item) {
						return item.id == id;
					});
					if (index != -1)
						that.data.edges.splice(index, 1)
				} else {
					that.data.nodes.splice(index, 1)
				}
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
				if (this.mode == 'edit') {
					const addItemPanel = new AddItemPanel({ container: this.$refs['addItemPanel'].$el });
					plugins.push(addItemPanel);
				}
				const width = this.$refs['canvas'].offsetWidth;
				this.graph = new G6.Graph({
					plugins: plugins,
					container: this.$refs['canvas'],
					height: this.height,
					width: width,
					modes: {
						default: ['drag-canvas', 'clickSelected'],
						view: [],
						edit: ['drag-canvas', 'hoverNodeActived', 'hoverAnchorActived', 'dragNode', 'dragEdge',
							'dragPanelItemAddNode', 'clickSelected', 'deleteItem', 'itemAlign', 'dragPoint', 'brush-select'],
					},
					defaultEdge: {
						shape: 'flow-polyline-round',
					},
				});
				if (this.isView)
					this.graph.setMode('view');
				else
					this.graph.setMode(this.mode);
				this.graph.data(this.initShape(this.data));
				this.graph.render();
				if (this.isView && this.data && this.data.nodes) {
					this.graph.fitView(5)
				}
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
    .root{
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
</style>
