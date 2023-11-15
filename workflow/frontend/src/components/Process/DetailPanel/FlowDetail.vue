<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ $t("sequenceFlow") }}</div>
    <div class="panelBody">
      <DefaultDetail :model="model" :onChange="onChange" :readOnly="readOnly" />
      <div class="row ml-3 align-items-center">
        <b class="col-form-label">{{ $t("sequenceFlow.reverse") }}:</b>
        <div class="ml-3">
          <div class="custom-control custom-switch switch-primary">
            <input type="checkbox" class="custom-control-input" id="check-reverse" v-model="model.reverse" />
            <label class="custom-control-label" for="check-reverse"></label>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail.vue";
export default {
  components: {
    DefaultDetail,
  },
  props: { 
    model: {
      type: Object,
      default: () => ({}),
    },
    onChange: {
      type: Function,
      default: () => { },
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
