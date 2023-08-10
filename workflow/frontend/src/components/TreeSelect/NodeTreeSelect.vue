<template>
  <TreeSelect
    :options="nodes"
    :multiple="multiple"
    :modelValue="modelValue"
    @update:modelValue="emit('update:modelValue', $event)"
  ></TreeSelect>
</template>

<script setup>
import { storeToRefs } from "pinia";
import { computed, onMounted } from "vue";
import { useProcess } from "../../stores/process";
const props = defineProps({
  modelValue: {
    type: [String, Array],
  },
  multiple: {
    type: Boolean,
    default: false,
  },
});
const emit = defineEmits(["update:modelValue"]);
const store = useProcess();
const { nodes } = storeToRefs(store);
onMounted(() => {
  console.log(nodes);
  // store.fetchDepartment();
});
</script>
