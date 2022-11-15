<template>
    <div>
        <div class="form-group">
            <div class="col-lg-12 mt-2">
                <b class="col-form-label">{{i18n['label']}}:<span class="text-danger">*</span></b>
                <div class="pt-1">
                    <input class="form-control form-control-sm" type='text'
                           v-model="model.label"
                           :disabled="readOnly"
                           @input="(value) => {onChange('label', value)}" />
                </div>
            </div>
            <div class="col-lg-12 mt-2">
                <b class="col-form-label">{{i18n['guide']}}：</b>
                <div class="pt-1">
                    <textarea class="form-control form-control-sm"
                              :disabled="readOnly"
                              v-model="model.guide"
                              @input="(value) => {onChange('guide', value)}"></textarea>
                </div>
            </div>
            <div class="col-lg-12 mt-2">
                <b class="col-form-label">{{i18n['stt']}}:</b>
                <div class="pt-1">
                    <input class="form-control form-control-sm" type='number'
                           v-model="model.stt"
                           :disabled="readOnly"
                           @input="(value) => {onChange('stt', value)}" />
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        inject: ['i18n'],
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
