<template>
  <TreeSelect
    :options="departments"
    :multiple="multiple"
    :modelValue="modelValue"
    value-consists-of="ALL_WITH_INDETERMINATE"
    @update:modelValue="emit('update:modelValue', $event)"
  ></TreeSelect>
</template>

<script setup>
import { useAuth } from "../../stores/auth";
import { storeToRefs } from "pinia";
import { computed, onMounted } from "vue";
const props = defineProps({
  modelValue: Array,
  multiple: {
    type: Boolean,
    default: false,
  },
});
const emit = defineEmits(["update:modelValue"]);
const store = useAuth();
const { departments } = storeToRefs(store);
onMounted(() => {
  store.fetchDepartment();
});
</script>
