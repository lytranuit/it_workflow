<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ $t("successEvent") }}</div>
    <div class="panelBody">
      <DefaultDetail :model="model" :onChange="onChange" :readOnly="readOnly" />
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
      default: () => {},
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
