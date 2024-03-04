<template>
    <input class="form-control form-control-sm number" type="number" :value="value" />
</template>

<script>
export default {
  props: {
    value: {
      type: String,
      default: () => (''),
    }
  },
};
</script>