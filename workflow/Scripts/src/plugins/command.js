import { mix, clone, isString } from '@antv/util';

import store from '../../example/store';

class Command {

	constructor() {
		this._cfgs = this.getDefaultCfg();
		this.list = [];
		this.queue = [];
	}

	getDefaultCfg() {
		return { _command: { zoomDelta: .1, queue: [], current: 0, clipboard: [] } };
	}

	get(key) {
		return this._cfgs[key];
	}
	set(key, val) {
		this._cfgs[key] = val;
	}

	initPlugin(graph) {
		this.initCommands();
		graph.getCommands = () => { return this.get('_command').queue };
		graph.getCurrentCommand = () => {
			const c = this.get('_command');
			return c.queue[c.current - 1]
		};
		graph.executeCommand = (name, cfg) => { this.execute(name, graph, cfg) };
		graph.commandEnable = (name) => { return this.enable(name, graph) };
	}

	registerCommand(name, cfg,) {
		if (this[name]) {
			mix(this[name], cfg);
		} else {
			const cmd = mix({}, {
				name: name,
				shortcutCodes: [],
				queue: true,
				executeTimes: 1,
				init() { },
				enable() { return true },
				execute(graph) {
					this.snapShot = graph.save();
					this.selectedItems = graph.get('selectedItems');
					this.method && (isString(this.method) ? graph[this.method]() : this.method(graph));
				},
				back(graph) {
					graph.read(this.snapShot);
					graph.set('selectedItems', this.selectedItems);
				}
			}, cfg);
			this[name] = cmd;
			this.list.push(cmd);
		}
	}

	execute(name, graph, cfg) {
		const cmd = mix({}, this[name], cfg);
		const manager = this.get('_command');
		if (cmd.enable(graph)) {
			cmd.init();
			if (cmd.queue) {
				manager.queue.splice(manager.current, manager.queue.length - manager.current, cmd);
				manager.current++;
			}
		}
		graph.emit('beforecommandexecute', { command: cmd });
		cmd.execute(graph);
		graph.emit('aftercommandexecute', { command: cmd });
		return cmd;
	}

	enable(name, graph) {
		return this[name].enable(graph);
	}

	destroyPlugin() {
		this._events = null;
		this._cfgs = null;
		this.list = [];
		this.queue = [];
		this.destroyed = true;
	}
	_deleteSubProcessNode(graph, itemId) {
		const subProcess = graph.find('node', (node) => {
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
			const resultModel = group.removeItem(subProcess, itemId);
			graph.updateItem(subProcess, resultModel);
		}
	}
	initCommands() {
		const cmdPlugin = this;
		cmdPlugin.registerCommand('add', {
			enable: function () {
				return this.type && this.addModel;
			},
			execute: function (graph) {
				const item = graph.add(this.type, this.addModel);
				if (this.executeTimes === 1)
					this.addId = item.get('id');
			},
			back: function (graph) {
				graph.remove(this.addId);
			},
		});
		cmdPlugin.registerCommand('update', {
			enable: function () {
				return this.itemId && this.updateModel;
			},
			execute: function (graph) {
				const item = graph.findById(this.itemId);
				if (item) {
					if (this.executeTimes === 1)
						this.originModel = mix({}, item.getModel());
					graph.update(item, this.updateModel);
				}
			},
			back: function (graph) {
				const item = graph.findById(this.itemId);
				graph.update(item, this.originModel);
			},
		});
		cmdPlugin.registerCommand('delete', {
			enable: function (graph) {
				const mode = graph.getCurrentMode();
				const selectedItems = graph.get('selectedItems');
				return mode === 'edit' && selectedItems && selectedItems.length > 0;
			},
			method: function (graph) {
				const selectedItems = graph.get('selectedItems');
				graph.emit('beforedelete', { items: selectedItems });
				if (selectedItems && selectedItems.length > 0) {
					selectedItems.forEach(i => {
						const node = graph.findById(i);
						if (node) {
							graph.remove(i);
						} else {
							cmdPlugin._deleteSubProcessNode(graph, i);
						}
					});
				}
				graph.emit('afterdelete', { items: selectedItems });
			},
			shortcutCodes: ['Delete', 'Backspace'],
		});
		cmdPlugin.registerCommand('redo', {
			queue: false,
			enable: function (graph) {
				const mode = graph.getCurrentMode();
				const manager = cmdPlugin.get('_command');
				return mode === 'edit' && manager.current < manager.queue.length;
			},
			execute: function (graph) {
				const manager = cmdPlugin.get('_command');
				const cmd = manager.queue[manager.current];
				cmd && cmd.execute(graph);
				manager.current++;
			},
			shortcutCodes: [['metaKey', 'shiftKey', 'z'], ['ctrlKey', 'shiftKey', 'z']],
		});
		cmdPlugin.registerCommand('undo', {
			queue: false,
			enable: function (graph) {
				const mode = graph.getCurrentMode();
				return mode === 'edit' && cmdPlugin.get('_command').current > 0;
			},
			execute: function (graph) {
				const manager = cmdPlugin.get('_command');
				const cmd = manager.queue[manager.current - 1];
				if (cmd) {
					cmd.executeTimes++;
					cmd.back(graph);
				}
				manager.current--;
			},
			shortcutCodes: [['metaKey', 'z'], ['ctrlKey', 'z']],
		});
		cmdPlugin.registerCommand('copy', {
			queue: false,
			enable: function (graph) {
				const mode = graph.getCurrentMode();
				const items = graph.get('selectedItems');
				return mode === 'edit' && items && items.length > 0;
			},
			method: function (graph) {
				const manager = cmdPlugin.get('_command');
				manager.clipboard = [];
				const items = graph.get('selectedItems');
				if (items && items.length > 0) {
					const item = graph.findById(items[0]);
					if (item) {
						manager.clipboard.push({ type: item.get('type'), model: item.getModel() });
					}
				}
			},
		});
		cmdPlugin.registerCommand('paste', {
			enable: function (graph) {
				const mode = graph.getCurrentMode();
				return mode === 'edit' && cmdPlugin.get('_command').clipboard.length > 0;
			},
			method: function (graph) {
				const manager = cmdPlugin.get('_command');
				this.pasteData = clone(manager.clipboard[0]);
				const addModel = this.pasteData.model;
				addModel.x && (addModel.x += 10);
				addModel.y && (addModel.y += 10);
				const id = rand();
				addModel.id = id;
				const item = graph.add(this.pasteData.type, addModel);
				item.toFront();
			},
		});
		cmdPlugin.registerCommand('zoomIn', {
			queue: false,
			enable: function (graph) {
				const zoom = graph.getZoom();
				const maxZoom = graph.get('maxZoom');
				const minZoom = graph.get('minZoom');
				return zoom <= maxZoom && zoom >= minZoom;
			},
			execute: function (graph) {
				const manager = cmdPlugin.get('_command');
				const maxZoom = graph.get('maxZoom');
				const zoom = graph.getZoom();
				this.originZoom = zoom;
				let currentZoom = zoom + manager.zoomDelta;
				if (currentZoom > maxZoom)
					currentZoom = maxZoom;
				graph.zoomTo(currentZoom);
			},
			back: function (graph) {
				graph.zoomTo(this.originZoom);
			},
			shortcutCodes: [['metaKey', '='], ['ctrlKey', '=']],
		});
		cmdPlugin.registerCommand('zoomOut', {
			queue: false,
			enable: function (graph) {
				const zoom = graph.getZoom();
				const maxZoom = graph.get('maxZoom');
				const minZoom = graph.get('minZoom');
				return zoom <= maxZoom && zoom >= minZoom;
			},
			execute: function (graph) {
				const manager = cmdPlugin.get('_command');
				const minZoom = graph.get('minZoom');
				const zoom = graph.getZoom();
				this.originZoom = zoom;
				let currentZoom = zoom - manager.zoomDelta;
				if (currentZoom < minZoom)
					currentZoom = minZoom;
				graph.zoomTo(currentZoom);
			},
			back: function (graph) {
				graph.zoomTo(this.originZoom);
			},
			shortcutCodes: [['metaKey', '-'], ['ctrlKey', '-']],
		});
		cmdPlugin.registerCommand('resetZoom', {
			queue: false,
			execute: function (graph) {
				const zoom = graph.getZoom();
				this.originZoom = zoom;
				graph.zoomTo(1);
			},
			back: function (graph) {
				graph.zoomTo(this.originZoom);
			},
		});
		cmdPlugin.registerCommand('autoFit', {
			queue: false,
			execute: function (graph) {
				const zoom = graph.getZoom();
				this.originZoom = zoom;
				graph.fitView(5);
			},
			back: function (graph) {
				graph.zoomTo(this.originZoom);
			},
		});
		cmdPlugin.registerCommand('allFlow', {
			queue: false,
			execute: function (graph) {
				const nodes = graph.getNodes();
				const edges = graph.getEdges();
				nodes.forEach((node) => {
					node.show();
				});
				edges.forEach((edge) => {
					edge.show();
				});
				graph.paint();
			},
			back: function (graph) {
			},
		});
		cmdPlugin.registerCommand('successFlow', {
			queue: false,
			execute: function (graph) {
				const nodes = graph.getNodes();
				const edges = graph.getEdges();
				edges.forEach((edge) => {
					let model = edge.get("model");
					if (model.reverse)
						edge.hide();
					else
						edge.show();
				});
				nodes.forEach((node) => {
					node.show();
				});
				graph.paint();
			},
			back: function (graph) {
			},
		});

		cmdPlugin.registerCommand('currentFlow', {
			queue: false,
			execute: function (graph) {
				const nodes = graph.getNodes();
				const edges = graph.getEdges();
				edges.forEach((edge) => {
					edge.hide();
				});
				nodes.forEach((node) => {
					node.hide();
				});



				var data_transition = store.state.data_transition;
				var data_activity = store.state.data_activity;
				//console.log(data_activity)
				for (var transition of data_transition) {
					var link_id = transition.link_id;
					var edge = graph.find("edge", (item) => {
						return item.get('model').id === link_id;
					});
					//console.log(edge)
					edge.show();
				}
				for (var activity of data_activity) {
					var block_id = activity.block_id;
					var node = graph.find("node", (item) => {
						return item.get('model').id === block_id;
					});
					node.show();
				}
				graph.paint();
			},
			back: function (graph) {
			},
		});
	}
}
export default Command;
