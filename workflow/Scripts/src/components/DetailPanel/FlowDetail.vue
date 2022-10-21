<template>
    <div :data-clazz="model.clazz">
        <div class="panelTitle">{{i18n['sequenceFlow']}}</div>
        <div class="panelBody">
            <DefaultDetail :model="model" :onChange="onChange" :readOnly="readOnly" />
            <div class="panelRow">
                <el-checkbox @change="(value) => onChange('reverse', value)"
                             :disabled="readOnly"
                             v-model="model.reverse">{{i18n['sequenceFlow.reverse']}}</el-checkbox>
            </div>
        </div>
    </div>
</template>
<script>
    import DefaultDetail from "./DefaultDetail";
    export default {
        inject: ['i18n'],
        components: {
            DefaultDetail
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
            readOnly: {
                type: Boolean,
                default: false,
            }
        },
    }
</script>
