<template>
  <TreeSelect :options="userdepartments" :multiple="multiple" :modelValue="modelValue" :flat="flat" :name="name"
    :required="required" :value-consists-of="valueConsistsOf" :append-to-body="true" @update:modelValue="update">
  </TreeSelect>
</template>

<script setup>
// import TreeSelect from "vue3-acies-treeselect";
import { useAuth } from "../../stores/auth";
import { storeToRefs } from "pinia";
import { computed, onMounted } from "vue";
const props = defineProps({
  modelValue: {
    type: [String, Array],
  },
  valueConsistsOf: {
    type: String,
    default: "LEAF_PRIORITY",
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
    default: "udep",
  },
  flat: {
    type: Boolean,
    default: false,
  },
});
const emit = defineEmits(["update:modelValue"]);
const store = useAuth();
const { userdepartments } = storeToRefs(store);

const update = (value) => {
  value = [...new Set(value)];
  emit('update:modelValue', value)
}
onMounted(() => {
  store.fetchUserDepartment();
});
</script>
