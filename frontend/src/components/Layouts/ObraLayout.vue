<script setup lang="ts"></script>
<template>
    <v-row class="mt-2">
        <v-col cols="12" lg="6" class="px-16">
            <slot name="map"></slot>
        </v-col>
        <v-col cols="12" lg="6" class="px-16">
            <slot name="content"></slot>
        </v-col>
    </v-row>
</template>
