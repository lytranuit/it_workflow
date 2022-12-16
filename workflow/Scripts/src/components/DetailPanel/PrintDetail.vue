<template>

    <div :data-clazz="model.clazz">
        <div class="panelTitle">{{i18n['print']}}</div>
        <div class="panelBody">
            <el-collapse v-model="activeName">
                <el-collapse-item :title="i18n['detail.general']" name="1">
                    <DefaultDetail :model="model" :onChange="onChange" />
                </el-collapse-item>
                <el-collapse-item :title="i18n['detail.template']" name="2">
                    <div class="form-group">
                        <div class="col-12 mt-2">
                            <b class="col-form-label">Mẫu：</b>
                            <div class="pt-1 flex-m" v-if="file_template">
                                <div class="file-icon" data-type="docx"></div>
                                <a :href="file_template.url" :download="file_template.name" style="margin-left: 5px;">
                                    {{file_template.name}}
                                </a>
                            </div>
                        </div>
                        <div class="text-center mt-3">
                            <button class="btn btn-success btn-sm" @click="open_select_file()">
                                <i class="fas fa-plus mr-1"></i>
                                Upload
                            </button>
                            <input type="file" class="d-none" ref="file" accept=".docx" @change="upload_file()" />
                        </div>
                    </div>

                </el-collapse-item>
            </el-collapse>
        </div>
    </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail";
import {
        DxHtmlEditor,
        DxToolbar,
        DxItem,
        DxVariables
} from 'devextreme-vue/html-editor';
export default {
        inject: ['i18n'],
        components: {
            DefaultDetail,
            DxHtmlEditor,
            DxItem,
            DxToolbar,
            DxVariables,
        },
        data() {
            return {
                activeName: '1',

            }
        },
        props: {
            model: {
                type: Object,
                default: () => ({}),
            },
            onChange: {
                type: Function,
                default: () => { }
            },
            nodes: {
                type: Array,
                default: () => ([]),
            },
        },
        computed: {
            file_template() {
                return this.model.data_setting.file_template;
            }
        },
        mounted() {

        },
        methods: {
            open_select_file() {
                $(this.$refs['file']).trigger("click");
            },
            async upload_file() {
                var files = $(this.$refs['file'])[0].files;
                var formData = new FormData();
                formData.append("files", files[0]);
                var resp = await $.ajax({
                    type: "POST",
                    url: path + "/admin/api/uploadFileTemplate",
                    data: formData,
                    dataType: "json",
                    contentType: false, // Not to set any content header
                    processData: false, // Not to process data
                });
                this.model.data_setting.file_template = resp.item;

                var label = this.model.label
                this.model.label = rand();
                this.model.label = label;
            }
        }
}
</script>
<style>
</style>