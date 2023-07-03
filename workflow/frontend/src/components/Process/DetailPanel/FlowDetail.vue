<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ i18n["sequenceFlow"] }}</div>
    <div class="panelBody">
      <DefaultDetail :model="model" :onChange="onChange" :readOnly="readOnly" />
      <div>
        <b class="col-form-label">{{ i18n["sequenceFlow.reverse"] }}:</b>
        <Checkbox
          v-model="model.reverse"
          @change="(value) => onChange('reverse', value)"
          :disabled="readOnly"
        />
      </div>
    </div>
  </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail.vue";
import Checkbox from "primevue/checkbox";
export default {
  inject: ["i18n"],
  components: {
    DefaultDetail,
    Checkbox,
  },
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
    onChange: {
      type: Function,
      default: () => {},
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
