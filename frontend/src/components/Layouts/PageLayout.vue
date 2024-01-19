<script setup lang="ts">
import TheAppBar from '@/components/TheAppBar.vue'
import TheFooter from '../TheFooter.vue';
</script>
<template>
    <v-app>
        <TheAppBar />
        <v-main>
            <slot></slot>
        </v-main>
        <TheFooter />
    </v-app>
</template>
