<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">
      {{
        model.clazz === "exclusiveGateway" || model.clazz === "gateway"
          ? $t("exclusiveGateway")
          : model.clazz === "parallelGateway"
          ? $t("parallelGateway")
          : $t("inclusiveGateway")
      }}
    </div>
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
