<template>
    <div id="app">
        <div class="alert alert-danger alert-dismissible fade show" role="alert" v-show="errors.length > 0">
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true"><i class="mdi mdi-close"></i></span>
            </button>
            <div v-for="error in errors" v-html="error"></div>
        </div>
        <button type="button" class="el-button el-button--success el-button--small" style="float: right;margin-top: 6px;margin-right: 6px;" @click="save_data"><span>Lưu lại</span></button>
        <DataFields :data="data"></DataFields>
        <wfd-vue ref="wfd" :data="data" :departments="departments" :users="users" :groups="groups" :height="600" :lang="lang" />

    </div>
</template>

<script>
    import store from './store'
    import WfdVue from '../src/components/Wfd'
    import DataFields from '../src/components/DataFields'
    var new_process_id = rand();
    var startNode1 = rand();
    var taskNode1 = rand();
    var taskNode2 = rand();
    var successNode = rand();
    var failNode = rand();
    export default {
        name: 'app',
        components: {
            WfdVue,
            DataFields
        },
        data() {

            return {
                modalVisible: false,
                lang: "vi",
                errors: [],
                demoData: {
                    nodes: [
                        { id: startNode1, process_id: new_process_id, x: 50, y: 200, label: 'Bắt đầu', stt: 0, clazz: 'start', type_performer: 1, },
                        { id: taskNode1, process_id: new_process_id, x: 300, y: 200, label: 'Biểu mẫu', stt: 1, clazz: 'formTask', type_performer: 1, data_setting: {}, fields: [] },
                        { id: taskNode2, process_id: new_process_id, x: 500, y: 200, label: 'Phê duyệt', stt: 2, clazz: 'approveTask', type_performer: 1, data_setting: {} },
                        { id: successNode, process_id: new_process_id, x: 800, y: 200, label: 'Hoàn thành', stt: 3, clazz: 'success', type_performer: 1, },
                        { id: failNode, process_id: new_process_id, x: 800, y: 320, label: 'Thất bại', stt: 3, clazz: 'fail', type_performer: 1 }
                    ],
                    edges: [
                        { id: rand(), process_id: new_process_id, source: startNode1, target: taskNode1, sourceAnchor: 1, targetAnchor: 3, clazz: 'flow' },
                        { id: rand(), process_id: new_process_id, source: taskNode1, target: taskNode2, sourceAnchor: 1, targetAnchor: 3, clazz: 'flow', label: 'Gửi đi' },
                        { id: rand(), process_id: new_process_id, source: taskNode2, target: successNode, sourceAnchor: 1, targetAnchor: 3, clazz: 'flow', label: 'Đồng ý' },
                        { id: rand(), process_id: new_process_id, source: taskNode2, target: failNode, sourceAnchor: 2, targetAnchor: 2, clazz: 'flow', label: 'Không đồng ý', reverse: true },
                    ],
                    model: {
                        id: new_process_id,
                        group_id: 1,
                        name: "",
                    },
                },
                departments: [],
                users: [],
                groups: [],

            }
        },
        computed: {
            data() {
                return store.state.data;
            }
        },
        beforeCreate() {
            var that = this;
        },
        mounted() {

            var that = this;
            if (process_id) {
                store.dispatch('init');
            } else {
                process_id = new_process_id;
                store.commit("SET_DATA", this.demoData);
            }
            /// Lấy department
            $.ajax({
                url: path + "/admin/api/department",
                success: function (data) {
                    that.departments = data;
                }
            })
            /// Lấy users
            $.ajax({
                url: path + "/admin/api/employee",
                success: function (data) {
                    that.users = data;
                }
            })
            /// Lấy groups
            $.ajax({
                url: path + "/admin/api/processgroup",
                success: function (data) {
                    that.groups = data;
                }
            })
        },

        methods: {
            save_data: function (e) {
                e.preventDefault();

                if (this.vaild()) {
                    var item = this.data.model;
                    var nodes = this.data.nodes;
                    var edges = this.data.edges;
                    nodes = nodes.map(function (d) {
                        delete d.type;
                        delete d.shape;
                        delete d.size;
                        delete d.style;
                        return d;
                    });
                    edges = edges.map(function (d) {
                        delete d.sourceNode;
                        delete d.targetNode;
                        delete d.endPoint;
                        delete d.startPoint;
                        delete d.style;
                        return d;
                    });
                    item = $.extendext(true, 'replace', {}, this.data.model, { blocks: nodes, links: edges });
                    //console.log(item);
                    $(".preloader").fadeIn();
                    $.ajax({
                        url: path + "/admin/api/saveprocess",
                        type: "POST",
                        data: item,
                        success(data) {
                            location.href = path + "/admin/process"
                        }
                    });
                }

            },
            vaild() {
                var model = this.data.model;
                this.errors = [];
                if (!model.name || model.name == '') {
                    this.errors.push("- <b>Tên quy trình</b> chưa nhập!")
                }
                if (!model.group_id || model.group_id == '') {
                    this.errors.push("- <b>Nhóm quy trình</b> chưa chọn!")
                }
                if (this.errors.length) {
                    return false;
                }
                this.errors = [];
                return true;

            }
        },
    }
</script>

<style>
    #app {
        color: #2c3e50;
        border: 1px solid #e1e1e1
    }
</style>
