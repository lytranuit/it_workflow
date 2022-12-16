<template>

    <div :data-clazz="model.clazz">
        <div class="panelTitle">{{i18n['mail']}}</div>
        <div class="panelBody">
            <el-collapse v-model="activeName">
                <el-collapse-item :title="i18n['detail.general']" name="1">
                    <DefaultDetail :model="model" :onChange="onChange" />
                    <div class="text-center mt-3">
                        <button class="btn btn-success btn-sm" type="button" data-toggle="modal" data-target="#myModal1">
                            <i class="fas fa-pencil-alt mr-1"></i>
                            Thiết lập
                        </button>
                    </div>
                    <div id="myModal1" class="modal modal-fullscreen" tabindex="-1" role="dialog" data-backdrop="static">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <b class="modal-title font-16">
                                        {{model.label}}
                                    </b>
                                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </div>
                                <div class="modal-body bg-light">
                                    <div class="row h-100">
                                        <div class="col-9">
                                            <div class="card">
                                                <div class="card-body">
                                                    <b class="col-form-label">Người nhận:<span class="text-danger">*</span></b>
                                                    <div class="pt-1">
                                                        <DxHtmlEditor v-model:value="mail.to" ref="editor_to" :hover-state-enabled="read" @focus-in="out($event,'to')" :mentions="mentions_employee">

                                                        </DxHtmlEditor>
                                                        <!--<input class="form-control form-control-sm" type='text' name="to" required="" v-model="model.data_setting.mail.to" placeholder="Người nhận" autocomplete="off" />-->
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="card">
                                                <div class="card-body">
                                                    <b class="col-form-label">Tiêu đề:<span class="text-danger">*</span></b>
                                                    <div class="pt-1">
                                                        <DxHtmlEditor v-model:value="mail.title" ref="editor_title" :hover-state-enabled="read" @focus-in="out($event,'title')" :mentions="mentions">

                                                        </DxHtmlEditor>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="card mb-0">
                                                <div class="card-body">
                                                    <b class="col-form-label">Nội dung:<span class="text-danger">*</span></b>
                                                    <div class="pt-1">
                                                        <DxHtmlEditor v-model:value="mail.content" ref="editor_content" :hover-state-enabled="read" @focus-in="out($event,'content')" :mentions="mentions" style="min-height: 370px;">
                                                            <DxToolbar>
                                                                <DxItem name="undo" />
                                                                <DxItem name="redo" />
                                                                <DxItem name="separator" />
                                                                <DxItem name="bold" />
                                                                <DxItem name="italic" />
                                                                <DxItem name="strike" />
                                                                <DxItem name="underline" />
                                                                <DxItem name="separator" />
                                                                <DxItem name="alignLeft" />
                                                                <DxItem name="alignCenter" />
                                                                <DxItem name="alignRight" />
                                                                <DxItem name="alignJustify" />
                                                                <DxItem name="separator" />
                                                            </DxToolbar>
                                                        </DxHtmlEditor>
                                                    </div>
                                                    <div class="row pt-3">
                                                        <b class="col-form-label col-2">File đính kèm:</b>
                                                        <div class="col-10">
                                                            <DxHtmlEditor v-model:value="mail.filecontent" ref="editor_filecontent" :hover-state-enabled="read" @focus-in="out($event,'filecontent')" :mentions="mentions_file">

                                                            </DxHtmlEditor>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-3">
                                            <div class="card h-100">
                                                <div class="card-body">
                                                    <div v-for="[Key, Value] in groups()">
                                                        <b>{{Key}}</b>
                                                        <div class="ml-3">
                                                            <a class="d-block" href="#" v-for="field in Value" @click="add_field(field)">
                                                                {{field.text}}
                                                            </a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-primary" @click="save_settings()">Lưu lại</button>
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy bỏ</button>
                                </div>
                            </div>
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
                read: true,
                mail: {},
                //variables: ['FirstName', 'LastName', 'Company'],
                //escapeCharacters: ['##', '##'],
                data: [],
                mentions: [],
                mentions_employee: [],
                mentions_file: [],
                focus: 'to' // to,title,content,filecontent
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

        },
        mounted() {
            this.initEvents();
            var prev_nodes = this.prev_nodes();
            this.data = [
                {
                    text: 'ID lượt chạy',
                    type: "number",
                    group: "Trường mặc định",
                    id: "id"
                }, {
                    text: 'Tiêu đề lượt chạy',
                    type: "text",
                    group: "Trường mặc định",
                    id: "title"
                }, {
                    text: 'Người tạo lượt chạy',
                    type: "employee",
                    group: "Trường mặc định",
                    id: "created_by_name"
                }, {
                    text: 'Ngày tạo lượt chạy',
                    type: "date",
                    group: "Trường mặc định",
                    id: "created_at"
                }
            ];
            for (var node of prev_nodes) {
                var fields = node.fields || [];
                var label = node.label;
                this.data.push({
                    text: "Người thực hiện " + label,
                    type: "employee",
                    group: label,
                    id: "created_by_name_" + node.id
                })
                this.data.push({
                    text: "Ngày thực hiện " + label,
                    type: "date",
                    group: label,
                    id: "created_at_" + node.id
                })
                for (var field of fields) {
                    this.data.push({
                        text: field.name,
                        type: field.type,
                        group: label,
                        id: field.variable
                    })
                }
            }
            //this.variables = this.data.map(function (item) {
            //    return item.id;
            //});
            //console.log(this.data);
            this.mentions = [{
                dataSource: this.data,
                searchExpr: 'text',
                displayExpr: 'text',
                valueExpr: 'id',
                marker: "#",
            }];
            this.mentions_employee = [{
                dataSource: this.data.filter(function (item) {
                    return item.type == 'employee' || item.type == 'employee_multiple'
                }),
                searchExpr: 'text',
                displayExpr: 'text',
                valueExpr: 'id',
                marker: "#",
            }];
            this.mentions_file = [{
                dataSource: this.data.filter(function (item) {
                    return item.type == 'file' || item.type == 'file_multiple'
                }),
                searchExpr: 'text',
                displayExpr: 'text',
                valueExpr: 'text',
                marker: "#",
            }];

            var mail = $.extendext(true, 'replace', { to: "!#created_by_name#", title: "", content: "", filecontent: "" }, this.model.data_setting.mail);
            for (var d of this.data) {
                var id = d.id;
                var html = `<span class="dx-mention" spellcheck="false" data-marker="#" data-mention-value="` + d.text + `" data-id="` + d.id + `">﻿<span contenteditable="false"><span>#</span>` + d.text + `</span>﻿</span>`;
                mail.content = mail.content.replace(new RegExp("!#" + id + "#", "g"), html);
                mail.to = mail.to.replace(new RegExp("!#" + id + "#", "g"), html);
                mail.title = mail.title.replace(new RegExp("!#" + id + "#", "g"), html);
                mail.filecontent = mail.filecontent.replace(new RegExp("!#" + id + "#", "g"), html);
            }
            this.mail = mail;
        },
        methods: {
            prev_nodes() {
                var nodes = this.nodes;
                var model = this.model;
                return nodes.filter(function (item) {
                    return item.stt < model.stt;
                })
            },
            initEvents() {
                var that = this;
                //$(this.$refs['editor_to'].$el).on("focusin", function () {
                //    that.focus = 'to';
                //});
            },
            out(event, tab) {
                this.focus = tab;

                //this.read = false;
            },
            groups() {
                var dataSource = this.data;
                if (this.focus == 'to') {
                    dataSource = dataSource.filter(function (item) {
                        return item.type == 'employee' || item.type == 'employee_multiple'
                    })
                } else if (this.focus == 'filecontent') {
                    dataSource = dataSource.filter(function (item) {
                        return item.type == 'file' || item.type == 'file_multiple'
                    })
                }
                var groups = this.groupBy(dataSource, item => item.group);
                return groups;
            },

            groupBy(list, keyGetter) {
                const map = new Map();
                list.forEach((item) => {
                    const key = keyGetter(item);
                    const collection = map.get(key);
                    if (!collection) {
                        map.set(key, [item]);
                    } else {
                        collection.push(item);
                    }
                });
                return map;
            },
            add_field(field) {
                //console.log(field);
                var editor_select = this.$refs['editor_' + this.focus].instance;
                //console.log(editor_select);
                //var mail = this.mail;
                var select = editor_select.getSelection();
                //console.log(select);
                editor_select.insertEmbed(select.index, "mention", {
                    marker: "#",
                    id: field.id,
                    value: field.text
                });
                editor_select.insertText(select.index + 1, " ");
                editor_select.focus();
                editor_select.setSelection(select.index + 2)
            },
            save_settings() {
                this.model.data_setting.mail = $.extendext(true, 'replace', {}, this.mail);

                var mail = this.model.data_setting.mail;
                var firstvariable = "!#"; //first input;
                var secondvariable = "#"; //first in
                ////content
                var $content = $("<div>" + mail.content + "</div>");
                var mention = $(".dx-mention", $content);
                mention.replaceWith(function () {
                    var id = $(this).data("id");
                    return firstvariable + id + secondvariable
                });

                mail.content = $content.html();
                /// to
                var $content = $("<div>" + mail.to + "</div>");
                var mention = $(".dx-mention", $content);
                mention.replaceWith(function () {
                    var id = $(this).data("id");
                    return firstvariable + id + secondvariable
                });
                mail.to = $content.text();
                //// title
                var $content = $("<div>" + mail.title + "</div>");
                var mention = $(".dx-mention", $content);
                mention.replaceWith(function () {
                    var id = $(this).data("id");
                    return firstvariable + id + secondvariable
                });
                mail.title = $content.text();
                /// file
                var $content = $("<div>" + mail.filecontent + "</div>");
                var mention = $(".dx-mention", $content);
                mention.replaceWith(function () {
                    var id = $(this).data("id");
                    return firstvariable + id + secondvariable
                });
                mail.filecontent = $content.text();

                ///
                $("#myModal1").modal("hide");


                //var list = mail.content.match(new RegExp("(?<=" + firstvariable + ")(.*?)(?=" + secondvariable + ")", "g"));
                //var list = mail.content.replace(new RegExp("!#id#", "g"), "tran");
                //console.log(list);
            }
        }
    }
</script>
<style>
</style>