<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ $t("timerEvent") }}</div>
    <div class="panelBody">
      <DefaultDetail :model="model" :onChange="onChange" :readOnly="readOnly" />
      <div class="panelRow">
        <div>{{ $t("timerEvent.cycle") }}：</div>
        <el-input
          style="width: 90%; font-size: 12px"
          type="textarea"
          :rows="4"
          :disabled="readOnly"
          :value="model.cycle"
          @input="
            (value) => {
              onChange('cycle', value);
            }
          "
        />
      </div>
      <div class="panelRow">
        <div>{{ $t("timerEvent.duration") }}：</div>
        <el-input
          style="width: 90%; font-size: 12px"
          type="textarea"
          :rows="4"
          :disabled="readOnly"
          :value="model.duration"
          @input="
            (value) => {
              onChange('duration', value);
            }
          "
        />
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
      default: () => {},
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
