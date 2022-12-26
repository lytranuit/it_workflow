<template>
    <form id="sidebar-right" :class="{'centered':isPopup == true}">
        <div class="header">
            <div class="flex-m">
                <div>
                    <div class="tilte">{{model.label}}</div>
                    <div class="flex-m" v-if="html != ''">
                        <div class=" flex-m"> Người thực hiện: <span v-html="html" class="ml-1"></span></div>
                    </div>
                </div>
                <div class="control ml-auto flex-m">
                    <div class="dropdown mr-3">
                        <a class="dropdown-toggle btn btn-outline-info" id="drop2" data-toggle="dropdown" href="#" role="button" aria-haspopup="false" aria-expanded="false">
                            <i class="fas fa-ellipsis-v text-muted"></i>
                        </a>
                        <div class="dropdown-menu" aria-labelledby="drop2" style="z-index:1999">
                            <a class="nav-link cursor-pointer flex-m items-center font-13" href="#" @click="assign_again(model.block_id)" v-if="model.blocking && hasPermission()">
                                <i class="fas fa-share font-16"></i>
                                <div class="ml-2"> Phân công lại</div>
                            </a>
                            <a class="nav-link cursor-pointer flex-m items-center font-13" href="#" @click="sign(model.block_id)" v-if="model.blocking && hasPermission() && model.clazz == 'approveTask'">
                                <i class="fas fa-signature"></i>
                                <div class="ml-2"> Yêu cầu ký nháy</div>
                            </a>
                            <a class="nav-link cursor-pointer flex-m items-center font-13" href="#" v-if="isPopup == false" @click="setIsPopup(true)">
                                <i class="fas fa-expand font-16"></i>
                                <div class="ml-2"> Phóng to </div>
                            </a>
                            <a class="nav-link cursor-pointer flex-m items-center font-13" href="#" v-if="isPopup == true" @click="setIsPopup(false)">
                                <i class="fas fa-location-arrow font-16"></i>
                                <div class="ml-2"> Thu lại </div>
                            </a>
                        </div>
                    </div>
                    <button type="button" aria-label="Close" class="close" @click="close"><span aria-hidden="true">×</span></button>
                </div>
            </div>
            <div class="box_transition" v-if="model.blocking && hasPermission()">
                <button class="mr-2" :class="{'btn-reverse':item.reverse,'btn-next':!item.reverse}" tabindex="1" type="button" name="button" v-for="item in model.outEdges" :key="item.id" @click="execute_transition(model.id,item.id)">
                    {{item.label}}
                </button>
            </div>
            <div class="box_transition" v-if="model.blocking && !hasPermission()">
               <span class="text-danger">Bạn không có quyền thực hiện bước này.</span>
            </div>
        </div>
        <div class="body" v-if="(model.blocking && hasPermission()) || model.executed">
            <FormTask :departments="departments" :users="users" :fields="fields" :readonly="readonly" v-if="model.clazz == 'formTask'"></FormTask>
            <ApproveTask :departments="departments" :users="users" :nodes="nodes" :readonly="readonly" v-if="model.clazz == 'approveTask'" :model="model"></ApproveTask>
            <SuggestTask :departments="departments" :users="users" :nodes="nodes" :readonly="readonly" v-if="model.clazz == 'suggestTask'" :model="model"></SuggestTask>
            <PrintSystem v-if="model.clazz == 'printSystem'" :nodes="nodes" :model="model"></PrintSystem>
        </div>
    </form>
</template>
<script>
    import store from '../../../example/store';
    import FormTask from './FormTask';
    import ApproveTask from './ApproveTask';
    import SuggestTask from './SuggestTask';
    import PrintSystem from './PrintSystem';
    export default {
        inject: ['i18n'],
        components: {
            FormTask,
            ApproveTask,
            SuggestTask,
            PrintSystem
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
        },
        data() {
            return {
                readonly: false,
                isPopup: false,
                html: ""
            }
        },
        watch: {
            model: {
                handler(newData, oldData) {
                    if (oldData !== newData) {
                        this.initHtml();
                        this.readonly = !this.model.blocking;
                    }
                },
                immediate: true,
                deep: true
            }
        },
        computed: {
            fields() {
                return this.model.fields;
            },
            current_user() {
                return store.state.current_user;
            }
        },
        mounted() {
            this.isPopup = JSON.parse(localStorage.getItem('isPopup')) || false;
            //console.log(this.isPopup);
        },
        methods: {
            setIsPopup(isPopup) {
                if (typeof localStorage !== 'undefined') {
                    this.isPopup = isPopup;
                    localStorage.setItem('isPopup', isPopup);
                }
            },
            execute_transition(from_activity_id, edge_id) {
                var that = this;
                that.$emit('execute_transition', from_activity_id, edge_id);
            },
            close() {
                this.$emit("close");
            },
            assign_again(block_id) {
                var that = this;
                that.$emit('assign_again', block_id);
            },
            getAll(departments) {
                var that = this;
                var list = [];
                departments.map(function (item) {
                    list.push(item);
                    var children = item.children || [];
                    children = that.getAll(children);
                    list = [...list, ...children];
                });
                return list;
            },
            initHtml() {
                var that = this;
                var html = "";
                var model = this.model;
                if (model.blocking) {
                    var data_setting = model.data_setting;
                    var type_performer = data_setting.type_performer;
                    if (type_performer == 4) {
                        var listuser = data_setting.listuser;
                        //console.log(data_setting);
                        //console.log(that.users)
                        listuser = listuser.map(function (item) {
                            var findUser = that.users.findLastIndex(function (user) {
                                return user.id == item;
                            })
                            if (findUser == -1)
                                return null;
                            var user = that.users[findUser];
                            return "<b>" + user.name + "</b>";
                        })
                        listuser = listuser.filter(function (item) {
                            return item != null;
                        });
                        if (listuser.length) {
                            html += listuser.join(",");
                        }
                    } else if (type_performer == 3) {
                        var listdepartment = data_setting.listdepartment;
                        var departments = this.getAll(that.departments);
                        listdepartment = listdepartment.map(function (item) {
                            var findDepartment = departments.findLastIndex(function (user) {
                                return user.id == item;
                            })
                            if (findDepartment == -1)
                                return null;
                            var department = departments[findDepartment];
                            return "<b>" + department.label + "</b>";
                        })
                        listdepartment = listdepartment.filter(function (item) {
                            return item != null;
                        });
                        if (listdepartment.length) {
                            html += listdepartment.join(",");
                        }
                    }
                } else {
                    var transitions = store.state.data_transition;
                    var findTransition = transitions.findLastIndex(function (item) {
                        return item.from_activity_id == model.id;
                    });
                    var transition = transitions[findTransition];

                    var created_at = this.model.created_at;
                    var user_created_by = this.model.user_created_by;
                    if (user_created_by) {
                        html += "<b>" + user_created_by.fullName + "</b>";
                    }
                    if (transition && transition.label) {
                        var reverse = transition.reverse;
                        var label = transition.label;
                        var Iclass = "ml-1";
                        if (reverse)
                            Iclass += " text-danger";
                        else
                            Iclass += " text-success";
                        html += "<span class='" + Iclass + "'>đã " + label.toLowerCase() + "</span>";
                    }
                    if (created_at) {
                        html += " lúc <b>" + moment(created_at).format("HH:mm DD/MM/YYYY") + "</b>";
                    }
                }

                this.html = html;
            },
            hasPermission() {
                var data_setting = this.model.data_setting || {};
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
            }
        }
    }
</script>
<style lang="scss" scoped>
    #sidebar-right {
        position: fixed;
        top: 0;
        right: 0;
        width: 600px;
        border: 1px solid #d5d5d5;
        height: 100vh;
        background: #f0f2f5;
        opacity: 1;
        z-index: 1999;
        box-shadow: -3px 3px 10px -4px #a5a5a5;

        &.centered {
            left: 50%;
            top: 50%;
            -webkit-transform: translate(-50%, -50%);
            transform: translate(-50%, -50%);
            width: 100%;
            height: 100%;
        }

        .header {
            padding: 20px 10px 10px;
            border-bottom: 1px solid #d5d5d5;

            .tilte {
                font-weight: bold;
                font-size: 16px;
            }

            .dropdown-menu {
                z-index: 1999;
            }

            .box_transition {
                margin-top: 10px;
                border-top: 1px solid #dddddd;
                padding-top: 10px;
                text-align: center;
            }

            .btn-reverse {
                border-radius: 4px;
                padding: 6px 16px;
                background-color: rgb(255, 255, 255);
                color: rgb(239, 41, 47);
                border: 1px solid rgb(239, 41, 47);
                cursor: pointer;
                font-weight: bold;
            }

            .btn-next {
                border-radius: 4px;
                padding: 6px 16px;
                background-color: #0c9cdd;
                color: white;
                border: 1px solid #0c9cdd;
                cursor: pointer;
                font-weight: bold;
            }
        }

        .body {
            padding: 20px 10px;
            height: calc(100% - 129px);
            overflow: auto;
        }
    }
</style>
