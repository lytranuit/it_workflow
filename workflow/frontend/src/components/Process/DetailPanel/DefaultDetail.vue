<template>
  <div>
    <div class="form-group">
      <div class="col-lg-12 mt-2">
        <b class="col-form-label"
          >{{ $t("label") }}:<span class="text-danger">*</span></b
        >
        <div class="pt-1">
          <input
            class="form-control form-control-sm"
            type="text"
            v-model="model.label"
            :disabled="readOnly"
            @input="
              (value) => {
                onChange('label', value);
              }
            "
          />
        </div>
      </div>
      <div class="col-lg-12 mt-2">
        <b class="col-form-label">{{ $t("guide") }}：</b>
        <div class="pt-1">
          <textarea
            class="form-control form-control-sm"
            :disabled="readOnly"
            v-model="model.guide"
            @input="
              (value) => {
                onChange('guide', value);
              }
            "
          ></textarea>
        </div>
      </div>
      <div class="col-lg-12 mt-2">
        <b class="col-form-label">{{ $t("variable") }}：</b>
        <div class="pt-1">
          <input
            class="form-control form-control-sm"
            :disabled="readOnly"
            v-model="model.variable"
            @input="
              (value) => {
                onChange('variable', value);
              }
            "
          />
        </div>
      </div>
      <div class="col-lg-12 mt-2">
        <b class="col-form-label">{{ $t("stt") }}:</b>
        <div class="pt-1">
          <input
            class="form-control form-control-sm"
            type="number"
            v-model="model.stt"
            :disabled="readOnly"
            @input="
              (value) => {
                onChange('stt', value);
              }
            "
          />
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
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
