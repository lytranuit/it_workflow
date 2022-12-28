<template>
    <div id="execution">
        <div class="header flex-m">
            <div>
                <div class="flex-m">
                    <span class="title">
                        <span v-show="!edit" ref="spanTitle">{{model.title}}</span>
                        <input v-show="edit" class="form-control form-control-sm tieu_de" v-model="model.title" ref="inputTitle" placeholder="Tiêu đề" />
                    </span>

                    <span class="edit-title" @click="toggle_edit()" :class="{'btn-success btn btn-sm':edit}">
                        <i class="fas fa-pen" v-if="!edit"></i>
                        <i class="fas fa-check" v-else></i>
                    </span>
                    <div class="status" :class="'status_' + model.status_id">{{model.status}}</div>
                </div>
                <div class="flex-m">
                    <span class="">
                        <span class="">ID</span>: <span class="font-weight-bold"> {{model.id}} </span>
                    </span>
                    <span class="mx-2">|</span>
                    <div class=" flex-m">
                        <span class="">Người tạo</span>: <span class="font-weight-bold" v-if="model.user"> {{model.user.fullName}} </span>
                    </div>
                    <span class="mx-2 ">|</span>
                    <div data-v-cf52cf0c=""><span class=""> Ngày tạo: </span><span class="font-weight-bold"> {{model.created_at}} </span></div>
                </div>
            </div>
            <!--<div class="" style="margin-left: auto">
                <div class="dropdown d-inline-block" style="font-size:15px">
                    <a class="nav-link dropdown-toggle" id="drop2" data-toggle="dropdown" href="#" role="button" aria-haspopup="false" aria-expanded="false">
                        <i class="fas fa-ellipsis-v text-muted"></i>
                    </a>
                    <div class="dropdown-menu" aria-labelledby="drop2">
                        <a class="dropdown-item" href="#">Phân công lại</a>
                        <a class="dropdown-item" href="#">Hủy</a>

                    </div>
                </div>
            </div>-->
        </div>
        <wfd-vue-execution ref="wfd" :data="data" :model="model" :data_custom_block="data_custom_block" :data_transition="data_transition" :data_activity="data_activity" :departments="departments" :users="users" :height="600" :lang="lang" mode="executing" @save_data="save_data" />
        <div class="row mt-2" v-if="model.id > 0">
            <div class="col-md-9">
                <comment :model="model"></comment>
            </div>
            <div class="col-md-3">
                <event :model="model"></event>
            </div>
        </div>
    </div>
</template>

<script>
    import store from './store';
    import WfdVueExecution from '../src/components/Wfd-Execution';
    import Comment from '../src/components/Comment';
    import Event from '../src/components/Event';
    export default {
        name: 'execution',
        data() {
            return {
                lang: "vi",
                errors: [],
                model: {

                },
                edit: false
            }
        },
        components: {
            WfdVueExecution,
            Comment,
            Event
        },
        computed: {
            data() {
                return store.state.data;
            },
            departments() {
                return store.state.departments;
            },
            users() {
                return store.state.users;
            },
            data_activity() {
                return store.state.data_activity;
            },
            data_transition() {
                return store.state.data_transition;
            },
            data_custom_block() {
                return store.state.data_custom_block;
            },

            current_user() {
                return store.state.current_user;
            }
        },
        async mounted() {
            var that = this;

            /// Lấy template
            var ress = await $.ajax({
                url: path + "/admin/api/ProcessVersion",
                data: { id: process_version_id },
            })
            //console.log(data);
            let res = ress.process;
            let data = {};
            data.edges = res.links.map(function (item) {
                var default_setting = {};
                item = $.extendext(true, 'replace', default_setting, item);
                return item;
            });
            var blocks = res.blocks.map(function (item) {
                var default_setting = { data_setting: {}, executed: false, failed: false, blocking: false };
                delete item.process;
                item = $.extendext(true, 'replace', default_setting, item);
                if (item.clazz == "start") {
                    item.executed = true;
                }
                return item;
            });
            data.nodes = blocks;
            delete res.blocks;
            delete res.links;
            delete res.fields;
            //data.model = res;
            store.commit('SET_DATA', data);
            /// Lấy department
            store.dispatch("fetchDepartment");
            /// Lấy users
            store.dispatch("fetchUsers");

            ///fetch_info
            var user = await $.ajax({
                url: path + "/admin/api/userinfo",
            })
            user = user || {};
            store.commit('SET_CURRENT_USER', user);

            if (execution_id) {
                var execution = await $.ajax({
                    url: path + "/admin/api/execution/" + execution_id
                })
                that.model = that.initModel(execution);
                var data_transition = await $.ajax({
                    url: path + "/admin/api/TransitionByExecution",
                    data: { execution_id: execution_id }
                })
                var data_activity = await $.ajax({
                    url: path + "/admin/api/ActivityByExecution",
                    data: { execution_id: execution_id }
                })
                var data_custom_block = await $.ajax({
                    url: path + "/admin/api/CustomBlockByExecution",
                    data: { execution_id: execution_id }
                })
                store.commit('SET_CUSTOM_BLOCK', data_custom_block);
                store.commit('SET_DATA_ACTIVITY', data_activity);
                store.commit('SET_DATA_TRANSITION', data_transition);
                setTimeout(function () {
                    var graph = that.$refs['wfd'].graph;
                    var data2 = that.$refs['wfd'].initShape();
                    graph.read(data2);
                    graph.fitView()
                    graph.executeCommand("currentFlow");
                    //// active block lên
                    var node_blocks = graph.findAll('node', (node) => {
                        return node.get("model").active == true;
                    });
                    for (var node of node_blocks) {
                        var index_activity_block = that.data_activity.findLastIndex(function (i) {
                            return i.block_id == node.get("model").id;
                        });
                        if (index_activity_block == -1) {
                            continue;
                        }
                        var activity = that.data_activity[index_activity_block];
                        if (that.hasPermission(activity)) {
                            graph.emit('node:click', { item: node });
                        }
                    }
                    /////Bỏ loadding
                    $(".preloader").fadeOut();

                }, 100)
            } else {
                that.model = {
                    title: '',
                    status: "Đang thực hiện",
                    status_id: 2,
                    process_version_id: process_version_id,
                    id: "---",
                    user_id: user.id,
                    user: user,
                    created_at: moment().format("DD/MM/YYYY"),
                }
                that.edit = true;
                /// INIT
                setTimeout(function () {
                    var graph = that.$refs['wfd'].graph;
                    //// activity START
                    var start = graph.find('node', (node) => {
                        return node.get('model').clazz === 'start';
                    });
                    var data_activity = [];
                    var activity_start = {
                        execution_id: null,
                        label: start.get("model").label,
                        block_id: start.get("model").id,
                        stt: 1,
                        clazz: start.get("model").clazz,
                        is_new: true,
                        executed: true,
                        failed: false,
                        blocking: false,
                        created_by: user.id,
                        created_at: moment().valueOf(),
                        id: rand()
                    }
                    data_activity.push(activity_start);

                    store.commit('SET_DATA_ACTIVITY', data_activity);

                    setTimeout(function () {
                        that.$refs['wfd'].create_next(activity_start);
                        var data2 = that.$refs['wfd'].initShape();
                        graph.read(data2);
                        graph.fitView()
                        graph.executeCommand("currentFlow")
                        //// active block lên
                        var node_blocks = graph.findAll('node', (node) => {
                            return node.get("model").active == true;
                        });
                        for (var node of node_blocks) {
                            var index_activity_block = that.data_activity.findLastIndex(function (i) {
                                return i.block_id == node.get("model").id;
                            });
                            if (index_activity_block == -1) {
                                continue;
                            }
                            var activity = that.data_activity[index_activity_block];
                            if (that.hasPermission(activity)) {
                                graph.emit('node:click', { item: node });
                            }
                        }


                        /////Bỏ loadding
                        $(".preloader").fadeOut();

                        //console.log(node_block);
                    }, 100)
                }, 100)


            }
        },
        methods: {
            hasPermission(activity) {
                var data_setting = activity.data_setting || {};
                var type_performer = data_setting.type_performer;
                var current_user = this.current_user;
                var user_id = current_user.id;
                var user_department = current_user.departments.map(function (item) {
                    return item.department_id;
                })
                if (type_performer == 4) {
                    var listuser = data_setting.listuser || [];
                    var result = listuser.filter(function (n) {
                        return n == user_id
                    });
                    if (result.length > 0)
                        return true;
                } else if (type_performer == 3) {
                    var listdepartment = data_setting.listdepartment || [];
                    var result = listdepartment.filter(function (n) {
                        return user_department.indexOf(n) !== -1;
                    });
                    if (result.length > 0)
                        return true;
                }
                return false;
            },
            update(e) {
                var value = $(e.target).text();
                this.model.title = value;
            },
            initModel(execution) {
                execution.created_at = moment(execution.created_at).format("DD/MM/YYYY")
                return execution;
            },
            toggle_edit() {
                this.edit = !this.edit;
                var that = this;
                if (this.edit) {
                    setTimeout(function () {
                        const spanTitle = that.$refs.spanTitle;
                        const inputTitle = that.$refs.inputTitle;
                        var width = $(spanTitle).width();
                        width = width > 200 ? width : 200;
                        $(inputTitle).css("width", width).focus();
                    }, 10);
                } else {
                    if (that.model.id > 0) {
                        $.ajax({ url: path + "/admin/api/updateexecution", data: { id: that.model.id, title: that.model.title }, type: "POST", dataType: "JSON" });

                    }

                }
            },
            findIndexNodes(id) {
                let index = this.data.nodes.findIndex(function (item) {
                    return item.id == id;
                });
                return index;
            },
            findIndexEdges(id) {
                let index = this.data.edges.findIndex(function (item) {
                    return item.id == id;
                });
                return index;
            },
            async save_data() {
                var data_custom_block = this.data_custom_block;
                var data_transition = this.data_transition;
                var data_activity = this.data_activity;
                //console.log(data_transition);
                //console.log(data_activity)
                //return;
                var tasks = [];
                $(".preloader").fadeIn();
                var model = this.model;
                if (model.id > 0) {

                } else {
                    ///create execute

                    var resp = await $.ajax({ url: path + "/admin/api/createexecution", data: model, type: "POST", dataType: "JSON" });
                    if (resp.success) {
                        model = resp.data;
                    }
                }
                for (var item of data_custom_block) {
                    item.execution_id = model.id;
                    if (item.is_new) {
                        var resp = await $.ajax({ url: path + "/admin/api/createcustomblock", data: item, type: "POST", dataType: "JSON" });
                    } else if (item.is_update) {
                        var resp = await $.ajax({ url: path + "/admin/api/updatecustomblock", data: item, type: "POST", dataType: "JSON" });
                    }
                }
                for (var item of data_transition) {
                    item.execution_id = model.id;
                    if (item.is_new) {
                        var task = { api: "createtransition", data: item };
                        tasks.push(task);
                        //var resp = await $.ajax({ url: path + "/admin/api/createtransition", data: item, type: "POST", dataType: "JSON" });
                    }
                }
                for (var item of data_activity) {
                    item.execution_id = model.id;
                    if ((item.is_new || item.is_update) && !item.failed) {
                        if (item.fields) {
                            for (var field of item.fields) {
                                field.execution_id = model.id;
                                field.id = rand();
                                if (field.type == 'file' || field.type == 'file_multiple') {
                                    var formData = new FormData();
                                    for (var i = 0; i < field.files.length; i++) {
                                        formData.append("files", field.files[i]);
                                    }
                                    formData.append("execution_id", model.id);
                                    var resp = await $.ajax({
                                        type: "POST",
                                        url: path + "/admin/api/uploadFile",
                                        data: formData,
                                        dataType: "json",
                                        contentType: false, // Not to set any content header
                                        processData: false, // Not to process data
                                    });
                                    field.values.files = resp.list;
                                    field.files = null;
                                }
                            }
                        }
                        if (item.sign) {
                            var sign = item.sign;
                            var resp = await $.ajax({ url: path + "/admin/api/SaveSign", data: sign, type: "POST", dataType: "JSON" });
                            delete item.sign;
                            if (resp.success == 1) {
                                var resp_sign = resp.sign;
                                var data_setting = item.data_setting || {};
                                var listusersign = data_setting.listusersign || [];
                                if (listusersign.length > 0) {
                                    var findindex = listusersign.findLastIndex(function (item) {
                                        return item.user_sign == resp_sign.user_sign;
                                    });
                                    listusersign[findindex] = resp_sign;
                                    item.data_setting.listusersign = listusersign;
                                }
                            }
                        }
                    }
                    if (item.is_new) {

                        var task = { api: "createactivity", data: item };
                        tasks.push(task);
                        //var resp = await $.ajax({ url: path + "/admin/api/createactivity", data: item, type: "POST", dataType: "JSON" });
                    } else if (item.is_update) {
                        var task = { api: "updateactivity", data: item };
                        tasks.push(task);
                        //var resp = await $.ajax({ url: path + "/admin/api/updateactivity", data: item, type: "POST", dataType: "JSON" });
                    }
                }
                tasks.sort(function (a, b) {
                    if (a.data.created_at == b.data.created_at)
                        return 0;
                    if (a.data.created_at > b.data.created_at)
                        return 1;
                    if (a.data.created_at < b.data.created_at)
                        return -1
                });
                for (var item of tasks) {
                    var resp = await $.ajax({ url: path + "/admin/api/" + item.api, data: item.data, type: "POST", dataType: "JSON" });
                }
                location.href = path + "/admin/execution/details/" + process_version_id + "?execution_id=" + model.id;
            }
        }
    }
</script>

<style lang="scss" scoped>



    #execution {
        color: #2c3e50;
    }

    .header {
        background: white;
        margin-bottom: 10px;
        padding: 10px 20px;

        .title {
            font-size: 20px !important;
            font-weight: 700 !important;
            color: rgb(27, 28, 30) !important;
            margin-right: 5px;

            &[contenteditable=true] {
                border: 2px solid #e8ebf3;
                padding: 0px 20px;
                border-radius: 5px;
            }
        }

        .edit-title {
            font-size: 12px;
            margin-right: 5px;
            padding: 5px 20px;
            cursor: pointer;
            line-height: 20px;
        }
    }
</style>