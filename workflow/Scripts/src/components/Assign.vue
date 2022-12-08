<template>
    <div id="popup-assign">
        <div id="myModal-assign" class="modal" tabindex="-1" role="dialog" :data-backdrop="checkModal()">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Phân công</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close" v-if="!required">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <form id="form-assign" class="col-12">
                                <el-collapse v-model="activeName">
                                    <el-collapse-item :title="element.block.label" :name="index" v-for="(element,index) in data_custom_block">
                                        <div class="row bg-white">
                                            <div class="col-12">Người thực hiện:</div>
                                            <div class="col-3">
                                                <select class="form-control" :name="'sel_' + index" v-model="element.data_setting.type_performer" required>
                                                    <option value="3">Bộ phận</option>
                                                    <option value="4">Người dùng</option>
                                                </select>
                                            </div>
                                            <div class="col-9">
                                                <div v-if="element.data_setting.type_performer == 4">
                                                    <treeselect multiple v-model="element.data_setting.listuser" :options="users" :name="'user_' + index" required />
                                                </div>
                                                <div v-if="element.data_setting.type_performer == 3">
                                                    <treeselect multiple v-model="element.data_setting.listdepartment" :options="departments" :name="'dep_' + index" required />
                                                </div>
                                            </div>
                                        </div>
                                    </el-collapse-item>
                                </el-collapse>
                            </form>
                        </div>
                    </div>
                    <div class="modal-footer justify-content-center">
                        <button type="button" class="btn btn-success" @click="save()">Lưu lại</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        components: {
        },
        props: {
            data_custom_block: {
                type: Array,
                default: () => ([]),
            },
            users: {
                type: Array,
                default: () => ([]),
            },
            departments: {
                type: Array,
                default: () => ([]),
            },
            required: {
                type: Boolean,
                default: () => (true),
            },
        },
        data() {
            return {
                //activeName: [0, 1, 2],
            }
        },
        computed: {
            activeName() {
                return this.data_custom_block.map(function (item, key) {
                    return key;
                })
            }
        },
        mounted() {

            var that = this;
            $("#myModal-assign").modal("show");
            $('#myModal-assign').on('hidden.bs.modal', function (e) {
                that.$emit("close");
            })
        },
        methods: {
            save() {
                var vaild = $("#form-assign").valid();
                if (!vaild) {
                    return;
                }
                ////VAILD TIEU DE
                if (!$(".tieu_de").val()) {
                    alert("Bạn chưa nhập tiêu đề!");
                    $(".tieu_de").focus();
                    return;
                }
                this.$emit("save_data");
            },
            checkModal() {
                var text = 'static';
                if (!this.required)
                    text = false;
                return text
            }
        }
    }
</script>

<style lang="scss"></style>
