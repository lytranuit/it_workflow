
<template>
  <div class="alert alert-danger icon-custom-alert">
    <i class="mdi mdi-close alert-icon"></i>
    <div class="alert-text" v-html="message">
    </div>
  </div>
</template>
<script setup>
const props = defineProps({
  message: String
})
</script>