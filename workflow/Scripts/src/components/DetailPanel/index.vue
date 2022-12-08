<template>
    <div class="detailPanel" :style="{'height':height+'px'}">
        <FormTaskDetail v-if="model.clazz === 'formTask'" :model="model" :onChange="onChange" :readOnly="readOnly" :users="users" :departments="departments" :nodes="nodes" />
        <GatewayDetail v-else-if="model.clazz === 'gateway' || model.clazz === 'exclusiveGateway' || model.clazz === 'parallelGateway' || model.clazz === 'inclusiveGateway'" :model="model" :onChange="onChange" :readOnly="readOnly" />
        <FlowDetail v-else-if="model.clazz === 'flow'" :model="model" :onChange="onChange" :readOnly="readOnly" />
        <StartEventDetail v-else-if="model.clazz === 'start'" :model="model" :onChange="onChange" :readOnly="readOnly" />
        <FailEventDetail v-else-if="model.clazz === 'fail'" :model="model" :onChange="onChange" :readOnly="readOnly" />
        <SuccessEventDetail v-else-if="model.clazz === 'success'" :model="model" :onChange="onChange" :readOnly="readOnly" />
        <ApproveTaskDetail v-else-if="model.clazz === 'approveTask'" :model="model" :onChange="onChange" :readOnly="readOnly" :nodes="nodes" />
        <MailDetail v-else-if="model.clazz === 'mailSystem'" :model="model" :onChange="onChange" :readOnly="readOnly" :nodes="nodes" />
    </div>
</template>
<script>
    import StartEventDetail from "./StartEventDetail"
    import FailEventDetail from "./FailEventDetail"
    import SuccessEventDetail from "./SuccessEventDetail"

    import ApproveTaskDetail from "./ApproveTaskDetail"
    import FormTaskDetail from "./FormTaskDetail"
    import TimerEventDetail from "./TimerEventDetail"

    import GatewayDetail from "./GatewayDetail"

    import FlowDetail from "./FlowDetail"

    import MailDetail from "./MailDetail"


    export default {
        inject: ['i18n'],
        components: {
            FormTaskDetail,
            TimerEventDetail,
            GatewayDetail,
            FlowDetail,
            StartEventDetail,
            FailEventDetail,
            SuccessEventDetail,
            ApproveTaskDetail,
            MailDetail,
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
<style lang="scss">
    .detailPanel {
        height: 100%;
        background: #f0f2f5;
        flex: 0 0 auto;
        float: left;
        width: 20%;
        border-right: 1px solid #E9E9E9;
        border-bottom: 1px solid #E9E9E9;
        border-left: 1px solid #E9E9E9;
        overflow: auto;

        .panelTitle {
            text-align: left;
            color: #000;
            background: #EBEEF2;
            border-bottom: 1px solid #DCE3E8;
            font-size: 16px;
            padding: 10px;
        }

        .panelBody {
            .panelRow {
                text-align: left;
                display: inline-block;
                font-size: 12px;
                width: 100%;
                padding: 5px 12px;
            }
        }
    }
</style>
