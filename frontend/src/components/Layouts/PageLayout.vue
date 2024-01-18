<script setup lang="ts">
import TheAppBar from '@/components/TheAppBar.vue'
const links = ['Home', 'About Us', 'Team', 'Services', 'Blog', 'Contact Us']
</script>
<template>
    <v-app>
        <TheAppBar />
        <v-main>
            <slot></slot>
        </v-main>
        <v-footer
            rounded
            w-auto
            color="primary"
        >
            <v-row
                justify="center"
                no-gutters
            >
                <v-btn
                    v-for="link in links"
                    :key="link"
                    color="white"
                    variant="text"
                    class="mx-2"
                    rounded="xl"
                >
                    {{ link }}
                </v-btn>
                <v-col
                    class="text-center mt-4"
                    cols="12"
                >
                    {{ new Date().getFullYear() }} — <strong>iHat</strong>
                </v-col>
            </v-row>
        </v-footer>
    </v-app>
</template>
