<template>
    <div class="root">
        <ToolbarPanel ref="toolbar" :mode="mode" />
        <div ref="canvas" class="canvasPanel" :style="{'height':height+'px','width':'100%'}"></div>
        <Transition>
            <div v-if="selectedModel != null">
                <sidebar :model="selectedModel" ref="sidebar" @execute_transition="execute_transition" @assign_again="assign_again" @close="close" :departments="departments" :users="users" :nodes="data.nodes"></sidebar>
            </div>
        </Transition>
        <assign :departments="departments" :users="users" :data_custom_block="custom_block" v-if="custom_block.length > 0" @save_data="save_data" :required="required" @close="close"></assign>
    </div>
</template>
<script>
    import G6 from '@antv/g6/lib';
    import { getShapeName } from '../util/clazz'
    import Command from '../plugins/command'
    import Toolbar from '../plugins/toolbar'
    import CanvasPanel from '../plugins/canvasPanel'
    import ToolbarPanel from '../components/ToolbarPanel'
    import Assign from '../components/Assign'
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
            Sidebar,
            Assign
        },
        provide() {
            return {
                i18n: i18n[this.lang]
            }
        },
        computed: {
            current_user() {
                return store.state.current_user;
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
                default: () => ({ nodes: [], edges: [] })
            },
            model: {
                type: Object,
                default: () => ({})
            },
            data_transition: {
                type: Array,
                default: () => ([])
            },
            data_activity: {
                type: Array,
                default: () => ([])
            },
            data_custom_block: {
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
                custom_block: [],
                required: true,
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
                var data_custom_block = this.data_custom_block;
                var data_activity = this.data_activity;
                var data_transition = this.data_transition;
                var model = that.model || {}
                var status_id = model.status_id;
                //console.log(data_transition)
                for (var i in data_transition) {
                    var transition = data_transition[i];
                    var edge_id = transition.link_id;
                    var indexEdge = this.findIndexEdge(edge_id);
                    var edge = edges[indexEdge];
                    if (!transition.reverse) {
                        edge.active = true;
                    } else {
                        edge.reverse = true;
                    }
                    if (i == data_transition.length - 1 && that.current_user.id == transition.created_by && status_id == 2) {
                        edge.allowDelete = true;
                    }
                }
                for (var activity of data_activity) {
                    if (activity.blocking) {
                        var block_id = activity.block_id;
                        var indexNode = this.findIndexNode(block_id);
                        var block = nodes[indexNode];
                        block.active = true;
                        var node = that.graph.find("node", (node) => {
                            return node.get("model").id == activity.block_id;
                        });
                        var outEdges = node.getOutEdges().map(function (i) {
                            return i.get("model");
                        });
                        var findCustomBlock = data_custom_block.findIndex(function (item) {
                            return item.block_id == block.id;
                        });
                        if (findCustomBlock != -1) {
                            activity.data_setting = data_custom_block[findCustomBlock].data_setting;
                        }
                        var fields = block.fields || [];
                        fields = fields.map(function (i) {
                            i.data_setting = i.data_setting || {};
                            i.values = {};
                            switch (i.type) {
                                case "number":
                                case "text":
                                case "email":
                                case "date":
                                case "date_month":
                                case "date_time":
                                case "select":
                                case "department":
                                case "textarea":
                                case "employee":
                                case "currency":
                                    var value = i.has_default ? i.data_setting.default_value : null;
                                    i.values = { value: value };
                                    break;
                                case "select_multiple":
                                case "employee_multiple":
                                case "department_multiple":
                                    var value_array = i.has_default ? i.data_setting.default_value_array : null;
                                    i.values = { value_array: value_array };
                                    break;
                                case "table":
                                    i.values = { list_data: [] };
                                    break;
                            }
                            return i;
                        });
                        activity.fields = fields;
                        activity.outEdges = outEdges;
                    }
                }
                if (data && data.nodes) {
                    //var edges = $.extendext(true, 'replace', [], data.edges);
                    return {
                        nodes: nodes.map(node => {
                            return {
                                type: getShapeName(node.clazz),
                                ...node,
                            }
                        }),
                        edges: edges
                    }
                }
                return data;
            },
            initEvents() {
                var that = this;
                this.graph.on('node:click', (e) => {
                    var item = e.item;
                    var id = item.get("model").id;
                    var findBlocking = that.data_activity.findLastIndex(function (item) {
                        return item.block_id == id;
                    });
                    if (findBlocking != -1) {
                        var model = that.data_activity[findBlocking];
                        that.selectedModel = model;
                        return;
                    }
                    that.selectedModel = null;
                });
                this.graph.on('delete-shape:click', async (e) => {

                    var text = "Bạn có muốn rút lại bước này!";
                    if (confirm(text)) {
                        var edge = e.item;
                        var id = edge.get("model").id;
                        var findTransition = that.data_transition.findLastIndex(function (item) {
                            return item.link_id == id;
                        });
                        if (findTransition != -1) {
                            var transition = that.data_transition[findTransition];
                            var resp = await $.ajax({ url: path + "/admin/api/withdraw", data: { transition_id: transition.id }, type: "POST", dataType: "JSON" });
                            location.reload();
                        }

                    }
                });


                //this.graph.on('delete-shape:mouseenter', async (e) => {
                //    console.log(e.item)
                //    this.graph.setItemState(e.item, 'hover', true);
                //});
                //this.graph.on('delete-shape:mouseleave', async (e) => {
                //    console.log(e.item)
                //    this.graph.setItemState(e.item, 'hover', false);
                //});
                //this.graph.on('canvas:click', (e) => {
                //    that.selectedModel = null;
                //});
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
                        type: 'flow-polyline-round',
                        // Other properties for all the nodes
                    },
                });
                this.graph.setMode(this.mode);
                this.graph.data(this.initShape());
                this.graph.render();
                this.graph.fitView();
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
                var data_custom_block = this.data_custom_block;
                var data_activity = this.data_activity;
                var data_transition = this.data_transition;
                //var nodes = state.data.nodes;
                var findIndexActivity = data_activity.findLastIndex(function (item) {
                    return item.id == from_activity_id
                });
                var activity = data_activity[findIndexActivity];

                var edge = this.graph.find('edge', (node) => {
                    return node.get('model').id === edge_id;
                });
                ////VAILD TIEU DE
                if (!$(".tieu_de").val()) {
                    Swal.fire({
                        title: 'Nhập tiêu đề',
                        input: 'text',
                        inputAttributes: {
                            autocapitalize: 'off'
                        },
                        showCancelButton: false,
                        confirmButtonText: 'Gửi đi',
                        showLoaderOnConfirm: true,
                        preConfirm: (data) => {
                            that.model.title = data;
                            //$(".tieu_de").val(data);
                            return true;
                        },
                        allowOutsideClick: () => !Swal.isLoading()
                    }).then((result) => {
                        that.execute_transition(from_activity_id, edge_id);
                    })
                    alert("Bạn chưa nhập tiêu đề!");
                    $(".tieu_de").focus();
                    return;
                }
                //////UPDATE Current
                if (!edge.get("model").reverse) {
                    var vaild = $("#sidebar-right").valid();
                    if (!vaild) {
                        return;
                    }
                    activity.failed = false;
                    $("#sidebar-right input[type='file']").each(function (item) {
                        var id = $(this).attr("name");
                        var findfield = activity.fields.findIndex(function (i) {
                            return i.id == id;
                        });
                        var field = activity.fields[findfield];
                        field.files = $(this)[0].files;
                    });
                } else {
                    activity.failed = true;
                }
                activity.blocking = false;
                activity.executed = true;
                activity.is_update = true;
                activity.created_by = that.current_user.id;
                //
                var source = edge.getSource();
                var target = edge.getTarget();

                /////ADD TRANSITION
                var transition_new = {
                    is_new: true,
                    label: edge.get("model").label,
                    reverse: edge.get("model").reverse,
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
                //TARGET
                var create_new = true;
                if (target.get("model").clazz == 'inclusiveGateway') {
                    var find_activity = that.data_activity.findLastIndex(function (item) {
                        return item.block_id == target.get("model").id;
                    });
                    if (find_activity != -1) {
                        create_new = false;
                        activity_new = that.data_activity[find_activity];
                    }
                }
                if (create_new) {
                    var blocking = false;
                    if (target.get("model").clazz == 'formTask' || target.get("model").clazz == 'approveTask' || target.get("model").clazz == 'mailSystem') {
                        blocking = true;
                    }

                    var data_setting = target.get("model").data_setting || {};
                    var activity_new = {
                        execution_id: null,
                        label: target.get("model").label,
                        block_id: target.get("model").id,
                        stt: data_activity.length + 1,
                        clazz: target.get("model").clazz,
                        is_new: true,
                        executed: !blocking,
                        failed: false,
                        blocking: blocking,
                        data_setting: data_setting,
                        in_transition_id: transition_new.id,
                        id: rand()
                    }
                    data_activity.push(activity_new);
                }
                transition_new.to_activity_id = activity_new.id;
                ////Custom Block
                if (blocking) {
                    var findCustomBlock = data_custom_block.findIndex(function (item) {
                        return item.block_id == target.get("model").id;
                    })
                    if (findCustomBlock == -1) {
                        var data_setting_block = target.get("model").data_setting || {};
                        var type_performer = target.get("model").type_performer;
                        var data_setting = {};
                        if (type_performer == 1 && data_setting_block.block_id == null) {
                            data_setting.type_performer = 4;
                            data_setting.listuser = [that.current_user.id];
                        } else if (type_performer == 1 && data_setting_block.block_id != null) {
                            data_setting.type_performer = 4;
                            var findIndexActivity = data_activity.findLastIndex(function (item) {
                                return item.block_id == data_setting_block.block_id;
                            })
                            var findActivity = data_activity[findIndexActivity];
                            data_setting.listuser = [findActivity.created_by];
                        } else if (type_performer == 3 || type_performer == 4) {
                            data_setting = data_setting_block;
                            data_setting.type_performer = type_performer;
                        } else if (type_performer == 5) {
                            data_setting.type_performer = 4;
                            data_setting.listuser = [this.model.user_id];
                        }
                        if (type_performer != 2) {
                            var custom_block = {
                                data_setting: data_setting,
                                block_id: target.get("model").id,
                                is_new: true,
                            }
                            data_custom_block.push(custom_block);
                            store.commit('SET_CUSTOM_BLOCK', data_custom_block);
                        }

                    }
                }
                store.commit('SET_DATA_ACTIVITY', data_activity);
                store.commit('SET_DATA_TRANSITION', data_transition);

                this.create_next(activity_new);
                setTimeout(function () {
                    //var data2 = that.initShape();
                    //that.graph.read(data2);
                    //that.graph.fitView();

                    that.selectedModel = null;
                    if (!activity.failed) {
                        that.check_assign(activity.block_id);
                    } else {
                        that.save_data();
                    }
                    //
                }, 100)

            },
            create_next(activity) {

                var that = this;
                var graph = this.graph;
                var node = graph.find('node', (node) => {
                    return node.get('model').id == activity.block_id;
                });
                if (activity.clazz == "inclusiveGateway") {
                    var ins = node.getInEdges();
                    var transitions = this.data_transition.filter(function (item) {
                        return item.to_activity_id == activity.id;
                    });
                    if (transitions.length < ins.length) {
                        activity.blocking = true;
                    } else {
                        activity.blocking = false;
                    }
                    ////
                    if (!activity.is_new)
                        activity.is_update = true;
                }
                if (activity.blocking)
                    return;
                var data_custom_block = this.data_custom_block;
                var data_activity = this.data_activity;
                var data_transition = this.data_transition;

                var outs = node.getOutEdges();
                if (outs.length) {
                    for (var out of outs) {
                        var source = out.getSource();
                        var target = out.getTarget();

                        var transition = {
                            is_new: true,
                            label: out.get("model").label,
                            reverse: out.get("model").reverse,
                            link_id: out.get("model").id,
                            execution_id: null,
                            from_block_id: source.get("model").id,
                            to_block_id: target.get("model").id,
                            from_activity_id: activity.id,
                            //to_activity_id: activity.id,
                            stt: data_transition.length + 1,
                            id: rand(),
                        }
                        data_transition.push(transition);

                        ///CREATE TARGET ACTIVITY

                        var create_new = true;
                        if (target.get("model").clazz == 'inclusiveGateway') {
                            var find_activity = that.data_activity.findLastIndex(function (item) {
                                return item.block_id == target.get("model").id;
                            });
                            if (find_activity != -1) {
                                create_new = false;
                                activity_new = that.data_activity[find_activity];
                            }
                        }
                        if (create_new) {
                            var fields = target.get("model").fields || [];
                            fields = fields.map(function (i) {
                                i.data_setting = i.data_setting || {};
                                i.values = {};
                                switch (i.type) {
                                    case "number":
                                    case "text":
                                    case "email":
                                    case "date":
                                    case "date_month":
                                    case "date_time":
                                    case "select":
                                    case "department":
                                    case "textarea":
                                    case "employee":
                                    case "currency":
                                        var value = i.has_default ? i.data_setting.default_value : null;
                                        i.values = { value: value };
                                        break;
                                    case "select_multiple":
                                    case "employee_multiple":
                                    case "department_multiple":
                                        var value_array = i.has_default ? i.data_setting.default_value_array : null;
                                        i.values = { value_array: value_array };
                                        break;
                                    case "table":
                                        i.values = { list_data: [] };
                                        break;
                                }
                                return i;
                            });
                            var data_setting = target.get("model").data_setting || {};
                            var blocking = false;
                            if (target.get("model").clazz == 'formTask' || target.get("model").clazz == 'approveTask' || target.get("model").clazz == 'mailSystem') {
                                blocking = true;
                            }
                            var activity_new = {
                                execution_id: null,
                                label: target.get("model").label,
                                block_id: target.get("model").id,
                                stt: data_activity.length + 1,
                                clazz: target.get("model").clazz,
                                is_new: true,
                                executed: !blocking,
                                failed: false,
                                blocking: blocking,
                                fields: fields,
                                data_setting: data_setting,
                                in_transition_id: transition.id,
                                created_by: that.current_user.id,
                                id: rand()
                            }
                            data_activity.push(activity_new);
                        }

                        transition.to_activity_id = activity_new.id;
                        ////Custom Block
                        if (blocking) {
                            var findCustomBlock = data_custom_block.findIndex(function (item) {
                                return item.block_id == target.get("model").id;
                            })
                            if (findCustomBlock == -1) {
                                var data_setting_block = target.get("model").data_setting || {};
                                var type_performer = target.get("model").type_performer;
                                var data_setting = {};
                                if (type_performer == 1 && data_setting_block.block_id == null) {
                                    data_setting.type_performer = 4;
                                    data_setting.listuser = [that.current_user.id];
                                } else if (type_performer == 1 && data_setting_block.block_id != null) {
                                    data_setting.type_performer = 4;
                                    var findIndexActivity = data_activity.findLastIndex(function (item) {
                                        return item.block_id == data_setting_block.block_id;
                                    })
                                    var findActivity = data_activity[findIndexActivity];
                                    data_setting.listuser = [findActivity.created_by];
                                } else if (type_performer == 3 || type_performer == 4) {
                                    data_setting = data_setting_block;
                                    data_setting.type_performer = type_performer;
                                }
                                var custom_block = {
                                    data_setting: data_setting,
                                    block_id: target.get("model").id,
                                    is_new: true,
                                }
                                data_custom_block.push(custom_block);
                                store.commit('SET_CUSTOM_BLOCK', data_custom_block);
                            }
                        }
                        store.commit('SET_DATA_ACTIVITY', data_activity);
                        store.commit('SET_DATA_TRANSITION', data_transition);

                        this.create_next(activity_new);
                    }
                }
            },
            check_assign(block_id) {
                var data_custom_block = this.data_custom_block;
                var nodes = this.data.nodes;
                var findNodes = nodes.filter(function (item) {
                    return item.type_performer == 2 && item.data_setting.block_id == block_id;
                });
                if (findNodes.length) {
                    var custom_block = this.custom_block;
                    for (var node of findNodes) {
                        var block_id = node.id;
                        var custom = {
                            data_setting: {},
                            block_id: block_id,
                            block: node
                        }
                        custom_block.push(custom);

                        var findCustom = data_custom_block.findIndex(function (item) {
                            return item.block_id == block_id;
                        });
                        if (findCustom != -1) {
                            if (data_custom_block[findCustom].id) {
                                custom.is_update = true;
                                custom.id = data_custom_block[findCustom].id;
                            } else {
                                custom.is_new = true;
                            }
                            data_custom_block[findCustom] = custom;
                        } else {
                            custom.is_new = true;
                            data_custom_block.push(custom);
                        }
                    }
                    this.required = true;
                    store.commit('SET_CUSTOM_BLOCK', data_custom_block);

                } else {
                    this.save_data();
                }
                ////

            },
            assign_again(block_id) {
                var nodes = this.data.nodes;
                var findIndex = nodes.findIndex(function (item) {
                    return item.id == block_id
                });
                var node = nodes[findIndex];
                var data_custom_block = this.data_custom_block;
                var custom_block = this.custom_block;
                var custom = {
                    data_setting: {},
                    block_id: block_id,
                    block: node
                }
                custom_block.push(custom);
                var findCustom = data_custom_block.findIndex(function (item) {
                    return item.block_id == block_id;
                });
                if (findCustom != -1) {
                    if (data_custom_block[findCustom].id) {
                        custom.is_update = true;
                        custom.id = data_custom_block[findCustom].id;
                    } else {
                        custom.is_new = true;
                    }
                    data_custom_block[findCustom] = custom;
                } else {
                    custom.is_new = true;
                    data_custom_block.push(custom);
                }

                this.selectedModel = null;
                this.required = false;
                store.commit('SET_CUSTOM_BLOCK', data_custom_block);
            },
            save_data() {
                this.$emit("save_data");
            },
            close() {
                this.custom_block = [];
                this.selectedModel = null;
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
        display: inline-block;
    }

    .canvasPanel {
        flex: 0 0 auto;
        float: left;
        background-color: #fff;
        border-bottom: 1px solid #E9E9E9;
    }

    .v-enter-active,
    .v-leave-active {
        transition: opacity 0.2s ease;
    }

    .v-enter-from,
    .v-leave-to {
        opacity: 0;
    }
</style>
