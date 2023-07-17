<template>
  <input ref="inputRef" type="text" class="form-control form-control-sm" />
</template>

<script>
import { useCurrencyInput } from "vue-currency-input";

export default {
  name: "CurrencyInput",
  props: {
    modelValue: Number, // Vue 2: value
    options: Object,
  },
  setup(props, { emit }) {
    const { inputRef, numberValue } = useCurrencyInput(props.options);

    //watchDebounced(numberValue, (value) => emit("update:modelValue", value), {
    //    debounce: 1000,
    //});

    return { inputRef };
  },
};
</script>
