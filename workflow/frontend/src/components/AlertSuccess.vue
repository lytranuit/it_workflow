
<template>
  <div class="alert alert-success icon-custom-alert">
    <i class="mdi mdi-check alert-icon"></i>
    <div class="alert-text" v-html="message">
    </div>
  </div>
</template>
<script setup>
const props = defineProps({
  message: String
})
</script>