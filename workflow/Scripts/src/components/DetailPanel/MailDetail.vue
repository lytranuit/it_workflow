<template>

    <div :data-clazz="model.clazz">
        <div class="panelTitle">{{i18n['mail']}}</div>
        <div class="panelBody">
            <el-collapse v-model="activeName">
                <el-collapse-item :title="i18n['detail.general']" name="1">
                    <DefaultDetail :model="model" :onChange="onChange" />
                    <SettingMail :model="model" :nodes="nodes"></SettingMail>
                </el-collapse-item>
            </el-collapse>
        </div>
    </div>
</template>
<script>
    import DefaultDetail from "./DefaultDetail";
    import SettingMail from "./SettingMail";
    export default {
        inject: ['i18n'],
        components: {
            SettingMail,
            DefaultDetail
        },
        data() {
            return {
                activeName: '1'
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
        },
        methods: {

        }
    }
</script>
<style>
</style>