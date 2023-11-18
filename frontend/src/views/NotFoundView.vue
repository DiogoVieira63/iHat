<script setup lang="ts">
import PageLayout from '../components/PageLayout.vue'
</script>

<template>
    <PageLayout>
        <div class="flex flex-col items-center justify-center">
            <h1 class="text-4xl font-bold text-gray-800">404</h1>
            <p class="text-gray-600">
                Page not found. Go back to <router-link to="/">Home</router-link>
            </p>
        </div>
    </PageLayout>
</template>
