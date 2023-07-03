<template>
  <Process :process_id="route.params.id"></Process>
</template>

<script setup>
import { onMounted } from "vue";
import Process from "../../../components/Process/index.vue";
import { useRoute } from "vue-router";
const route = useRoute();
onMounted(() => {});
</script>
