<template>
    <div :data-clazz="model.clazz">
        <div class="panelTitle">{{i18n['formTask']}}</div>
        <div class="panelBody">
            <el-collapse v-model="activeName">
                <el-collapse-item :title="i18n['detail.general']" name="1">
                    <DefaultDetail :model="model" :onChange="onChange" :readOnly="readOnly" />
                </el-collapse-item>
                <el-collapse-item :title="i18n['detail.user_performer']" name="2">
                    <div class="p-3">
                        <div class="my-2">
                            <b>Người thực hiện:</b>
                            <select class="form-control" v-model="model.type_performer">
                                <option value="1">Người thực hiện bước trước</option>
                                <option value="2">Người thực hiện bước trước tự chọn</option>
                                <option value="3">Bộ phận</option>
                                <option value="4">Người dùng</option>
                                <option value="5">Người khởi tạo</option>
                            </select>
                        </div>
                        <div class="my-2" v-if="model.type_performer == 1 || model.type_performer == 2">
                            <b>Bước:</b>
                            <treeselect append-to-body :options="nodes" v-model="model.data_setting.block_id"></treeselect>
                        </div>
                        <div class="my-2" v-if="model.type_performer == 4">
                            <b>Người dùng:</b>
                            <treeselect multiple v-model="model.data_setting.listuser" :options="users" append-to-body />
                        </div>
                        <div class="my-2" v-if="model.type_performer == 3">
                            <b>Bộ phận:</b>
                            <treeselect multiple v-model="model.data_setting.listdepartment" :options="departments" append-to-body />
                        </div>
                        <SettingMail :model="model" :nodes="nodes"></SettingMail>
                    </div>
                </el-collapse-item>
                <el-collapse-item :title="i18n['detail.time']" name="3">
                    <div class="p-3">

                        <div class="flex-m m-2">
                            <div class="">Thời hạn xử lý</div>
                            <div class="ml-auto">
                                <div class="custom-control custom-switch switch-primary">
                                    <input type="checkbox" class="custom-control-input" id="check" v-model="model.has_deadline">
                                    <label class="custom-control-label" for="check"></label>
                                </div>
                            </div>
                        </div>
                        <div class="flex-m m-2">
                            <div class="">Bước có hạn xử lý sau:</div>
                        </div>
                        <div class="flex-m">
                            <div class="mx-2">
                                <b>Ngày</b>
                                <input class="form-control" v-model="model.data_setting.days" :disabled="!model.has_deadline" />
                            </div>
                            <div class="mx-2">
                                <b>Giờ</b>
                                <input class="form-control" v-model="model.data_setting.hours" :disabled="!model.has_deadline" />
                            </div>
                            <div class="mx-2">
                                <b>Phút</b>
                                <input class="form-control" v-model="model.data_setting.minutes" :disabled="!model.has_deadline" />
                            </div>
                        </div>
                    </div>
                </el-collapse-item>
                <el-collapse-item :title="i18n['detail.fields']" name="4">
                    <setting-field :model="model" :users="users"
                                   :departments="departments"></setting-field>
                </el-collapse-item>
            </el-collapse>

        </div>
    </div>
</template>
<script>
    import DefaultDetail from "./DefaultDetail";
    import SettingField from "./Fields/SettingField";
    import SettingMail from "./SettingMail";
    export default {
        inject: ['i18n'],
        components: {
            DefaultDetail,
            SettingField,
            SettingMail
        },
        data() {
            return {
                activeName: '1',
            }
        },
        methods: {

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
            onChange: {
                type: Function,
                default: () => { }
            },
            readOnly: {
                type: Boolean,
                default: false,
            }
        },
    }
</script>
