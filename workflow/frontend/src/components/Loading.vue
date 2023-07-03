<template>
    <Teleport to="body">
        <div class="loading" v-if="waiting">
            <ProgressSpinner style="width:50px;height:50px" strokeWidth="8" fill="var(--surface-ground)"
                animationDuration=".5s" aria-label="Custom ProgressSpinner" />
        </div>
    </Teleport>
</template>
<script setup>
import ProgressSpinner from 'primevue/progressspinner';
const props = defineProps({
    waiting: Boolean
})
</script>
<style scoped>
.loading {
    position: fixed;
    display: flex;
    width: 100%;
    height: 100%;
    top: 0;
    left: 0;
    z-index: 1000;
    align-items: center;
    background: #40404073;
}
</style>