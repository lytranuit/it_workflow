<template>
  <TreeSelect
    :options="users"
    :multiple="multiple"
    :modelValue="modelValue"
    :name="name"
    :required="required"
    :append-to-body="true"
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
  required: {
    type: Boolean,
    default: false,
  },
  name: {
    type: String,
    default: "user",
  },
});
const emit = defineEmits(["update:modelValue"]);
const store = useProcess();
const { users } = storeToRefs(store);
onMounted(() => {
  store.fetchUsers();
});
</script>
