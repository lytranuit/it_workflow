<template>
    <div id="app">
        <div class="alert alert-danger alert-dismissible fade show" role="alert" v-show="errors.length > 0">
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true"><i class="mdi mdi-close"></i></span>
            </button>
            <div v-for="error in errors" v-html="error"></div>
        </div>
        <button type="button" class="el-button el-button--default el-button--small" style="float: right;margin-top: 6px;margin-right: 6px;" @click="save_data"><span>Lưu lại</span></button>
        <wfd-vue ref="wfd" :data="data" :departments="departments" :users="users" :groups="groups" :height="600" :lang="lang" />

    </div>
</template>

<script>
    var startNode1 = rand();
    var taskNode1 = rand();
    var taskNode2 = rand();
    var successNode = rand();
    var failNode = rand();
    var process_id = rand();
    import WfdVue from '../src/components/Wfd'
    export default {
        name: 'app',
        components: {
            WfdVue
        },
        data() {
            return {
                modalVisible: false,
                lang: "vi",
                errors: [],
                data: {
                    nodes: [
                        { id: startNode1, process_id: process_id, x: 50, y: 200, label: 'Bắt đầu', clazz: 'start', type_performer: 1, },
                        { id: taskNode1, process_id: process_id, x: 300, y: 200, label: 'Biểu mẫu', clazz: 'formTask', type_performer: 1, data_setting: {}, fields: [] },
                        { id: taskNode2, process_id: process_id, x: 500, y: 200, label: 'Phê duyệt', clazz: 'approveTask', type_performer: 1, data_setting: {} },
                        { id: successNode, process_id: process_id, x: 800, y: 200, label: 'Hoàn thành', clazz: 'success', type_performer: 1, },
                        { id: failNode, process_id: process_id, x: 800, y: 320, label: 'Thất bại', clazz: 'fail', type_performer: 1 }
                    ],
                    edges: [
                        { id: rand(), process_id: process_id, source: startNode1, target: taskNode1, sourceAnchor: 1, targetAnchor: 3, clazz: 'flow' },
                        { id: rand(), process_id: process_id, source: taskNode1, target: taskNode2, sourceAnchor: 1, targetAnchor: 3, clazz: 'flow' },
                        { id: rand(), process_id: process_id, source: taskNode2, target: successNode, sourceAnchor: 1, targetAnchor: 3, clazz: 'flow', label: 'yes' },
                        { id: rand(), process_id: process_id, source: taskNode2, target: failNode, sourceAnchor: 2, targetAnchor: 2, clazz: 'flow', label: 'no', reverse: true },
                    ],
                    model: {
                        id: process_id,
                        group_id: 1,
                        name: "",
                    },
                },
                departments: [],
                users: [],
                groups: []
            }
        },

        mounted() {
            if (process_id) {
                console.log(process_id);
                /// Lấy department
                $.ajax({
                    url: path + "/admin/process/get",
                    data: { id: process_id },
                    success: function (data) {
                        that.data.model = data;
                        that.data.edges = data.links;
                        var blocks = data.blocks.map(function (item) {
                            var default_setting = { data_setting: {} };
                            item = $.extendext(true, 'replace', default_setting, item);
                            return item;
                        });
                        that.data.nodes = blocks;
                    }
                })
            } else {

            }
            var that = this;
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

                    item = $.extendext(true, 'replace', {}, this.data.model, { blocks: nodes, links: edges });

                    $.ajax({
                        url: path + "/admin/process/save",
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
