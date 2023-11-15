<template>
  <TreeSelect :options="options" :multiple="multiple" :modelValue="modelValue"
    @update:modelValue="emit('update:modelValue', $event)"></TreeSelect>
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
const { prev_fields_file } = storeToRefs(store);
const options = computed(() => {
  return prev_fields_file.value.map((item) => {
    return { id: item.id, label: item.name };
  })
})
onMounted(() => {
  // console.log(prev_fields_file);
  // store.fetchDepartment();
});
</script>
