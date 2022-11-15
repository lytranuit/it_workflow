<template>
    <div class="itemPanel" :style="{'height': height+'px'}">
        <el-collapse v-model="activeNames" >
            <el-collapse-item :title="i18n['setting']" name="1">
                <div class="form-group text-left">
                    <div class="col-lg-12 mt-2">
                        <b class="col-form-label">{{i18n['process.name']}}<span class="text-danger">*</span></b>
                        <div class="pt-1">
                            <input class="form-control form-control-sm" type='text' name="name" required="" v-model="model.name" />
                        </div>
                    </div>
                    <div class="col-lg-12 mt-2">
                        <b class="col-form-label">{{i18n['process.group']}}:<span class="text-danger">*</span></b>
                        <div class="pt-1">
                            <treeselect v-model="model.group_id" :options="groups" append-to-body/>
                        </div>
                    </div>
                    <div class="col-lg-12 mt-2">
                        <b class="col-form-label">{{i18n['process.description']}}:</b>
                        <div class="pt-1">
                            <textarea id="description" class="form-control form-control-sm" v-model="model.description"></textarea>
                        </div>
                    </div>
                </div>
            </el-collapse-item>
            <el-collapse-item :title="i18n['start']" name="2">
                <img data-item="{clazz:'start',size:'30*30',label:''}"
                     :src="require('../assets/flow/start.svg')" style="width:42px;height:42px" />
                <div>{{i18n['startEvent']}}</div>
                <img data-item="{clazz:'timerStart',size:'30*30',label:''}"
                     :src="require('../assets/flow/timer-start.svg')" style="width:42px;height:42px" />
                <div>{{i18n['timerEvent']}}</div>
            </el-collapse-item>
            <el-collapse-item :title="i18n['task']" name="3">
                <img data-item="{clazz:'formTask',size:'80*44',label:''}"
                     :src="require('../assets/flow/script-task.svg')" style="width:80px;height:44px" />
                <div>{{i18n['formTask']}}</div>
                <img data-item="{clazz:'approveTask',size:'48*60',label:''}"
                     :src="require('../assets/flow/approve.svg')" style="width: 44px; height: 44px" />
                <div>{{i18n['approveTask']}}</div>
            </el-collapse-item>
            <el-collapse-item :title="i18n['gateway']" name="4">
                <img data-item="{clazz:'exclusiveGateway',size:'40*40',label:''}"
                     :src="require('../assets/flow/exclusive-gateway.svg')" style="width:48px;height:48px" />
                <div>{{i18n['exclusiveGateway']}}</div>
                <img data-item="{clazz:'parallelGateway',size:'40*40',label:''}"
                     :src="require('../assets/flow/parallel-gateway.svg')" style="width:48px;height:48px" />
                <div>{{i18n['parallelGateway']}}</div>
                <img data-item="{clazz:'inclusiveGateway',size:'40*40',label:''}"
                     :src="require('../assets/flow/inclusive-gateway.svg')" style="width:48px;height:48px" />
                <div>{{i18n['inclusiveGateway']}}</div>
            </el-collapse-item>
            <el-collapse-item :title="i18n['end']" name="5">
                <img data-item="{clazz:'fail',size:'30*30',label:''}"
                     :src="require('../assets/flow/end.svg')" style="width:42px;height:42px" />
                <div>{{i18n['failEvent']}}</div>
                <img data-item="{clazz:'success',size:'30*30',label:''}"
                     :src="require('../assets/flow/success.png')" style="width:42px;height:42px" />
                <div>{{i18n['successEvent']}}</div>
            </el-collapse-item>
        </el-collapse>
    </div>
</template>
<template>
    <div class="itemPanel" :style="{'height': height+'px'}">
        <el-collapse v-model="activeNames" >
            <el-collapse-item :title="i18n['setting']" name="1">
                <div class="form-group text-left">
                    <div class="col-lg-12 mt-2">
                        <b class="col-form-label">{{i18n['process.name']}}<span class="text-danger">*</span></b>
                        <div class="pt-1">
                            <input class="form-control form-control-sm" type='text' name="name" required="" v-model="model.name" />
                        </div>
                    </div>
                    <div class="col-lg-12 mt-2">
                        <b class="col-form-label">{{i18n['process.group']}}:<span class="text-danger">*</span></b>
                        <div class="pt-1">
                            <treeselect v-model="model.group_id" :options="groups" append-to-body/>
                        </div>
                    </div>
                    <div class="col-lg-12 mt-2">
                        <b class="col-form-label">{{i18n['process.description']}}:</b>
                        <div class="pt-1">
                            <textarea id="description" class="form-control form-control-sm" v-model="model.description"></textarea>
                        </div>
                    </div>
                </div>
            </el-collapse-item>
            <el-collapse-item :title="i18n['start']" name="2">
                <img data-item="{clazz:'start',size:'30*30',label:''}"
                     :src="require('../assets/flow/start.svg')" style="width:42px;height:42px" />
                <div>{{i18n['startEvent']}}</div>
                <img data-item="{clazz:'timerStart',size:'30*30',label:''}"
                     :src="require('../assets/flow/timer-start.svg')" style="width:42px;height:42px" />
                <div>{{i18n['timerEvent']}}</div>
            </el-collapse-item>
            <el-collapse-item :title="i18n['task']" name="3">
                <img data-item="{clazz:'formTask',size:'80*44',label:''}"
                     :src="require('../assets/flow/script-task.svg')" style="width:80px;height:44px" />
                <div>{{i18n['formTask']}}</div>
                <img data-item="{clazz:'approveTask',size:'48*60',label:''}"
                     :src="require('../assets/flow/approve.svg')" style="width: 44px; height: 44px" />
                <div>{{i18n['approveTask']}}</div>
            </el-collapse-item>
            <el-collapse-item :title="i18n['gateway']" name="4">
                <img data-item="{clazz:'exclusiveGateway',size:'40*40',label:''}"
                     :src="require('../assets/flow/exclusive-gateway.svg')" style="width:48px;height:48px" />
                <div>{{i18n['exclusiveGateway']}}</div>
                <img data-item="{clazz:'parallelGateway',size:'40*40',label:''}"
                     :src="require('../assets/flow/parallel-gateway.svg')" style="width:48px;height:48px" />
                <div>{{i18n['parallelGateway']}}</div>
                <img data-item="{clazz:'inclusiveGateway',size:'40*40',label:''}"
                     :src="require('../assets/flow/inclusive-gateway.svg')" style="width:48px;height:48px" />
                <div>{{i18n['inclusiveGateway']}}</div>
            </el-collapse-item>
            <el-collapse-item :title="i18n['end']" name="5">
                <img data-item="{clazz:'fail',size:'30*30',label:''}"
                     :src="require('../assets/flow/end.svg')" style="width:42px;height:42px" />
                <div>{{i18n['failEvent']}}</div>
                <img data-item="{clazz:'success',size:'30*30',label:''}"
                     :src="require('../assets/flow/success.png')" style="width:42px;height:42px" />
                <div>{{i18n['successEvent']}}</div>
            </el-collapse-item>
        </el-collapse>
    </div>
</template>
<script>
    export default {
        inject: ['i18n'],
        components: {
        },
        props: {
            height: {
                type: Number,
                default: 800,
            },
            model: {
                type: Object,
                default: () => ({}),
            },
            groups: {
                type: Array,
                default: () => ([]),
            },
        },
        data() {
            return {
                activeNames: ['1'],
            };
        },
    }
</script>

<style lang="scss" >
    .itemPanel {
        float: left;
        width: 15%;
        background: #f0f2f5;
        overflow-y: auto;
        border-left: 1px solid #E9E9E9;
        border-bottom: 1px solid #E9E9E9;
        img{
            width: 92px;
            height: 96px;
            padding: 4px;
            border: 1px solid rgba(0,0,0,0);
            border-radius: 2px;
            &:hover{
                border: 1px solid #ccc;
                cursor: move;
            }
        }
        .el-collapse {
            border: 0;
            .el-collapse-item {
                > div[role=tab] > div {
                    padding-left: 10px;
                    border: 1px solid #E9E9E9;
                    border-left:0;
                }
                &:first-child{
                    > div[role=tab] > div {
                        border-top: 0;
                    }
                }
                &:last-child{
                    > div[role=tab] > div {
                        border-bottom: 1px solid #E9E9E9;
                    }
                }
                .el-collapse-item__wrap{
                    border-top: 0;
                    background: #f0f2f5;
                    text-align: center;
                }
            }
        }

    }
</style>
