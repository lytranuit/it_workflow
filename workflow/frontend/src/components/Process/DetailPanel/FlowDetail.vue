<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ $t("sequenceFlow") }}</div>
    <div class="panelBody">
      <DefaultDetail :model="model" :onChange="onChange" :readOnly="readOnly" />
      <div>
        <b class="col-form-label">{{ $t("sequenceFlow.reverse") }}:</b>
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
